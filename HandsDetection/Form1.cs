using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace HandsDetection
{
    public partial class Form1 : Form
    {
        HandDetection detection = new HandDetection();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Image<Bgr, Byte> imgOrg;

            try
            {
                imgOrg = new Image<Bgr, Byte>("D:\\dev\\HandsDetection\\HandsDetection\\img\\IMG_2358.JPG");
            }
            catch (Exception ex)
            {
                return;
            }

            if (imgOrg == null)
            {
                return;
            }

            List<Contour<Point>> contoursList = new List<Contour<Point>>();
            List<Rectangle> boundingBoxes = new List<Rectangle>();

            Image<Gray, Byte> imgSkin = detection.FindSkinContours(imgOrg, contoursList, boundingBoxes);
            Image<Bgr, Byte> imgProc = imgOrg.Copy();

            Rectangle biggestBoundingBox = detection.FindBiggestBoundingBox(boundingBoxes);
            Contour<Point> biggestContour = detection.FindBiggestContour(contoursList);

            CvInvoke.cvDrawContours(imgOrg, biggestContour, new Bgr(Color.Transparent).MCvScalar, new Bgr(Color.LimeGreen).MCvScalar, 2, 0, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, new Point(0, 0));

            imgOrg.Draw(biggestBoundingBox, new Bgr(Color.Red), 2);

            detection.ExtractHull(imgProc, biggestContour);

            ImageBoxOrig.Image = imgOrg.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            ImageBoxSkin.Image = imgSkin.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            //ImageBoxProc.Image = imgProc.Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
        }
    }
}
