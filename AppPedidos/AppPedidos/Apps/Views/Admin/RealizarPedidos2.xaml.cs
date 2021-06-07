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
    public partial class RealizarPedidos2 : ContentPage
    {
        public ObservableCollection<Customer> ListadoClientes { get; private set; }
        public ObservableCollection<Pedido> ListadoDireccion { get; private set; }
        public ObservableCollection<ProductosAPI> ListadoProductosAPI { get; private set; }
        int contador = 0;
        public RealizarPedidos2()
        {
            InitializeComponent();
            cargarBoxquienAprueba();
            cargarBoxTipoPedido();
            ListadoProductosAPI = new ObservableCollection<ProductosAPI>();
            ListadoClientes = new ObservableCollection<Customer>();
            ListadoDireccion = new ObservableCollection<Pedido>();
            BindingContext = this;
        }
        private void bsrCliente_Clicked(object sender, EventArgs e)
        {
            if (txtCliente.Text == null || txtCliente.Text == "")
                DisplayAlert("Error", "Debe ingresar un cliente ", "Aceptar");
            else {
                cargarClientes(txtCliente.Text.Trim());
                lstCLiente.IsVisible = true;
                lstCLiente.HeightRequest = (contador <=2) ? (88*contador) : 166;
            }       
        }

        public void cargarClientes(string name)
        {
            try
            {
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.CargarCLientes(name));
                ListadoClientes.Clear();
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());
                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        Customer p = CompletarInformacionClientes(item);
                        ListadoClientes.Add(p);
                        contador++;
                    }

                }
                else
                    DisplayAlert("Error", "No existen productos asociadas", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message.ToString(), "OK");
            }
        }
        private Customer CompletarInformacionClientes(JObject item) => new Customer
        {
            IDCliente = item.GetValue("custid").ToString(),
            NombreCliente = item.GetValue("name").ToString()
        };

        private void lstCLiente_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (Customer)e.SelectedItem;
            cargarDatosCliente(item.IDCliente);
            txtCliente.Text = item.NombreCliente;
            lstCLiente.IsVisible = false;
            txtCliente.IsEnabled = false;
        }
        public void cargarDatosCliente(string idCliente)
        {
            MetodosApi api = new MetodosApi();
            ListadoDireccion.Clear();
            var respuesta = JArray.Parse(api.ObtenerDatosClientes(idCliente));
            if (respuesta[0].ToString() == "S")
            {
                JArray jsonString = JArray.Parse(respuesta[1].ToString());
                foreach (JObject item in jsonString.OfType<JObject>())
                {
                    Pedido p = CompletarInformacionDatosClientes(item);
                    Pedido pe = DatoDireccionCliente(item);
                    CompletarDatoscliente(p);
                    CompletarDatosDireccion(pe);
                }
            }
        }
        private Pedido CompletarInformacionDatosClientes(JObject item) => new Pedido
        {
            clasePrecio = item.GetValue("ClasePrecio").ToString(),
            estadoCliente = item.GetValue("Status").ToString(),
            formaPago = item.GetValue("FormaPAgo").ToString(),
            correElectronico = item.GetValue("EMailAddr").ToString()
        };
        private Pedido DatoDireccionCliente(JObject item) => new Pedido
        {
            Local = item.GetValue("ShipToID").ToString(),
            direccionCliente = item.GetValue("Descr").ToString()
        };
        private void CompletarDatoscliente(Pedido p)
        {
            txtClasePrecio.Text = p.clasePrecio.Substring(0, 2);
            txtEstadoCliente.Text = p.estadoCliente;
            txtEstadoCredito.Text = p.formaPago;
            txtCorreo.Text = p.correElectronico;
            Application.Current.Properties["claseprecio"] = p.clasePrecio.Substring(0, 2);
            txtClasePrecio.IsEnabled = false;
            txtEstadoCliente.IsEnabled = false;
            txtEstadoCredito.IsEnabled = false;
            txtCorreo.IsEnabled = false;
        }
        private void CompletarDatosDireccion(Pedido pe)
        {
            ListadoDireccion.Add(pe);
        }
        public void cargarBoxTipoPedido()
        {
            string resultado = string.Empty;
            try
            {
                List<TipoPedido> tipoPedidos = new List<TipoPedido>();
                {
                    tipoPedidos.Add(new TipoPedido() { ID = 1, Name = "Orden Compra" });
                    tipoPedidos.Add(new TipoPedido() { ID = 2, Name = "Nota Venta" });
                };
                foreach (var Name in tipoPedidos)
                {
                    tipoPedido.Items.Add(Name.Name);
                }
                resultado = "S";
            }
            catch (Exception)
            {
                resultado = "N";
            }
        }
        public void cargarBoxquienAprueba()
        {
            string resultado = string.Empty;
            try
            {
                List<QuienAprueba> quienApruebas = new List<QuienAprueba>();
                {
                    quienApruebas.Add(new QuienAprueba() { ID = 1, Name = "Gianfranco Solari" });
                    quienApruebas.Add(new QuienAprueba() { ID = 2, Name = "Antonella Constanzi" });
                    quienApruebas.Add(new QuienAprueba() { ID = 3, Name = "Angela Riera" });
                    quienApruebas.Add(new QuienAprueba() { ID = 4, Name = "Luciana Tieppo" });
                    quienApruebas.Add(new QuienAprueba() { ID = 5, Name = "Pablo Droguett" });
                };
                foreach (var name in quienApruebas)
                {
                    quienAprueba.Items.Add(name.Name);
                }
                resultado = "S";
            }
            catch (Exception)
            {
                resultado = "N";
            }
        }
        private void btnDesplegarProd_Clicked(object sender, EventArgs e)
        {
            if (txtClasePrecio.Text == null || txtClasePrecio.Text == "")
            {
                DisplayAlert("Alerta", "Debe Buscar y Seleccionar Cliente", "Aceptar");
            }
            else
            {
                gvAgregarProductos.IsVisible = true;
                gvAgregarPedidos.IsVisible = false;
                cmdGuardar.IsVisible = false;
                btnLimpiar.IsVisible = false;
                BindingContext = this;
                btnDesplegarProd.IsVisible = false;
                btnCerrarDesplegar.IsVisible = true;
            }
        }
        private void btnCerrarDesplegar_Clicked(object sender, EventArgs e)
        {
            gvAgregarProductos.IsVisible = false;
            gvAgregarPedidos.IsVisible = true;
            cmdGuardar.IsVisible = true;
            btnLimpiar.IsVisible = true;
            btnDesplegarProd.IsVisible = true;
            btnCerrarDesplegar.IsVisible = false;
        }
        private void btnBuscarprod_Clicked(object sender, EventArgs e)
        {
            if (bscProducto.Text == "" || bscProducto.Text == null)
                DisplayAlert("Error", "Debe ingresar un producto para buscar", "Aceptar");
            else
            {
                if (bscProducto.CursorPosition <= 4)
                    DisplayAlert("Error", "Debe ingresar al menos 5 letras", "Aceptar");
                else
                {
                    cargarBoxProductos(bscProducto.Text.Trim());
                    lstProd.IsVisible = true;
                    lstProd.HeightRequest = (contador <= 2) ? (88 * contador) : 166;
                }
            }
        }
        public void cargarBoxProductos(string Codigo)
        {
            try
            {
                ListadoProductosAPI.Clear();
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.obtenerProductos(Codigo));
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());

                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        ProductosAPI rm = CompletarInformacion(item);
                        ListadoProductosAPI.Add(rm);
                        contador++;
                    }

                    lstProd.HeightRequest = 88 * contador;
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
        private void btnLimpiarBuscarProd_Clicked(object sender, EventArgs e)
        {
            LimpiarAgregarProductos();
        }
        private void LimpiarAgregarProductos()
        {
            bscProducto.Text = "";
            txtNrolinea.Text = "";
            txtCantidad.Text = "";
            txtTotal.Text = "";
            txtPrecioUnitario.Text = "";
            txtStock.Text = "";
            bscProducto.IsEnabled = true;
            txtNrolinea.IsEnabled = true;
            txtCantidad.IsEnabled = true;
            txtTotal.IsEnabled = true;
            txtPrecioUnitario.IsEnabled = true;
            txtStock.IsEnabled = true;
        }
        private void lstProd_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            bscProducto.IsEnabled = false;
            bscProducto.IsEnabled = false;
            txtTotal.IsEnabled = false;
            txtPrecioUnitario.IsEnabled = false;
            txtStock.IsEnabled = false;
            var item = (ProductosAPI)e.SelectedItem;
            var invtID = item.INVTID;
            cargarDatosProductos(invtID, txtClasePrecio.Text);
        }
        public void cargarDatosProductos(string InvtID, string ClasePrecio)
        {
            try
            {
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.obtenerdatosProductos(InvtID, ClasePrecio));
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());

                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        Productos rm = CompletarInformacionProd(item);
                        CompletarInformacionProductos(rm);
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
        private Productos CompletarInformacionProd(JObject item) => new Productos
        {
            ID = item.GetValue("InvtId").ToString(),
            PrecioUnitario = (int)item.GetValue("Precio"),
            Stock = (int)item.GetValue("QtyAvail")
        };
        private void CompletarInformacionProductos(Productos p)
        {

            bscProducto.Text = p.ID.ToString();
            txtPrecioUnitario.Text = p.PrecioUnitario.ToString();
            txtStock.Text = p.Stock.ToString();
            lstProd.IsVisible = false;
        }
    }
}