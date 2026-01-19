using AppMcvill.Resources.Strings;
using AppMcvill.Data;
using BCrypt.Net;
using AppMcvill.Models;
namespace AppMcvill;

public partial class NuevoUsuario : ContentPage
{
	public NuevoUsuario()
	{
		InitializeComponent();
	}
    private bool isPasswordVisible = false;
    private void OnTogglePasswordVisibility(object sender, TappedEventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;

        PasswordEntryy.IsPassword = !isPasswordVisible;
        PasswordToggleIcon.Source = isPasswordVisible ? "eyenu.svg" : "eyeslashnu.svg";
    }

    private async void CreateUser(object sender, EventArgs e)
    {
        string usuario = UsernameEntryy.Text;
        string contrasena = PasswordEntryy.Text;

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
        {
            await DisplayAlert("Error",
                               "Por favor introduzca un usuario y contraseña",
                               "Ok");
            return;
        }

        using (var db = new DBconection())
        {
            // 1. Verificar si el usuario ya existe
            var usuarioExistente = db.UsuariosApp
                .FirstOrDefault(u => u.Usuario == usuario);

            if (usuarioExistente != null)
            {
                await DisplayAlert("Error",
                                   "El usuario ya existe",
                                   "OK");
                return;
            }

            // 2. Crear hash seguro
            string hash = BCrypt.Net.BCrypt.HashPassword(contrasena);

            // 3. Crear un nuevo usuario
            var nuevoUsuario = new UsuarioApp
            {
                Usuario = usuario,
                Contraseña = hash
            };

            // 4. Guardar en base de datos
            db.UsuariosApp.Add(nuevoUsuario);
            await db.SaveChangesAsync();

            await DisplayAlert("Éxito",
                               "Usuario registrado correctamente",
                               "OK");

            UsernameEntryy.Text = "";
            PasswordEntryy.Text = "";
            await Navigation.PopModalAsync();
        }
    }

}