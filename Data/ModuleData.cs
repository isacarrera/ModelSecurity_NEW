using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    ///<summary>
    ///Repositorio encargador de la gestion de la entidad Module en la base de datos
    ///</summary>
    public class ModuleData : IData<Module>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de la base de datos
        ///</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext"/> Para la conexion con la base de datos.</param>
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }



        /// <summary>
        /// Obtiene todos los Modules almacenados en la base de datos LINQ
        /// </summary>
        ///<returns>Lista de Modules</returns>
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Set<Module>()
                         .Where(m => m.Active)
                         .ToListAsync();
        }


        /// <summary>
        /// Obtiene un Module específico por su identificacion LINQ
        /// </summary>
        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener un Module con ID {ModuleId}", id);
                throw;
            }
        }




        /// <summary>
        /// Crea un nuevo Module en la base de datos LINQ
        /// </summary>
        /// <param name="module"></param>
        /// <returns>El Module Creado</returns>
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Module: {ex.Message}");
                throw;
            }
        }




        /// <summary>
        /// Actualiza un Module existente en la base de datos LINQ
        /// </summary>
        /// <param name="module">Objeto con la inmoduleación actualizada.</param>
        /// <returns>True si la actualizacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Module: {ex.Message}");
                return false;
            }
        }



        /// <summary>
        /// Elimina un Module de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Module a eliminar </param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Module: {ex.Message}");
                return false;
            }
        }




        /// <summary>
        /// Elimina un Module de manera logica de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Module a eliminar de manera logica</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var module = await GetByIdAsync(id);
                if (module == null)
                {
                    return false;
                }

                module.Active = false;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar de manera logica el Module: {ex.Message}");
                return false;
            }
        }
    }
}