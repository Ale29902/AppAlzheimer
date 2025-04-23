using Newtonsoft.Json;
using System.Text;
using TuApp.Styles;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;

namespace TuApp.Vistas
{
    public partial class PerfilPage : ContentPage
    {
        private bool modoEdicion = false;
        private string nombreOriginal;
        private DateTime fechaNacimientoOriginal;
        private string direccionOriginal;
        private PinDialog pinDialog;
        private CustomAlertDialog customAlertDialog;
        private NoChangesDialog noChangesDialog;

        public PerfilPage()
        {
            InitializeComponent();

            // Crear los diálogos
            pinDialog = new PinDialog();
            customAlertDialog = new CustomAlertDialog();
            noChangesDialog = new NoChangesDialog();

            // Guardar el contenido original
            var originalContent = Content;

            // Crear una nueva grid para contener todo
            var mainGrid = new Grid();

            // Añadir el contenido original
            mainGrid.Children.Add(originalContent);

            // Añadir los diálogos (inicialmente invisibles)
            mainGrid.Children.Add(pinDialog);
            mainGrid.Children.Add(customAlertDialog);
            mainGrid.Children.Add(noChangesDialog);

            // Establecer la grid como el nuevo contenido
            Content = mainGrid;

            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            lblBienvenida.Text = $"Hola {usuario.Nombre}";
            lblNombre.Text = usuario.Nombre;
            lblFecha.Text = $"{usuario.FechaNacimiento:yyyy-MM-dd}";
            lblDireccion.Text = usuario.Direccion;

            if (usuario.Codigo != null)
            {
                lblUbicacion.Text = usuario.Codigo;
            }
            else
            {
                // Usar el frame con nombre
                frameUbicacion.IsVisible = false;
            }

            nombreOriginal = usuario.Nombre;
            fechaNacimientoOriginal = usuario.FechaNacimiento;
            direccionOriginal = usuario.Direccion;

            if (usuario.FotoPerfil != null && usuario.FotoPerfil.Length > 0)
            {
                imgPerfil.Source = ImageSource.FromStream(() => new MemoryStream(usuario.FotoPerfil));
            }
            else
            {
                imgPerfil.Source = "usuario_default.png";
            }

            // Configurar visibilidad inicial de los controles
            txtNombre.IsVisible = false;
            dpFechaNacimiento.IsVisible = false;
            txtDireccion.IsVisible = false;
            txtPasswordActual.IsVisible = false;
            txtPasswordNueva.IsVisible = false;
        }

        private async void EditarDatos_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            if (!modoEdicion && usuario.IdTipoUsuario == 1 && usuario.pin != null && !string.IsNullOrEmpty(usuario.pin.Codigo))
            {
                // Mostrar el diálogo PIN
                string pinIngresado = await pinDialog.ShowAsync();

                // Si el usuario canceló (pinIngresado es null) o el PIN es incorrecto
                if (pinIngresado == null || pinIngresado != usuario.pin.Codigo)
                {
                    if (pinIngresado != null) // Solo mostrar error si el usuario realmente ingresó un PIN incorrecto
                    {
                        // Usar el diálogo de alerta personalizado
                        await customAlertDialog.ShowAsync("Verificación fallida", "El PIN ingresado es incorrecto.", "Aceptar");
                    }
                    return;
                }
                else
                {
                    // PIN correcto - mostrar confirmación personalizada
                    await customAlertDialog.ShowAsync("Verificación exitosa", "PIN correcto. Ahora puede editar sus datos.", "Continuar");
                }
            }

            modoEdicion = !modoEdicion;

