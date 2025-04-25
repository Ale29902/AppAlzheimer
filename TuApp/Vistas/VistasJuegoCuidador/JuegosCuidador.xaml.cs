using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp;
using TuApp.VistasModelo;
using TuApp.Vistas.VistasJuegoCuidador;
using TuApp.Styles; // A�adido para importar los custom dialogs

namespace TuApp.Vistas;
public partial class JuegosCuidador : ContentPage
{
    private readonly JuegoViewModel viewModel;

    // A�adir el di�logo personalizado
    private CustomAlertDialog customAlertDialog;
    private NoChangesDialog noChangesDialog; // Opcional: a�adir si se necesita este tipo de di�logo

    public JuegosCuidador()
    {
        InitializeComponent();

        // Crear los di�logos personalizados
        customAlertDialog = new CustomAlertDialog();
        noChangesDialog = new NoChangesDialog(); // Opcional: a�adir si se necesita este tipo de di�logo

        // Guardar el contenido original
        var originalContent = Content;

        // Crear una nueva grid para contener todo
        var mainGrid = new Grid();

        // A�adir el contenido original
        mainGrid.Children.Add(originalContent);

        // A�adir los di�logos (inicialmente invisibles)
        mainGrid.Children.Add(customAlertDialog);
        mainGrid.Children.Add(noChangesDialog); // Opcional: a�adir si se necesita este tipo de di�logo

        // Establecer la grid como el nuevo contenido
        Content = mainGrid;

        viewModel = new JuegoViewModel();
        BindingContext = viewModel;
    }

    private async void Juego_Tapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is JuegoCuidador juego)
        {
            await Navigation.PushAsync(new DetalleJuego(juego));
        }
    }

    private async void AgregarJuego_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as JuegoViewModel;
        var popup = new InsertarJuegoPopup(vm);
        var nuevoJuego = await this.ShowPopupAsync(popup) as JuegoCuidador;
        if (nuevoJuego != null && vm != null)
        {
            vm.ListaJuegosCuidador.Add(nuevoJuego);
        }
    }

    private async void EliminarJuego_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as JuegoViewModel;
        var button = sender as Button;
        if (button?.CommandParameter is int idJuego && vm != null)
        {
            // Aqu� podr�as a�adir una confirmaci�n con customAlertDialog si lo necesitas
            // Por ejemplo:
            var resultado = await customAlertDialog.ShowWithConfirmationAsync(
                "Confirmar",
                "�Est� seguro que desea eliminar este juego?",
                "S�", "No");

            if (resultado == "S�")
            {
                await vm.EliminarJuego(idJuego);
            }
        }
    }

    private async void AbrirPacientesAsignados_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var juego = button?.CommandParameter as JuegoCuidador;
        if (juego != null)
        {
            await Navigation.PushAsync(new PacientesJuego(juego, viewModel));
        }
    }
}