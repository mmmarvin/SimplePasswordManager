using Xamarin.Forms;

namespace SimplePasswordManager
{
    public static class Constant
    {
        public static readonly string APP_NAME = "PasswordManager";
        public static string APP_OS
        {
            get
            {
                return Device.RuntimePlatform;
            }
        }

        public static readonly int APP_VERSION_MAJOR = 1;
        public static readonly int APP_VERSION_MINOR = 0;
        public static readonly int APP_VERSION_PATCH = 0;
        public static string APP_VERSION_STRING
        {
            get
            {
                return APP_VERSION_MAJOR.ToString() + "." + APP_VERSION_MINOR.ToString() + APP_VERSION_PATCH.ToString();
            }
        }
        public static readonly string APP_NAME_AND_VERSION = APP_NAME + " v." + APP_VERSION_STRING;

        public static readonly string PREF_FIRST_RUN = "settings_first_run";
    }
}
