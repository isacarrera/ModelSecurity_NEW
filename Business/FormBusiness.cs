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
    ///Clase de negocio encargada de la logica relacionada con Form en el sistema;
    ///</summary>
    public class FormBusiness : IBusiness<FormDTO, FormDTO>
    {
        private readonly IData<Form> _formData;
        private readonly IDeleteStrategyResolver<Form> _strategyResolver;
        private readonly ILogger<FormBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FormBusiness"/>.
        /// </summary>
        /// <param name="formData">Capa de acceso a datos para Form.</param>
        /// <param name="logger">Logger para registro de Form</param>
        public FormBusiness(IData<Form> formData, IDeleteStrategyResolver<Form> strategyResolver, ILogger<FormBusiness> logger, IMapper mapper)
        {
            _formData = formData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los Forms y los mapea a objetos <see cref="FormDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="FormDTO"/> que representan todos los Forms existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<FormDTO>> GetAllAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                return _mapper.Map<IEnumerable<FormDTO>>(forms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Forms.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Forms.", ex);
            }
        }


        /// <summary>
        /// Obtiene un Form especifico por su identificador y lo mapea a un objeto <see cref="FormDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del form a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="FormDTO"/> que representa el form solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún form con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el form desde la base de datos.
        /// </exception>
        public async Task<FormDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un Form con ID inválido: {FormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Form debe ser mayor que cero");
            }

            var form = await _formData.GetByIdAsync(id);

            if (form == null)
            {
                _logger.LogInformation("No se encontró ningún Form con ID: {FormId}", id);
                throw new EntityNotFoundException("Form", id);
            }

            try
            { 
                return _mapper.Map<FormDTO>(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Form con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Form con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo Form en la base de datos a partir de un objeto <see cref="FormDTO"/>.
        /// </summary>
        /// <param name="FormDto">Objeto <see cref="FormDTO"/> que contiene la información del form a crear.</param>
        /// <returns>El objeto <see cref="FormDTO"/> que representa el Form recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del form no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el form en la base de datos.
        /// </exception>
        public async Task<FormDTO> CreateAsync(FormDTO formDTO)
        {
            try
            {
                ValidateForm(formDTO);

                var form = _mapper.Map<Form>(formDTO);
                var createdForm = await _formData.CreateAsync(form);

                return _mapper.Map<FormDTO>(createdForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Form: {FormNombre}", formDTO?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Form.", ex);
            }
        }


        /// <summary>
        /// Actualiza un Form existente en la base de datos con los datos proporcionados en el <see cref="FormDTO"/>.
        /// </summary>
        /// <param name="formDTO">Objeto <see cref="FormDTO"/> con la información actualizada del Form. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del form no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún form con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el form en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(FormDTO formDTO)
        {
            if (formDTO.Id <= 0)
            {
                throw new ValidationException("El ID del Form debe ser mayor a cero.");
            }

            ValidateForm(formDTO);

            var existingForm = await _formData.GetByIdAsync(formDTO.Id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Form", formDTO.Id);
            }

            try
            {
                existingForm.Active = formDTO.Active;
                existingForm.Name = formDTO.Name;
                existingForm.Description = formDTO.Description;

                return await _formData.UpdateAsync(existingForm);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error inesperado al actualizar el Form con ID: {FormId}", formDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error inesperado al actualizar el Form.", ex);
            }
        }


        /// <summary>
        /// Elimina un Form. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del Form</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del Form debe ser un número mayor a cero.", nameof(id));
            }

            var existingForm = await _formData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Form", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _formData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Form con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Form.", ex);
            }
        }


        /// <summary>
        /// Valida los datos del formulario.
        /// </summary>
        private void ValidateForm(FormDTO formDTO)
        {
            if (formDTO == null)
            {
                throw new ValidationException("El objeto Form no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(formDTO.Name))
            {
                _logger.LogWarning("Intento de crear/actualizar un Form con Name vacío.");
                throw new ValidationException("Name", "El nombre del Form es obligatorio.");
            }
        }


        /// <summary>
        /// Mapea un objeto Form a FormDTO.
        /// </summary>
        
    }
}
