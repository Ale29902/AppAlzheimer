using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui;
using SkiaSharp;
using System;
using System.IO;
using TuApp.Entidades;
using TuApp.VistasModelo;
using TuApp.Styles; // A�adido para importar los custom dialogs

namespace TuApp
{
    public partial class InsertarPreguntaPopup : Popup
    {
        private PreguntaViewModel viewModel;
        // Declaraci�n del di�logo personalizado est�tico para los popups
        private static readonly CustomAlertDialog customAlertDialog = new CustomAlertDialog();

        public InsertarPreguntaPopup()
        {
            InitializeComponent();

            // Para popups, necesitamos asegurar que el di�logo est� en la p�gina principal
            EnsureDialogInMainPage();
        }

        // M�todo para asegurar que el di�logo est� en la p�gina principal
        private void EnsureDialogInMainPage()
        {
            var mainPage = Application.Current.MainPage;
            if (mainPage != null)
            {
                // Si mainPage es una ContentPage directamente
                if (mainPage is ContentPage contentPage)
                {
                    AddDialogToPage(contentPage);
                }
                // Si mainPage es NavigationPage
                else if (mainPage is NavigationPage navPage && navPage.CurrentPage is ContentPage currentPage)
                {
                    AddDialogToPage(currentPage);
                }
            }
        }

        // M�todo para a�adir el di�logo a una p�gina
        private void AddDialogToPage(ContentPage page)
        {
            // Verificar si el di�logo ya ha sido a�adido
            if (page.Content is Grid grid)
            {
                bool dialogExists = false;
                foreach (var child in grid.Children)
                {
                    if (child is CustomAlertDialog)
                    {
                        dialogExists = true;
                        break;
                    }
                }

                if (!dialogExists)
                {
                    grid.Children.Add(customAlertDialog);
                }
            }
            else
            {
                // Guardar el contenido original
                var originalContent = page.Content;

                // Crear una nueva grid para contener todo
                var mainGrid = new Grid();

                // A�adir el contenido original
                mainGrid.Children.Add(originalContent);

                // A�adir el di�logo (inicialmente invisible)
                mainGrid.Children.Add(customAlertDialog);

                // Establecer la grid como el nuevo contenido
                page.Content = mainGrid;
            }
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            string descripcion = descripcionEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                // Reemplazar DisplayAlert con customAlertDialog
                await customAlertDialog.ShowAsync("Error", "Debe ingresar una descripci�n.", "OK");
                return;
            }
            var pregunta = new Pregunta
            {
                Descripcion = descripcion,
            };
            Close(pregunta);
        }
    }
}