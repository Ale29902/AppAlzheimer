using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;
using CommunityToolkit.Maui.Storage;
using System.IO;
using SkiaSharp;
using CommunityToolkit.Maui.Views;
using TuApp.Styles; // A�adido para importar los custom dialogs

namespace TuApp.Vistas;
public partial class DetalleJuego : ContentPage
{
    private PreguntaViewModel viewModel;
    private JuegoCuidador juego;

    // A�adir el di�logo personalizado
    private CustomAlertDialog customAlertDialog;

    public DetalleJuego(JuegoCuidador juego)
    {
        InitializeComponent();

        // Crear el di�logo personalizado
        customAlertDialog = new CustomAlertDialog();

        // Guardar el contenido original
        var originalContent = Content;

        // Crear una nueva grid para contener todo
        var mainGrid = new Grid();

        // A�adir el contenido original
        mainGrid.Children.Add(originalContent);

        // A�adir el di�logo (inicialmente invisible)
        mainGrid.Children.Add(customAlertDialog);

        // Establecer la grid como el nuevo contenido
        Content = mainGrid;

        try
        {
            viewModel = BindingContext as PreguntaViewModel;
            this.juego = juego;
            CargarDatos(juego);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cargando DetalleJuego: {ex.Message}");
        }
    }

    private async void CargarDatos(JuegoCuidador juegocuidador)
    {
        await viewModel.CargarPreguntas(juegocuidador.idJuego);
        // Si no hay preguntas, muestra una alerta
        if (viewModel.ListaPregunta == null || !viewModel.ListaPregunta.Any())
        {
            // Reemplazar DisplayAlert con customAlertDialog
            await customAlertDialog.ShowAsync("Atenci�n", "No se encontraron preguntas para este juego.", "Aceptar");
        }
    }

    private async void Juego_Tapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Pregunta pregunta)
        {
            await Navigation.PushAsync(new DetallePregunta(pregunta));
        }
    }

    private async void AgregarPregunta_Clicked(object sender, EventArgs e)
    {
        var popup = new InsertarPreguntaPopup();
        var pregunta = await this.ShowPopupAsync(popup) as Pregunta;
        if (pregunta != null)
        {
            int idJuego = juego.idJuego;
            int idUsuario = SesionActiva.sesionActiva.usuario.IdUsuario;
            await Navigation.PushAsync(new DetallePregunta(pregunta, viewModel, idJuego, idUsuario));
        }
    }
}