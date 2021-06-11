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
        public string custOrdNbr = "";
        public ObservableCollection<Productos> ListaProductos { get; set; }
        public string codigoPrduc { get; set; }
        public int cantidadPrduc { get; set; }
        public int totalPrduc { get; set; }
        public int precioUnitario { get; set; }
        public int Stock { get; set; }
        int nroLinea = 0;
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
        public RealizarPedidos2()
        {
            InitializeComponent();
            cargarBoxquienAprueba();
            cargarBoxTipoPedido();
            ListadoProductosAPI = new ObservableCollection<ProductosAPI>();
            ListadoClientes = new ObservableCollection<Customer>();
            ListadoDireccion = new ObservableCollection<Pedido>();
            ListaProductos = new ObservableCollection<Productos>();
            txtDireccion.Text = "DEFAULT";
            BindingContext = this;
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
        /// <summary>
        /// Cargar productos y datos productos
        /// </summary>
        #region
        private void btnBuscarprod_Clicked(object sender, EventArgs e)
        {
            if (bscProducto.Text == "" || bscProducto.Text == null)
                DisplayAlert("Alerta", "Debe ingresar un producto para buscar", "Aceptar");
            else
            {
                if (bscProducto.CursorPosition <= 4)
                    DisplayAlert("Alerta", "Debe ingresar al menos 5 letras", "Aceptar");
                else
                {
                    cargarBoxProductos(bscProducto.Text.Trim(), txtClasePrecio.Text);
                    lstProd.IsVisible = true;
                    lstProd.HeightRequest = (contador <= 2) ? (88 * contador) : 166;
                }
            }
        }
        public void cargarBoxProductos(string Codigo,string ClasePrecio)
        {
            try
            {
                ListadoProductosAPI.Clear();
                contador = 0;
                MetodosApi api = new MetodosApi();
                var respuesta = JArray.Parse(api.obtenerProductos(Codigo,ClasePrecio));
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
                    DisplayAlert("Alerta", "No existen productos asociadas", "OK");
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
                    DisplayAlert("Alerta", "No existen productos asociadas", "OK");
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
        #endregion
        /// <summary>
        /// Agregar pedido y productos a BD, enviando datos a la API
        /// </summary>
        #region
        private void AgregarPedido()
        {
            string resultado = "";
            Pedido pe = new Pedido();
            MetodosApi api = new MetodosApi();
            var usuarioCreacion = Application.Current.Properties["usuarioSistema"];
            contador = ListaProductos.Count;
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
                pe.NroProductos = contador.ToString();
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
                if (contador != 0)
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
                    int noLinea = item.nroLinea;
                    Productos addproductos = new Productos() { nroLinea = noLinea, ID = codigo, Cantidad = cantidad, PrecioUnitario = precioUnitario, Stock = stock, Total = precioUnitario * cantidad };
                    respuesta = "S";

                    if (respuesta == "S")
                    {
                        api.guardarDetallePedido(addproductos, noLinea);
                    }
                    else
                        DisplayAlert("Alerta", "Ha ocurrido un error", "OK");
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        /// <summary>
        /// Cargar clientes, datos clientes y direccion cliente
        /// </summary>
        #region
        private void bsrCliente_Clicked(object sender, EventArgs e)
        {
            if (txtCliente.Text == null || txtCliente.Text == "")
                DisplayAlert("Alerta", "Debe ingresar un cliente ", "Aceptar");
            else
            {
                cargarClientes(txtCliente.Text.Trim());
                lstCLiente.IsVisible = true;
                lstCLiente.HeightRequest = (contador <= 2) ? (88 * contador) : 166;
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
                    DisplayAlert("Alerta", "No existen clientes asociadas", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("Alerta", "Cliente no existe", "OK");
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
        #endregion
        /// <summary>
        /// Agregar Productos en tabla, (temporal)
        /// </summary>
        #region 

        private void agregarTablaProductos()
        {
            if (bscProducto.Text == null || bscProducto.Text=="")
            {
                DisplayAlert("Alerta", "Debe buscar producto", "Aceptar");
            }
            else
            {
                if (txtCantidad.Text == null || txtCantidad.Text == "")
                {
                    DisplayAlert("Alerta", "Debe indicar cantidad", "Aceptar");
                }
                else
                {
                    if (txtNrolinea.Text == "" || txtNrolinea.Text == null)
                    {
                        DisplayAlert("Alerta", "Debe indicar Nro de linea", "Aceptar");
                    }
                    else
                    {

                        string codigo = bscProducto.Text;
                        int cantidad = Convert.ToInt32(txtCantidad.Text);
                        int PrecioUnitario = Convert.ToInt32(txtPrecioUnitario.Text);
                        int stock = Convert.ToInt32(txtStock.Text);
                        int nroLinea = Convert.ToInt32(txtNrolinea.Text);
                        Productos addproductos = new Productos() { nroLinea = nroLinea, ID = codigo, Cantidad = cantidad, PrecioUnitario = PrecioUnitario, Stock = stock, Total = PrecioUnitario * cantidad };
                        string id = "";
                        foreach (var item in ListaProductos)
                        {
                            id = item.ID;
                        }
                        if (id == codigo)
                        {
                            DisplayAlert("Alerta", "Producto ya existe", "Aceptar");
                        }
                        else
                        {
                            ListaProductos.Add(addproductos);

                            BindingContext = this;
                            DisplayAlert("Alerta", "Producto Agregado", "Aceptar");
                            LimpiarAgregarProductos();
                        }
                    }
                }
            }
       
        }
        private void btnGuardarProd_Clicked(object sender, EventArgs e)
        {
            agregarTablaProductos();
            contador++;
        }
        #endregion
        /// <summary>
        /// Editar Cantidad Productos
        /// </summary>
        #region
        private void EditarProducto()
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
                        btnEditar.IsVisible = false;
                        btnGuardarProd.IsVisible = true;
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
                        btnEditar.IsVisible = false;
                        btnGuardarProd.IsVisible = true;
                        LimpiarAgregarProductos();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
        private void limpiarPedido()
        {
            txtCliente.Text = "";
            txtClasePrecio.Text = "";
            txtEstadoCliente.Text = "";
            txtEstadoCredito.Text = "";
            ListadoDireccion.Clear();
            tipoPedido.SelectedIndex = -1;
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
            txtNrolinea.IsEnabled = true;
            txtCantidad.IsEnabled = true;
            txtTotal.IsEnabled = true;
            txtPrecioUnitario.IsEnabled = true;
            txtStock.IsEnabled = true;
        }
        private void cmdGuardar_Clicked(object sender, EventArgs e)
        {
            if (txtCliente.Text == "" || txtCliente.Text == null)
            {
                DisplayAlert("Alerta", "Debe buscar y seleccionar un cliente", "Aceptar");
            }
            else
            {
                AgregarPedido();
                limpiarPedido();
                ListaProductos.Clear();
            }
        }

        private void btnLimpiarBuscar_Clicked(object sender, EventArgs e)
        {
            txtCliente.Text = "";
            ListadoClientes.Clear();
            limpiarPedido();
        }

        private void chkRetiroDrogueria_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (txtCliente.Text==""|| txtCliente.Text==null)
            {
                chkRetiroDrogueria.IsChecked = true;
                DisplayAlert("Alerta","Debe buscar un cliente","Aceptar");
            }
            else
            {
                if (chkRetiroDrogueria.IsChecked==false)
                {
                    lstDireccion.IsVisible = true;
                }
                else
                {
                    lstDireccion.IsVisible = false;
                    txtDireccion.Text = "DEFAULT";
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
        private void Eliminar_Tapped(object sender, EventArgs e)
        {
            
            var imagen = sender as Image;
            var producto = imagen?.BindingContext as Productos;
            var vm = BindingContext as RealizarPedidos2;
            vm?.RemoveCommand.Execute(producto);

            DisplayAlert("Alerta","Producto eliminado ","Aceptar");
        }
        private void lstProductos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                lstProd.IsVisible = true;
                lstProd.HeightRequest = 0;
                btnGuardarProd.IsVisible = false;
                btnEditar.IsVisible = true;
                ListadoProductosAPI.Clear();
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
                txtNrolinea.Text = nroLinea.ToString();
                txtNrolinea.IsEnabled = false;
                txtCantidad.Text = cantidadPrduc.ToString();
                var TotalProductos = totalPrduc * cantidadPrduc;
                txtTotal.Text = TotalProductos.ToString();
                txtTotal.IsEnabled = false;
                txtPrecioUnitario.Text = precioUnitario.ToString();
                txtPrecioUnitario.IsEnabled = false;
                txtStock.Text = Stock.ToString();
                txtStock.IsEnabled = false;
                btnEditar.IsVisible = true;
                btnGuardarProd.IsVisible = false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private void btnCancelar_Clicked(object sender, EventArgs e)
        {
            LimpiarAgregarProductos();
        }
        private void btnEditar_Clicked(object sender, EventArgs e)
        {
            EditarProducto();
        }

        private void txtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int cantidad = Convert.ToInt32(txtCantidad.Text);
                int precio = Convert.ToInt32(txtPrecioUnitario.Text);
                int total = cantidad * precio;
                txtTotal.Text = total.ToString();
            }
            catch (Exception ex)
            {
                txtTotal.Text = "0";
            }
        }

        private void btnLimpiar_Clicked(object sender, EventArgs e)
        {
            limpiarPedido();
        }
    }
}