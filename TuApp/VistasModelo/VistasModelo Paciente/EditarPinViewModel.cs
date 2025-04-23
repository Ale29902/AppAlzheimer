using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;
using TuApp.Vistas;
using TuApp.Styles;
namespace TuApp.ViewModels
{
    public class EditarPinPacienteViewModel : INotifyPropertyChanged
    {
        private string _pinActual;
        private string _nuevoPin;
        private bool _isLoading;
        private string _errorMessage;

        public ICommand RegresarInicioCommand { get; private set; }
        public ICommand ActualizarPinCommand { get; private set; }

        public string PinActual
        {
            get => _pinActual;
            set
            {
                _pinActual = value;
                OnPropertyChanged();
            }
        }

        public string NuevoPin
        {
            get => _nuevoPin;
            set
            {
                _nuevoPin = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private readonly INavigation _navigation;
        private readonly Page _page;
        private readonly CustomAlertDialog _customAlertDialog;
        private readonly NoChangesDialog _nochangesDialog;
        private readonly PinDialog _pinDialog;
        public EditarPinPacienteViewModel(INavigation navigation, Page page, CustomAlertDialog customAlertDialog, NoChangesDialog nochangesDialog, PinDialog pinDialog)
        {
            _navigation = navigation;
            _page = page;
            _customAlertDialog = customAlertDialog;
            _nochangesDialog = nochangesDialog;
            _pinDialog = pinDialog;

            RegresarInicioCommand = new Command(async () => await RegresarInicio());
            ActualizarPinCommand = new Command(async () => await ActualizarPin());
        }

     

        private async Task RegresarInicio()
        {
            await _navigation.PushAsync(new InicioPaciente());
        }

        private async Task ActualizarPin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(PinActual) || string.IsNullOrWhiteSpace(NuevoPin))
                {
                    await _customAlertDialog.ShowAsync("Campos requeridos", "Debe ingresar su PIN actual y el nuevo PIN", "Aceptar");
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                var req = new
                {
                    IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario,
                    PinActual = PinActual.Trim(),
                    NuevoCodigo = NuevoPin.Trim()
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(req),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage respuestaHttp;
                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync("https://localhost:44347/api/usuario/actualizarping", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResActualizarPinPaciente>(responseContent);

                    if (res != null && res.resultado)
                    {
                        SesionActiva.sesionActiva.usuario.pin.Codigo = NuevoPin;
                        await _customAlertDialog.ShowAsync("Actualización correcta", "PIN actualizado correctamente", "Aceptar");
                        await _navigation.PushAsync(new InicioPaciente());
                    }
                    else
                    {
                        // También podrías usar PinDialog si quieres algo más visual
                        await _customAlertDialog.ShowAsync("Error", "PIN actual incorrecto", "Aceptar");
                        // await _pinDialog.ShowAsync("PIN actual incorrecto"); // si lo tienes implementado
                    }
                }
                else
                {
                    string errorContent = await respuestaHttp.Content.ReadAsStringAsync();
                    string errorMessage = $"Error de conexión: {respuestaHttp.StatusCode}";
                    if (!string.IsNullOrEmpty(errorContent))
                    {
                        errorMessage += $"\nDetalle: {errorContent}";
                    }
                    await _customAlertDialog.ShowAsync("Error de conexión", errorMessage, "Aceptar");
                }
            }
            catch (Exception ex)
            {
                await _customAlertDialog.ShowAsync("Error", $"Ocurrió un error: {ex.Message}", "Aceptar");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}