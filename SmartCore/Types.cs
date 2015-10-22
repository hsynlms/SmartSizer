namespace SmartCore
{
    public class Types
    {
        public enum imageQuality : int { High, Performance };
        public static string[] includedQuality = { "High Quality", "Good Performance" };

        public enum supportedOS : int { Android, iOS };
        public static string[] includedOS = { "Android", "iOS" };

        public enum supportediOSScreenSize : int { X, X2, X3 };
        public static string[] includediOSScreenSize = { "X", "2X", "3X" };

        public enum supportedAndroidScreenSize : int { MDPI, HDPI, XHDPI, XXHDPI };
        public static string[] includedAndroidScreenSize = { "mdpi", "hdpi", "xhdpi", "xxhdpi" };

        public enum allScreenSizes : int { X, X2, X3, MDPI, HDPI, XHDPI, XXHDPI }

        public enum compatiblePartners : int { Native, Smartface }

        public enum iOSScreenSizes : int { XHeight = 569, XWidth = 320, X2Height = 1138, X2Width = 640, X3Height = 2208, X3Width = 1242 }
        public enum AndroidScreenSizes : int { MDPIHeight = 569, MDPIWidth = 320, HDPIHeight = 853, HDPIWidth = 480, XHDPIHeight = 1280, XHDPIWidth = 720, XXHDPIHeight = 1920, XXHDPIWidth = 1080 }
    }
}