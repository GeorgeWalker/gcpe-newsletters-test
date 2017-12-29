using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.ENewsletters.Templates
{
    using System.IO;
    using System.Web;
    
    public class Utility
    {

        public static string SUBSCRIBE_SYSTEM_GENERATED = "SUBSCRIBE_SYSTEM_GENERATED";
        public static string UNSUBSCRIBE_SYSTEM_GENERATED = "UNSUBSCRIBE_SYSTEM_GENERATED";







        public static string GetHexFromRGB(int red, int green, int blue)
        {
            string a, b, c, d, e, f, g;
            a = GetHex(red / 16);
            b = GetHex(red % 16);
            c = GetHex(green / 16);
            d = GetHex(green % 16);
            e = GetHex(blue / 16);
            f = GetHex(blue % 16);
            g = a + b + c + d + e + f;
            return g;
        }

        public static string GetHex(int num)
        {
            string value = "";
            switch (num)
            {
                case 10:
                    value = "A";
                    break;
                case 11:
                    value = "B";
                    break;
                case 12:
                    value = "C";
                    break;
                case 13:
                    value = "D";
                    break;
                case 14:
                    value = "E";
                    break;
                case 15:
                    value = "F";
                    break;
                default:
                    value = "" + num;
                    break;
            }
            return value;
        }



        public static string MakePictureSrc(string baseUrl, string strPictureName)
        {
            string strSrc = strPictureName;
            if (strPictureName.Trim().IndexOf("http://") != 0)
                strSrc = baseUrl + strPictureName;
            return strSrc;
        }




        public static System.Drawing.Bitmap CreateImage(Gcpe.ENewsletters.Templates.Model.BoxStyle.BoxCornerType ct, 
            int width, int height, int red, int green, int blue)
        {
            System.Drawing.Bitmap objBitmap = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(objBitmap);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
            int x, y, w, h, s, e;
            switch (ct)
            {
                case Gcpe.ENewsletters.Templates.Model.BoxStyle.BoxCornerType.BottomLeft:
                    x = 0;
                    y = -1 * height;
                    w = 2 * width;
                    h = 2 * height;
                    s = 90;
                    e = 180;
                    break;
                case Gcpe.ENewsletters.Templates.Model.BoxStyle.BoxCornerType.BottomRight:
                    x = -1 * width;
                    y = -1 * height;
                    w = 2 * width;
                    h = 2 * height;
                    s = 0;
                    e = 90;
                    break;
                case Gcpe.ENewsletters.Templates.Model.BoxStyle.BoxCornerType.TopLeft:
                    x = 0;
                    y = 0;
                    w = 2 * width;
                    h = 2 * height;
                    s = 180;
                    e = 270;
                    break;
                case Gcpe.ENewsletters.Templates.Model.BoxStyle.BoxCornerType.TopRight:
                    x = -1 * width;
                    y = 0;
                    w = 2 * width;
                    h = 2 * height;
                    s = 270;
                    e = 360;
                    break;
                default:
                    g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(red, green, blue)), rect);
                    return objBitmap;
            }
            g.FillRectangle(new System.Drawing.SolidBrush(System.Drawing.Color.Red), rect);
            //g.FillPie(new System.Drawing.SolidBrush(System.Drawing.Color.Red), -100, -100, 200, 200, 0, 90); //Bottom Right
            //g.FillPie(new System.Drawing.SolidBrush(System.Drawing.Color.Red), 0, -100, 200, 200, 90, 180); //Bottom Left
            //g.FillPie(new System.Drawing.SolidBrush(System.Drawing.Color.Red), -100, 0, 200, 200, 270, 360); //Top Right
            //g.FillPie(new System.Drawing.SolidBrush(System.Drawing.Color.Red), 0, 0, 200, 200, 180, 270); //Top Left
            g.FillPie(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(red, green, blue)), x, y, w, h, s, e); //Top Left
            objBitmap = MakeTransparentGif(objBitmap, System.Drawing.Color.Red);
            return objBitmap;
        }

        /// <summary>
        /// Returns a transparent background GIF image from the specified Bitmap.
        /// </summary>
        /// <param name="bitmap">The Bitmap to make transparent.</param>
        /// <param name="color">The Color to make transparent.</param>
        /// <returns>New Bitmap containing a transparent background gif.</returns>
        private static System.Drawing.Bitmap MakeTransparentGif(System.Drawing.Bitmap bitmap, System.Drawing.Color color)
        {
            byte R = color.R;
            byte G = color.G;
            byte B = color.B;
            MemoryStream fin = new MemoryStream();
            bitmap.Save(fin, System.Drawing.Imaging.ImageFormat.Gif);
            MemoryStream fout = new MemoryStream((int)fin.Length);
            int count = 0;
            byte[] buf = new byte[256];
            byte transparentIdx = 0;
            fin.Seek(0, SeekOrigin.Begin);
            //header
            count = fin.Read(buf, 0, 13);
            if ((buf[0] != 71) || (buf[1] != 73) || (buf[2] != 70)) return null; //GIF
            fout.Write(buf, 0, 13);
            int i = 0;
            if ((buf[10] & 0x80) > 0)
            {
                i = 1 << ((buf[10] & 7) + 1) == 256 ? 256 : 0;
            }
            for (; i != 0; i--)
            {
                fin.Read(buf, 0, 3);
                if ((buf[0] == R) && (buf[1] == G) && (buf[2] == B))
                {
                    transparentIdx = (byte)(256 - i);
                }
                fout.Write(buf, 0, 3);
            }
            bool gcePresent = false;
            while (true)
            {
                fin.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
                if (buf[0] != 0x21) break;
                fin.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
                gcePresent = (buf[0] == 0xf9);
                while (true)
                {
                    fin.Read(buf, 0, 1);
                    fout.Write(buf, 0, 1);
                    if (buf[0] == 0) break;
                    count = buf[0];
                    if (fin.Read(buf, 0, count) != count) return null;
                    if (gcePresent)
                    {
                        if (count == 4)
                        {
                            buf[0] |= 0x01;
                            buf[3] = transparentIdx;
                        }
                    }
                    fout.Write(buf, 0, count);
                }
            }
            while (count > 0)
            {
                count = fin.Read(buf, 0, 1);
                fout.Write(buf, 0, 1);
            }
            fin.Close();
            fout.Flush();
            return new System.Drawing.Bitmap(fout);
        }


    }
}
