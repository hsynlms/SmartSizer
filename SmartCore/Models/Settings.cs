namespace SmartCore.Models
{
    public class Settings
    {
        public Types.allScreenSizes currentFromSize { get; set; }
        public Types.allScreenSizes currentToSize { get; set; }
        public Types.imageQuality currentQuality { get; set; }
        public Types.compatiblePartners currentPartner { get; set; }
    }
}
