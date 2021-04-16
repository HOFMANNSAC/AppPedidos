using AppPedidos.Apps.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppPedidos.Apps.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Prueba : ContentPage
    {
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public Prueba()
        {
            InitializeComponent();
            ListaProductos = new ObservableCollection<Productos>();
            ListaProductos.Clear();
            Productos p = new Productos() {Codigo="TEXBAB01",Cantidad=1,Total=500 };
            ListaProductos.Add(p);
            BindingContext = this;
        }

        private void cmdAgregar_Clicked(object sender, EventArgs e)
        { 
        }
    }
}