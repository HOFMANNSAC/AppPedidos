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
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public Command<Productos> RemoveCommand {
            get
            {
                return new Command<Productos>((productos) =>
                {
                    ListaProductos.Remove(productos);
                });
            }
        }
        public string codigoPrduc { get; set; }
        public int cantidadPrduc { get; set; }
        public int totalPrduc { get; set; }
        public RealizarPedidos()
        {
            InitializeComponent();
            ListaProductos = new ObservableCollection<Productos>();
            ListaProductos.Clear();
            modalAgregar.IsVisible = false;
            Productos p = new Productos() { Codigo = "TEXBAB01", Cantidad = 1, Total = 500 };
            ListaProductos.Add(p);
            BindingContext = this;

        }
        private void cmdAgregar_Clicked_1(object sender, EventArgs e)
        {
            modalAgregar.IsVisible = true;
            txtCodigo.Text = "";
            txtCantidad.Text = "";
            txtTotal.Text = "";
            txtCodigo.IsEnabled = true;
            txtTotal.IsEnabled = true;
            btnGuardarProd.IsVisible = true;
            btnSumarProd.IsVisible = false;
            btnRestarProd.IsVisible = false;
        }

        private void tipoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void quienAprueba_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btnGuardarProd_Clicked(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text;
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            int total = Convert.ToInt32( txtTotal.Text);
            Productos addproductos = new Productos() { Codigo = codigo, Cantidad = cantidad, Total = total };
            ListaProductos.Add(addproductos);
            BindingContext = this;
            DisplayAlert("Mensaje", "Agregado", "Aceptar");
        }
        private void Lista_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                btnGuardarProd.IsVisible = false;
                btnSumarProd.IsVisible = true;
                btnRestarProd.IsVisible = true;
                modalAgregar.IsVisible = true;

                //Datos Tabla LISTA
                var item = (Productos)e.SelectedItem;
                codigoPrduc = item.Codigo.ToString();
                cantidadPrduc = item.Cantidad;
                totalPrduc = Convert.ToInt32(item.Total);


                txtCodigo.Text = codigoPrduc;
                txtCodigo.IsEnabled =false;

                txtCantidad.Text = cantidadPrduc.ToString();

                txtTotal.Text = totalPrduc.ToString();
                txtTotal.IsEnabled = false;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void btnSumarProd_Clicked(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            try
            {
                if (cantidad > cantidadPrduc)
                {
                    Productos productosSelec = new Productos() { Codigo = codigoPrduc, Cantidad = cantidadPrduc, Total = totalPrduc };
                    ListaProductos.Clear();

                    Productos addproductos = new Productos() { Codigo = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = totalPrduc };

                    ListaProductos.Add(addproductos);

                    DisplayAlert("Mensaje", "Edito", "Aceptar");
                    modalAgregar.IsVisible = false;
                    BindingContext = this;
                }
                else
                {
                    DisplayAlert("Mensaje", "valor debe ser mayor al que existe", "Aceptar");
                    modalAgregar.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                throw;
                
            }
        }
        private void btnRestarProd_Clicked(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            try
            {
                if (cantidad < cantidadPrduc)
                {
                    if (cantidad ==0)
                    {
                        DisplayAlert("Mensaje", "Solo se puede Eliminar", "Aceptar");
                    }
                    else
                    {
                        Productos addproductos = new Productos() { Codigo = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = totalPrduc };

                        ListaProductos.Add(addproductos);




                        DisplayAlert("Mensaje", "Edito", "Aceptar");
                        modalAgregar.IsVisible = false;
                    }
                }
                else
                {
                    DisplayAlert("Mensaje", "valor debe ser menor al que existe", "Aceptar");
                    modalAgregar.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void Eliminar_Tapped(object sender, EventArgs e)
        {
            var imagen = sender as Image;
            var producto = imagen?.BindingContext as Productos;
            var vm = BindingContext as RealizarPedidos;
            vm?.RemoveCommand.Execute(producto);
        }
    }
}