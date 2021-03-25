using System;
using System.Net;
using System.Text;
using AppPedidos.Apps.Model;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace AppPedidos.Apps.Helpers
{
    public class MetodosApi
    {
        public string ValidarAcceso(Usuario u)
        {
            string respuestaString = "";
            try
            {
                Uri uri = new Uri("https://sellout.drogueriahofmann.cl/Rutas/Login");
                NameValueCollection parametros = new NameValueCollection
                    {
                        { "usuario", u.UsuarioSistema },
                        { "password", u.Password }
                    };
                byte[] respuestaByte = new WebClient().UploadValues(uri, "POST", parametros);
                respuestaString = Encoding.UTF8.GetString(respuestaByte);
            }
            catch (Exception)
            {
                respuestaString = "[\"N\",\"Error al Enviar la petición.\"]";
            }
            return respuestaString;
        }
    }
}
