using CommunityToolkit.Maui.Views;
using TuApp.Entidades;
using TuApp.VistasModelo;
using TuApp.Styles; // A�adido para importar los custom dialogs

namespace TuApp.Vistas.VistasJuegoCuidador;
public partial class PacientesJuego : ContentPage
{
    private readonly JuegoViewModel viewModel;
    private readonly JuegoCuidador juego;

    // A�adir el di�logo personalizado
    private CustomAlertDialog customAlertDialog;

    public PacientesJuego(JuegoCuidador juego, JuegoViewModel viewModel)
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

        this.viewModel = viewModel;
        // Buscar la instancia actualizada del juego desde el ViewModel
        this.juego = viewModel.ListaJuegosCuidador.FirstOrDefault(j => j.idJuego == juego.idJuego);
        BindingContext = this.juego;
    }

    private async void AgregarPaciente_Clicked(object sender, EventArgs e)
    {
        var popup = new InsertarPacienteJuegoPopup();
        var paciente = await this.ShowPopupAsync(popup) as Usuario;
        if (paciente != null)
        {
            // Verificar si ya existe
            if (juego.pacientes.Any(p => p.id_Usuario == paciente.IdUsuario))
            {
                // Reemplazar DisplayAlert con customAlertDialog
                await customAlertDialog.ShowAsync("Ya existe", "Este paciente ya est� asignado al juego.", "Aceptar");
                return;
            }
            else
            {
                this.juego.pacientes.Add(new PacienteAsignado
                {
                    id_Usuario = paciente.IdUsuario,
                    nombre = paciente.Nombre
                });
            }

            // Llama al ViewModel que ya fue pasado
            await viewModel.InsertarRelacionJuego(juego.idJuego, paciente.IdUsuario);
            // La recarga ya est� impl�cita en InsertarRelacionJuego (hace await CargarJuegos)
        }
    }

    private async void VerPuntajes_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is PacienteAsignado paciente)
        {
            int idPaciente = paciente.id_Usuario; // Ajusta si tu propiedad se llama distinto
            int idJuego = juego.idJuego;
            // Abre el popup
            var popup = new PuntajesPopup(idPaciente, idJuego);
            await Application.Current.MainPage.ShowPopupAsync(popup);
        }
    }
}
