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

        public int IdProducto = 0;
        public int contador = 0;
        public int contadorpedido = 0;
        public string ClasePrecio = "";
        public string custOrdNbr = "";
        int nroLinea = 0;
        public static BDLocal BD;
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public ObservableCollection<ProductosAPI> ListadoProductosAPI { get; private set; }
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
            mdlVerProductos.IsVisible = false;
            ListadoProductosAPI = new ObservableCollection<ProductosAPI>();
            cmdGuardar.Clicked += CmdGuardar_Clicked;
            cargarBoxquienAprueba();
            cargarBoxTipoPedido();
            ListadoClientes = new ObservableCollection<Customer>();
            ListadoDireccion = new ObservableCollection<Pedido>();
            BindingContext = this;
        }
        private void CmdGuardar_Clicked(object sender, EventArgs e)
        {
            if (txtCliente.Text=="" || txtCliente.Text==null)
            {
                DisplayAlert("Error","Debe buscar y seleccionar un cliente","Aceptar");
            }
            else
            {
                AgregarPedido();
                limpiarPedido();              
            }
         
        }
        private void limpiarPedido()
        {
            txtCliente.Text = "";
            txtClasePrecio.Text = "";
            txtEstadoCliente.Text = "";
            txtEstadoCredito.Text = "";
            chkRetiroDrogueria.IsChecked = false;
            ListadoDireccion.Clear();
            txtDireccion.Text = "";
            tipoPedido.SelectedIndex= -1;
            txtNroOC.Text = "";
            txtCorreo.Text = "";
            txtObsGeneral.Text = "";
            chkReqDescuento.IsChecked = false;
            quienAprueba.SelectedIndex = -1;
            txtObsDescuento.Text = "";
            txtCliente.IsEnabled = true;
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
            txtNrolinea.IsEnabled =true;
            txtCantidad.IsEnabled=true;
            txtTotal.IsEnabled=true;
            txtPrecioUnitario.IsEnabled = true;
            txtStock.IsEnabled = true; 
        }

        private void AgregarPedido()
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
            if (chkRetiroDrogueria.IsChecked == false)
            {
                txtDireccion.Text = "DEFAULT";

            }

            foreach (var item in ListaProductos)
            {
                totalOrden = +0;
                totalOrden = item.Total;
            }
            try
            {
                pe.CustID = txtCliente.Text;
                pe.SHipToId = txtDireccion.Text;
                pe.CustOrdNbr = txtNroOC.Text;
                pe.UsuarioCrea = (string)usuarioCreacion;
                pe.TipoPedido = (string)tipoPedido.SelectedItem;
                pe.ObsGeneral = txtObsGeneral.Text;
                pe.ReqDescuento = chekReqDescuento;
                pe.QuienAprueba = (string)quienAprueba.SelectedItem;
                pe.ObsDescuento = txtObsDescuento.Text;
                pe.TotalOrden = totalOrden.ToString();
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
                if (contadorpedido != 0)
                {

                    AgregarProducto();
                }
                DisplayAlert("Alerta", "Pedido Ingresado Correctamente", "OK");
            }
            catch (Exception)
            {
                return;
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
            ListadoClientes.Clear();
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
            txtCliente.IsEnabled = false;
        }

        private void bsrCliente_Clicked(object sender, EventArgs e)
        {
            if (txtCliente.Text == null || txtCliente.Text == "")
            {

                DisplayAlert("Error", "Debe ingresar un cliente ", "Aceptar");
            }
            else 
            {
                if (txtCliente.CursorPosition <= 4 )
                {
                    DisplayAlert("Error", "Debe ingresar al menos 5 letras ", "Aceptar");
                }
                else
                {
                    cargarClientes(txtCliente.Text.Trim());
                    lstclientes.IsVisible = true;
                    lstclientes.HeightRequest = 150;
                }
           
            }    
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
        //private void cargarProductos()
        //{
        //    Productos p = new Productos() { nroLinea = 1, ID = "TEXBAB01", Cantidad = 1, Total = 500, PrecioUnitario = 500, Stock = 115 };
        //    ListaProductos.Add(p);
        //}
        private void lstProductos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                lstProd.IsVisible = true;
                btnGuardarProd.IsVisible = false;
                btnEditar.IsVisible = true;
                mdlVerProductos.IsVisible = true;
                //Datos Tabla LISTA
                var item = (Productos)e.SelectedItem;
                codigoPrduc = item.ID.ToString();
                cantidadPrduc = item.Cantidad;
                totalPrduc = Convert.ToInt32(item.Total);
                nroLinea = item.nroLinea;
                precioUnitario = item.PrecioUnitario;
                Stock = item.Stock;

                bscProducto.Text = codigoPrduc;
                bscProducto.IsEnabled = false;
                txtCantidad.Text = cantidadPrduc.ToString();
                var TotalProductos = totalPrduc * cantidadPrduc;
                txtTotal.Text = TotalProductos.ToString();
                txtTotal.IsEnabled = false;
                txtPrecioUnitario.Text = precioUnitario.ToString();
                txtPrecioUnitario.IsEnabled = false;
                txtStock.Text = Stock.ToString();
                txtStock.IsEnabled = false;
                btnAgregarProducto.IsVisible = false;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void agregarProducto_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                foreach (var item in ListaProductos)
                {
                    string codigo = item.ID;
                    int cantidad = item.Cantidad;
                    int precioUnitario = item.PrecioUnitario;
                    int stock = item.Stock;
                    int noLinea =item.nroLinea;
                    Productos addproductos = new Productos() { nroLinea = noLinea, ID = codigo, Cantidad = cantidad, PrecioUnitario = precioUnitario, Stock = stock, Total = precioUnitario * cantidad };
                    respuesta = "S";

                    if (respuesta == "S")
                    {
                        api.guardarDetallePedido(addproductos, noLinea);
                    }
                    else
                        DisplayAlert("Error", "Ha ocurrido un error", "OK");
                }
     
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void agregarTablaProductos()
        {
            if (txtCantidad.Text== null || txtCantidad.Text=="")
            {
                DisplayAlert("Error", "Debe indicar cantidad", "Aceptar");
            }
            else
            {
                if (txtNrolinea.Text=="" || txtNrolinea.Text==null)
                {
                    DisplayAlert("Error", "Debe indicar Nro de linea", "Aceptar");
                }
                else
                {

                    string codigo = bscProducto.Text;
                    int cantidad = Convert.ToInt32(txtCantidad.Text);
                    int PrecioUnitario = Convert.ToInt32(txtPrecioUnitario.Text);
                    int stock = Convert.ToInt32(txtStock.Text);
                    int nroLinea = Convert.ToInt32(txtNrolinea.Text);
                    Productos addproductos = new Productos() { nroLinea=nroLinea, ID = codigo, Cantidad = cantidad, PrecioUnitario = PrecioUnitario, Stock = stock, Total = PrecioUnitario * cantidad };
                    string id = "";
                    foreach (var item in ListaProductos)
                    {
                        id = item.ID;
                    }
                    if (id == codigo)
                    {
                        DisplayAlert("Error", "Producto ya existe", "Aceptar");
                    }
                    else
                    {
                        ListaProductos.Add(addproductos);

                        BindingContext = this;
                        DisplayAlert("Mensaje", "Producto Agregado", "Aceptar");
                        mdlVerProductos.IsVisible = false;
                        btnAgregarProducto.IsVisible = true;
                        mdlVerProductos.IsVisible = false;
                        LimpiarAgregarProductos();
                    }
                }
            }
        }
        private void Eliminar_Tapped(object sender, EventArgs e)
        {
            var imagen = sender as Image;
            var producto = imagen?.BindingContext as Productos;
            var vm = BindingContext as RealizarPedidos;
            vm?.RemoveCommand.Execute(producto);

        }

        public void cargarBoxProductos(string Codigo)
        {
            try
            {
                ListadoProductosAPI.Clear();
                contador = 0;
                /*
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
                */
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
        
            bscProducto.Text = p.ID.ToString();
            txtPrecioUnitario.Text = p.PrecioUnitario.ToString();
            txtStock.Text = p.Stock.ToString();
            lstProductosAPI.IsVisible = false;
        }
        private void CompletarDatosListas(ProductosAPI pa)
        {
            ListadoProductosAPI.Add(pa);
            contador++;
        }
        private void PCKproducto_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void btnBuscarprod_Clicked(object sender, EventArgs e)
        {
            if (bscProducto.Text == "" || bscProducto.Text == null)
            {
                DisplayAlert("Error","Debe ingresar un producto para buscar","Aceptar");
            }
            else
            {
                if (bscProducto.CursorPosition <= 4)
                {
                    DisplayAlert("Error", "Debe ingresar al menos 4 letras", "Aceptar");
                }
                else
                {
                    cargarBoxProductos(bscProducto.Text.Trim());
                    lstProductosAPI.IsVisible = true;
                }
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
                BindingContext = this;
            }
        }
        private void btnCerrarDesplegar_Clicked(object sender, EventArgs e)
        {
            gvAgregarProductos.IsVisible = false;
            gvAgregarPedidos.IsVisible = true;
        }

        private void btnAgregarProducto_Clicked(object sender, EventArgs e)
        {
            mdlVerProductos.IsVisible = true;
            LimpiarAgregarProductos();
        }

        private void btnEditar_Clicked(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(txtCantidad.Text);

            try
            {
                if (cantidad == 0)
                {
                    DisplayAlert("Mensaje", "Solo se puede Eliminar", "Aceptar");
                }
                else
                {
                    if (cantidad > cantidadPrduc)
                    {
                        Productos prod = new Productos() { nroLinea = nroLinea, ID = codigoPrduc, Cantidad = cantidadPrduc, Total = totalPrduc, PrecioUnitario = precioUnitario, Stock = Stock };
                        Productos addproductos = new Productos() { nroLinea = nroLinea, ID = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = precioUnitario * Convert.ToInt32(txtCantidad.Text) };
                        if (prod.ID == addproductos.ID)
                        {
                            ListaProductos.Clear();

                        }
                        ListaProductos.Add(addproductos);
                        DisplayAlert("Mensaje", "Cantidad Editada", "Aceptar");
                        mdlVerProductos.IsVisible = false;
                        btnAgregarProducto.IsVisible = true;
                        mdlVerProductos.IsVisible = false;
                        LimpiarAgregarProductos();
                    }
                    else
                    {
                        Productos prod = new Productos() { nroLinea = nroLinea, ID = codigoPrduc, Cantidad = cantidadPrduc, Total = totalPrduc, PrecioUnitario = precioUnitario, Stock = Stock };
                        Productos addproductos = new Productos() { nroLinea = nroLinea, ID = codigoPrduc, Cantidad = Convert.ToInt32(txtCantidad.Text), Total = precioUnitario * Convert.ToInt32(txtCantidad.Text) };
                        if (prod.ID == addproductos.ID)
                        {
                            ListaProductos.Clear();

                        }
                        ListaProductos.Add(addproductos);
                        DisplayAlert("Mensaje", "Cantidad Editada", "Aceptar");
                        mdlVerProductos.IsVisible = false;
                        btnAgregarProducto.IsVisible = true;
                        mdlVerProductos.IsVisible = false;
                        LimpiarAgregarProductos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void btnBorrarCampos_Clicked(object sender, EventArgs e)
        {
            LimpiarAgregarProductos();
            mdlVerProductos.IsVisible = false;
        }

        private void btnLimpiar_Clicked(object sender, EventArgs e)
        {
            limpiarPedido();
        }

        private void btnLimpiarBuscar_Clicked(object sender, EventArgs e)
        {
            limpiarPedido();
            ListadoClientes.Clear();
            lstclientes.HeightRequest = 0;
        }

        private void btnLimpiarBuscarProd_Clicked(object sender, EventArgs e)
        {
            LimpiarAgregarProductos();
        }
    }
    #endregion
}
