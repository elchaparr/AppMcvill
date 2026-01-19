using AppMcvill.Data;
using AppMcvill.Models;
using AppMcvill.Popups;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using ClosedXML.Excel;
using System.IO;
using CommunityToolkit.Maui.Storage;

namespace AppMcvill;

public partial class CollectionCPage : ContentPage
{
    public ObservableCollection<RequisicionDetail> Requisiciones { get; set; }
    private bool FiltroActivo = false;

    public CollectionCPage()
    {
        InitializeComponent();
        Requisiciones = new ObservableCollection<RequisicionDetail>();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!FiltroActivo)
        {
            Requisiciones.Clear();
            CargarDatos();
        }
    }

    private void CargarDatos()
    {
        using (var db = new DBconection())
        {
            var datos = db.Requisicion_Detail
                .Where(r => r.Autorizada == "NO")
                .Select(r => new RequisicionDetail
                {
                    IdRequisicion = r.IdRequisicion,
                    Linea = r.Linea,
                    Material = r.Material,
                    Descripcion = r.Descripcion,
                    Cantidad = r.Cantidad,
                    UM = r.UM,
                    AutorizadoPor = r.AutorizadoPor,
                    FechaAutorizada = r.FechaAutorizada,
                    MotivoRechazo = r.MotivoRechazo
                }).OrderByDescending(r => r.FechaAutorizada)
                .ToList();

            foreach (var item in datos)
                Requisiciones.Add(item);
        }
    }

    private void OnBorderTappedc(object sender, EventArgs e)
    {
        var border = sender as Border;
        var item = border.BindingContext as RequisicionDetail;

        if (item != null)
        {
            string cantidadStr = item.Cantidad.ToString("N2");
            string idrequi = item.IdRequisicion.ToString();
            string lineast = item.Linea;
            // Abrir popup pasando los valores
            this.ShowPopup(new DetalleRechazadas(item.Material, item.Descripcion, cantidadStr, idrequi, item.AutorizadoPor, lineast, item.UM, () =>
            {
                Requisiciones.Clear(); // limpia
                CargarDatos();         // vuelve a cargar
            }, item.MotivoRechazo));
        }
    }

    private async void Filtros(object sender, EventArgs e)
    {
        var filtro = new RejectRequisitionFilter();

        filtro.OnSearchCompleted += (listaFiltrada) =>
        {
            FiltroActivo = true;

            Requisiciones.Clear();
            foreach (var item in listaFiltrada)
                Requisiciones.Add(item);
        };

        await Navigation.PushAsync(filtro);
    }

    private async void Excel(object sender, EventArgs e)
    {
        if (Requisiciones == null || Requisiciones.Count == 0)
        {
            await DisplayAlert("Aviso", "No hay datos para exportar", "OK");
            return;
        }

        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Requisiciones");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Id Requisición";
            worksheet.Cell(1, 2).Value = "Linea";
            worksheet.Cell(1, 3).Value = "Material";
            worksheet.Cell(1, 4).Value = "Descripción";
            worksheet.Cell(1, 5).Value = "Cantidad";
            worksheet.Cell(1, 6).Value = "UM";
            worksheet.Cell(1, 7).Value = "Autorizado Por";
            worksheet.Cell(1, 8).Value = "Motivo Rechazo";
            worksheet.Cell(1, 9).Value = "Fecha Autorizada";

            int fila = 2;
            foreach (var r in Requisiciones)
            {
                worksheet.Cell(fila, 1).Value = r.IdRequisicion;
                worksheet.Cell(fila, 2).Value = r.Linea;
                worksheet.Cell(fila, 3).Value = r.Material;
                worksheet.Cell(fila, 4).Value = r.Descripcion;
                worksheet.Cell(fila, 5).Value = r.Cantidad;
                worksheet.Cell(fila, 6).Value = r.UM;
                worksheet.Cell(fila, 7).Value = r.AutorizadoPor;
                worksheet.Cell(fila, 8).Value = r.MotivoRechazo;
                worksheet.Cell(fila, 9).Value = r.FechaAutorizada?.ToString("yyyy-MM-dd");
                fila++;
            }

            worksheet.Columns().AdjustToContents();

            // Guardar en memoria
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Abrir diálogo Guardar como
            var resultado = await FileSaver.Default.SaveAsync(
                "Requisiciones.xlsx",
                stream,
                CancellationToken.None
            );

            if (resultado.IsSuccessful)
                await DisplayAlert("Excel", "Archivo guardado correctamente", "OK");
            else
                await DisplayAlert("Excel", "No se guardó el archivo", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

}
