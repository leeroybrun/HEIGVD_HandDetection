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
        // Cherche les contours et bounding boxes de la peau sur une image fournie
        public Image<Gray, Byte> FindSkinContours(Image<Bgr, Byte> imgOrg, List<Contour<Point>> contoursList, List<Rectangle> boundingBoxes)
        {
            // Converti les couleurs de l'image en HSV
            Image<Hsv, Byte> hsvImg = imgOrg.Convert<Hsv, Byte>();

            // Filtre les pixels de l'image afin de ne garder que ceux qui se rapprochent de la couleur de la peau
            Image<Gray, Byte> skin = hsvImg.InRange(new Hsv(0, 48, 80), new Hsv(20, 255, 255));

            skin = skin.ThresholdBinary(new Gray(145), new Gray(255));

            //skin = skin.SmoothGaussian(3);

            // On erode et dilate pour éliminer les imperfections
            skin = skin.Erode(7);
            skin = skin.Dilate(7);

            using (MemStorage storage = new MemStorage())
            {
                // Boucle sur tous les contours trouvés dans l'image
                for (Contour<Point> contours = skin.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, storage); contours != null; contours = contours.HNext)
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
                    }
                }
            }

            return skin; // Retourne l'image avec uniquement la peau
        }

        // Trouve la plus grande bounding box
        public Rectangle FindBiggestBoundingBox(List<Rectangle> boundingBoxes)
        {
            boundingBoxes.Sort((X, Y) => ((Y.Height * Y.Width).CompareTo(X.Height * X.Width)));

            return boundingBoxes.First<Rectangle>();
        }

        // Trouve le plus grand contour
        public Contour<Point> FindBiggestContour(List<Contour<Point>> contours)
        {
            contours.Sort((X, Y) => ((Y.BoundingRectangle.Height * Y.BoundingRectangle.Width).CompareTo(X.BoundingRectangle.Height * X.BoundingRectangle.Width)));

            return contours.First<Contour<Point>>();
        }

        public void ExtractHull(Image<Bgr, Byte> img, Contour<Point> contour)
        {
            try
            {
                Seq<Point> hull = contour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                /*MCvBox2D box = contour.GetMinAreaRect();
                PointF[] points = box.GetVertices();

                Point[] ps = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                    ps[i] = new Point((int)points[i].X, (int)points[i].Y);

                img.DrawPolyline(hull.ToArray(), true, new Bgr(200, 125, 75), 2);
                img.Draw(new CircleF(new PointF(box.center.X, box.center.Y), 3), new Bgr(200, 125, 75), 2);*/
            } catch(Exception ex) {
                return;
            }
        }
    }
}
