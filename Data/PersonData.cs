using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    ///<summary>
    ///Repositorio encargador de la gestion de la entidad Person en la base de datos
    ///</summary>

    public class PersonData : IData<Person>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly ILogger<PersonData> _looger;

        ///<summary>
        ///Constructor que recibe el contexto de la base de datos
        ///</summary>
        ///<param name="context">Instancia de <see cref="ApplicationDbContext"/> Para la conexion con la base de datos.</param>
        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _looger = logger;
        }


        /// <summary>
        /// Obtiene todos los Persons almacenados en la base de datos LINQ
        /// </summary>
        ///<returns>Lista de Persons</returns>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>()
             .Where(p => p.Active)
             .ToListAsync();
        }


        /// <summary>
        /// Obtiene un Person especifico por su identificacion LINQ
        /// </summary
        /// <param name="id"></param>
        /// <returns>El Person Obtenido</returns>
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _looger.LogError(ex, $"Error al obtener Person con id {id}");
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo Person en la base de datos LINQ
        /// </summary>
        /// <param name="person"></param>
        /// <returns>El Person Creado</returns>
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al crear Person {ex.Message}");
                throw;
            }

        }


        /// <summary>
        /// Actualiza un Person existente en la base de datos LINQ
        /// </summary>
        /// <param name="person">Objeto con la informacion actualizada.</param>
        /// <returns>True si la actualizacion fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al actualizar Person {ex.Message}");
                throw;
            }

        }


        /// <summary>
        /// Elimina un Person de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Person a eliminar </param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                    return false;

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al eliminar Person {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Elimina un Person de manera logica de la base de datos LINQ
        /// </summary>
        /// <param name="id">Identificador unico del Person a eliminar de manera logica</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                var person = await GetByIdAsync(id);
                if (person == null)
                {
                    return false;
                }

                person.Active = false;
                await _context.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _looger.LogError($"Error al eliminar de manera logica Person: {ex.Message}");
                return false;
            }
        }

    }
}