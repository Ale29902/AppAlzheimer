using TuApp.Entidades.Entity;
namespace TuApp.Vistas;

public partial class InicioPaciente : ContentPage
{

	public InicioPaciente()
	{

		InitializeComponent();
        if (SesionActiva.sesionActiva != null && SesionActiva.sesionActiva.usuario != null)
        {
            lblBienvenida.Text = $"Bienvenido {SesionActiva.sesionActiva.usuario.Nombre}..!!";
        }
        else
        {
            lblBienvenida.Text = "Bienvenido..!!";
        }

    }

    private async void OnPerfilClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new PerfilPage());
    }

    private async void OnJuegosClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new JuegosPacientePage());
    }

    private async void OnEventosClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new EventosPacientePage());
    }

    private async void OnChatsClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new ChatsPacientePage());
    }
        
    private async void OnConfiguracionClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new ConfiguracionPacientePage());
    }

}