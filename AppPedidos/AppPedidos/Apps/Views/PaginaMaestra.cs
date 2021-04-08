using System;
using System.Text;
using Xamarin.Forms;
using System.Collections.Generic;
using AppPedidos.Apps.Views.Admin;

namespace AppPedidos.Apps.Views
{
    public class PaginaMaestra : MasterDetailPage
    {
        readonly MasterPage masterPage;
        public PaginaMaestra(String origen)
        {
            try
            {
                masterPage = new MasterPage();
                Master = masterPage;
                switch (origen)
                {
                    case "Login":
                    case "Pedidos":
                        Detail = new NavigationPage(new Prueba());
                        break;
                    default:
                        break;
                }
                masterPage.ListView.ItemSelected += ListView_ItemSelected;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                masterPage.ListView.SelectedItem = null;
                IsPresented = false;
                if (item.Title == "Relizar Pedidos")
                {
                    Navigation.PushModalAsync(new PaginaMaestra("Login"));
                }
            }
        }
    }
}
