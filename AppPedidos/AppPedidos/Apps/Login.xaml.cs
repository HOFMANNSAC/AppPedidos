using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using AppPedidos.Apps.Data;
using AppPedidos.Apps.Model;
using AppPedidos.Apps.Views;
using System.Threading.Tasks;
using AppPedidos.Apps.Helpers;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppPedidos.Apps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public static BDLocal BD;
        public Login()
        {
            string db = "bd_local.db3";
            string ruta = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), db);
            BD = new BDLocal(ruta);
            InitializeComponent();
            cmdIniciarSesion.Clicked += CmdIniciarSesion_Clicked;
        }
        private async void CmdIniciarSesion_Clicked(object sender, EventArgs e)
        {
            try
            {
                Usuario um = new Usuario();
                um.UsuarioSistema = txtUsuario.Text;
                um.Password = txtPassword.Text;
                Application.Current.Properties["usuarioSistema"] = um.UsuarioSistema;
                if (um.UsuarioSistema != "" && um.Password != "")
                {
                    if (Metodos.HayConexion())
                    {
                        MetodosApi api = new MetodosApi();
                        var resultado = JObject.Parse(api.ValidarAcceso(um));
                        if (resultado["respuesta"].ToString() == "S")
                        {
                            um = CompletarInformacion(resultado, um);
                            if (!BD.ExisteUsuario(um.ID.ToString()))
                                BD.AgregarUsuario(um);
                            else
                                BD.ActualizarUsuario(um);

                            await Navigation.PushModalAsync(new PaginaMaestra("Pedidos"));
                        }
                        else
                            await DisplayAlert("Alerta", resultado["mensaje"].ToString(), "OK");
                    }
                    else
                    {
                        um = BD.ValidarUsuario(um.UsuarioSistema, um.Password);
                        if (um.ID != "")
                        {
                            Application.Current.Properties["id_usuario"] = um.ID;
                            Application.Current.Properties["nombre"] = um.Nombre;
                            await Navigation.PushModalAsync(new PaginaMaestra("Login"));
                        }
                        else
                            await DisplayAlert("Alerta", "Sin conexion, no se encuentra el usuario en telefono", "OK");
                    }
                }
                else
                    await DisplayAlert("Alerta", "Ingrese Usuario y Contraseña", "OK");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private Usuario CompletarInformacion(JObject resultado, Usuario u)
        {
            var datos = JObject.Parse(JArray.Parse(resultado["data"].ToString())[0].ToString());
            Application.Current.Properties["id_usuario"] = datos["ID"];
            Application.Current.Properties["nombre"] = datos["NOMBRE_COMPLETO"].ToString();
            u.ID = datos["ID"].ToString();
            u.Nombre = datos["NOMBRE_COMPLETO"].ToString();
            return u;
        }
    }
}