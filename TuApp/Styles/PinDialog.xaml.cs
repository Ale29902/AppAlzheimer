using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TuApp.Styles
{
    public partial class PinDialog : ContentView
    {
        private TaskCompletionSource<string> tcs;

        public PinDialog()
        {
            InitializeComponent();
        }

        public Task<string> ShowAsync()
        {
            tcs = new TaskCompletionSource<string>();
            // Limpiar entrada anterior
            PinEntry.Text = string.Empty;
            // Mostrar el diálogo
            IsVisible = true;
            return tcs.Task;
        }

        private void OnAcceptClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;
            tcs?.TrySetResult(PinEntry.Text);
        }

        private void OnCancelClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;
            tcs?.TrySetResult(null);
        }

        // Método simple para mostrar alertas usando el sistema estándar
        public Task ShowCustomAlert(string title, string message, string buttonText)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, buttonText);
        }
    }
}