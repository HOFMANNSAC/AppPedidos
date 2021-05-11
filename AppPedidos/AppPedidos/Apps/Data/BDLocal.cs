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
        public ObservableCollection<Productos> obtenerProductos(string Codigo)
        {
            return new ObservableCollection<Productos>(this.Table<Productos>().Where(t => t.ID == Codigo).ToList());
        }
        public Boolean ExisteProducto(string id)
        {
            try
            {
                Productos rm = Table<Productos>().Where(t => t.ID == id).FirstOrDefault();
                return rm != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void AgregarProd(Productos productos)
        {
            this.Insert(productos);
        }
        public void ActualizarProd(Productos productos)
        {
            this.Update(productos);
        }

    }
}
