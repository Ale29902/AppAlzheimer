using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TuApp.Styles
{
    public partial class CustomAlertDialog : ContentView
    {
        private TaskCompletionSource<bool> tcs;
        private TaskCompletionSource<string> confirmationTcs;

        public CustomAlertDialog()
        {
            InitializeComponent();
        }

        public Task ShowAsync(string title, string message, string buttonText)
        {
            tcs = new TaskCompletionSource<bool>();

            // Configurar el contenido
            TitleLabel.Text = title;
            MessageLabel.Text = message;
            ActionButton.Text = buttonText;

            // Ocultar el bot�n de cancelar si estuviera visible
            CancelButton.IsVisible = false;

            // Mostrar el di�logo
            IsVisible = true;

            return tcs.Task;
        }

        private void OnButtonClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;

            // Si estamos en modo confirmaci�n
            if (confirmationTcs != null && !confirmationTcs.Task.IsCompleted)
            {
                confirmationTcs.TrySetResult(ActionButton.Text);
                confirmationTcs = null;
            }
            else
            {
                // Modo normal
                tcs?.TrySetResult(true);
            }
        }

        private void OnCancelButtonClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;

            // Solo act�a en modo confirmaci�n
            if (confirmationTcs != null && !confirmationTcs.Task.IsCompleted)
            {
                confirmationTcs.TrySetResult(CancelButton.Text);
                confirmationTcs = null;
            }
        }

        public Task<string> ShowWithConfirmationAsync(string title, string message, string acceptButtonText, string cancelButtonText)
        {
            confirmationTcs = new TaskCompletionSource<string>();

            // Configurar el di�logo
            TitleLabel.Text = title;
            MessageLabel.Text = message;
            ActionButton.Text = acceptButtonText;
            CancelButton.Text = cancelButtonText;

            // Mostrar bot�n de cancelar para este tipo de di�logo
            CancelButton.IsVisible = true;

            // Mostrar el di�logo
            IsVisible = true;

            // Devolver la tarea que se completar� cuando se pulse un bot�n
            return confirmationTcs.Task;
        }
    }
}