            if (modoEdicion)
            {
                lblNombre.IsVisible = false;
                lblFecha.IsVisible = false;
                lblDireccion.IsVisible = false;

                txtNombre.IsVisible = true;
                dpFechaNacimiento.IsVisible = true;
                txtDireccion.IsVisible = true;

                // Mostrar el stack de cambio de contraseña
                stackCambiarPassword.IsVisible = true;
                txtPasswordNueva.IsVisible = true;
                txtPasswordActual.IsVisible = true;
                btnEditar.Text = "Guardar";

                txtNombre.Text = usuario.Nombre;
                dpFechaNacimiento.Date = usuario.FechaNacimiento;
                txtDireccion.Text = usuario.Direccion;
               
            }
            else
            {
                // Ocultar el stack cuando no está en edición
                stackCambiarPassword.IsVisible = false;

                await GuardarCambios();

                // Limpiar contraseñas por seguridad
                txtPasswordActual.Text = string.Empty;
                txtPasswordNueva.Text = string.Empty;
            }

        }
        private async void VolverInicio_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            // Navegar a la página correspondiente según el tipo de usuario
            if (usuario.IdTipoUsuario == 1)
            {
                // Usuario tipo 1 - Paciente
                await Navigation.PushAsync(new InicioPaciente());
            }
            else if (usuario.IdTipoUsuario == 2)
            {
                // Usuario tipo 2 - Cuidador
                await Navigation.PushAsync(new InicioCuidadorPage());
            }
            else
            {
                // Si hay más tipos, puedes manejarlos aquí o mostrar un mensaje
                await DisplayAlert("Información", "Tipo de usuario no reconocido.", "Aceptar");
            }
        }

        private async Task GuardarCambios()
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                await DisplayAlert("Datos incompletos", "El nombre es obligatorio", "Aceptar");
                return;
            }

            bool huboCambios = txtNombre.Text != nombreOriginal ||
                                dpFechaNacimiento.Date != fechaNacimientoOriginal ||
                                txtDireccion.Text != direccionOriginal ||
                                (!string.IsNullOrWhiteSpace(txtPasswordActual.Text) && !string.IsNullOrWhiteSpace(txtPasswordNueva.Text));

            if (!huboCambios)
            {
                // Usar el nuevo diálogo para cuando no hay cambios
                await noChangesDialog.ShowAsync();

                // Restaurar la vista a modo lectura
                RestaurarModoLectura();
                return;
            }

            string pin = usuario.pin?.Codigo;
            ReqActualizarUsuario reqUsuario = new ReqActualizarUsuario
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = txtNombre.Text,
                FechaNacimiento = dpFechaNacimiento.Date,
                Direccion = txtDireccion.Text,
                Pin = pin
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(reqUsuario), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var respuesta = await client.PostAsync("https://localhost:44347/api/usuario/actualizarusuario", jsonContent);
                if (!respuesta.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error de actualización", "No se pudieron actualizar los datos personales. Por favor, intente nuevamente.", "Aceptar");
                    return;
                }
            }

            // Si se proporcionaron ambos campos de contraseña, actualizar la contraseña
            if (!string.IsNullOrWhiteSpace(txtPasswordActual.Text) && !string.IsNullOrWhiteSpace(txtPasswordNueva.Text))
            {
                ReqActualizarContrasena reqPass = new ReqActualizarContrasena
                {
                    IdUsuario = usuario.IdUsuario,
                    ContrasenaActual = txtPasswordActual.Text,
                    NuevaContrasena = txtPasswordNueva.Text,
                    Pin = pin
                };

                var passContent = new StringContent(JsonConvert.SerializeObject(reqPass), Encoding.UTF8, "application/json");
                using (HttpClient client = new HttpClient())
                {
                    var respuesta = await client.PostAsync("https://localhost:44347/api/usuario/actualizarcontrasena", passContent);
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error de actualización", "No se pudo actualizar la contraseña. Verifique la contraseña actual.", "Aceptar");
                        return;
                    }
                }
            }

            await DisplayAlert("Actualización exitosa", "Sus datos han sido actualizados correctamente", "Aceptar");

            // Actualizar los campos visuales con los nuevos datos
            lblNombre.Text = txtNombre.Text;
            lblFecha.Text = $"{dpFechaNacimiento.Date:yyyy-MM-dd}";
            lblDireccion.Text = txtDireccion.Text;

            // Actualizar las variables de control
            nombreOriginal = txtNombre.Text;
            fechaNacimientoOriginal = dpFechaNacimiento.Date;
            direccionOriginal = txtDireccion.Text;

            RestaurarModoLectura();
        }

        private void RestaurarModoLectura()
        {
            txtNombre.IsVisible = false;
            dpFechaNacimiento.IsVisible = false;
            txtDireccion.IsVisible = false;
            txtPasswordActual.IsVisible = false;
            txtPasswordNueva.IsVisible = false;

            lblNombre.IsVisible = true;
            lblFecha.IsVisible = true;
            lblDireccion.IsVisible = true;

            btnEditar.Text = "Editar Datos";
            modoEdicion = false;

            // Limpiar campos de contraseña por seguridad
            txtPasswordActual.Text = string.Empty;
            txtPasswordNueva.Text = string.Empty;
        }
    }
}