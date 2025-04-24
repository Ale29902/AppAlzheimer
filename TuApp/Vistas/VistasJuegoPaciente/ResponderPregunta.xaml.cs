using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;

namespace TuApp;

public partial class ResponderPregunta : ContentPage
{
    private PreguntaViewModel viewModel;
    private int preguntaActualIndex = 0;
    private List<bool> respuestasCorrectas = new();
    private JuegoPaciente juegoSeleccionado;
    private int idUsuario;

    public ResponderPregunta(JuegoPaciente juegoSeleccionado, int idUsuario)
    {
        InitializeComponent();
        this.BindingContext = viewModel = new PreguntaViewModel();
        this.juegoSeleccionado = juegoSeleccionado;
        this.idUsuario = idUsuario;
        _ = CargarPreguntasAsync();
    }

    private async Task CargarPreguntasAsync()
    {
        await viewModel.CargarPreguntas(juegoSeleccionado.idJuego);

        if (viewModel.ListaPregunta.Any())
        {
            MostrarPregunta();
        }
        else
        {
            await DisplayAlert("Aviso", "No se encontraron preguntas.", "OK");
            await Navigation.PopAsync();
        }
    }

    private void MostrarPregunta()
    {
        if (preguntaActualIndex < viewModel.ListaPregunta.Count)
        {
            var pregunta = viewModel.ListaPregunta[preguntaActualIndex];
            lblPregunta.Text = pregunta.Descripcion;
            OpcionesCollection.ItemsSource = pregunta.opciones;
            imgPregunta.Source = pregunta.ImagenSource;
        }
    }
    private async void Opcion_Tapped(object sender, TappedEventArgs e)
    {
        // Verificar que tengamos una opci�n v�lida
        if (e.Parameter is not Opcion opcionSeleccionada)
            return;

        // Obtener el frame que se toc�
        Frame frameSeleccionado = null;

        // Esta es una forma m�s robusta de encontrar el frame
        if (sender is TappedEventArgs)
        {
            // El sender es el event args
            var element = (sender as TappedEventArgs).Parameter as Element;
            while (element != null && !(element is Frame))
            {
                element = element.Parent as Element;
            }
            frameSeleccionado = element as Frame;
        }
        else if (sender is Frame)
        {
            // El sender es directamente el frame
            frameSeleccionado = sender as Frame;
        }
        else if (sender is Element)
        {
            // El sender es un elemento, buscar su frame padre
            Element element = sender as Element;
            while (element != null && !(element is Frame))
            {
                element = element.Parent as Element;
            }
            frameSeleccionado = element as Frame;
        }

        // Evaluar si la opci�n es correcta
        bool esCorrecta = opcionSeleccionada.Condicion;
        respuestasCorrectas.Add(esCorrecta);

        if (frameSeleccionado == null)
        {
            // Si no podemos encontrar el frame, al menos procesamos la selecci�n
            // y continuamos con la siguiente pregunta
            preguntaActualIndex++;

            if (preguntaActualIndex < viewModel.ListaPregunta.Count)
            {
                MostrarPregunta();
            }
            else
            {
                await EnviarPuntaje();
            }

            return;
        }

        // Aplicar color din�mico del tema
        var colorCorrecto = Application.Current.Resources.TryGetValue("OkColor", out var ok) ? (Color)ok : Colors.Green;
        var colorIncorrecto = Application.Current.Resources.TryGetValue("ErrorColor", out var err) ? (Color)err : Colors.Red;

        frameSeleccionado.BackgroundColor = esCorrecta ? colorCorrecto : colorIncorrecto;

        // Deshabilitar las opciones temporalmente para evitar m�ltiples selecciones
        OpcionesCollection.IsEnabled = false;

        await Task.Delay(500);

        preguntaActualIndex++;

        if (preguntaActualIndex < viewModel.ListaPregunta.Count)
        {
            MostrarPregunta();
            OpcionesCollection.IsEnabled = true;
        }
        else
        {
            await EnviarPuntaje();
        }
    }

    private async Task EnviarPuntaje()
    {
        int correctas = respuestasCorrectas.Count(r => r);
        int total = respuestasCorrectas.Count;

        if (total == 0)
        {
            await DisplayAlert("Aviso", "No se respondieron preguntas.", "OK");
            return;
        }

        await InsertarPuntaje(juegoSeleccionado.idJuego, correctas, total);
    }

    private void MostrarResultadoFinal()
    {
        lblPregunta.Text = "�Preguntas respondidas correctamente!";
        OpcionesCollection.IsVisible = false;
        imgPregunta.IsVisible = false;
        btnFlechaVolver.IsVisible = false;
        lblResultadoFinal.IsVisible = true;
        lblResultadoFinal.Text = $"Tuviste {respuestasCorrectas.Count(r => r)} respuestas correctas de {respuestasCorrectas.Count}.";
        btnRegresar.IsVisible = true;
    }

    private async void btnRegresar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    public async Task InsertarPuntaje(int idJuego, int totalCorrectas, int totalPreguntas)
    {
        int puntaje = (int)Math.Round(((double)totalCorrectas / totalPreguntas) * 100);

        var req = new ReqInsertarPuntaje
        {
            idPaciente = SesionActiva.sesionActiva.usuario.IdUsuario,
            idJuego = idJuego,
            puntaje = puntaje
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

        using HttpClient httpClient = new();
        var respuestaHttp = await httpClient.PostAsync(App.API_URL + "juego/insertarpuntaje", jsonContent);

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var contenido = await respuestaHttp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResInsertarPuntaje>(contenido);

            if (res?.resultado == true)
            {
                MostrarResultadoFinal();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el puntaje.", "OK");
                await Navigation.PopAsync();
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Error de conexi�n al registrar puntaje.", "OK");
            await Navigation.PopAsync();
        }
    }

    private async void BtnFlechaVolver_Clicked(object sender, EventArgs e)
    {
        if (respuestasCorrectas.Count > 0)
        {
            bool confirmacion = await DisplayAlert(
                "Advertencia",
                "Si regresas, se perder�n las respuestas registradas. �Deseas continuar?",
                "S�", "No");

            if (!confirmacion)
                return;
        }

        respuestasCorrectas.Clear();
        await Navigation.PopAsync();
    }
}
