using System;
using System.Collections.Generic;
using System.Text;

namespace AppPedidos.Apps.Model
{
    public class Productos
    {
        public int nroLinea { get; set; }
        public string ID { get; set; }
        public string nroPedido { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }
        public int RestriccionVenta { get; set; }
        public int Presentacion { get; set; }
        public int Stock { get; set; }
        public int Total { get; set; }
    }
}
