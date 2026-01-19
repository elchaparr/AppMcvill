using AppMcvill.Resources.Strings;

namespace AppMcvill
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // RUTAS NORMALES DEL SISTEMA
            //Routing.RegisterRoute("MainPage", typeof(MainPage));
            //Routing.RegisterRoute("CollectionA", typeof(SegundaPantalla));
            //Routing.RegisterRoute("ReqA", typeof(CollectionAPage));
            //Routing.RegisterRoute("ReqB", typeof(CollectionBPage));
            //Routing.RegisterRoute("ReqC", typeof(CollectionCPage));
            Routing.RegisterRoute("ModNue", typeof(ModuloNuevo));
            Routing.RegisterRoute(nameof(FilterSearch), typeof(FilterSearch));
            Routing.RegisterRoute("ModNue2", typeof(ModuloNuevo2));
        }

        private async void OnToggleRequisiciones(object sender, EventArgs e)
        {
            if (!ReqSubMenu.IsVisible)
            {
                // Mostrar animación
                ReqSubMenu.Opacity = 0;
                ReqSubMenu.TranslationY = -10;
                ReqSubMenu.IsVisible = true;

                await Task.WhenAll(
                    ReqSubMenu.FadeTo(1, 350, Easing.SinOut),            // más suave
                    ReqSubMenu.TranslateTo(0, 0, 350, Easing.SinOut)     // baja suave
                );
            }
            else
            {
                // Ocultar animación
                await Task.WhenAll(
                    ReqSubMenu.FadeTo(0, 200, Easing.CubicIn),
                    ReqSubMenu.TranslateTo(0, -10, 200, Easing.CubicIn)
                );

                ReqSubMenu.IsVisible = false;
            }
        }


        // NAVEGACIÓN DESDE EL MENÚ
        private async void OnNavigateCollection(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CollectionA");
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnNavigateReqA(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ReqA");
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnNavigateReqB(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ReqB");
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnNavigateReqC(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ReqC");
            Shell.Current.FlyoutIsPresented = false;
        }
        ///-----------------------------------------------------///////
        private void OnToggleOtrosModulos(object sender, EventArgs e)
        {
            ReqSubMenuu.IsVisible = !ReqSubMenuu.IsVisible;

            // Cambiar la flecha para abrir/cerrar
            ReqButtonn.Text = ReqSubMenuu.IsVisible ?
                "Proximos modulos" :
                "Proximos modulos";
        }
        
        private async void OnNavigateModNu(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ModNue");
            Shell.Current.FlyoutIsPresented = false;
        }

        private async void OnNavigateModNu2(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ModNue2");
            Shell.Current.FlyoutIsPresented = false;
        }
        // CERRAR SESIÓN
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Cerrar sesión",
                "¿Estás seguro que deseas cerrar sesión?",
                "SI",
               "Cancelar");

            if (confirm)
            {
                Preferences.Clear();
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}
