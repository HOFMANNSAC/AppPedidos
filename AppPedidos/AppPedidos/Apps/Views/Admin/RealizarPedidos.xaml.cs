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
        ObservableCollection<MediaModel> Photos = new ObservableCollection<MediaModel>();
        public ObservableCollection<Customer> ListadoClientes { get; private set; }
        public ObservableCollection<Pedido> ListadoDireccion { get; private set; }
        int contador = 0;

        public RealizarPedidos()
        {
            InitializeComponent();
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
            Usuario u = new Usuario();
            Archivo a = new Archivo();
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
                pe.CustID = "";
                pe.SHipToId = "";
                pe.CustOrdNbr = txtNroOC.Text;
                pe.UsuarioCrea = u.UsuarioSistema;
                pe.TipoPedido = cboTipoPedido.ToString();
                pe.ObsGeneral = txtObsGeneral.Text;
                pe.ReqDescuento = chekReqDescuento;
                pe.QuienAprueba = cboQuienAprueba.ToString();
                pe.ObsDescuento = txtObsDescuento.Text;
                pe.TotalOrden = "";
                pe.NroProductos = "";
                pe.RetiroDrogueria = chekRetiroDrogueria;
                var array = new ArrayList();
                foreach (var item in Photos)
                {
                    string imagen = item.Path;
                    byte[] Convert = File.ReadAllBytes(item.Path);
                    a.NombreArchivo = imagen;
                    a.ByteArchivo = Convert;
                    a.NroPedido = a.NroPedido;
                }
                if (Photos.Count > 0)
                {
                    var respuesta = JArray.Parse(api.InsertarArchivos(a));
                }
                resultado = "S";
                if (resultado == "S")
                {
                    api.InsertarPedidos(pe);
                }
                else
                {
                    resultado = "N";
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        private async void cmdAgregar_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new PaginaMaestra("Productos"));
          
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
                        Customer p = CompletarInformacion(item);
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

        private Customer CompletarInformacion(JObject item) => new Customer
        {
            IDCliente = item.GetValue("custid").ToString(),
            NombreCliente = item.GetValue("name").ToString()
        };
        private Pedido CompletarInformacionDatosClientes(JObject item) => new Pedido
        {
           clasePrecio = item.GetValue("ClasePrecio").ToString()
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
            Application.Current.Properties["claseprecio"] = p.clasePrecio.Substring(0,2);
            txtClasePrecio.IsEnabled = true;
        }
        private void agregarProducto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void agregarCliente_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void lstCLiente_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = (Customer)e.SelectedItem;
            cargarDatosCliente(item.IDCliente);
        }

        private void bsrCliente_Clicked(object sender, EventArgs e)
        {
            cargarClientes(agregarCliente.Text);
            lstclientes.IsVisible = true;
        }

        private void lstDireccionCliente_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

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
    }
}