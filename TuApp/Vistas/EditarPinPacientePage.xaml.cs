using Newtonsoft.Json;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;
using TuApp.ViewModels;
using TuApp.Styles;
using CommunityToolkit.Mvvm.Input;



namespace TuApp.Vistas;

public partial class EditarPinPacientePage : ContentPage
{
    private PinDialog pinDialog;
    private CustomAlertDialog customAlertDialog;
    private NoChangesDialog noChangesDialog;
    private EditarPinPacienteViewModel _viewModel;

    public EditarPinPacientePage()
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

        _viewModel = new EditarPinPacienteViewModel(Navigation, this, customAlertDialog, noChangesDialog, pinDialog);

        BindingContext = _viewModel;
    }

    // Keeping the original event handlers to maintain compatibility with the XAML
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }

    private void ActualizarPin_Clicked(object sender, EventArgs e)
    {
        // Update ViewModel properties with current Entry values
        _viewModel.PinActual = PinActualEntry.Text;
        _viewModel.NuevoPin = NuevoPinEntry.Text;

        // Execute the command directly
        if (_viewModel.ActualizarPinCommand.CanExecute(null))
            _viewModel.ActualizarPinCommand.Execute(null);
    }
}