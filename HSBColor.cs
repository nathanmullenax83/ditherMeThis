using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DitherMeThis
{
    public struct HSBColor
    {
        float hue;
        float sat;
        float bri;

        public float Hue
        {
            get { return hue; }
            set { hue = value; }
        }

        public float Saturation
        {
            get { return sat; }
            set { sat = value; }
        }

        public float Brightness
        {
            get { return bri; }
            set { bri = value; }
        }



        public HSBColor(Color c)
        {
            hue = c.GetHue();
            sat = c.GetSaturation();
            bri = c.GetBrightness();
        }

        private static Color fcolor(float r, float g, float b)
        {
            int ir = (int)(255.0f * r);
            int ig = (int)(255.0f * g);
            int ib = (int)(255.0f * b);
            return Color.FromArgb(ir, ig, ib);
        }

        public Color RGB
        {
            set
            {
                hue = value.GetHue();
                sat = value.GetSaturation();
                bri = value.GetBrightness();
            }
            get
            {
                float h = hue;
                float s = sat;
                float b = bri;

                float red = 0.0f;
                float green = 0.0f;
                float blue = 0.0f;
                float minc = 0.0f;
                float maxc = 0.0f;
                float delta = 0.0f;
                float sum = 2.0f * b;

                if (b <= 0.5f)
                {
                    delta = s * 2.0f * b;
                }
                else
                {
                    delta = s * 2.0f * (1.0f - b);
                }

                maxc = (2 * b + delta) / 2.0f;
                minc = (2 * b - delta) / 2.0f;

                // maxc = red
                if (h <= 60.0f || h > 300f)
                {
                    red = maxc;
                    float hp = (h > 300) ? (h - 360) : h;
                    hp /= 60.0f;
                    // (g-b)/delta
                    if (hp < 0)	// b > g --> g = minc
                    {
                        green = minc;
                        blue = green - hp * delta;
                    }
                    else  			// g >= b --> b = minc 
                    {
                        blue = minc;
                        green = hp * delta + blue;
                    }

                }
                // maxc = green
                else if (h > 60.0 && h <= 180.0)
                {
                    green = maxc;
                    float hp = h / 60.0f;
                    // 2.0 + (b-r)/delta;
                    hp -= 2.0f;
                    if (hp < 0)
                    {
                        blue = minc;
                        red = blue - hp * delta;
                    }
                    else
                    {
                        red = minc;
                        blue = hp * delta + red;
                    }

                }
                // maxc = blue
                else
                {
                    blue = maxc;
                    float hp = h / 60.0f;
                    // 4.0 + (r-g)/delta;
                    hp -= 4.0f;
                    if (hp < 0)
                    {
                        red = minc;
                        green = red - hp * delta;
                    }
                    else
                    {
                        green = minc;
                        red = hp * delta + green;
                    }
                }
                return fcolor(red, green, blue);
            }
        }
    }
}
