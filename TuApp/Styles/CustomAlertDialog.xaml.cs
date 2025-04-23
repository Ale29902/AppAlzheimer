using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TuApp.Styles
{
    public partial class CustomAlertDialog : ContentView
    {
        private TaskCompletionSource<bool> tcs;

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

            // Mostrar el diálogo
            IsVisible = true;

            return tcs.Task;
        }

        private void OnButtonClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;
            tcs?.TrySetResult(true);
        }
    }
}