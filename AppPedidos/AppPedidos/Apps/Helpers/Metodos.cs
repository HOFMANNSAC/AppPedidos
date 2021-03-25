using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace AppPedidos.Apps.Helpers
{
    public class Metodos
    {
        public static bool HayConexion(string huesped = "https://gestion.drogueriahofmann.cl/")
        {
            try
            {
                using (var client = new WebClient())
                using(client.OpenRead(huesped))
                {
                    return true;
                }
            }
            catch 
            {
                return false;
            }
        }
    }
}
