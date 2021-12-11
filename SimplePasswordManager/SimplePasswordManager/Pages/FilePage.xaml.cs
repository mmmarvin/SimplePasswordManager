using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.PlatformDependent;

namespace SimplePasswordManager.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilePage : ContentPage
    {
        public FilePage(bool local, string url)
        {
            InitializeComponent();

            if(local)
            {
                c_WebView.Source = DependencyService.Get<IBaseUrl>().Get() + url;
            }
            else
            {
                c_WebView.Source = url;
            }
        }
    }
}