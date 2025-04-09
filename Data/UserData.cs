using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs.UserDTOs;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class UserData : IData<User>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los UserData almacenados en la base de datos LINQ
        /// </summary>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>()
                .Where(user => user.Active)
                .Include(user => user.Person)
                .ToListAsync();
        }


        /// <summary>
        /// Obtiene un UserData especifico por su identificacion LINQ
        /// </summary
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>()
                    .Include(user => user.Person)
                    .FirstOrDefaultAsync(user => user.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener una usuario con ID {UserId}", id);
                throw;
            }

        }


        /// <summary>
        /// Crear un nuevo UserData en la base de datos LINQ
        /// </summary>
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario: {ex.Message}");
                throw;
            }
        }

        
        /// <summary>
        /// Actualiza un UserData existente en la base de datos LINQ
        /// </summary>
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario : {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un UserData de la base de datos SQL LINQ
        /// </summary>
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try 
            { 
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el usuario {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un FormData de manera logica de la base  de datos LINQ
        /// </summary>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var user = await GetByIdAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.Active = false;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar de manera logica el User: {ex.Message}");
                return false;
            }
        }
    }
}
