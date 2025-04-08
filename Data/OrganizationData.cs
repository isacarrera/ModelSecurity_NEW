using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class OrganizationData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public OrganizationData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Organization>> GetAllAsync()
        {
            return await _context.Set<Organization>().ToListAsync();
        }

        public async Task<Organization?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Organization>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Organization con id {OrganizationId}", id);
                throw;
            }
        }

        public async Task<Organization> CreateAsync(Organization organization)
        {
            try
            {
                await _context.Set<Organization>().AddAsync(organization);
                await _context.SaveChangesAsync();
                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Organization: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Organization organization)
        {
            try
            {
                _context.Set<Organization>().Update(organization);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error al actualizar el Organization: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var organization = await _context.Set<Organization>().FindAsync(id);
                if (organization == null)
                    return false;
                _context.Set<Organization>().Remove(organization);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error al eliminar el Organization: {ex.Message}");
                return false;
            }
        }
    }
}
