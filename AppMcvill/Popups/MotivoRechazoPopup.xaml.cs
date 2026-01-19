using CommunityToolkit.Maui.Views;

namespace AppMcvill.Popups;

public partial class MotivoRechazoPopup : Popup
{
    public string MotivoSeleccionado { get; private set; }

    public MotivoRechazoPopup()
    {
        InitializeComponent();
    }

    private void OnChecked(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            MotivoSeleccionado = ((RadioButton)sender).Content.ToString();
        }
    }

    private void OnAccept(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(MotivoSeleccionado))
        {
            Application.Current.MainPage.DisplayAlert(
                "Error", "Seleccione un motivo", "OK");
            return;
        }

        Close(MotivoSeleccionado);
    }

    private void OnCancel(object sender, EventArgs e)
    {
        Close();
    }
}
