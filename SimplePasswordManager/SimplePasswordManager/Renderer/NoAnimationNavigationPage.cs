using System.Threading.Tasks;
using Xamarin.Forms;

namespace SimplePasswordManager.Renderer
{
    public class NoAnimationNavigationPage : NavigationPage
    {
        public NoAnimationNavigationPage(ContentPage page) : base(page)
        {
        }

        public new Task PushAsync(Page page)
        {
            return base.PushAsync(new NavigationPageRoot((ContentPage)page));
        }
    }
}
