using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DitherMeThis
{
    public partial class Form1 : Form
    {
        List<Color> colors = new List<Color>();
        string imFileName;
        Random random = new Random();
        EntropyCalculator entropyCalculator = new EntropyCalculator();
        
        
        public Form1()
        {
           
            InitializeComponent();
            openFileDialog1.FileOk += new CancelEventHandler(openFileDialog1_FileOk);
            
        }

        

        void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            imFileName = openFileDialog1.FileName;
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        void assert(bool cond, string msg)
        {
            if (!cond) throw new Exception(msg);
        }

        private int hexdig(char c)
        {
            if (c >= '0' && c <= '9') return (int)(c - '0');
            return 10 + (int)(c - 'A');
        }

        private Color parse_color(string c)
        {
            assert(c.Length == 6, "Color must be specified as a 6-digit hex string (RGB).");
            int r = hexdig(c[0]) * 16 + hexdig(c[1]);
            int g = hexdig(c[2]) * 16 + hexdig(c[3]);
            int b = hexdig(c[4]) * 16 + hexdig(c[5]);
            return Color.FromArgb(r, g, b);
        }

        private void parse_colors()
        {
            colors.Clear();
            foreach( DataGridViewRow dr in dataGridView1.Rows )
            {
                if (dr != null && dr.Cells.Count > 0 && dr.Cells[0].Value != null)
                {
                    string c = dr.Cells[0].Value.ToString();
                    colors.Add(parse_color(c));
                }
            }
        }

        private double color_dist(Color a, Color b)
        {
            double dr = a.R - b.R;
            double dg = a.G - b.G;
            double db = a.B - b.B;
            return Math.Sqrt(dr * dr + dg * dg + db * db);
        }

        private Color nearest_palette_color(Color c)
        {
            double best = double.MaxValue;
            Color bcolor = Color.Black;

            foreach (Color cp in colors)
            {
                double dist = color_dist(cp, c);
                if (dist < best)
                {
                    best = dist;
                    bcolor = cp;
                }
            }

            return bcolor;
        }

        private int intclamp(int m)
        {
            if (m < 0) return 0;
            if (m > 255) return 255;
            return m;
        }

        private Color color_scaleadd(Color a, Color b, float scale)
        {
            int rp = intclamp(a.R + (int)(b.R * scale));
            int gp = intclamp(a.G + (int)(b.G * scale));
            int bp = intclamp(a.B + (int)(b.B * scale));
            return Color.FromArgb(rp, gp, bp);
        }

        private Color random_color()
        {
            int r = random.Next(256);
            int g = random.Next(256);
            int b = random.Next(256);
            return Color.FromArgb(r, g, b);
        }

        private char tohex(int c)
        {
            if (c < 10) return (char)(c + '0');
            else return (char)(c + 'A' - 10);
        }


        private string hexcolor(Color c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(tohex(c.R / 16));
            sb.Append(tohex(c.R % 16));
            sb.Append(tohex(c.G / 16));
            sb.Append(tohex(c.G % 16));
            sb.Append(tohex(c.B / 16));
            sb.Append(tohex(c.B % 16));
            return sb.ToString();

        }

        private void add_random_color()
        {
            Color c = random_color();
            string hx = hexcolor(c);
            object[] hs = {hx};
            dataGridView1.Rows.Add(hs);
        }

        private void do_dither()
        {
            parse_colors();
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    /* oldpixel := pixel[x][y]
                       newpixel := find_closest_palette_color(oldpixel)
                       pixel[x][y] := newpixel
                       quant_error := oldpixel - newpixel
                       pixel[x+1][y] := pixel[x+1][y] + 7/16 * quant_error
                       pixel[x-1][y+1] := pixel[x-1][y+1] + 3/16 * quant_error
                       pixel[x][y+1] := pixel[x][y+1] + 5/16 * quant_error
                       pixel[x+1][y+1] := pixel[x+1][y+1] + 1/16 * quant_error */
                    Color oldpixel = bmp.GetPixel(x, y);
                    Color c = nearest_palette_color(oldpixel);
                    bmp.SetPixel(x, y, c);
                    int rerror = intclamp(oldpixel.R - c.R);
                    int gerror = intclamp(oldpixel.G - c.G);
                    int berror = intclamp(oldpixel.B - c.B);
                    Color qerror = Color.FromArgb(rerror, gerror, berror);

                    if (x < bmp.Width - 1)
                    {
                        bmp.SetPixel(x + 1, y, color_scaleadd(bmp.GetPixel(x + 1, y), qerror, 7f / 16f));
                        if (y < bmp.Height - 1)
                        {
                            bmp.SetPixel(x + 1, y + 1, color_scaleadd(bmp.GetPixel(x + 1, y + 1), qerror, 1f / 16f));
                        }
                    }

                    
                    if (y < bmp.Height - 1)
                    {
                        //   pixel[x][y+1] := pixel[x][y+1] + 5/16 * quant_error
                        bmp.SetPixel(x, y + 1, color_scaleadd(bmp.GetPixel(x, y + 1), qerror, 5f / 16f));
                        if (x > 0 && y < bmp.Height - 1)
                        {
                            //pixel[x-1][y+1] := pixel[x-1][y+1] + 3/16 * quant_error
                            bmp.SetPixel(x - 1, y + 1, color_scaleadd(bmp.GetPixel(x - 1, y + 1), qerror, 3f / 16f));
                        }
                    }
                    
                    
                }
            }
            pictureBox2.Image = bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            do_dither();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            parse_colors();
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            for (int y = 0; y < bmp.Height; ++y)
            {
                for (int x = 0; x < bmp.Width; ++x)
                {
                    Color c = nearest_palette_color(bmp.GetPixel(x, y));
                    bmp.SetPixel(x, y, c);
                }
            }
            pictureBox2.Image = bmp;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            add_random_color();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        /// <summary>
        /// Extract the palette from an existing image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtract_Click(object sender, EventArgs e)
        {
            PaletteExtractor pe = new PaletteExtractor();
            pe.AddColors(imFileName);
            SortedDictionary<int, List<Color>> cs = pe.ByFrequency;
            foreach (int freq in cs.Keys)
            {
                foreach( Color c in cs[freq] )
                {
                    object[] row = {hexcolor(c), freq};
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        private void btnHEntropy_Click(object sender, EventArgs e)
        {
            using (Bitmap b = new Bitmap(pictureBox1.Image))
            {
                pictureBox2.Image = entropyCalculator.HorizontalEntropyImage(b);
            }
        }

        private void btnVEntropy_Click(object sender, EventArgs e)
        {
            using (Bitmap b = new Bitmap(pictureBox1.Image))
            {
                pictureBox2.Image = entropyCalculator.VerticalEntropyImage(b);
            }
        }
    }
}
