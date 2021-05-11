using AppPedidos.Apps.Data;
using AppPedidos.Apps.Helpers;
using AppPedidos.Apps.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppPedidos.Apps.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealizarProductos : ContentPage
    {
        public int contador = 0;
        public static BDLocal BD;
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public IList<ProductosAPI> ListadoProductos { get; private set; }
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
            ListadoProductos = new List<ProductosAPI>();
            cargarProductos();
            BindingContext = this;
        }
        private void cargarProductos()
        {
            Productos p = new Productos() { ID = "TEXBAB01", Cantidad = 1, Total = 500, PrecioUnitario= 500,Stock=115 };
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
                codigoPrduc = item.ID.ToString();
                cantidadPrduc = item.Cantidad;
                totalPrduc = Convert.ToInt32(item.Total);
                precioUnitario = item.PrecioUnitario;
                Stock = item.Stock;

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
                btnAgregarProducto.IsVisible = false;
                btnBack.IsVisible = false;

            }
            catch (Exception)
            {
                throw;
            }

        }
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
                        Productos addproductos = new Productos() { ID = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = totalPrduc };
                        ListaProductos.Add(addproductos);
                        DisplayAlert("Mensaje", "Edito", "Aceptar");
                        modalAgregar.IsVisible = false;
                        btnAgregarProducto.IsVisible = true;
                        btnBack.IsVisible = true;
                    }
                }
                else
                {
                    DisplayAlert("Mensaje", "valor debe ser menor al que existe", "Aceptar");
                    modalAgregar.IsVisible = false;
                    btnAgregarProducto.IsVisible = true;
                    btnBack.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void btnGuardarProd_Clicked(object sender, EventArgs e)
        {
            string codigo = bscProducto.Text;
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            int precioUnitario = Convert.ToInt32(txtPrecioUnitario.Text);
            int stock = Convert.ToInt32(txtStock.Text);
            Productos addproductos = new Productos() { ID = codigo, Cantidad = cantidad, Total = precioUnitario * cantidad , PrecioUnitario = precioUnitario,Stock=stock };
            ListaProductos.Add(addproductos);
            BindingContext = this;
            DisplayAlert("Mensaje", "Agregado", "Aceptar");
            modalAgregar.IsVisible = false;
            btnAgregarProducto.IsVisible = true;
            btnBack.IsVisible = true;
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
                    Productos productosSelec = new Productos() { ID = codigoPrduc, Cantidad = cantidadPrduc, Total = totalPrduc };
                    ListaProductos.Clear();
                    Productos addproductos = new Productos() { ID = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = totalPrduc * cantidad };
                    ListaProductos.Add(addproductos);
                    DisplayAlert("Mensaje", "Edito", "Aceptar");
                    modalAgregar.IsVisible = false;
                    BindingContext = this;
                    btnAgregarProducto.IsVisible = true;
                    btnBack.IsVisible = true;
                }
                else
                {
                    DisplayAlert("Mensaje", "valor debe ser mayor al que existe", "Aceptar");
                    modalAgregar.IsVisible = false;
                    btnAgregarProducto.IsVisible = true;
                    btnBack.IsVisible = true;
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
            btnAgregarProducto.IsVisible = false;
            btnBack.IsVisible = false;
        }
        public  void cargarBoxProductos(string Codigo)
        {
            try
            {
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse( api.obtenerProductos(Codigo));
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());

                    foreach (JObject item in jsonString.OfType<JObject>())
                    {                        
                        ProductosAPI rm = CompletarInformacion(item);
                        CompletarDatosListas(rm);
                    }
                }
                else
                    DisplayAlert("Error", "No existen productos asociadas", "OK");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void cargarDatosProductos(string InvtID)
        {
            try
            {
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.obtenerdatosProductos(InvtID));
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());

                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        Productos rm = CompletarInformacionProd(item);
                    }
                }
                else
                    DisplayAlert("Error", "No existen productos asociadas", "OK");
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private ProductosAPI CompletarInformacion(JObject item) => new ProductosAPI
        {
            INVTID = item.GetValue("INVTID").ToString(),
            DESCRIPCION = item.GetValue("DESCRIPCION").ToString()
        };
        private Productos CompletarInformacionProd(JObject item) => new Productos
        {
            PrecioUnitario = (int)item.GetValue("Precio"),
            Stock = (int)item.GetValue("QtyAvail")
        };
        private void CompletarDatosListas(ProductosAPI pa)
        {
            ListadoProductos.Add(pa);
        }
        private void PCKproducto_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PaginaMaestra("Pedidos"));
        }
        private void lstProd_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            var item = (Productos)e.SelectedItem;
            var invtID = item.ID;
            cargarDatosProductos(invtID);
        }

        private void btnBuscarprod_Clicked(object sender, EventArgs e)
        {
            cargarBoxProductos(bscProducto.Text);
        }
    }
}