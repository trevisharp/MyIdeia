using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;

namespace MyIdeia
{
    public class OrganicFilter : Command
    {
        public override bool CommandOk(string command, params object[] parameters)
        {
            return command == "organic";
        }

        public override void Do(Rectangle sel, Bitmap bmp, Graphics g, params object[] parameters)
        {
            int size,
                quan,
                jump;
            if (parameters.Length == 3)
            {
                size = parameters[0] is int ? (int)parameters[0] : 10;
                quan = parameters[1] is int ? (int)parameters[1] : 50;
                jump = parameters[2] is int ? (int)parameters[2] : 20;
            }
            else
            {
                size = 3;
                quan = 10000;
                jump = 5;
            }

            int[] data = new int[sel.Width * sel.Height];

            Random rand = new Random(DateTime.Now.Millisecond);
            int[] targets = new int[quan];
            int total = sel.Width * sel.Height;

            for (int k = 0; k < quan; k++)
                targets[k] = rand.Next(total);
            
            int c = 0;
            while (c < targets.Length)
            {
                total = 0;
                for (int j = 0; j < data.Length; j++)
                {
                    total += data[j] + 1;
                    if (targets[c] < total)
                    {
                        int x = j % sel.Width - size,
                            y = j / sel.Width - size;
                        int lx = x + 2 * size,
                            ly = y + 2 * size;
                        if (x < 0) x = 0;
                        if (y < 0) y = 0;
                        if (lx > sel.Width) lx = sel.Width;
                        if (ly > sel.Height) ly = sel.Height;

                        for (int _y = y; _y < ly; _y++)
                            for (int _x = x; _x < lx; _x++)
                                data[_y * sel.Width + _x] += jump;
                        c++;
                        if (c == targets.Length)
                            break;
                    }
                }
            }

            var bmpdata = bmp.LockBits(sel, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* p = (byte*)bmpdata.Scan0.ToPointer(), line;
                int value;
                for (int j = 0; j < sel.Height; j++)
                {
                    line = p + j * bmpdata.Stride;
                    for (int i = 0; i < sel.Width; i++, line += 3)
                    {
                        value = data[j * sel.Width + i];
                        line[0] = line[1] = line[2] =
                        value > 255 ? 255 : (byte)value;
                    }
                }

            }

            bmp.UnlockBits(bmpdata);
        }
    }
}