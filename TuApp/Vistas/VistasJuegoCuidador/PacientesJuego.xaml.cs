using CommunityToolkit.Maui.Views;
using TuApp.Entidades;
using TuApp.VistasModelo;
namespace TuApp.Vistas.VistasJuegoCuidador;
public partial class PacientesJuego : ContentPage
{
    private readonly JuegoViewModel viewModel;
    private readonly JuegoCuidador juego;
    public PacientesJuego(JuegoCuidador juego, JuegoViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        // Buscar la instancia actualizada del juego desde el ViewModel
        this.juego = viewModel.ListaJuegos.FirstOrDefault(j => j.idJuego == juego.idJuego);
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
                await DisplayAlert("Ya existe", "Este paciente ya está asignado al juego.", "Aceptar");
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
            // La recarga ya está implícita en InsertarRelacionJuego (hace await CargarJuegos)
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