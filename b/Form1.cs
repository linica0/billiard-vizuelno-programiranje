using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace b
{
    public partial class Form1 : Form
    {
        Scene scene;
        public Form1()
        { scene = new Scene();
            InitializeComponent();
            //golemina na prozorec
            this.Width = 1000;
            this.Height = 700;
            //pozicija
            this.StartPosition = FormStartPosition.CenterParent;
            timer1.Interval = 5;
            timer1.Start();
            DoubleBuffered = true;
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
    

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            scene.Draw(e.Graphics);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //na mouse move se pomestuva palkata
            int cueX = scene.cueBall.ballX + scene.cueBall.radius;
            int cueY = scene.cueBall.ballY + scene.cueBall.radius;

            int dx = e.X - cueX;
            int dy = e.Y - cueY;

            scene.currentAngle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);

            this.Invalidate();
        }
        //stapot se vrakja nanazad pri mouse down
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            scene.isCuePullingBack = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            scene.isCuePullingBack = false;
            
          
            // presmetuvanje nasoka
            double angleRad = scene.currentAngle * Math.PI / 180.0;

            // jacina
            float strength = scene.cueOffset * 0.5f;

            // pridvizuvanje po x oska
            scene.cueBall.VelocityX = (float)(Math.Cos(angleRad) * strength);
            //pridvizuvanje po y oska
            scene.cueBall.VelocityY = (float)(Math.Sin(angleRad) * strength);
            scene.cueOffset = 0;
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (scene.isCuePullingBack)
            {
                // oddalecuvanje od topce se do 50 px
                scene.cueOffset = Math.Min(scene.cueOffset + 2, 50); // Max 50 пиксели
            }
            else
            {
                // vrakjanje nanazad pri pustanje
                scene.cueOffset = Math.Max(scene.cueOffset - 4, 0);
            }
            scene.HandleCollisions();

            // update 
            scene.cueBall.ballX += (int)scene.cueBall.VelocityX;
            scene.cueBall.ballY += (int)scene.cueBall.VelocityY;
            //foreach (Ball b in scene.balls)
            //{
            //    b.originalX = b.ballX;
            //    b.originalY = b.ballY;
            //}

            // namaluvanje brzina
            scene.cueBall.VelocityX *= 0.98f;
            scene.cueBall.VelocityY *= 0.98f;

            // sopiranje
            if (Math.Abs(scene.cueBall.VelocityX) < 0.5f)
                scene.cueBall.VelocityX = 0;
            if (Math.Abs(scene.cueBall.VelocityY) < 0.5f)
                scene.cueBall.VelocityY = 0;

            // proverka za udar so tabla
            int minX = 100;   
            int minY = 60;
            int maxX = 100 + scene.poolWidth - scene.cueBall.radius * 2;
            int maxY = 60 + scene.poolHeight - scene.cueBall.radius * 2;
            foreach (Ball b in scene.balls)
            {//50px e ramkata na poolot
                if (b.ballX <= minX+50 || b.ballX >= maxX-50)
                {
                    b.VelocityX *= -0.8f;

                    
                    b.ballX = Math.Max(minX, Math.Min(b.ballX, maxX));
                }
                if (b.ballY <= minY+50 || b.ballY >= maxY-50)
                {
                    b.VelocityY *= -0.8f;
                    b.ballY = Math.Max(minY, Math.Min(b.ballY, maxY));
                }
            }
            foreach (Ball b in scene.balls)
            {
                b.ballX += (int)b.VelocityX;
                b.ballY += (int)b.VelocityY;

                b.VelocityX *= 0.98f;
                b.VelocityY *= 0.98f;

                if (Math.Abs(b.VelocityX) < 0.5f) b.VelocityX = 0;
                if (Math.Abs(b.VelocityY) < 0.5f) b.VelocityY = 0;
            }

            this.Invalidate();
        }


        private void Form1_Resize(object sender, EventArgs e)
{
    int oldWidth = scene.poolWidth;
    int oldHeight = scene.poolHeight;

    scene.winWidth = this.ClientSize.Width;
    scene.winHeight = this.ClientSize.Height;

    
    scene.poolWidth = (int)(4f / 5f * scene.winWidth);
    scene.poolHeight = scene.poolWidth * 9 / 16;

    scene.ScaleBalls(oldWidth, oldHeight);

    this.Invalidate();
}
    }
}
