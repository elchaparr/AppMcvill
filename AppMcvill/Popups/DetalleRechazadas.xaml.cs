using CommunityToolkit.Maui.Views;
//using PassKit;
using AppMcvill.Data;
using AppMcvill.Models;
using Microsoft.EntityFrameworkCore;
namespace AppMcvill.Popups;

public partial class DetalleRechazadas : Popup
{
    public List<Requisicion> Detalles { get; set; }

    private readonly Action _onCloseCallback;
    public DetalleRechazadas(
    string material,
    string descripcion,
    string cantidad,
    string id,
    string usuario,
    string linea,
    string um,
    Action onCloseCallback,
    string motivorechazo)
    {
        InitializeComponent();

        _onCloseCallback = onCloseCallback;

        MaterialLabel.Text = material;
        DescripcionLabel.Text = descripcion;
        IdLabel.Text = id;
        Linealabel.Text = linea;
        UsuarioLabel.Text = usuario;
        UMLabel.Text = um;
        MotivoLabel.Text = motivorechazo;

        int idRequisicion = int.Parse(id);

        using (var db = new DBconection())
        {
            Detalles = db.Requisicion_Prueba
                .Where(r => r.IdRequi == idRequisicion)
                //.Where(r => r.Linea == linea)
                .ToList();
        }

        BindingContext = this;
    }


    private async void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }

}