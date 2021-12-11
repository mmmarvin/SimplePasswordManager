using Xamarin.Essentials;

namespace SimplePasswordManager
{
    public static class Settings
    {
        public static bool FirstRun
        {
            get
            {
                return Preferences.Get(Constant.PREF_FIRST_RUN, true);
            }

            set
            {
                Preferences.Set(Constant.PREF_FIRST_RUN, value);
            }
        }
    }
}
