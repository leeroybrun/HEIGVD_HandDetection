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
        HandDetection detection = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Image<Bgr, Byte> imgOrg;

            try
            {
                imgOrg = new Image<Bgr, Byte>("D:\\dev\\HandsDetection\\HandsDetection\\img\\IMG_2383.JPG");
            }
            catch (Exception ex)
            {
                return;
            }

            if (imgOrg == null)
            {
                return;
            }

            detection = new HandDetection(imgOrg);

            detection.FindSkinContours();

            detection.DrawSkinContour();
            detection.DrawSkinBoundingBox();

            detection.ExtractHull();

            detection.DrawAndComputeFingers();

            ImageBoxOrig.Image = detection.GetImgOrg().Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            ImageBoxSkin.Image = detection.GetImgSkin().Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            ImageBoxProc.Image = detection.GetImgProc().Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
        }
    }
}
