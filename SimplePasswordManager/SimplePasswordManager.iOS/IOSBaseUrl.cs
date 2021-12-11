using Foundation;
using Xamarin.Forms;
using SimplePasswordManager.PlatformDependent;
using SimplePasswordManager.iOS;

[assembly: Dependency(typeof(IOSBaseUrl))]
namespace SimplePasswordManager.iOS
{
    public class IOSBaseUrl : IBaseUrl
    {
        public string Get()
        {
            return NSBundle.MainBundle.BundlePath;
        }
    }
}