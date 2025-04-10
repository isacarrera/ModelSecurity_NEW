using System.Data;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    ///<summary>
    ///Repositorio encargador de la gestion de la entidad Rol en la base de datos
    ///</summary>
    public  class RolData : IData<Rol>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;


        ///<summary>
        ///Constructor que recibe el contexto de la base de datos
        ///</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext"/> Para la conexion con la base de datos.</param>
        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        /// <summary>
        /// Obtiene todos los Rols almacenados en la base de datos LINQ
        /// </summary>
        ///<returns>Lista de Rols</returns>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Set<Rol>()
             .Where(r => r.Active)
             .ToListAsync();
        }


        /// <summary>
        /// Obtiene un Rol específico por su identificación LINQ
        /// </summary>
        /// <param name="id"></param>
        /// <returns>El Rol Obtenido</returns>
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Rol con ID {RolId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Rol en la base de datos LINQ
        /// </summary>
        /// <param name="rol"></param>
        /// <returns>El Rol Creado</returns>
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear Rol: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Actualiza un Rol existente en la base de datos LINQ
        /// </summary>
        /// <param name="rol">Objeto con la informacion actualizada.</param>
        /// <returns>True si la actualizacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar Rol: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un Rol de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Rol a eliminar </param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                _context.Set<Rol>().Remove(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar Rol: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un Rol de manera logica de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Rol a eliminar de manera logica</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            var rol = await _context.Set<Rol>().FindAsync(id);
            if (rol == null)
                return false;

            rol.Active = false; 
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
