using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace WindowsApplication1
{
    public partial class Frm_histogram : Form
    {
        public static int valores = 256;
        public static Bitmap bmpimg;
        public static int[] histograma = new int[valores];
        public static int[] histogramaR = new int[valores];
        public static int[] histogramaG = new int[valores];
        public static int[] histogramaB = new int[valores];
        public static int tamanio;
        public  int valor;
        public int valorR; 
        public int valorG;
        public int valorB;
        public int posiciony = 123;
        int T = 0;

        public Frm_histogram()
        {
            InitializeComponent();
            rb_histogramaBN.Checked = true;
            comboBox1.Items.Add("Pseudo-colores...");
            comboBox1.Items.Add("Escala gris");
            comboBox1.Items.Add("Invertir");
            comboBox1.Items.Add("Verde-Amarillo");
            comboBox1.Items.Add("Rojo-Azul");
            comboBox1.Items.Add("Azul-Amarillo");
            comboBox1.Items.Add("Celeste-Rosado");
            comboBox1.Items.Add("Colores 4");
            comboBox1.SelectedIndex = 0;
            
        }
        public void abrir()
        {
            OpenFileDialog filechooser = new OpenFileDialog();
            filechooser.RestoreDirectory = true;

            if (filechooser.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.Image = new Bitmap(new Bitmap(filechooser.FileName), 300, 300);
            }

            bmpimg = new Bitmap(this.pictureBox1.Image);
            
            tamanio = bmpimg.Width + bmpimg.Height;

            for (int i = 0; i < valores; i++)
            {
                histograma[i] = 0;
                histogramaR[i] = 0;
                histogramaG[i] = 0;
                histogramaB[i] = 0;
            }
            panel1.CreateGraphics().Clear(Color.White);
           
            pictureBox2.CreateGraphics().Clear(Color.LightGray);
            pictureBox2.Refresh();
            
            
        }
        public void CalcularHistograma()
        {

            Color color;
            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    if (rb_histogramaBN.Checked == true)
                    {
                        valor = (int)(color.R + color.B + color.G) / 3;
                        histograma[valor]++;
                    }
                   
                    

                }
            }
            
            Dibujar();
        }

        public void Dibujar()
        {
            Bitmap bm = new Bitmap(1, 1);
            Bitmap bmR = new Bitmap(1, 1);
            Bitmap bmG = new Bitmap(1, 1);
            Bitmap bmB = new Bitmap(1, 1);


            Graphics g = this.panel1.CreateGraphics();
           
            Normalizar();

            if (rb_histogramaBN.Checked == true)//////////Dibujar BN
            {
                for (int x = 0; x < valores; x++)
                {
                    if (x != 0)
                    {   int cant = histograma[x];
                        if (cant != 0)
                        {
                            for (int y = posiciony; y > (posiciony-cant); y--)
                            {   bm.SetPixel(0, 0, Color.FromArgb(x,x,x));
                                g.DrawImageUnscaled(bm, x, y);
                            }     
                        }
                    }
                }
            }
            

        }
        public void Normalizar()
        {
            
            for (int i = 0; i < valores; i++)
            {
                histograma[i] = (int)histograma[i] / 10;
                histogramaR[i] = (int)histogramaR[i] / 10;
                histogramaG[i] = (int)histogramaG[i] / 10;
                histogramaB[i] = (int)histogramaB[i] / 10;
            }
        }

        private void btn_generar_Click(object sender, EventArgs e)
        {
            if (rb_histogramaBN.Checked == true) 
                CalcularHistograma();
           
            if (rb_limiar.Checked == true)
                CalcularLimiarOtsu();
            if ( rb_binariza.Checked == true)
                CalcularBinariza();

        }
        public void CalcularLimiarOtsu()
        {
            double[] prob=new double[257];
            double u1=0;
            double u2=0;
            double w1=0;
            double w2=0;
            double uT = 0; double max = 0; 
            
            double[] Ovarianza = new double[257]; 
            double numPixel=pictureBox1.Width*pictureBox1.Height;
            //Probabilidad de c/pixel - intensidad
            for (int i = 1; i <= valores; i++)
                prob[i] =  Convert.ToDouble(histograma[i-1]) / numPixel;
            //Dividir en clase C1 y C2
            for (int t = 1; t <= valores; t++)
            {
                u1 = 0; u2 = 0;
                w1 = 0; w2 = 0;
                for (int i = 1; i <= t; i++)
                {
                    w1 += prob[i];
                    u1 += (i) * prob[i];
                }
                for (int i = t + 1; i <= valores; i++)
                {
                    w2 += prob[i];
                    u2 += (i) * prob[i];
                }
                u1 = u1 / w1;
                u2 = u2 / w2;
                uT = (w1 * u1) + (w2 * u2);

                //Varianza
                Ovarianza[t] = (w1 * Math.Pow(u1 - uT, 2)) + (w2 * Math.Pow(u2 - uT, 2));
                if (max < Ovarianza[t])
                {
                    max = Ovarianza[t];
                    T = t;
                }

            }
            DibujarLimiar();
        }
        public void DibujarLimiar()
        {
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.panel1.CreateGraphics();
            bm.SetPixel(0, 0, Color.Red);
            for (int y = 0; y <panel1.Height ;y++ )
                g.DrawImageUnscaled(bm,Convert.ToInt32(T), y);
                
        }
        public void CalcularBinariza()
        {
            Color color;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();
            
            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    valor = (int)(color.R + color.B + color.G) / 3;
                    if (valor < Convert.ToInt32(T))
                    {
                        bm.SetPixel(0, 0, Color.Black);
                        g.DrawImageUnscaled(bm, i, j);
                    }
                    else
                    {
                        bm.SetPixel(0, 0, Color.White);
                        g.DrawImageUnscaled(bm, i, j);
                    } 
                }
            }
        }

        public void Trackbar_mov()
        {
            T = trackBar1.Value;
            CalcularBinariza();

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Trackbar_mov();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 1)
                 DibujaGris();
             if (comboBox1.SelectedIndex == 2)
                 Invertir();
            if (comboBox1.SelectedIndex == 3)
                DibujaVerdeAmarillo(); 
             if (comboBox1.SelectedIndex == 4)
                 DibujaRojoAzul();
            if (comboBox1.SelectedIndex == 5)
                 DibujaAzulAmarillo();
             if (comboBox1.SelectedIndex == 6)
                 DibujaCelesteRosado();
             if (comboBox1.SelectedIndex == 7)
                 DibujaColores();
             
        }

      
        public void DibujaGris()
        {
            Color color;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    valor = (int)(color.R + color.B + color.G) / 3;
                    bm.SetPixel(0, 0, Color.FromArgb(valor,valor,valor));
                    g.DrawImageUnscaled(bm, i, j);
                 }
            }

            Graphics graphics = this.pic_colores.CreateGraphics();
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 10), new Point(256, 10), Color.Black, Color.White);
            graphics.FillRectangle(linGrBrush, 0, 0, 256, 15);
        }

        public void DibujaRojoAzul()
        {
            //Graphics graphics = this.pic_colores.CreateGraphics();
            Bitmap YourBMP = new Bitmap(256,15);
            Graphics gg = Graphics.FromImage(YourBMP);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 10), new Point(256, 10), Color.Red, Color.Blue);
            gg.FillRectangle(linGrBrush, 0, 0, 256, 15);
                       
            ///Draw anything you want on g
            pic_colores.Image = YourBMP;
            pic_colores.Refresh();
 
           //Bitmap picbmpimg = new Bitmap(pic_colores.Image);


            Color color;
            Color colorNuevo;
            int val;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            for ( int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    val = (int)(color.R + color.B + color.G) / 3;

                    colorNuevo = YourBMP.GetPixel(val,10);

                    bm.SetPixel(0, 0, colorNuevo);
                    g.DrawImageUnscaled(bm, i, j);
                    //pictureBox2.Refresh();
                }
            }     

        }

        public void DibujaAzulAmarillo()
        {
            //Graphics graphics = this.pic_colores.CreateGraphics();
            Bitmap BMP = new Bitmap(256, 15);
            Graphics gg = Graphics.FromImage(BMP);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 10), new Point(256, 10), Color.Blue, Color.Yellow);
            gg.FillRectangle(linGrBrush, 0, 0, 256, 15);

            ///Draw anything you want on g
            pic_colores.Image = BMP;
            pic_colores.Refresh();

            //Bitmap picbmpimg = new Bitmap(pic_colores.Image);


            Color color;
            Color colorNuevo;
            int val;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    val = (int)(color.R + color.B + color.G) / 3;

                    colorNuevo = BMP.GetPixel(val, 10);

                    bm.SetPixel(0, 0, colorNuevo);
                    g.DrawImageUnscaled(bm, i, j);
                    //pictureBox2.Refresh();
                }
            }

        }
        public void DibujaVerdeAmarillo()
        {
            //Graphics graphics = this.pic_colores.CreateGraphics();
            Bitmap BMP = new Bitmap(256, 15);
            Graphics gg = Graphics.FromImage(BMP);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 10), new Point(256, 10), Color.Green, Color.Yellow);
            gg.FillRectangle(linGrBrush, 0, 0, 256, 15);

            ///Draw anything you want on g
            pic_colores.Image = BMP;
            pic_colores.Refresh();

            //Bitmap picbmpimg = new Bitmap(pic_colores.Image);


            Color color;
            Color colorNuevo;
            int val;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    val = (int)(color.R + color.B + color.G) / 3;

                    colorNuevo = BMP.GetPixel(val, 10);

                    bm.SetPixel(0, 0, colorNuevo);
                    g.DrawImageUnscaled(bm, i, j);
                    //pictureBox2.Refresh();
                }
            }

        }
        public void DibujaCelesteRosado()
        {
            //Graphics graphics = this.pic_colores.CreateGraphics();
            Bitmap BMP = new Bitmap(256, 15);
            Graphics gg = Graphics.FromImage(BMP);
            LinearGradientBrush linGrBrush = new LinearGradientBrush(new Point(0, 10), new Point(256, 10), Color.CadetBlue, Color.Thistle);
            gg.FillRectangle(linGrBrush, 0, 0, 256, 15);

            ///Draw anything you want on g
            pic_colores.Image = BMP;
            pic_colores.Refresh();

            //Bitmap picbmpimg = new Bitmap(pic_colores.Image);


            Color color;
            Color colorNuevo;
            int val;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    val = (int)(color.R + color.B + color.G) / 3;

                    colorNuevo = BMP.GetPixel(val, 10);

                    bm.SetPixel(0, 0, colorNuevo);
                    g.DrawImageUnscaled(bm, i, j);
                    //pictureBox2.Refresh();
                }
            }

        }
        public void DibujaColores()
        {
             Bitmap BMP = new Bitmap(256, 15);
            Graphics gg = Graphics.FromImage(BMP);

            ////////////////
            Color[] EndColors = {Color.Red,Color.Red,Color.Blue,Color.Yellow,Color.Yellow,Color.Green};
            float[] ColorPositions = { 0.0f, .20f, .40f, .60f, .80f, 1.0f };

            ColorBlend C_Blend = new ColorBlend();
            C_Blend.Colors = EndColors;
            C_Blend.Positions = ColorPositions;

            LinearGradientBrush B = new LinearGradientBrush(new Point(0, 10),new Point(256, 10),Color.White,Color.Black);
            B.InterpolationColors = C_Blend;
            /////////////////

          
            gg.FillRectangle(B, 0, 0, 256, 15);

            ///Draw anything you want on g
            pic_colores.Image = BMP;
            pic_colores.Refresh();

             Color color;
            Color colorNuevo;
            int val;
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            for (int i = 0; i < bmpimg.Height; i++)
            {
                for (int j = 0; j < bmpimg.Width; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    val = (int)(color.R + color.B + color.G) / 3;

                    colorNuevo = BMP.GetPixel(val, 10);

                    bm.SetPixel(0, 0, colorNuevo);
                    g.DrawImageUnscaled(bm, i, j);
                    //pictureBox2.Refresh();
                }
            }

        }
        public void Invertir()
        {
            
            Bitmap bm = new Bitmap(1, 1);
            Graphics g = this.pictureBox2.CreateGraphics();

            Color color;
            for (int i = 0; i < bmpimg.Width; i++)
            {
                for (int j = 0; j < bmpimg.Height; j++)
                {
                    color = bmpimg.GetPixel(i, j);
                    bm.SetPixel(0, 0,Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B));
                    g.DrawImageUnscaled(bm, i, j);
                }
            }
            
        }

    }   
}