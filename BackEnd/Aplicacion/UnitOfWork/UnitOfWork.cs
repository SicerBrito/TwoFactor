using Aplicacion.Repository;
using Dominio.Entities;
using Dominio.Interfaces;
using Persistencia.Data;

namespace Aplicacion.UnitOfWork;
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbAppContext ? _Context;
        public UnitOfWork(DbAppContext context){
            _Context = context;
        }

        private UsuarioRepository ? _Usuario;
        private RolRepository ? _Rol;

        public IUsuario? Usuarios => _Usuario ??= new UsuarioRepository(_Context!);

        public IRol? Roles => _Rol ??= new RolRepository(_Context!);

        public void Dispose(){
            _Context!.Dispose();
            GC.SuppressFinalize(this); 
        }

        public Task<int> SaveAsync(){
            return _Context!.SaveChangesAsync();
        }
    }
