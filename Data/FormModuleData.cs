using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    ///<summary>
    ///Repositorio encargador de la gestion de la entidad FormModule en la base de datos
    ///</summary>
    public class FormModuleData : IData<FormModule>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de la base de datos
        ///</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext"/> Para la conexion con la base de datos.</param>
        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los FormModules almacenados en la base de datos LINQ
        /// </summary>
        ///<returns>Lista de FormModules</returns>
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
        /// <param name="id"></param>
        /// <returns>El FormModule Obtenido</returns>
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
        /// <param name="formModule"></param>
        /// <returns>El FormModule Creado</returns>
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
        /// <param name="formModule">Objeto con la informacion actualizada.</param>
        /// <returns>True si la actualizacion fue exitosa, False en caso contrario.</returns>
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
        /// <param name="id">Identificador unico del FormModule a eliminar </param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.
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
        /// Elimina un FormModule de manera logica de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del FormModule a eliminar de manera logica</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
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
