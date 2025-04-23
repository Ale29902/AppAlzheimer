using CommunityToolkit.Maui.Views;
using TuApp.Entidades;
using TuApp.ViewModels;

namespace TuApp;

public partial class InsertarPacienteJuegoPopup : Popup
{
    private readonly RelacionViewModel viewModel;

    public InsertarPacienteJuegoPopup()
    {
        InitializeComponent();
        viewModel = new RelacionViewModel();
        BindingContext = viewModel;
    }

    private void OnPacienteSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        var paciente = e.CurrentSelection.FirstOrDefault() as Usuario;

        if (paciente != null)
        {
            
            Close(paciente); // Devuelve el paciente seleccionado
        }

        ((CollectionView)sender).SelectedItem = null;
    }

    private void OnCerrarClicked(object sender, EventArgs e)
    {
        Close(null);
    }
}
