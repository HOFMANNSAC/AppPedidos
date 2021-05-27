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
using AppPedidos.Apps.Data;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace AppPedidos.Apps.Views.Admin
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealizarPedidos : ContentPage
    {     
        ObservableCollection<MediaModel> Photos = new ObservableCollection<MediaModel>();
        public ObservableCollection<Customer> ListadoClientes { get; private set; }
        public ObservableCollection<Pedido> ListadoDireccion { get; private set; }

        public int contador = 0;
        public int contadorpedido = 0;
        public string ClasePrecio = "";
        public string custOrdNbr = "";
        int nroLinea = 0;
        public static BDLocal BD;
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public ObservableCollection<ProductosAPI> ListadoProductos { get; private set; }
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

        public RealizarPedidos()
        {
            InitializeComponent();
            ListaProductos = new ObservableCollection<Productos>();
            ListaProductos.Clear();
            modalAgregar.IsVisible = false;
            ListadoProductos = new ObservableCollection<ProductosAPI>();
            cargarProductos();
            cmdGuardar.Clicked += CmdGuardar_Clicked;
            cargarBoxquienAprueba();
            cargarBoxTipoPedido();
            ListadoClientes = new ObservableCollection<Customer>();
            ListadoDireccion = new ObservableCollection<Pedido>();
            BindingContext = this;
        }
        private void CmdGuardar_Clicked(object sender, EventArgs e)
        {
            string resultado = "";
            Pedido pe = new Pedido();
            MetodosApi api = new MetodosApi();
            var usuarioCreacion = Application.Current.Properties["usuarioSistema"];
            contadorpedido = ListaProductos.Count;
            var totalOrden = 0;
            bool chekReqDescuento = false;
            bool chekRetiroDrogueria = false;
            if (chkReqDescuento.IsChecked)
            {
                chekReqDescuento = true;
            }
            if (chkRetiroDrogueria.IsChecked)
            {
                chekRetiroDrogueria = true;
            }
            try
            {
                pe.CustID = txtCliente.Text;
                pe.SHipToId = custOrdNbr;
                pe.CustOrdNbr = txtNroOC.Text;
                pe.UsuarioCrea = (string)usuarioCreacion;
                pe.TipoPedido = (string)tipoPedido.SelectedItem;
                pe.ObsGeneral = txtObsGeneral.Text;
                pe.ReqDescuento = chekReqDescuento;
                pe.QuienAprueba = (string)quienAprueba.SelectedItem;
                pe.ObsDescuento = txtObsDescuento.Text;
                pe.TotalOrden = "";
                pe.NroProductos = contadorpedido.ToString();
                pe.RetiroDrogueria = chekRetiroDrogueria;
                resultado = "S";
                if (resultado == "S")
                {
                    api.InsertarPedidos(pe);
                }
                else
                {
                    resultado = "N";
                }
                if (contadorpedido!=0)
                {

                    AgregarProducto();
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        private  void cmdAgregar_Clicked_1(object sender, EventArgs e)
        {
            if (txtClasePrecio.Text!=null)
            {

                AgregarProductos.IsVisible = true;
                AgregarPedido.IsVisible = false;
                BindingContext = this;
            }
            else
            {
                DisplayAlert("Alerta","Debe Seleccionar Cliente","Aceptar");
            }
          
        }
        private void tipoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void quienAprueba_SelectedIndexChanged(object sender, EventArgs e)
        {

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
        public void cargarClientes(string name)
        {
            try
            {
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.CargarCLientes(name));
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());

                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        Customer p = CompletarInformacionClientes(item);
                        CompletarDatosListas(p);
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
        public void cargarDatosCliente(string idCliente)
        {
            MetodosApi api = new MetodosApi();
            var respuesta = JArray.Parse(api.ObtenerDatosClientes(idCliente));
            if (respuesta[0].ToString()=="S")
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
        private Customer CompletarInformacionClientes(JObject item) => new Customer
        {
            IDCliente = item.GetValue("custid").ToString(),
            NombreCliente = item.GetValue("name").ToString()

        };
        private Pedido CompletarInformacionDatosClientes(JObject item) => new Pedido
        {
           clasePrecio = item.GetValue("ClasePrecio").ToString(),
           estadoCliente = item.GetValue("Status").ToString(),
           formaPago=item.GetValue("FormaPAgo").ToString(),
           correElectronico=item.GetValue("EMailAddr").ToString()

           
        };
        private Pedido DatoDireccionCliente(JObject item) => new Pedido
        { 
            Local = item.GetValue("ShipToID").ToString(),
            direccionCliente = item.GetValue("Descr").ToString()
        };
        private void CompletarDatosDireccion(Pedido pe)
        {
            ListadoDireccion.Add(pe);

        }
        private void CompletarDatosListas(Customer p)
        {
            ListadoClientes.Add(p);
        }
        private void CompletarDatoscliente(Pedido p) 
        {
            txtClasePrecio.Text = p.clasePrecio.Substring(0,2);
            txtEstadoCliente.Text = p.estadoCliente;
            txtEstadoCredito.Text = p.formaPago;
            txtCorreo.Text = p.correElectronico;
            Application.Current.Properties["claseprecio"] = p.clasePrecio.Substring(0,2);
            txtClasePrecio.IsEnabled = false;
            txtEstadoCliente.IsEnabled = false;
            txtEstadoCredito.IsEnabled = false;
            txtCorreo.IsEnabled = false;

        }
        private void agregarCliente_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void lstCLiente_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (Customer)e.SelectedItem;
            cargarDatosCliente(item.IDCliente);
            txtCliente.Text = item.NombreCliente;
            lstclientes.IsVisible = false;
        }

        private void bsrCliente_Clicked(object sender, EventArgs e)
        {
            cargarClientes(txtCliente.Text.Trim());
            lstclientes.IsVisible = true;
        }

        private void lstDireccionCliente_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (Pedido)e.SelectedItem;
            txtDireccion.Text = item.direccionCliente;
            custOrdNbr = item.Local;
            lstDireccion.IsVisible = false;
        }

        private void chkRetiroDrogueria_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (chkRetiroDrogueria.IsChecked)
            {
                lstDireccion.IsVisible = true;
            }
            else
            {
                lstDireccion.IsVisible = false;
            }
        }

        /// AGREGAR PRODUCTOS
        #region AGREGAR PRODUCTOS
        private void cargarProductos()
        {
            Productos p = new Productos() { nroLinea = 1, ID = "TEXBAB01", Cantidad = 1, Total = 500, PrecioUnitario = 500, Stock = 115 };
            ListaProductos.Add(p);
        }
        private void Lista_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                lstProd.IsVisible = true;
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
            agregarTablaProductos();
            contadorpedido++;
        }
        private void AgregarProducto()
        {
            try
            {
                contador = 0;
            MetodosApi api = new MetodosApi();
            var respuesta = string.Empty;

            string codigo = txtCodigo.Text;
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            int precioUnitario = Convert.ToInt32(txtPrecioUnitario.Text);
            int stock = Convert.ToInt32(txtStock.Text);
            int noLinea = ListaProductos.Count + 1;
            Productos addproductos = new Productos() {nroLinea=noLinea, ID = codigo, Cantidad = cantidad, PrecioUnitario = precioUnitario, Stock = stock, Total = precioUnitario * cantidad };
                respuesta = "S";
           
                if (respuesta == "S")
                {
                    api.guardarDetallePedido(addproductos, noLinea);
                }
                else
                    DisplayAlert("Error", "Ha ocurrido un error", "OK");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void agregarTablaProductos()
        {
            string codigo = txtCodigo.Text;
            int cantidad = Convert.ToInt32(txtCantidad.Text);
            int precioUnitario = Convert.ToInt32(txtPrecioUnitario.Text);
            int stock = Convert.ToInt32(txtStock.Text);
            Productos addproductos = new Productos() { ID = codigo, Cantidad = cantidad, PrecioUnitario = precioUnitario, Stock = stock, Total = precioUnitario * cantidad };
            ListaProductos.Add(addproductos);

            BindingContext = this;
            DisplayAlert("Mensaje", "Producto Agregado", "Aceptar");
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
                    DisplayAlert("Mensaje", "Producto Modificado (Se ha sumado la suma de '" + cantidad + "' a la cantidad del producto )", "Aceptar");
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
        public void cargarBoxProductos(string Codigo)
        {
            try
            {
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.obtenerProductos(Codigo));
                if (respuesta[0].ToString() == "S")
                {
                    JArray jsonString = JArray.Parse(respuesta[1].ToString());

                    foreach (JObject item in jsonString.OfType<JObject>())
                    {
                        ProductosAPI rm = CompletarInformacion(item);
                        CompletarDatosListas(rm);
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
        private ProductosAPI CompletarInformacion(JObject item) => new ProductosAPI
        {
            INVTID = item.GetValue("INVTID").ToString(),
            DESCRIPCION = item.GetValue("DESCRIPCION").ToString()
        };
        private Productos CompletarInformacionProd(JObject item) => new Productos
        {
            ID = item.GetValue("InvtId").ToString(),
            PrecioUnitario = (int)item.GetValue("Precio"),
            Stock = (int)item.GetValue("QtyAvail")
        };
        private void CompletarInformacionProductos(Productos p)
        {
            lblCodigo.IsVisible = true;
            txtCodigo.IsVisible = true;
            txtCodigo.Text = p.ID.ToString();
            txtPrecioUnitario.Text = p.PrecioUnitario.ToString();
            txtStock.Text = p.Stock.ToString();
            lstProductos.IsVisible = false;
        }
        private void CompletarDatosListas(ProductosAPI pa)
        {
            ListadoProductos.Add(pa);
            contador++;
        }
        private void PCKproducto_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            AgregarPedido.IsVisible = true;
            AgregarProductos.IsVisible = false;
        }
        private void lstProd_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
                var item = (ProductosAPI)e.SelectedItem;
                var invtID = item.INVTID;
                cargarDatosProductos(invtID, txtClasePrecio.Text);
        }

        private void btnBuscarprod_Clicked(object sender, EventArgs e)
        {
            cargarBoxProductos(bscProducto.Text.Trim());
            lstProductos.IsVisible = true;
        }

        private void cmdCerrar_Clicked(object sender, EventArgs e)
        {
            AgregarProductos.IsVisible = false;
        }
    }
    #endregion
}
