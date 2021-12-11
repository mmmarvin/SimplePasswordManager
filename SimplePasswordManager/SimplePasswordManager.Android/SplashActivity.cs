using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace SimplePasswordManager.Droid
{
    [Activity(Theme = "@style/Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startup_work = new Task(() => { Startup(); });
            startup_work.Start();
        }

        public override void OnBackPressed() 
        {
        }

        private void Startup()
        {
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}