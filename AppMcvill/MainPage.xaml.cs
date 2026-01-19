using AppMcvill.Data;
using AppMcvill.Models;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using AppMcvill.Resources.Strings;
using System.Threading.Tasks;

namespace AppMcvill
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string usuario = UsernameEntry.Text;
            string contrasena = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                await DisplayAlert(AppResources.ErrorTitle,
                                   AppResources.EmptyFields,
                                   AppResources.Ok);
                return;
            }

            using (var db = new DBconection())
            {
                // 1. Buscar usuario por nombre solamente
                var usuarioValido = db.UsuariosApp
                    .FirstOrDefault(u => u.Usuario == usuario);

                if (usuarioValido != null)
                {
                    // 2. Verificar la contraseña usando BCrypt
                    bool passwordCorrecta = BCrypt.Net.BCrypt.Verify(contrasena, usuarioValido.Contraseña);

                    if (passwordCorrecta)
                    {
                        // 3. Iniciar sesión
                        SesionUsuario.UsuarioActual = usuarioValido.Usuario;
                        SesionUsuario.TelefonoActual = "+5218713337397";

                        await Shell.Current.GoToAsync("//CollectionA");

                        UsernameEntry.Text = "";
                        PasswordEntry.Text = "";
                        return;
                    }
                }

                // 4. Si no existe o contraseña incorrecta
                await DisplayAlert(AppResources.AccessDeniedTitle,
                                   AppResources.InvalidUserPassword,
                                   AppResources.Ok);
            }
        }


        private bool isPasswordVisible = false;

        private void OnTogglePasswordVisibility(object sender, TappedEventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;

            PasswordEntry.IsPassword = !isPasswordVisible;
            PasswordToggleIcon.Source = isPasswordVisible ? "eye_solid.svg" : "eye_slash.svg";
        }

        private async void CrearUsuCon(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NuevoUsuario());
        }

        private bool isEnglish = false;

        private void IdiomaChange(object sender, TappedEventArgs e)
        {
            isEnglish = !isEnglish;

            if (isEnglish)
                SetIdioma("en");
            else
                SetIdioma("es");

            // Refrescar textos
            LoginTitleLabel.Text = AppResources.LoginTitle;
            UsernameEntry.Placeholder = AppResources.Username;
            PasswordEntry.Placeholder = AppResources.Password;
            LoginButton.Text = AppResources.LoginButton;

            ActualizarTitulosShell();
        }

        private void SetIdioma(string idioma)
        {
            var culture = new CultureInfo(idioma);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        private void ActualizarTitulosShell()
        {
            foreach (var item in Shell.Current.Items)
            {
                if (item is TabBar tabBar)
                {
                    foreach (var shellContent in tabBar.Items.OfType<ShellContent>())
                    {
                        switch (shellContent.Route)
                        {
                            case "CollectionA":
                                shellContent.Title = AppResources.Shell_SecondScreen;
                                break;

                            case "CollectionAPage":
                                shellContent.Title = AppResources.Shell_Requisitions;
                                break;

                            case "CollectionBPage":
                                shellContent.Title = AppResources.Shell_CollectionB;
                                break;

                            case "CollectionCPage":
                                shellContent.Title = AppResources.Shell_CollectionC;
                                break;
                        }
                    }
                }
            }
        }

    }
}
