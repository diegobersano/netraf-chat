using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Toasts;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace SignalRChat.Droid
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);

            DependencyService.Register<ToastNotification>();
            ToastNotification.Init(this);

            LoadApplication(new App());
        }
    }
}