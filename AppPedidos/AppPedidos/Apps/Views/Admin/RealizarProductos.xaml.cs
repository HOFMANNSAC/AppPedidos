using AppPedidos.Apps.Helpers;
using AppPedidos.Apps.Model;
using Newtonsoft.Json.Linq;
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
    public partial class RealizarProductos : ContentPage
    {
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public string codigoPrduc { get; set; }
        public int cantidadPrduc { get; set; }
        public int totalPrduc { get; set; }
        public int precioUnitario { get; set; }
        public int Stock { get; set; }
        public Command<Productos> RemoveCommand
        {
            get
            {
                return new Command<Productos>((productos) =>
                {
                    ListaProductos.Remove(productos);
                });
            }
        }
        public RealizarProductos()
        {
            InitializeComponent();
            ListaProductos = new ObservableCollection<Productos>();
            ListaProductos.Clear();
            modalAgregar.IsVisible = false;
            cargarProductos();

            cargarBoxProductos();
            BindingContext = this;
        }
        private void cargarProductos()
        {
            Productos p = new Productos() { Codigo = "TEXBAB01", Cantidad = 1, Total = 500, PrecioUnitario= 500,Stock=115 };
            ListaProductos.Add(p);
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
                precioUnitario = item.PrecioUnitario;
                Stock = item.Stock;

                //agregarProducto.SelectedItem = Convert.ToInt32(codigoPrduc);
                //agregarProducto.IsEnabled = false;


                txtCodigo.Text = codigoPrduc;
                txtCodigo.IsEnabled = false;
                txtCantidad.Text = cantidadPrduc.ToString();
                var TotalProductos = totalPrduc * cantidadPrduc;
                txtTotal.Text = TotalProductos.ToString();
                txtTotal.IsEnabled = false;
                txtPrecioUnitario.Text = precioUnitario.ToString();
                txtPrecioUnitario.IsEnabled = false;
                txtStock.Text = Stock.ToString();
                txtStock.IsEnabled = false;

            }
            catch (Exception)
            {
                throw;
            }

        }
        //private void cargarDropProdocutos()
        //{
        //    MetodosApi api = new MetodosApi();
        //    var respuesta = JArray.Parse(api.obtenerProductos());
        //    if (respuesta[0].ToString() == "S")
        //    {
        //        JArray jsonString = JArray.Parse(respuesta[1].ToString());
        //        foreach (JObject item in jsonString.OfType<JObject>())
        //        {
        //            //Si no esta agregada la ruta lo incluimos
        //            Productos p = CompletarInformacion(item);
        //            agregarProducto.Items.Add(p.Codigo);
        //        }
        //    }
        //    else
        //        DisplayAlert("Error", "No existen productos ", "OK");
        //}
        private void agregarProducto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void btnRestarProd_Clicked(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            try
            {
                if (cantidad < cantidadPrduc)
                {
                    if (cantidad == 0)
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

        private void btnGuardarProd_Clicked(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text;
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            int precioUnitario = Convert.ToInt32(txtPrecioUnitario.Text);
            int stock = Convert.ToInt32(txtStock.Text);
            Productos addproductos = new Productos() { Codigo = codigo, Cantidad = cantidad, Total = precioUnitario * cantidad , PrecioUnitario = precioUnitario,Stock=stock };
            ListaProductos.Add(addproductos);
            BindingContext = this;
            DisplayAlert("Mensaje", "Agregado", "Aceptar");
        }

        private void Eliminar_Tapped(object sender, EventArgs e)
        {
            var imagen = sender as Image;
            var producto = imagen?.BindingContext as Productos;
            var vm = BindingContext as RealizarProductos;
            vm?.RemoveCommand.Execute(producto);
        }

        private void btnSumarProd_Clicked_1(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            try
            {
                if (cantidad > cantidadPrduc)
                {
                    Productos productosSelec = new Productos() { Codigo = codigoPrduc, Cantidad = cantidadPrduc, Total = totalPrduc };
                    ListaProductos.Clear();
                    Productos addproductos = new Productos() { Codigo = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = totalPrduc * cantidad };
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
        private void btnAgregarProducto_Clicked(object sender, EventArgs e)
        {
            modalAgregar.IsVisible = true;
            btnSumarProd.IsVisible = false;
            btnRestarProd.IsVisible = false;
            txtTotal.IsEnabled = false;
            txtCodigo.IsVisible = false;
        }
        public void cargarBoxProductos()
        {
            string resultado = string.Empty;
            try
            {
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.obtenerProductos());
                if (respuesta[0].ToString() == "")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());
                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        Productos p = CompletarInformacion(item);
                        PCKproducto.Items.Add(p.Codigo);
                    }
                }
                List<Productos> productos  = new List<Productos>();
                {
                    productos.Add(new Productos() { Codigo = codigoPrduc});
                };
                foreach (var codigo in productos)
                {
                    PCKproducto.Items.Add(codigo.Codigo);
                }
                resultado = "S";
            }
            catch (Exception)
            {
                resultado = "N";
            }
        }
        private Productos CompletarInformacion(JObject item) => new Productos
        {
            Codigo = item.GetValue("Codigo").ToString()
        };
        private void PCKproducto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}