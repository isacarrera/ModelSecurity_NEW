using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con Person en el sistema;
    ///</summary>
    public class PersonBusiness : IBusiness<PersonDTO, PersonDTO>
    {
        private readonly IData<Person> _personData;
        private readonly IDeleteStrategyResolver<Person> _strategyResolver;
        private readonly ILogger<PersonBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PersonBusiness"/>.
        /// </summary>
        /// <param name="personData">Capa de acceso a datos para Person.</param>
        /// <param name="logger">Logger para registro de Person</param>
        public PersonBusiness(IData<Person> personData, IDeleteStrategyResolver<Person> strategyResolver, ILogger<PersonBusiness> logger, IMapper mapper)
        {
            _personData = personData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los Persons y los mapea a objetos <see cref="PersonDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="PersonDTO"/> que representan todos los Persons existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<PersonDTO>> GetAllAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                return _mapper.Map<IEnumerable<PersonDTO>>(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Persons");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Persons", ex);
            }
        }


        /// <summary>
        /// Obtiene un Person especifico por su identificador y lo mapea a un objeto <see cref="PersonDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del person a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="PersonDTO"/> que representa el person solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún person con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el person desde la base de datos.
        /// </exception>
        public async Task<PersonDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Person con ID inválido: {PersonId}", id);
                throw new ValidationException("id", "El ID de un Person debe ser mayor que cero");
            }

            var person = await _personData.GetByIdAsync(id);
            if (person == null)
            {
                _logger.LogInformation("No se encontró un Person con ID: {PersonId}", id);
                throw new EntityNotFoundException("Person", id);
            }

            try
            {
                return _mapper.Map<PersonDTO>(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo Person en la base de datos a partir de un objeto <see cref="PersonDTO"/>.
        /// </summary>
        /// <param name="PersonDto">Objeto <see cref="PersonDTO"/> que contiene la inpersonación del person a crear.</param>
        /// <returns>El objeto <see cref="PersonDTO"/> que representa el Person recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del person no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el person en la base de datos.
        /// </exception>
        public async Task<PersonDTO> CreateAsync(PersonDTO personDto)
        {
            ValidatePerson(personDto);

            try
            {
                var person = _mapper.Map<Person>(personDto);

                var createdPerson = await _personData.CreateAsync(person);

                return _mapper.Map<PersonDTO>(createdPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo Person: {PersonNombre}", personDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear Person", ex);
            }
        }


        /// <summary>
        /// Actualiza un Person existente en la base de datos con los datos proporcionados en el <see cref="PersonDTO"/>.
        /// </summary>
        /// <param name="personDTO">Objeto <see cref="PersonDTO"/> con la inpersonación actualizada del Person. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del person no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún person con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el person en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(PersonDTO personDto)
        {
            if (personDto.Id <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un Person con ID inválido: {PersonId}", personDto.Id);
                throw new ValidationException("id", "El ID de Person debe ser mayor que cero");
            }

            ValidatePerson(personDto);

            var person = await _personData.GetByIdAsync(personDto.Id);
            if (person == null)
            {
                _logger.LogWarning("No se encontró  Person con ID {PersonId} para actualizar", personDto.Id);
                throw new EntityNotFoundException("Person", personDto.Id);
            }

            try
            {
                person.Name = personDto.Name;
                person.LastName = personDto.LastName;
                person.Email = personDto.Email;
                person.DocumentType = personDto.DocumentType;
                person.DocumentNumber = personDto.DocumentNumber;
                person.Phone = personDto.Phone;
                person.Address = personDto.Address;
                person.BloodType = personDto.BloodType;
                person.Active = personDto.Status;

                return await _personData.UpdateAsync(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID: {PersonId}", personDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la persona con ID {personDto.Id}", ex);
            }
        }


        /// <summary>
        /// Elimina un Person. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del Person</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún person con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el person en la base de datos.
        /// </exception>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del Person debe ser un número mayor a cero.", nameof(id));
            }

            var existingForm = await _personData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Person", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _personData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Person con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Person.", ex);
            }
        }


        /// <summary>
        /// Valida un objeto PersonDTO
        /// </summary>
        private void ValidatePerson(PersonDTO personDto)
        {
            if (personDto == null)
            {
                throw new ValidationException("El objeto Person no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar Person con Name vacío");
                throw new ValidationException("Name", "El nombre de la persona es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(personDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar Person con Email vacío");
                throw new ValidationException("Email", "El correo electrónico de Person es obligatorio");
            }
        }
    }
}