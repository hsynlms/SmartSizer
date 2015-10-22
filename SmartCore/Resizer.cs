using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System;

namespace SmartCore
{
    public class Resizer
    {
        private static Image resizeImage(string stPhotoPath, Models.Settings settings)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            int fromHeight = 0, fromWidth = 0, toHeight = 0, toWidth = 0;

            switch (settings.currentFromSize)
            {
                case Types.allScreenSizes.X:
                    if (settings.currentPartner == Types.compatiblePartners.Smartface)
                        fromHeight = (int)Types.iOSScreenSizes.XHeight;
                    else
                        fromHeight = 480;
                    fromWidth = (int)Types.iOSScreenSizes.XWidth;
                    break;
                case Types.allScreenSizes.X2:
                    if (settings.currentPartner == Types.compatiblePartners.Smartface)
                        fromHeight = (int)Types.iOSScreenSizes.X2Height;
                    else
                        fromHeight = 1136;
                    fromWidth = (int)Types.iOSScreenSizes.X2Width;
                    break;
                case Types.allScreenSizes.X3:
                    fromHeight = (int)Types.iOSScreenSizes.X3Height;
                    fromWidth = (int)Types.iOSScreenSizes.X3Width;
                    break;
                case Types.allScreenSizes.MDPI:
                    if (settings.currentPartner == Types.compatiblePartners.Smartface)
                        fromHeight = (int)Types.AndroidScreenSizes.MDPIHeight;
                    else
                        fromHeight = 480;
                    fromWidth = (int)Types.AndroidScreenSizes.MDPIWidth;
                    break;
                case Types.allScreenSizes.HDPI:
                    fromHeight = (int)Types.AndroidScreenSizes.HDPIHeight;
                    fromWidth = (int)Types.AndroidScreenSizes.HDPIWidth;
                    break;
                case Types.allScreenSizes.XHDPI:
                    fromHeight = (int)Types.AndroidScreenSizes.XHDPIHeight;
                    fromWidth = (int)Types.AndroidScreenSizes.XHDPIWidth;
                    break;
                case Types.allScreenSizes.XXHDPI:
                    fromHeight = (int)Types.AndroidScreenSizes.XXHDPIHeight;
                    fromWidth = (int)Types.AndroidScreenSizes.XXHDPIWidth;
                    break;
            }

            switch (settings.currentToSize)
            {
                case Types.allScreenSizes.X:
                    if (settings.currentPartner == Types.compatiblePartners.Smartface)
                        toHeight = (int)Types.iOSScreenSizes.XHeight;
                    else
                        toHeight = 480;
                    toWidth = (int)Types.iOSScreenSizes.XWidth;
                    break;
                case Types.allScreenSizes.X2:
                    if (settings.currentPartner == Types.compatiblePartners.Smartface)
                        toHeight = (int)Types.iOSScreenSizes.X2Height;
                    else
                        toHeight = 1136;
                    toWidth = (int)Types.iOSScreenSizes.X2Width;
                    break;
                case Types.allScreenSizes.X3:
                    toHeight = (int)Types.iOSScreenSizes.X3Height;
                    toWidth = (int)Types.iOSScreenSizes.X3Width;
                    break;
                case Types.allScreenSizes.MDPI:
                    if (settings.currentPartner == Types.compatiblePartners.Smartface)
                        toHeight = (int)Types.AndroidScreenSizes.MDPIHeight;
                    else
                        toHeight = 480;
                    toWidth = (int)Types.AndroidScreenSizes.MDPIWidth;
                    break;
                case Types.allScreenSizes.HDPI:
                    toHeight = (int)Types.AndroidScreenSizes.HDPIHeight;
                    toWidth = (int)Types.AndroidScreenSizes.HDPIWidth;
                    break;
                case Types.allScreenSizes.XHDPI:
                    toHeight = (int)Types.AndroidScreenSizes.XHDPIHeight;
                    toWidth = (int)Types.AndroidScreenSizes.XHDPIWidth;
                    break;
                case Types.allScreenSizes.XXHDPI:
                    toHeight = (int)Types.AndroidScreenSizes.XXHDPIHeight;
                    toWidth = (int)Types.AndroidScreenSizes.XXHDPIWidth;
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

            switch (settings.currentQuality)
            {
                case Types.imageQuality.High:
                default:
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.SmoothingMode = SmoothingMode.HighQuality;
                    grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    grPhoto.CompositingQuality = CompositingQuality.HighQuality;

                    break;
                case Types.imageQuality.Performance:
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

        public static bool ResizeAndSaveImage(Models.SaveFile sf, Models.Settings settings)
        {
            try
            {
                var file = resizeImage(sf.FilePath, settings);

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