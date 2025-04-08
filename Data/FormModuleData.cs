using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs.FormModuleDTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class FormModuleData : IData<FormModule>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        

        /// <summary>
        /// Obtiene todos los FormModule almacenados en la base de datos LINQ
        /// </summary>
        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _context.Set<FormModule>()
                .Where(formModule => formModule.Active)
                .Include(formModule => formModule.Form)
                .Include(formModule => formModule.Module)
                .ToListAsync();
        }




        /// <summary>
        /// Obtiene un FormModule específico por su identificación LINQ
        /// </summary>
        public async Task<FormModule?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<FormModule>()
                    .Include(formModule => formModule.Form)
                    .Include(formModule => formModule.Module)
                    .FirstOrDefaultAsync(formModule => formModule.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener un FormModule con ID {FormModuleId}", id);
                throw;
            }
        }




        /// <summary>
        /// Crea un nuevo FormModule en la base de datos LINQ
        /// </summary>
        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                await _context.Set<FormModule>().AddAsync(formModule);
                await _context.SaveChangesAsync();
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el FormModule: {ex.Message}");
                throw;
            }
        }



        
        /// <summary>
        /// Actualiza un FormModule existente en la base de datos LINQ
        /// </summary>
        public async Task<bool> UpdateAsync(FormModule formModule)
        {
            try
            {
                _context.Set<FormModule>().Update(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el FormModule: {ex.Message}");
                return false;
            }
        }



        

        /// <summary>
        /// Elimina un FormModule de la base de datos LINQ
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var formModule = await _context.Set<FormModule>().FindAsync(id);
                if (formModule == null)
                    return false;

                _context.Set<FormModule>().Remove(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el FormModule: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un FormModuleData de manera logica de la base  de datos LINQ
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var formModule = await GetByIdAsync(id);
                if (formModule == null)
                {
                    return false;
                }

                formModule.Active = false;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar de manera logica el FormModule: {ex.Message}");
                return false;
            }
        }
    }
}
