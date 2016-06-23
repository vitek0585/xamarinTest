using Android.Content;
using Android.Content.PM;
using Xamarin.Forms;
using XamarinForm.Droid.Resources.layout;

namespace XamarinForm.Droid
{
    public class App : Application
    {
        public App()
        {
        }

        public App(Context baseContext, PackageManager packageManager)
        {
            MainPage = new MainPage(baseContext,packageManager);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
