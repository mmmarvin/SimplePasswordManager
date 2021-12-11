using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Pages;
using SimplePasswordManager.Renderer;

namespace SimplePasswordManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var navigation_page = new NoAnimationNavigationPage(new MainPage());
            MainPage = navigation_page;
            Global.NavigationPage = navigation_page;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
