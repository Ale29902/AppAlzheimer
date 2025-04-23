namespace TuApp.Vistas;

public partial class ConfiguracionPacientePage : ContentPage
{

	public ConfiguracionPacientePage()
	{
		InitializeComponent();
	}
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new InicioPaciente());
    }
    
    private async void GenerarPin_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new GenerarPinPacientePage());
    }
    private async void EditarPin_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new EditarPinPacientePage());
    }

    private async void EliminarPin_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new EliminarPinPacientePage());
    }

    private async void RelacionCuidador_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new RelacionCuidadorPage());
    }
}