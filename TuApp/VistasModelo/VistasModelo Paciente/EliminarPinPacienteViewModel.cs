using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using TuApp.Entidades;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;
using TuApp.Vistas;
using TuApp.Styles;
using TuApp.Entidades.Entity;
namespace TuApp.ViewModels
{
    public class EliminarPinPacienteViewModel : INotifyPropertyChanged
    {
        private string _pin;
        private bool _isLoading;
        private string _errorMessage;

        public ICommand RegresarInicioCommand { get; private set; }
        public ICommand EliminarPinCommand { get; private set; }

        public string Pin
        {
            get => _pin;
            set
            {
                _pin = value;
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

        public EliminarPinPacienteViewModel(INavigation navigation, Page page, CustomAlertDialog customAlertDialog, NoChangesDialog nochangesDialog, PinDialog pinDialog)
        {
            _navigation = navigation;
            _page = page;
            _customAlertDialog = customAlertDialog;
            _nochangesDialog = nochangesDialog;
            _pinDialog = pinDialog;

            RegresarInicioCommand = new Command(async () => await RegresarInicio());
            EliminarPinCommand = new Command(async () => await EliminarPin());
        }

        private async Task RegresarInicio()
        {
            await _navigation.PushAsync(new InicioPaciente());
        }

        private async Task EliminarPin()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Pin))
                {
                    await _customAlertDialog.ShowAsync("Campos requeridos", "Debe ingresar el PIN a eliminar", "Aceptar");
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                ReqEliminarPinPaciente req = new ReqEliminarPinPaciente
                {
                    Codigo = Pin,
                    IdUsuario = SesionActiva.sesionActiva.usuario.IdUsuario
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(req),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage respuestaHttp;
                using (HttpClient httpClient = new HttpClient())
                {
                    respuestaHttp = await httpClient.PostAsync("https://localhost:44347/api/usuario/eliminarping", jsonContent);
                }

                if (respuestaHttp.IsSuccessStatusCode)
                {
                    var responseContent = await respuestaHttp.Content.ReadAsStringAsync();
                    var res = JsonConvert.DeserializeObject<ResEliminarPinPaciente>(responseContent);

                    if (res != null && res.resultado)
                    {
                        SesionActiva.sesionActiva.usuario.pin.Codigo = null;
                        await _customAlertDialog.ShowAsync("Eliminación correcta", "PIN eliminado correctamente", "Aceptar");
                        await _navigation.PushAsync(new InicioPaciente());
                    }
                    else
                    {
                        // Aquí podrías usar PinDialog para mostrar un PIN incorrecto con estilo
                        await _customAlertDialog.ShowAsync("PIN incorrecto", "El PIN ingresado es erróneo", "Aceptar");
                        // await _pinDialog.ShowAsync("Pin incorrecto"); // si lo configuras con ese método
                    }
                }
                else
                {
                    await _customAlertDialog.ShowAsync("Error de conexión", "No hay respuesta del servidor", "Aceptar");
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
