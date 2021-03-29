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
        public Pedidos()
        {
            InitializeComponent();
            cmdGuardar.Clicked += CmdGuardar_Clicked;
            //CargarClientes();
        }
        private void CmdGuardar_Clicked(object sender, EventArgs e)
        {
                string resultado = "";
                Pedido pe = new Pedido();
                MetodosApi api = new MetodosApi();
                Usuario u = new Usuario();
                Archivo a = new Archivo();

            try
            {
                pe.CustID = (string)cboClientes.SelectedItem;
                pe.SHipToId = (string)cboDirecDespacho.SelectedItem;
                pe.CustOrdNbr = txtNroOC.Text;
                pe.UsuarioCrea = u.UsuarioSistema;
                pe.TipoPedido = (string)cboTipoPedido.SelectedItem;
                pe.ObsGeneral = txtObsGeneral.Text;
                pe.ReqDescuento = chkReqDescuento.ToString();
                pe.QuienAprueba = (string)cboQuienAprueba.SelectedItem;
                pe.ObsDescuento = txtObsDescuento.Text;
                pe.TotalOrden = "";
                pe.NroProductos = "";
                pe.RetiroDrogueria = "";
                var array = new ArrayList();
                foreach (var item in Photos)
                {
                    string imagen = item.Path;
                    byte[] Convert = File.ReadAllBytes(item.Path);
                    a.NombreArchivo = imagen;
                    a.ByteArchivo = Convert;
                    a.NroPedido = a.NroPedido;
                }
                var respuesta = JArray.Parse(api.InsertarArchivos(a));
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
                c.NombreCliente = (string)cboClientes.SelectedItem;
                resultado = "S";
                if (resultado == "S")
                    api.CargarCLientes(c);
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
    }
}