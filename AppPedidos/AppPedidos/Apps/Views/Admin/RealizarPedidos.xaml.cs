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
        ObservableCollection<MediaModel> Photos = new ObservableCollection<MediaModel>();
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
            cmdGuardar.Clicked += CmdGuardar_Clicked;
            cargarBoxquienAprueba();
            cargarBoxTipoPedido();
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
                pe.SHipToId = cboDirecDespacho.ToString();
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
            cargarDropProdocutos();
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
                    ListaProductos.Remove(productosSelec);

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
        private void cargarDropProdocutos()
        {            
            MetodosApi api = new MetodosApi();
            var respuesta = JArray.Parse(api.obtenerProductos());
            if (respuesta[0].ToString() == "S")
            {
                JArray jsonString = JArray.Parse(respuesta[1].ToString());

                foreach (JObject item in jsonString.OfType<JObject>())
                {
                    //Si no esta agregada la ruta lo incluimos
                    Productos p = CompletarInformacion(item);
                    agregarProducto.Items.Add(p.Codigo);
                }
            }
            else
                DisplayAlert("Error", "No existen productos ", "OK");
        }
        private Productos CompletarInformacion(JObject item) => new Productos
        {
         Codigo = item.GetValue("Codigo").ToString(),
         Cantidad = Convert.ToInt32(item.GetValue("Cantidad").ToString()),
         Total = Convert.ToInt32(item.GetValue("Total").ToString())
        };
        private async void selecArchivo_Clicked(object sender, EventArgs e)
        {
            var isInitialize = await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.IsSupported || !isInitialize)
            {
                await DisplayAlert("Error", "No cuenta con permisos para seleccionar una foto", "OK");
                return;
            }
            var newPhotoID = Guid.NewGuid();
            using (var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
            {
                Name = newPhotoID.ToString(),
                SaveToAlbum = true,
                DefaultCamera = CameraDevice.Front,
                Directory = "Camera",
                CustomPhotoSize = 50,
                PhotoSize = PhotoSize.Small,
                CompressionQuality = 50
            }))
            {
                if (string.IsNullOrWhiteSpace(photo?.Path))
                {
                    return;
                }
                var newPhotoMedia = new MediaModel()
                {
                    MediaID = newPhotoID,
                    Path = photo.Path,
                    LocalDateTime = DateTime.Now
                };
                Photos.Add(newPhotoMedia);
                photo.Dispose();
            }
            listPhotos.ItemsSource = Photos;
        }

        private void agregarProducto_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}