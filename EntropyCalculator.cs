using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DitherMeThis
{
    public class EntropyCalculator
    {
        public EntropyCalculator()
        {
        }

        public float Delta(Color c1, Color c2)
        {
            float dh = (c1.GetHue() - c2.GetHue())/360.0f;
            float db = c1.GetBrightness() - c2.GetBrightness();
            float ds = c1.GetSaturation() - c2.GetSaturation();
            return (float)Math.Sqrt(dh * dh + db * db + ds * ds);
        }

        private static int smallest3(int a, int b, int c)
        {
            if (a < b && a < c) return a;
            if (c < a && c < b) return c;
            return b;
        }

        private static SeemDirection least3(int a, int b, int c)
        {
            if (a < b && a < c)
                return SeemDirection.LEFT;
            if (c < a && c < b)
                return SeemDirection.RIGHT;
            else
                return SeemDirection.UP;
        }

        public Bitmap removeVerticalSeems(Bitmap im, IEnumerable<Seem> seems)
        {
            Bitmap b = new Bitmap(im.Width - seems.Count(), im.Height);

            

            return b;
        }

        public IEnumerable<Seem> getVerticalSeems(Bitmap im, int count)
        {
            List<Seem> seems = new List<Seem>();

            using (Bitmap bmp = VerticalEntropyImage(im))
            {
                int[,] vs = new int[bmp.Width,bmp.Height];
                for (int y = 0; y < bmp.Height; ++y)
                {
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        if (y == 0)
                        {
                            vs[x, y] = bmp.GetPixel(x, y).R;
                        }
                        else
                        {
                            int pleft = (x > 0) ? bmp.GetPixel(x - 1, y - 1).R : 256;
                            int pup = bmp.GetPixel(x, y - 1).R;
                            int pright = (x < bmp.Width - 1) ? bmp.GetPixel(x + 1, y - 1).R : 256;

                            vs[x, y] = bmp.GetPixel(x,y).R + smallest3(pleft, pup, pright);
                        }
                    }
                }

                // find n cheapest paths' endpoints 
                List<SeemPixelRecord> pxls = new List<SeemPixelRecord>();

                for (int x = 0; x < bmp.Width; ++x)
                {
                    pxls.Add(new SeemPixelRecord(vs[x, bmp.Height - 1], x, bmp.Height - 1));
                }

                pxls.Sort();

                for (int p = 0; p < Math.Min(count, bmp.Width); ++p)
                {
                    Seem seem = new Seem(pxls[p]);
                    seems.Add(seem);
                    int cx = pxls[p].x;
                    for (int y = bmp.Height - 1; y > 0; --y)
                    {
                        if (cx == 0)
                        {
                            if (vs[cx, y - 1] < vs[cx + 1, y - 1])
                            {
                                seem.Steps.Add(SeemDirection.UP);
                            }
                            else
                            {
                                seem.Steps.Add(SeemDirection.RIGHT);
                                ++cx;
                            }
                        }
                        else if (cx == bmp.Width - 1)
                        {
                            if (vs[cx, y - 1] < vs[cx - 1, y - 1])
                            {
                                seem.Steps.Add(SeemDirection.UP);
                            }
                            else
                            {
                                seem.Steps.Add(SeemDirection.LEFT);
                                cx--;
                            }
                        }
                        else
                        {
                            SeemDirection d = least3(vs[cx - 1, y - 1], vs[cx, y - 1], vs[cx + 1, y - 1]);
                            seem.Steps.Add(d);
                            switch (d)
                            {
                                case SeemDirection.LEFT: --cx; break;
                                case SeemDirection.RIGHT: ++cx; break;
                                case SeemDirection.UP: break;
                            }
                        }
                    }
                }
            }
            return seems;
        }

        public Bitmap VerticalEntropyImage(Bitmap im)
        {
            Bitmap em = new Bitmap(im.Width, im.Height);

            for (int y = 0; y < im.Height; ++y)
            {
                for (int x = 0; x < im.Width; ++x)
                {
                    Color c0, c1, c2;
                    c1 = im.GetPixel(x, y);

                    if (y > 0)
                        c0 = im.GetPixel(x, y - 1);
                    else
                        c0 = c1;

                    if (y < im.Height - 1)
                        c2 = im.GetPixel(x, y + 1);
                    else
                        c2 = c1;

                    float d1 = Delta(c0, c1);   // max root(3)
                    float d2 = Delta(c1, c2);   // max root(3)

                    float sd = (d1 + d2) / (2.0f * (float)Math.Sqrt(3));
                    int c = (int)(255 * sd);
                    em.SetPixel(x, y, Color.FromArgb(c, c, c));
                }
            }

            return em;
        }

        public Bitmap HorizontalEntropyImage(Bitmap im)
        {
            Bitmap em = new Bitmap(im.Width,im.Height);

            for (int y = 0; y < im.Height; ++y)
            {
                for (int x = 0; x < im.Width; ++x)
                {
                    Color c0, c1, c2;
                    c1 = im.GetPixel(x,y);

                    if (x > 0)
                        c0 = im.GetPixel(x - 1, y);
                    else
                        c0 = c1;

                    if (x < im.Width - 1)
                        c2 = im.GetPixel(x + 1, y);
                    else
                        c2 = c1;

                    float d1 = Delta(c0, c1);   // max root(3)
                    float d2 = Delta(c1, c2);   // max root(3)

                    float sd = (d1 + d2) / (2.0f * (float)Math.Sqrt(3));
                    int c = (int)(255 * sd);
                    em.SetPixel(x, y, Color.FromArgb(c, c, c));
                }
            }

            return em;
        }
    }
}
