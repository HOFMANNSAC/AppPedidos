using System;
using System.Text;
using System.Collections.Generic;

namespace AppPedidos.Apps.Model
{
    public class Pedido
    {
        public string ID { get; set; }
        public string CustID { get; set; }
        public string  Name { get; set; }
        public string SHipToId { get; set; }
        public string clasePrecio { get; set; }
        public string formaPago { get; set; }
        public string  estadoCliente { get; set; }
        public string  correElectronico { get; set; }
        public string CustOrdNbr { get; set; }
        public string UsuarioCrea { get; set; }
        public string  direccionCliente { get; set; }
        public string Local { get; set; }
        public string TipoPedido { get; set; }
        public string ObsGeneral { get; set; }
        public bool ReqDescuento { get; set; }
        public string QuienAprueba { get; set; }
        public string ObsDescuento { get; set; }
        public string TotalOrden { get; set; }
        public string NroProductos { get; set; }
        public bool RetiroDrogueria { get; set; }
    }
}
