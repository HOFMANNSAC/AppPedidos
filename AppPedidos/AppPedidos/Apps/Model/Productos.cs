using System;
using System.Collections.Generic;
using System.Text;

namespace AppPedidos.Apps.Model
{
    public class Productos
    {
        public string ID { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public int Total { get; set; }
        public int PrecioUnitario { get; set; }
        public int Stock { get; set; }
    }
}
