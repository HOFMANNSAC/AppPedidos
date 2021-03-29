﻿using System;
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
                        { "ReqDescuento", pe.ReqDescuento },
                        { "QuienAprueba", pe.QuienAprueba },
                        { "ObsDescuento", pe.ObsDescuento },
                        { "TotalOrden", pe.TotalOrden },
                        { "NroProductos", pe.NroProductos },
                        { "RetiroDrogueria", pe.RetiroDrogueria },
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
        public string CargarCLientes(Customer c)
        {
            string respuestaString = "";
            try
            {
                Uri uri = new Uri("http://localhost:54297/api/ObtenerClientes");
                NameValueCollection parametros = new NameValueCollection
                {
                    {"NombreCLiente", c.NombreCliente },
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
    }
}
