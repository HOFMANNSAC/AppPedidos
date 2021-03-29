using System;
using SQLite;
using System.Text;
using AppPedidos.Apps.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AppPedidos.Apps.Data
{
    public class BDLocal : SQLiteConnection
    {
        public BDLocal(string path) : base(path)
        {
            CrearTablas();
        }
        void CrearTablas()
        {
            CreateTable<Usuario>();
        }
        public Usuario ValidarUsuario(string usuario, string password)
        {
            try
            {
                Usuario um = Table<Usuario>().Where(t => t.UsuarioSistema == usuario).Where(t => t.Password == password).FirstOrDefault();
                return um;
            }
            catch (Exception ex)
            {
                return new Usuario();
            }
        }
        public Boolean ExisteUsuario(string id)
        {
            try
            {
                Usuario um = Table<Usuario>().Where(t => t.ID == id).FirstOrDefault();
                return um != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void AgregarUsuario(Usuario usuario)
        {
            this.Insert(usuario);
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            this.Update(usuario);
        }
    }
}
