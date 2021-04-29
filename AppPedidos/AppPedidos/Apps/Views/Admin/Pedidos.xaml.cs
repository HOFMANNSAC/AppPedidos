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
    public partial class Pedidos : ContentPage
    {
        ObservableCollection<MediaModel> Photos = new ObservableCollection<MediaModel>();
        public IList<Productos> ProductosList { get; set; }
        public Pedidos()
        {
            InitializeComponent();
            cargarBoxquienAprueba();
            cmdGuardar.Clicked += CmdGuardar_Clicked;
            cargarBoxTipoPedido();
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
                if (Photos.Count>0)
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
        private void CargarClientes()
        {
            try
            {
                MetodosApi api = new MetodosApi();
                Customer c = new Customer();
                string resultado = string.Empty;
                c.NombreCliente = "";
                resultado = "S";
                if (resultado == "S")
                    api.CargarCLientes();
                resultado = "N";
            }
            catch (Exception)
            {
                return;
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
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
        private void tipoPedido_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void quienaprueba_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}