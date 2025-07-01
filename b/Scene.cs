using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace b
{
    public class Scene
    {
        List<Ball> stripedBalls;
        List<Ball> solidBalls;
        public List<Ball> balls;
        Ball ball8;
        public Ball cueBall;
        public int winHeight;
        public int winWidth;
        public float currentAngle;
        public bool isCuePullingBack;
        public int cueOffset = 0;
        Random random=new Random();
        public int poolWidth;
        public int poolHeight;
        public int poolOffsetX;
        public int poolOffsetY;
        public int startPoolWidth;
        public Scene()
        {
            winWidth = 1000; 
            winHeight = 700;
            UpdateLayout(); //funkcija za update na pool width i height (se menuva pri resize)
            startPoolWidth = poolWidth;
            poolOffsetX = 100;
            poolOffsetY = 60;
            stripedBalls = new List<Ball>();
            solidBalls = new List<Ball>();
            balls = new List<Ball>();
            balls.Add(new Ball(-1, -1, 12, false, 0, Color.LightGray));
            balls.Add(new Ball(-1, -1, 12, false, 1, Color.Yellow));
            balls.Add(new Ball(-1, -1, 12, false, 2, Color.Blue));
            balls.Add(new Ball(-1, -1, 12, false, 3, Color.Red));
            balls.Add(new Ball(-1, -1, 12, false, 4, Color.Purple));
            balls.Add(new Ball(-1, -1, 12, false, 5, Color.Orange));
            balls.Add(new Ball(-1, -1, 12, false, 6, Color.Green));
            balls.Add(new Ball(-1, -1, 12, false, 7, Color.Maroon));
            balls.Add(new Ball(-1, -1, 12, false, 8, Color.Black));
            balls.Add(new Ball(-1, -1, 12, true, 9, Color.Yellow));
            balls.Add(new Ball(-1, -1, 12, true, 10, Color.Blue));
            balls.Add(new Ball(-1, -1, 12, true, 11, Color.Red));
            balls.Add(new Ball(-1, -1, 12, true, 12, Color.Purple));
            balls.Add(new Ball(-1, -1, 12, true, 13, Color.Orange));
            balls.Add(new Ball(-1, -1, 12, true, 14, Color.Green));
            balls.Add(new Ball(-1, -1, 12, true, 15, Color.Maroon));
            foreach( Ball b in balls)
            {
                if (b.IsStriped && b.BaseColor!=Color.White && b.BaseColor != Color.Black)
                    stripedBalls.Add(b);
                else
                    solidBalls.Add(b);
            }
            cueBall = balls[0];
            ball8 = balls[8];
            initialPos();
        }

        public void initialPos()
        {
            int rows = 5;
            int radius = 12;
            int baseX = poolOffsetX+poolWidth*3/5; // pocetok na triagolnik
            int baseY =(int) (poolOffsetY+poolHeight/2-2.5*ball8.radius); // centar
            List<int> availableStripedBalls = new List<int> { 9, 10, 11, 12, 13, 14, 15 };
            List<int> availableSolidBalls = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            balls[0].ballX = 301;
            balls[0].ballY = (int)(poolOffsetY + poolHeight / 2 - balls[0].radius*2);
            balls[0].originalX = balls[0].ballX;
            balls[0].originalY = balls[0].ballY;
            //naizmenicno postavuvanje na topkite striped-solid vo triagolnik
            for (int i = 0; i < rows; i++)
            {
                int ballsInRow = i + 1;
                //int offsetX = i * radius * 2;
                int offsetX = (int)(i * radius * Math.Sqrt(3));
                int startY = baseY - (ballsInRow - 1) * radius;
                bool striped = false; // moze da se randomizira, nz dali treba? bool striped = random.Next(2) == 1 ? true : false; **najv ke treba promena vo logikata dolu
                for (int j = 0; j < ballsInRow; j++)
                {
                    int x = baseX + offsetX;
                    int y = startY + j * radius * 2;
                    int num, index;
                    if (ballsInRow==3 && j == 1)
                    {
                        balls[8].ballY = y;
                        balls[8].ballX = x;
                        balls[8].originalY = y;
                        balls[8].originalX = x;
                        ball8 = balls[8];
                    }
                   else if (!striped && availableSolidBalls.Count != 0)
                    {
                        num = random.Next(availableSolidBalls.Count);
                        index = availableSolidBalls[num];
                        balls[index].ballY = y;
                        balls[index].ballX = x;
                        balls[index].originalY = y;
                        balls[index].originalX = x;
                        availableSolidBalls.RemoveAt(num);
                        striped = !striped;
                    }
                    else 
                    {
                        num = random.Next(availableSolidBalls.Count);
                        index = availableStripedBalls[num];
                        balls[index].ballY = y;
                        balls[index].ballX = x;
                        balls[index].originalY = y;
                        balls[index].originalX = x;
                        availableStripedBalls.RemoveAt(num);
                        striped = !striped;
                    }
                    
                        
                }
            }
        }
        public void Draw(Graphics g)
        {
            //pool
            Image poolImage = Image.FromFile("pool5.png");
            g.DrawImage(poolImage, new Rectangle(poolOffsetX, poolOffsetY, poolWidth, poolHeight));
           
            //topcinja
            foreach (Ball b in balls)
            {
                b.Draw(g);
            }
            //stap (so chatgpt)
            //stapot se crta samo ako ne se dvizat site topcinja 
            bool f = true;
            foreach (Ball b in balls) {
                if (b.VelocityX != 0 || b.VelocityY != 0)
                {
                    f = false;
                }
            }
            if (f)
            {
                Image cueStick = Image.FromFile("cue3.png");

                float pivotX = cueBall.ballX + cueBall.radius;
                float pivotY = cueBall.ballY + cueBall.radius;

                g.TranslateTransform(pivotX, pivotY);

                g.RotateTransform(currentAngle + 180);

                int scaledWidth = cueStick.Width / 5;
                int scaledHeight = cueStick.Height / 5;

                Rectangle destRect = new Rectangle(
                    (int)(cueOffset),
                    -scaledHeight / 2 + 2,
                    scaledWidth,
                    scaledHeight
                );

                g.DrawImage(cueStick, destRect);
                g.ResetTransform();

            }
        }
        //potrebno za resize - golemina na pool
        public void UpdateLayout()
        {
            poolWidth = (int)(4f / 5f * winWidth);
            poolHeight = poolWidth * 9 / 16;
        }
        //potrebno za resize - golemina i pozicija na topcinja
        public void ScaleBalls(int oldPoolWidth, int oldPoolHeight)
        {
            float scaleFactorX = poolWidth / (float)startPoolWidth;
            float scaleFactorY = poolHeight / (float)(startPoolWidth * 9 / 16);

            foreach (Ball b in balls)
            {

                b.ballX = poolOffsetX + (int)((b.ballX - poolOffsetX) * scaleFactorX);
                b.ballY = poolOffsetY + (int)((b.ballY - poolOffsetY) * scaleFactorY);
                b.radius = Math.Max(5, (int)(12 * scaleFactorX));

            }
        }


        public void HandleCollisions()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                Ball b1 = balls[i];
                for (int j = i + 1; j < balls.Count; j++)
                {
                    Ball b2 = balls[j];

                    float dx = b1.ballX - b2.ballX;
                    float dy = b1.ballY - b2.ballY;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    float minDist = b1.radius + b2.radius;
                    //so chatgpt
                    if (distance < minDist && distance > 0.001f) 
                    {
                        // Нормален вектор
                        float nx = dx / distance;
                        float ny = dy / distance;

                        // Тангенцијален вектор
                        float tx = -ny;
                        float ty = nx;

                        // Проекции на брзините
                        float v1n = b1.VelocityX * nx + b1.VelocityY * ny;
                        float v1t = b1.VelocityX * tx + b1.VelocityY * ty;

                        float v2n = b2.VelocityX * nx + b2.VelocityY * ny;
                        float v2t = b2.VelocityX * tx + b2.VelocityY * ty;

                        // Еластично одбивање: размена на нормалните брзини
                        float boost = 1.5f; // 1.5x енергија при судир

                       float  v1nAfter = v2n * boost;
                        float v2nAfter = v1n * boost;

                        // Нови вектори на брзини
                        b1.VelocityX = v1nAfter * nx + v1t * tx;
                        b1.VelocityY = v1nAfter * ny + v1t * ty;

                        b2.VelocityX = v2nAfter * nx + v2t * tx;
                        b2.VelocityY = v2nAfter * ny + v2t * ty;

                        // Мало поместување за да се избегне преклопување
                        float overlap = minDist - distance;

                        b1.ballX +=(int) (nx * (overlap / 2));
                        b1.ballY += (int)(ny * (overlap / 2));

                        b2.ballX -= (int)(nx * (overlap / 2));
                        b2.ballY -= (int)(ny * (overlap / 2));
                    }
                }
            }
        }
    }
}
