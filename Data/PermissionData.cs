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
    public class PermissionData : IData<Permission>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Permission almacenados en la base de datos LINQ
        /// </summary>
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Set<Permission>()
             .Where(p => p.Active)
             .ToListAsync();
        }


        /// <summary>
        /// Obtiene un Permission específico por su identificación LINQ
        /// </summary>
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener un Permission con ID {PermissionId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Permission en la base de datos LINQ
        /// </summary>
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Permission: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Actualiza un Permission existente en la base de datos LINQ
        /// </summary>
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Permission: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un Permission de la base de datos LINQ
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            var permission = await _context.Set<Permission>().FindAsync(id);
            if (permission == null)
                return false;
            try
            {
                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Permission: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Permission de manera logica de la base  de datos LINQ
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var permission = await GetByIdAsync(id);
                if (permission == null)
                {
                    return false;
                }

                permission.Active = false;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar de manera logica el Perrmission: {ex.Message}");
                return false;
            }
        }
    }
}