using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Windows.Forms;

namespace HandsDetection
{
    public class HandDetection
    {
        Image<Bgr, Byte>  imgOrg = null;
        Image<Gray, Byte> imgSkin = null;
        Image<Bgr, Byte>  imgProc = null;
        MemStorage contoursStorage = new MemStorage();
        List<Contour<Point>> contoursList = new List<Contour<Point>>();
        List<Rectangle> boundingBoxes = new List<Rectangle>();

        Rectangle biggestBoundingBox = new Rectangle();
        Contour<Point> biggestContour = null;

        Seq<Point> hull;
        Seq<Point> filteredHull;
        Seq<MCvConvexityDefect> defects;
        MCvConvexityDefect[] defectArray;
        Rectangle handRect;
        MCvBox2D box;
        Ellipse ellip;
        MemStorage hullStorage = new MemStorage();

        public HandDetection(Image<Bgr, Byte> imgOrg)
        {
            this.imgOrg = imgOrg;
            this.imgProc = imgOrg.Copy();
        }

        public Image<Bgr, Byte> GetImgOrg()
        {
            return imgOrg.Copy();
        }

        public Image<Gray, Byte> GetImgSkin()
        {
            return imgSkin.Copy();
        }

        public Image<Bgr, Byte> GetImgProc()
        {
            return imgProc.Copy();
        }

