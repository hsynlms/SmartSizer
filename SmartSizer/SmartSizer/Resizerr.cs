using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using SmartSizer.Models;
using System;

namespace SmartSizer
{
    public class Resizer
    {
        private static Image resizeImage(string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int fromHeight = 0, fromWidth = 0, toHeight = 0, toWidth = 0;

            switch (General.currentFromSize)
            {
                case General.allScreenSizes.X:
                    fromHeight = (int)General.iOSScreenSizes.XHeight;
                    fromWidth = (int)General.iOSScreenSizes.XWidth;
                    break;
                case General.allScreenSizes.X2:
                    fromHeight = (int)General.iOSScreenSizes.X2Height;
                    fromWidth = (int)General.iOSScreenSizes.X2Width;
                    break;
                case General.allScreenSizes.X3:
                    fromHeight = (int)General.iOSScreenSizes.X3Height;
                    fromWidth = (int)General.iOSScreenSizes.X3Width;
                    break;
                case General.allScreenSizes.MDPI:
                    fromHeight = (int)General.AndroidScreenSizes.MDPIHeight;
                    fromWidth = (int)General.AndroidScreenSizes.MDPIWidth;
                    break;
                case General.allScreenSizes.HDPI:
                    fromHeight = (int)General.AndroidScreenSizes.HDPIHeight;
                    fromWidth = (int)General.AndroidScreenSizes.HDPIWidth;
                    break;
                case General.allScreenSizes.XHDPI:
                    fromHeight = (int)General.AndroidScreenSizes.XHDPIHeight;
                    fromWidth = (int)General.AndroidScreenSizes.XHDPIWidth;
                    break;
                case General.allScreenSizes.XXHDPI:
                    fromHeight = (int)General.AndroidScreenSizes.XXHDPIHeight;
                    fromWidth = (int)General.AndroidScreenSizes.XXHDPIWidth;
                    break;
            }

            switch (General.currentToSize)
            {
                case General.allScreenSizes.X:
                    toHeight = (int)General.iOSScreenSizes.XHeight;
                    toWidth = (int)General.iOSScreenSizes.XWidth;
                    break;
                case General.allScreenSizes.X2:
                    toHeight = (int)General.iOSScreenSizes.X2Height;
                    toWidth = (int)General.iOSScreenSizes.X2Width;
                    break;
                case General.allScreenSizes.X3:
                    toHeight = (int)General.iOSScreenSizes.X3Height;
                    toWidth = (int)General.iOSScreenSizes.X3Width;
                    break;
                case General.allScreenSizes.MDPI:
                    toHeight = (int)General.AndroidScreenSizes.MDPIHeight;
                    toWidth = (int)General.AndroidScreenSizes.MDPIWidth;
                    break;
                case General.allScreenSizes.HDPI:
                    toHeight = (int)General.AndroidScreenSizes.HDPIHeight;
                    toWidth = (int)General.AndroidScreenSizes.HDPIWidth;
                    break;
                case General.allScreenSizes.XHDPI:
                    toHeight = (int)General.AndroidScreenSizes.XHDPIHeight;
                    toWidth = (int)General.AndroidScreenSizes.XHDPIWidth;
                    break;
                case General.allScreenSizes.XXHDPI:
                    toHeight = (int)General.AndroidScreenSizes.XXHDPIHeight;
                    toWidth = (int)General.AndroidScreenSizes.XXHDPIWidth;
                    break;
            }

            int newWidth = CalculateNewSize(fromWidth, toWidth, sourceWidth);
            int newHeight = CalculateNewSize(fromHeight, toHeight, sourceHeight);

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format32bppArgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);

            switch (General.currentQuality)
            {
                case General.imageQuality.High:
                default:
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.SmoothingMode = SmoothingMode.HighQuality;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    grPhoto.CompositingQuality = CompositingQuality.HighQuality;

                    break;
                case General.imageQuality.Performance:
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.SmoothingMode = SmoothingMode.HighSpeed;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    grPhoto.CompositingQuality = CompositingQuality.HighSpeed;

                    break;
            }

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }

        private static int CalculateNewSize(double fromValue, double toValue, double height)
        {
            return Convert.ToInt32(Math.Ceiling((toValue * height) / fromValue));
        }

        public static bool ResizeAndSaveImage(SaveFile sf)
        {
            try
            {
                var file = resizeImage(sf.FilePath);

                using (FileStream fs = new FileStream(sf.SavePath, FileMode.Create, FileAccess.Write))
                {
                    file.Save(fs, ImageFormat.Png);
                    fs.Flush();
                    fs.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}