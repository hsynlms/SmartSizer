using System;

namespace SmartSizer
{
    public class Methods
    {
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return Math.Round((bytes / 1024d) / 1024d, 2);
        }

        public static double ConvertBytesToKilobytes(long bytes)
        {
            return Math.Round(bytes / 1024d, 2);
        }

        public static void SaveSetting(string key, object value)
        {
            switch (key)
            {
                case "ios_suffix":
                    Properties.Settings.Default.ios_suffix = (bool)value;
                    break;
                case "folders":
                    Properties.Settings.Default.folders = (bool)value;
                    break;
                case "ratio":
                    Properties.Settings.Default.ratio = (bool)value;
                    break;
                case "smartface":
                    Properties.Settings.Default.smartface = (bool)value;
                    break;
                case "sub_folders":
                    Properties.Settings.Default.sub_folders = (bool)value;
                    break;
                case "skip_first":
                    Properties.Settings.Default.skip_first = (bool)value;
                    break;
                case "quality":
                    Properties.Settings.Default.quality = (int)value;
                    break;
            }

            Properties.Settings.Default.Save();
        }

        public static object GetSetting(string key)
        {
            return Properties.Settings.Default[key];
        }
    }
}
