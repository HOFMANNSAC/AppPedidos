using System;
using System.Net;
using System.Text;
using AppPedidos.Apps.Model;
using System.Collections.Generic;
using System.Collections.Specialized;

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
        public string InsertarPedidos(Pedido pe)
        {
            string respuestaString = "";
            try
            {
                Uri uri = new Uri("https://localhost:44370/Pedidos/InsertarPedidos");
                NameValueCollection parametros = new NameValueCollection
                    {
                        { "ID", pe.ID },
                        { "SHipToId", pe.SHipToId },
                        { "CustOrdNbr", pe.CustOrdNbr },
                        { "UsuarioCrea", pe.UsuarioCrea },
                        { "TipoPedido", pe.TipoPedido },
                        { "ObsGeneral", pe.ObsGeneral },
                        { "ReqDescuento", pe.ReqDescuento.ToString()},
                        { "QuienAprueba", pe.QuienAprueba },
                        { "ObsDescuento", pe.ObsDescuento },
                        { "TotalOrden", pe.TotalOrden },
                        { "NroProductos", pe.NroProductos },
                        { "RetiroDrogueria", pe.RetiroDrogueria.ToString()},
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
        public string CargarCLientes(string name)
        {
            string respuestaString = "";
            try
            {
                Uri uri = new Uri("https://sellout.drogueriahofmann.cl/App/ObtenerCliente");
                NameValueCollection parametros = new NameValueCollection
                {
                    {"Id", name  }
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
        public string InsertarArchivos(Archivo a)
        {
            string respuestaString = string.Empty;
            try
            {
                Uri uri = new Uri("http://localhost:54297/api/InsertarArchivo");
                NameValueCollection parametros = new NameValueCollection
                {
                    { "NroPedido", a.NroPedido},
                    {"NombreArchivo", a.NombreArchivo },
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
        public string ObtenerDireccionCliente(Pedido p)
        {
            string respuestaString = string.Empty;
            try
            {
                Uri uri = new Uri("http://localhost:54297/api/ObtenerDireccionCliente");
                NameValueCollection parametros = new NameValueCollection
                {
                    {"CustId", p.CustID }
                };
                byte[] respuestaByte = new WebClient().UploadValues(uri,"POST",parametros);
                respuestaString = Encoding.UTF8.GetString(respuestaByte);
            }
            catch (Exception)
            {
                respuestaString= "[\"N\",\"Error al Enviar la petición.\"]";
            }
            return respuestaString;
        }
        public string obtenerProductos(string bsrProducto)
        {
            string respuestaString = string.Empty;
            try
            {
                Uri uri = new Uri("https://sellout.drogueriahofmann.cl/App/ObtenerProducto");
                NameValueCollection parametros = new NameValueCollection
                {
                    {"ID", bsrProducto }
                };
                byte[] respuestaByte = new WebClient().UploadValues(uri, "POST",parametros);
                respuestaString = Encoding.UTF8.GetString(respuestaByte);
                
            }
            catch (Exception ex)
            {
                respuestaString = "[\"N\",\"Error al Enviar la petición.\"]";
            }
            return respuestaString;
        }
        public string obtenerdatosProductos(string InvtID)
        {
            string respuestaString = string.Empty;
            try
            {
                Uri uri = new Uri("https://sellout.drogueriahofmann.cl/App/ObtenerdatosProductos");
                NameValueCollection parametros = new NameValueCollection
                {
                    {"ID", InvtID }
                };
                byte[] respuestaByte = new WebClient().UploadValues(uri, "POST", parametros);
                respuestaString = Encoding.UTF8.GetString(respuestaByte);

            }
            catch (Exception ex)
            {
                respuestaString = "[\"N\",\"Error al Enviar la petición.\"]";
            }
            return respuestaString;
        }
    }
}
