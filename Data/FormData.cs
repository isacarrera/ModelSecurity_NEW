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
    ///Repositorio encargador de la gestion de la entidad Form en la base de datos
    ///</summary>
    public class FormData : IData<Form>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de la base de datos
        ///</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext"/> Para la conexion con la base de datos.</param>
        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }
  

        /// <summary>
        /// Obtiene todos los Forms almacenados en la base de datos LINQ
        /// </summary>
        ///<returns>Lista de Forms</returns>
        public async Task<IEnumerable<Form>> GetAllAsync()
{
            return await _context.Set<Form>()
                         .Where(f => f.Active) 
                         .ToListAsync();
        }


        /// <summary>
        /// Obtiene un Form específico por su identificación LINQ
        /// </summary>
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener un Form con ID {FormId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Form en la base de datos LINQ
        /// </summary>
        /// <param name="form"></param>
        /// <returns>El Form Creado</returns>
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Form: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Actualiza un Form existente en la base de datos LINQ
        /// </summary>
        /// <param name="form">Objeto con la información actualizada.</param>
        /// <returns>True si la actualizacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Form: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un Form de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Form a eliminar </param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var form = await _context.Set<Form>().FindAsync(id);
                if (form == null)
                    return false;

                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Form: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un Form de manera logica de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Form a eliminar de manera logica</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var form = await GetByIdAsync(id);
                if (form == null)
                {
                    return false;
                }

                form.Active = false; 
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar de manera logica el Form: {ex.Message}");
                return false;
            }
        }
    }
}
