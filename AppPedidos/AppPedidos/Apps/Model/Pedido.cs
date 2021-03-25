using System;
using System.Collections.Generic;
using System.Text;

namespace AppPedidos.Apps.Model
{
    public class Pedido
    {
        public string ID { get; set; }
        public string CustID { get; set; }
        public string SHipToId { get; set; }
        public string CustOrdNbr { get; set; }
        public string UsuarioCrea { get; set; }
        public string TipoPedido { get; set; }
        public string ObsGeneral { get; set; }
        public string ReqDescuento { get; set; }
        public string QuienAprueba { get; set; }
        public string ObsDescuento { get; set; }
        public string TotalOrden { get; set; }
        public string NroProductos { get; set; }
        public string RetiroDrogueria { get; set; }
    }
}
