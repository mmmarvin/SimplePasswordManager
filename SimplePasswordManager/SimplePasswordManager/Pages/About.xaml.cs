using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.PlatformDependent;

namespace SimplePasswordManager.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();
            c_LicenseView.Source = DependencyService.Get<IBaseUrl>().Get() + "LICENSE.txt";
        }
    }
}