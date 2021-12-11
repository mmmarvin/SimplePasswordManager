using Xamarin.Forms;
using SimplePasswordManager.PlatformDependent;
using SimplePasswordManager.Droid;

[assembly: Dependency(typeof(AndroidBaseUrl))]
namespace SimplePasswordManager.Droid
{
    public class AndroidBaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "file:///android_asset/";
        }
    }
}