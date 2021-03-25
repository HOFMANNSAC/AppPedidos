using System;
using System.Text;
using Xamarin.Forms;
using System.Collections.Generic;
using AppPedidos.Apps.Views.Admin;

namespace AppPedidos.Apps.Views
{
    public class PaginaMaestra:MasterDetailPage
    {
        readonly MasterPage masterPage;
        public PaginaMaestra(String origen)
        {
            masterPage = new MasterPage();
            Master = masterPage;
            switch (origen)
            {
                case "Login":
                case "Pedidos":
                    Detail = new NavigationPage(new Pedidos());
                    break;
                default:
                    break;
            }
        }
    }
}