        // Cherche les contours et bounding boxes de la peau sur une image fournie
        public void FindSkinContours()
        {
            // Converti les couleurs de l'image en HSV
            Image<Hsv, Byte> hsvImg = imgOrg.Convert<Hsv, Byte>();

            //hsvImg = hsvImg.SmoothGaussian(4);

            // Filtre les pixels de l'image afin de ne garder que ceux qui se rapprochent de la couleur de la peau
            imgSkin = hsvImg.InRange(new Hsv(0, 48, 80), new Hsv(20, 255, 255));

            /*IntPtr comp = IntPtr.Zero;
            MemStorage storage = new MemStorage();
            CvInvoke.cvPyrSegmentation(imgSkin, imgSkin, storage, out comp, 4, 200, 255);*/

            //CvInvoke.cvSmooth(imgSkin, imgSkin, Emgu.CV.CvEnum.SMOOTH_TYPE.CV_GAUSSIAN, 5, 5, 9, 9);

            imgSkin = imgSkin.ThresholdBinary(new Gray(200), new Gray(255));
            //imgSkin = imgSkin.ThresholdAdaptive(new Gray(150), Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_GAUSSIAN_C, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY, 2, new Gray(200));

            // On erode et dilate pour éliminer les imperfections
            StructuringElementEx erodeStrctEl = new StructuringElementEx(12, 12, 6, 6, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvErode(imgSkin, imgSkin, erodeStrctEl, 2);
            StructuringElementEx dilateStrctEl = new StructuringElementEx(6, 6, 3, 3, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvDilate(imgSkin, imgSkin, dilateStrctEl, 4);
            //imgSkin = imgSkin.Erode(7);
            //imgSkin = imgSkin.Dilate(7);

            //int biggestSize = 0;

            // Boucle sur tous les contours trouvés dans l'image
            for (Contour<Point> contours = imgSkin.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, contoursStorage); contours != null; contours = contours.HNext)
            {
                // Si un contour est plus grand que 20x20 pixels, on le traite
                if (contours.BoundingRectangle.Width > 20 && contours.BoundingRectangle.Height > 20)
                {
                    // On crée une bounding box autour du contour
                    Rectangle rect = contours.BoundingRectangle;

                    rect.X = rect.X - 10;
                    rect.X = (rect.X < 0) ? 0 : rect.X;
                    rect.Y = rect.Y - 10;
                    rect.Y = (rect.Y < 0) ? 0 : rect.Y;

                    rect.Height = (rect.Height + 20);
                    rect.Width = (rect.Width + 20);

                    // Ajout de la bounding box et du contour aux listes
                    boundingBoxes.Add(rect);
                    contoursList.Add(contours);

                    /* Ne fonctionne pas...
                    int size = contours.BoundingRectangle.Width * contours.BoundingRectangle.Height;
                    if(size > biggestSize)
                    {
                        biggestBoundingBox = rect;
                        biggestContour = contours;
                    }*/
                }
            }

            // Pas très optimisé... devrait trouver le plus grand contour & bounding box lors de la boucle sur tous les contours
            FindBiggestBoundingBox();
            FindBiggestContour();
        }

        public void DrawSkinContour()
        {
            CvInvoke.cvDrawContours(imgOrg, biggestContour, new Bgr(Color.Transparent).MCvScalar, new Bgr(Color.LimeGreen).MCvScalar, 2, 0, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, new Point(0, 0));
        }

        public void DrawSkinBoundingBox()
        {
            imgOrg.Draw(biggestBoundingBox, new Bgr(Color.Red), 2);
        }

        // Trouve la plus grande bounding box
        public void FindBiggestBoundingBox()
        {
            boundingBoxes.Sort((X, Y) => ((Y.Height * Y.Width).CompareTo(X.Height * X.Width)));

            biggestBoundingBox = boundingBoxes.First<Rectangle>();
        }

        // Trouve le plus grand contour
        public void FindBiggestContour()
        {
            contoursList.Sort((X, Y) => ((Y.BoundingRectangle.Height * Y.BoundingRectangle.Width).CompareTo(X.BoundingRectangle.Height * X.BoundingRectangle.Width)));

            biggestContour = contoursList.First<Contour<Point>>();
        }

        public void ExtractHull()
        {
            try
            {
                hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                box = biggestContour.GetMinAreaRect();
                PointF[] points = box.GetVertices();

                Point[] ps = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                    ps[i] = new Point((int)points[i].X, (int)points[i].Y);

                imgProc.DrawPolyline(hull.ToArray(), true, new Bgr(200, 125, 75), 2);
                imgProc.Draw(new CircleF(new PointF(box.center.X, box.center.Y), 3), new Bgr(200, 125, 75), 2);

                filteredHull = new Seq<Point>(hullStorage);
                for (int i = 0; i < hull.Total; i++)
                {
                    if (Math.Sqrt(Math.Pow(hull[i].X - hull[i + 1].X, 2) + Math.Pow(hull[i].Y - hull[i + 1].Y, 2)) > box.size.Width / 10)
                    {
                        filteredHull.Push(hull[i]);
                    }
                }

                defects = biggestContour.GetConvexityDefacts(hullStorage, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);

                defectArray = defects.ToArray();
            } catch(Exception ex) {
                return;
            }
        }

        public void DrawAndComputeFingers()
        {
            int fingerNum = 0;

            for (int i = 0; i < defects.Total; i++)
            {
                PointF startPoint = new PointF((float)defectArray[i].StartPoint.X,
                                                (float)defectArray[i].StartPoint.Y);

                PointF depthPoint = new PointF((float)defectArray[i].DepthPoint.X,
                                                (float)defectArray[i].DepthPoint.Y);

                PointF endPoint = new PointF((float)defectArray[i].EndPoint.X,
                                                (float)defectArray[i].EndPoint.Y);

                LineSegment2D startDepthLine = new LineSegment2D(defectArray[i].StartPoint, defectArray[i].DepthPoint);

                LineSegment2D depthEndLine = new LineSegment2D(defectArray[i].DepthPoint, defectArray[i].EndPoint);

                CircleF startCircle = new CircleF(startPoint, 5f);

                CircleF depthCircle = new CircleF(depthPoint, 5f);

                CircleF endCircle = new CircleF(endPoint, 5f);

                //Custom heuristic based on some experiment, double check it before use
                if ((startCircle.Center.Y < box.center.Y || depthCircle.Center.Y < box.center.Y) && (startCircle.Center.Y < depthCircle.Center.Y) && (Math.Sqrt(Math.Pow(startCircle.Center.X - depthCircle.Center.X, 2) + Math.Pow(startCircle.Center.Y - depthCircle.Center.Y, 2)) > box.size.Height / 6.5))
                {
                    fingerNum++;
                    imgProc.Draw(startDepthLine, new Bgr(Color.Green), 2);
                    //imgProc.Draw(depthEndLine, new Bgr(Color.Magenta), 2);
                }


                imgProc.Draw(startCircle, new Bgr(Color.Red), 2);
                imgProc.Draw(depthCircle, new Bgr(Color.Yellow), 5);
                //imgProc.Draw(endCircle, new Bgr(Color.DarkBlue), 4);
            }

            MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_DUPLEX, 5d, 5d);
            imgProc.Draw(fingerNum.ToString(), ref font, new Point(50, 150), new Bgr(Color.White));
        }
    }
}
