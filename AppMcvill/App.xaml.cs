namespace AppMcvill
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Preferences.Clear();
            MainPage = new AppShell();

        }

    }
}