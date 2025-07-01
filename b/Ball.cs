using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace b
{
public class Ball
{
    public int ballX;
    public int ballY;
    public int originalX; //inicijalni x i y potrebni za skaliranje
    public int originalY;
    public int radius;
    public bool IsStriped;
    public int Number;
    public Color BaseColor;
    public float VelocityX = 0;
    public float VelocityY = 0;


        public Ball(int x, int y, int radius, bool striped = false, int number = 10, Color? color = null)
    {
        this.ballX = x;
        this.ballY = y;
        originalX = x;
        originalY = y;
        this.radius = radius;
        this.IsStriped = striped;
        this.Number = number;
        this.BaseColor = color ?? Color.DarkRed;
    }
        //ima using za da ne se koristi dispose na site brushes
        public void Draw(Graphics g)
        {
            Rectangle ballBounds = new Rectangle(ballX, ballY, radius * 2, radius * 2);

            //senka pozadi topceto (so chatgpt)
            for (int i = 0; i < 4; i++)
            {
                int offset = i + 2;
                int alpha = 40 - (i * 8);
                using (Brush layeredShadowBrush = new SolidBrush(Color.FromArgb(alpha, Color.Black)))
                {
                    g.FillEllipse(layeredShadowBrush, ballX + offset, ballY + offset, radius * 2, radius * 2);
                }
            }
            //GraphicsPath i PathGradientBrush se koristat za crtanje gradient na topceto so gradient 
            //topce bez linija
             if (!IsStriped) 
             { 
                using (GraphicsPath path = new GraphicsPath())
                {
                path.AddEllipse(ballBounds);
                using (PathGradientBrush pgb = new PathGradientBrush(path))
                {
                    pgb.CenterColor = Lighten(BaseColor);
                    pgb.SurroundColors = new Color[] { BaseColor };
                    g.FillEllipse(pgb, ballBounds);
                }
                }
             }
            //topce so linija
            else
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(ballBounds);
                    using (PathGradientBrush pgb = new PathGradientBrush(path))
                    {
                        pgb.CenterColor = Color.White ; //Lighten(BaseColor);
                        pgb.SurroundColors = new Color[] { Color.White };
                        g.FillEllipse(pgb, ballBounds);
                    }
                }
                //linija
                int stripeHeight = (int)(radius * 1.1);

                Rectangle stripeRect = new Rectangle(ballX, ballY + radius - stripeHeight / 2, radius * 2, stripeHeight);

                using (var stripeBrush = new SolidBrush(BaseColor))
                {
                    g.FillEllipse(stripeBrush, stripeRect);
                }

                using (var stripeHighlight = new LinearGradientBrush(stripeRect,
                    Color.FromArgb(120, Color.White), Color.Transparent, LinearGradientMode.Vertical))
                {
                    g.FillEllipse(stripeHighlight, stripeRect);
                }
            }

            //bel krug vo sredina
            using (Brush whitec = new SolidBrush(Color.White))
            {
                g.FillEllipse(whitec, ballX + radius / 2 - 1,  ballY + radius / 2 , radius+1, radius+1);
            }

            //sjaj
            using (Brush highlight = new SolidBrush(Color.FromArgb(100, Color.White)))
            {

                g.FillEllipse(highlight, ballX + radius / 3, ballY + radius / 4, radius, radius);
            }

            //broj na topce
            if (this.Number != 0) { 
            using (Font font = new Font("Arial", radius * 0.9f, FontStyle.Bold, GraphicsUnit.Pixel))
            using (StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                Rectangle numberArea = new Rectangle(ballX, ballY+1, radius * 2, radius * 2);

                //senka na br
                using (Brush shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
                {
                    Rectangle shadowArea = new Rectangle(numberArea.X + 1, numberArea.Y + 1, numberArea.Width, numberArea.Height);
                    g.DrawString(Number.ToString(), font, shadowBrush, shadowArea, format);
                }

                using (Brush textBrush = new SolidBrush(Color.Black))
                {
                    g.DrawString(Number.ToString(), font, textBrush, numberArea, format);
                }
            }
            }
        }

    private Color Lighten(Color color)
    {
        int r = Math.Min(color.R + 60, 255);
        int g = Math.Min(color.G + 60, 255);
        int b = Math.Min(color.B + 60, 255);
        return Color.FromArgb(r, g, b);
    }
}
}
