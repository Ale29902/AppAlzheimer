using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;

namespace TuApp;

public partial class PuntajesPopup : Popup
{
    public PuntajesPopup(int idPaciente, int idJuego)
    {
        InitializeComponent();
        ObtenerPuntajes(idPaciente, idJuego);
    }

    private async void ObtenerPuntajes(int idPaciente, int idJuego)
    {

        ReqObtenerPuntajes req = new ReqObtenerPuntajes
        {
            idPaciente = idPaciente,
            idCuidador = SesionActiva.sesionActiva.usuario.IdUsuario,
            idJuego = idJuego
        };

        var jsonContent = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

        HttpResponseMessage respuestaHttp = null;

        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://localhost:44347/api/");
            respuestaHttp = await httpClient.PostAsync("juego/obtenerpuntaje", jsonContent);
        }

        if (respuestaHttp.IsSuccessStatusCode)
        {
            var contenido = await respuestaHttp.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResObtenerPuntajes>(contenido);

            if (res != null && res.resultado && res.puntajes != null)
            {
                if (res.puntajes.Count > 0)
                {
                    var listaFormateada = res.puntajes.Select(p =>
                        $"Puntaje: {p.ValorPuntaje} - Fecha: {p.FechaHora:dd/MM/yyyy}").ToList();

                    collectionPuntajes.ItemsSource = listaFormateada;
                }
                else
                {
                    collectionPuntajes.ItemsSource = new List<string> { "No hay puntajes registrados." };
                }
            }
            else
            {
                collectionPuntajes.ItemsSource = new List<string> { "No se pudieron cargar los puntajes." };
            }
        }
    }


    private void Cerrar_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}