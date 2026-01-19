using CommunityToolkit.Maui.Views;
//using PassKit;
using AppMcvill.Data;
using AppMcvill.Models;
using Microsoft.EntityFrameworkCore;
namespace AppMcvill.Popups;

public partial class DetallesAceptadas : Popup
{
    public List<Requisicion> Detalles { get; set; }
    private readonly Action _onCloseCallback;
    public DetallesAceptadas(string material, string descripcion, string cantidad, string id, string usuario, string linea, string um, Action onCloseCallback)
    {
        InitializeComponent();
        _onCloseCallback = onCloseCallback;
        MaterialLabel.Text = material;
        DescripcionLabel.Text = descripcion;
        //CantidadLabel.Text = cantidad;
        IdLabel.Text = id;
        Linealabel.Text = linea;
        UsuarioLabel.Text = usuario;
        UMLabel.Text = um;

        int idRequisicion = int.Parse(id);

        using (var db = new DBconection())
        {
            Detalles = db.Requisicion_Prueba
                .Where(r => r.IdRequi == idRequisicion)
                .Where(r=>r.Linea==linea)
                .ToList();
        }

        BindingContext = this;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }

}