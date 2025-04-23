using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TuApp.Styles
{
    public partial class NoChangesDialog : ContentView
    {
        private TaskCompletionSource<bool> tcs;

        public NoChangesDialog()
        {
            InitializeComponent();
        }

        public Task ShowAsync()
        {
            tcs = new TaskCompletionSource<bool>();

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