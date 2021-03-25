using System;
using SQLite;
using System.Text;
using System.Collections.Generic;

namespace AppPedidos.Apps.Model
{
    [Table("Usuario")]
    public class Usuario
    {
        [PrimaryKey]
        public string ID { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string UsuarioSistema { get; set; }
        public string Password { get; set; }
        public string Estado { get; set; }
    }
}
