using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DitherMeThis
{
    public class PaletteExtractor
    {
        private Dictionary<Color, int> colors = new Dictionary<Color,int>();

        public IEnumerable<Color> Colors
        {
            get { return colors.Keys; }
        }

        public int GetCount(Color c)
        {
            return colors.ContainsKey(c) ? colors[c] : 0;
        }

        public int GetCount(List<Color> cs)
        {
            int c = 0;
            foreach (Color col in cs)
                c += GetCount(col);
            return c;
        }

        /// <summary>
        /// Remove all colors from the palette.
        /// </summary>
        public void ClearColors()
        {
            colors.Clear();
        }

        /// <summary>
        /// Open an image and extract its palette.
        /// </summary>
        /// <param name="filename">Path to image + filename</param>
        public void AddColors(string filename)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(filename);

            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    Color c = bmp.GetPixel(x, y);
                    if (!colors.ContainsKey(c))
                    {
                        colors.Add(c, 0);
                    }
                    colors[c]++;
                }
            }

            bmp.Dispose();
        }

        public SortedDictionary<int, List<Color>> ByFrequency
        {
            get
            {
                SortedDictionary<int, List<Color>> freqs = new SortedDictionary<int, List<Color>>();

                foreach (Color c in Colors)
                {
                    int freq = colors[c];
                    if (!freqs.ContainsKey(freq))
                    {
                        freqs.Add(freq,new List<Color>());
                    }
                    freqs[freq].Add(c);
                }

                return freqs;
            }
        }

        public Dictionary<float, List<Color>> ByBrightness
        {
            get
            {
                Dictionary<float, List<Color>> bris = new Dictionary<float, List<Color>>();
                foreach (Color c in Colors)
                {
                    float bri = c.GetBrightness();
                    if (!bris.ContainsKey(bri))
                    {
                        bris.Add(bri, new List<Color>());
                    }
                    bris[bri].Add(c);
                }
                return bris;
            }
        }

        public Dictionary<float, List<Color>> BySaturation
        {
            get
            {
                Dictionary<float, List<Color>> sats = new Dictionary<float, List<Color>>();
                foreach (Color c in Colors)
                {
                    float sat = c.GetSaturation();
                    if (!sats.ContainsKey(sat))
                    {
                        sats.Add(sat, new List<Color>());
                    }
                    sats[sat].Add(c);
                }
                return sats;
            }
        }

        public Dictionary<float, List<Color>> ByHue
        {
            get
            {
                Dictionary<float, List<Color>> hues = new Dictionary<float, List<Color>>();
                foreach (Color c in Colors)
                {
                    float hue = c.GetHue();
                    if (!hues.ContainsKey(hue))
                    {
                        hues.Add(hue, new List<Color>());
                    }
                    hues[hue].Add(c);
                }
                return hues;
            }
        }

        public PaletteExtractor()
        {
        }
    }
}
