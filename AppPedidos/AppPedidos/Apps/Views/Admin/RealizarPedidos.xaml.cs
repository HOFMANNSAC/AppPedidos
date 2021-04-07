using System;
using System.IO;
using System.Linq;
using System.Text;
using Plugin.Media;
using System.Collections;
using Newtonsoft.Json.Linq;
using AppPedidos.Apps.Model;
using System.Threading.Tasks;
using AppPedidos.Apps.Helpers;
using Plugin.Media.Abstractions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Essentials.Permissions;

namespace AppPedidos.Apps.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealizarPedidos : ContentPage
    {
        public IList<Productos> ProductosList { get; set; }
        public RealizarPedidos()
        {

            InitializeComponent();
            BindingContext = this;
            cmdAgregar.Clicked += CmdAgregar_Clicked;
        }

        private void CmdAgregar_Clicked(object sender, EventArgs e)
        {

            cargarListaProductos();

        }

        private void cargarListaProductos()
        {
            string resultado = string.Empty;
            try
            {
                ProductosList = new List<Productos>();
                Productos pm = new Productos { Codigo = "ANEANE010", Cantidad = 3, Total = 10000 };
                ProductosList.Add(pm);
            }
            catch (Exception)
            {
                resultado = "N";
            }

        }
    }

}