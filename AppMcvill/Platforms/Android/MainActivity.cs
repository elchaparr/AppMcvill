using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Color = Android.Graphics.Color; // ← esta línea es clave

namespace AppMcvill
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Cambiar color de la barra de estado
            Window.SetStatusBarColor(Color.ParseColor("#FF0C5896")); // Azul
        }
    }
}