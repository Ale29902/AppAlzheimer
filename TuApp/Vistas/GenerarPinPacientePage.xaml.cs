using TuApp.ViewModels;
using TuApp.Styles;
namespace TuApp.Vistas;

public partial class GenerarPinPacientePage : ContentPage
{
    private GenerarPinPacienteViewModel _viewModel;
    private PinDialog pinDialog;
    private CustomAlertDialog customAlertDialog;
    private NoChangesDialog noChangesDialog;
    public GenerarPinPacientePage()
    {
        InitializeComponent();

        pinDialog = new PinDialog();
        customAlertDialog = new CustomAlertDialog();
        noChangesDialog = new NoChangesDialog();

        var originalContent = Content;
        var mainGrid = new Grid();
        mainGrid.Children.Add(originalContent);
        mainGrid.Children.Add(pinDialog);
        mainGrid.Children.Add(customAlertDialog);
        mainGrid.Children.Add(noChangesDialog);

        Content = mainGrid;

        _viewModel = new GenerarPinPacienteViewModel(Navigation, this, customAlertDialog, noChangesDialog, pinDialog);
        BindingContext = _viewModel;
    }

    // Manteniendo los manejadores de eventos originales para compatibilidad con el XAML
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }

    private void GenerarPin_Clicked(object sender, EventArgs e)
    {
        // Actualizar la propiedad del ViewModel con el valor actual del Entry
        _viewModel.Pin = PinEntry.Text;

        // Ejecutar el comando directamente
        if (_viewModel.GenerarPinCommand.CanExecute(null))
            _viewModel.GenerarPinCommand.Execute(null);
    }
}