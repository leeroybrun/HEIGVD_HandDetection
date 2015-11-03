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
    // Cette classe a été développée par nous afin de gérer toute la détection de la peau, du nombre de doigts, générer les différentes images à afficher, etc
    public class HandDetection
    {
        Image<Bgr, Byte>  imgOrg = null;  // Image originale
        Image<Gray, Byte> imgSkin = null; // Image noir/blanc avec la peau (peau en blanc, le reste en noir)
        Image<Bgr, Byte>  imgProc = null; // Image transformée, qui affiche les doigts détectés, les verticles, etc

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

        int fingerNum = 0;

        bool isHandDetected = false;

        public HandDetection(Image<Bgr, Byte> imgOrg)
        {
            this.imgOrg = imgOrg;
            this.imgProc = imgOrg.Copy();
        }

        public int GetFingerNum()
        {
            return fingerNum;
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

        public bool IsHandDetected()
        {
            return isHandDetected;
        }

        // Cherche les contours et bounding boxes de la peau sur une image fournie
        // Cette partie a été développée par nous-même
        // Elle détecte et sépare les parties de peau de l'image fournie à la classe, d'après les couleurs HSV
        // On va ensuite eroder et dilater les pixels restants, afin d'obtenir une image "plus nette" et réduire le bruit et les imperfections
        // On va ensuite rechercher tous les contours dans l'image obtenur et mettre le plus grand de côté afin de pouvoir le traiter plus tard
        public void FindSkinContours()
        {
            // Converti les couleurs de l'image en HSV
            Image<Hsv, Byte> hsvImg = imgOrg.Convert<Hsv, Byte>();

            // Filtre les pixels de l'image afin de ne garder que ceux qui se rapprochent de la couleur de la peau
            // Nous avons trouvé ces valeurs sur Internet et les avons un peu adaptées pour notre utilisation
            imgSkin = hsvImg.InRange(new Hsv(0, 48, 80), new Hsv(20, 255, 255));

            imgSkin = imgSkin.ThresholdBinary(new Gray(200), new Gray(255));

            // On erode et dilate pour éliminer les imperfections
            // Nous avons trouvé ces valeurs par "tattonement" en essayant d'avoir un contour de main le plus net possible
            StructuringElementEx erodeStrctEl = new StructuringElementEx(4, 4, 2, 2, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvErode(imgSkin, imgSkin, erodeStrctEl, 1);
            StructuringElementEx dilateStrctEl = new StructuringElementEx(6, 6, 3, 3, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvDilate(imgSkin, imgSkin, dilateStrctEl, 3);

            // Boucle sur tous les contours trouvés dans l'image
            for (Contour<Point> contours = imgSkin.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_TREE, contoursStorage); contours != null; contours = contours.HNext)
            {
                // Si un contour est plus grand que 20x20 pixels, on le traite
                if (contours.BoundingRectangle.Width > 20 && contours.BoundingRectangle.Height > 20)
                {
                    // Récupère la bounding box autour du contour
                    Rectangle rect = contours.BoundingRectangle;

                    // On agrandi la bourning box et on vérifie qu'elle ne sorte pas de l'image
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

            // Si des contours ont été trouvés, on cherche le plus grand
            if(boundingBoxes.Count > 0)
            {
                FindBiggestBoundingBox();
                FindBiggestContour();
                isHandDetected = true;
            }
            else
            {
                // Sinon, c'est que rien n'a été détecté
                isHandDetected = false;
                fingerNum = 0;
            }
        }

        // Dessine le contour de la peau sur l'image originale
        public void DrawSkinContour()
        {
            CvInvoke.cvDrawContours(imgOrg, biggestContour, new Bgr(Color.Transparent).MCvScalar, new Bgr(Color.LimeGreen).MCvScalar, 2, 0, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, new Point(0, 0));
        }

        // Dessine la bouding box de la peau sur l'image originale
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

        // Extrait la "coque" du contour et les verticles afin de pouvoir calculer plus tard le nombre de doigts détectés
        // Ce code vient en partie de ce projet : https://www.youtube.com/watch?v=Fjj9gqTCTfc
        public void ExtractHull()
        {
            try
            {
                // Récupère la "coque" du plus grand contour ainsi que le rectangle qui l'englobe
                hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                box = biggestContour.GetMinAreaRect();

                // Récupère les vertexes de la box
                PointF[] points = box.GetVertices();

                // On va créer un tableau de "Point" avec tous les points trouvés par "GetVerticles"
                Point[] ps = new Point[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    ps[i] = new Point((int)points[i].X, (int)points[i].Y);
                }

                // Dessine la "coque" (qui entoure tous les verticles) en rouge sur l'image traitée
                imgProc.DrawPolyline(hull.ToArray(), true, new Bgr(Color.Red), 2);

                // Dessine un cercle bleu au centre de la box
                imgProc.Draw(new CircleF(new PointF(box.center.X, box.center.Y), 5), new Bgr(Color.Blue), 2);

                // Va filtrer les points de la "coque" afin de ne garder que ceux qui sont vraiment utiles
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

        // Détecte et dessine le nombre de doigts sur l'image traitée
        // Ce code vient en partie de ce projet : https://www.youtube.com/watch?v=Fjj9gqTCTfc
        public void DrawAndComputeFingers()
        {
            fingerNum = 0;

            // Boucle sur tous les "defects"
            for (int i = 0; i < defects.Total; i++)
            {
                // Détecte les points de départ, de fin et de profondeur
                PointF startPoint = new PointF((float)defectArray[i].StartPoint.X,
                                                (float)defectArray[i].StartPoint.Y);
                PointF depthPoint = new PointF((float)defectArray[i].DepthPoint.X,
                                                (float)defectArray[i].DepthPoint.Y);
                PointF endPoint = new PointF((float)defectArray[i].EndPoint.X,
                                                (float)defectArray[i].EndPoint.Y);

                // Lignes entre les différents points (départ - profondeur, profondeur - fin)
                LineSegment2D startDepthLine = new LineSegment2D(defectArray[i].StartPoint, defectArray[i].DepthPoint);
                LineSegment2D depthEndLine = new LineSegment2D(defectArray[i].DepthPoint, defectArray[i].EndPoint);

                // Cercles liés aux points de départ, profondeur et fin
                CircleF startCircle = new CircleF(startPoint, 5f);
                CircleF depthCircle = new CircleF(depthPoint, 5f);
                CircleF endCircle = new CircleF(endPoint, 5f);

                // Heuristique personnalisée d'après diverses expériences pour détecter si il s'agit d'un doigt ou non, d'après la position des points de départ, de profondeur et de fin et la taille de la box qui les contient
                if ((startCircle.Center.Y < box.center.Y || depthCircle.Center.Y < box.center.Y) && (startCircle.Center.Y < depthCircle.Center.Y) && (Math.Sqrt(Math.Pow(startCircle.Center.X - depthCircle.Center.X, 2) + Math.Pow(startCircle.Center.Y - depthCircle.Center.Y, 2)) > box.size.Height / 6.5))
                {
                    fingerNum++;
                    imgProc.Draw(startDepthLine, new Bgr(Color.Green), 2);
                }

                // Dessine les différents points
                imgProc.Draw(startCircle, new Bgr(Color.Red), 2);
                imgProc.Draw(depthCircle, new Bgr(Color.Yellow), 5);
                imgProc.Draw(endCircle, new Bgr(Color.DarkBlue), 4);
            }
        }
    }
}
