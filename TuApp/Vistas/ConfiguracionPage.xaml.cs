namespace TuApp.Vistas;

public partial class ConfiguracionPage : ContentPage
{
    public ConfiguracionPage()
    {
        InitializeComponent();
    }

    private async void ActualizarContrasena_Clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina para actualizar contrase�a
        await DisplayAlert("Acci�n", "Actualizar contrase�a", "OK");
    }

    private async void EditarPin_Clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina para definir/editar PIN
        await DisplayAlert("Acci�n", "Editar PIN", "OK");
    }

    private async void EliminarPin_Clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina para eliminar PIN
        await DisplayAlert("Acci�n", "Eliminar PIN", "OK");
    }

    private async void RelacionPaciente_Clicked(object sender, EventArgs e)
    {
        // Navegar a la p�gina para relaci�n paciente-cuidador
        await DisplayAlert("Acci�n", "Relaci�n Paciente", "OK");
    }
}
