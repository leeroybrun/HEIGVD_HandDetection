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
            // Crée une nouvelle capture de la webcam
            Capture capture = new Capture();

            // Pendant que l'application est au "repos", on va traiter les images reçues de la webcam
            Application.Idle += new EventHandler(delegate (object se, EventArgs ev)
            {
                // Récupère une image
                Image<Bgr, Byte> imgOrg = capture.QueryFrame();

                // Crée nouvel objet HandDetection avec l'image fournie
                detection = new HandDetection(imgOrg);

                // Détecte et sépare la peau du reste de l'image + trouve le contour qui correspond à la peau
                detection.FindSkinContours();

                // Si une main est détectée
                if(detection.IsHandDetected())
                {
                    // Dessine le contour de la peau et la bounding box sur l'image originale
                    detection.DrawSkinContour();
                    detection.DrawSkinBoundingBox();

                    // Extrait la "coque" et les verticles qui permettent de calculer le nombre de doigts
                    detection.ExtractHull();

                    // Calcule et dessine le nombre de doigts détectés
                    detection.DrawAndComputeFingers();
                }

                // Récupère les différentes images et les affiche (en les redimenssionnant pour qu'elles s'affichent en entier dans les ImageBox)
                ImageBoxOrig.Image = detection.GetImgOrg().Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                ImageBoxSkin.Image = detection.GetImgSkin().Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
                ImageBoxProc.Image = detection.GetImgProc().Resize(0.5, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);

                // Affiche le nombre de doigts détectés
                NbFingersLabel.Text = detection.GetFingerNum().ToString();
            });
        }
    }
}
