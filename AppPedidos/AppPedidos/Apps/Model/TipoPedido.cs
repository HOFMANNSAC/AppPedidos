using System;
using System.Collections.Generic;
using System.Text;

namespace AppPedidos.Apps.Model
{
   public class TipoPedido
    {
        public int ID { get; set; }
        public string  Name { get; set; }
        public override string ToString()
        {
            return this.ID + "" + this.Name;
        }
    }
}
