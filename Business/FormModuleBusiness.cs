using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs.FormModuleDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con FormModule en el sistema;
    ///</summary>
    public class FormModuleBusiness : IBusiness<FormModuleDTO, FormModuleOptionsDTO>
    {
        private readonly IData<FormModule> _formModuleData;
        private readonly IDeleteStrategyResolver<FormModule> _strategyResolver;
        private readonly ILogger<FormModuleBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="FormModuleBusiness"/>.
        /// </summary>
        /// <param name="formModuleData">Capa de acceso a datos para FormModule.</param>
        /// <param name="logger">Logger para registro de FormModule</param>
        public FormModuleBusiness(IData<FormModule> formModuleData, IDeleteStrategyResolver<FormModule> strategyResolver, ILogger<FormModuleBusiness> logger, IMapper mapper)
        {
            _formModuleData = formModuleData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los FormModules y los mapea a objetos <see cref="FormModuleDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="FormModuleDTO"/> que representan todos los FormModules existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<FormModuleDTO>> GetAllAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                return _mapper.Map<IEnumerable<FormModuleDTO>>(formModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las relaciones FormModule.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de relaciones FormModule.", ex);
            }
        }


        /// <summary>
        /// Obtiene un FormModule especifico por su identificador y lo mapea a un objeto <see cref="FormModuleDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del formModule a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="FormModuleDTO"/> que representa el formModule solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún formModule con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el formModule desde la base de datos.
        /// </exception>
        public async Task<FormModuleDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener una relación FormModule con ID inválido: {FormModuleId}", id);
                throw new ValidationException("id", "El ID de la relación FormModule debe ser mayor que cero.");
            }

            var formModule = await _formModuleData.GetByIdAsync(id);
            if (formModule == null)
            {
                _logger.LogInformation("No se encontró ninguna relación FormModule con ID: {FormModuleId}", id);
                throw new EntityNotFoundException("FormModule", id);
            }

            try
            {
                return _mapper.Map<FormModuleDTO>(formModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación FormModule con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la relación FormModule con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo FormModule en la base de datos a partir de un objeto <see cref="FormModuleDTO"/>.
        /// </summary>
        /// <param name="FormModuleDto">Objeto <see cref="FormModuleDTO"/> que contiene la informModuleación del formModule a crear.</param>
        /// <returns>El objeto <see cref="FormModuleDTO"/> que representa el FormModule recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del formModule no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el formModule en la base de datos.
        /// </exception>
        public async Task<FormModuleOptionsDTO> CreateAsync(FormModuleOptionsDTO formModuleDTO)
        {
            ValidateFormModule(formModuleDTO);

            try
            {
                var formModule = _mapper.Map<FormModule>(formModuleDTO);
                var createdFormModule = await _formModuleData.CreateAsync(formModule);

                return _mapper.Map<FormModuleOptionsDTO>(createdFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear una nueva relación FormModule.");
                throw new ExternalServiceException("Base de datos", "Error al crear la relación FormModule.", ex);
            }
        }


        /// <summary>
        /// Actualiza un FormModule existente en la base de datos con los datos proporcionados en el <see cref="FormModuleDTO"/>.
        /// </summary>
        /// <param name="formModuleDTO">Objeto <see cref="FormModuleDTO"/> con la informModuleación actualizada del FormModule. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del formModule no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún formModule con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el formModule en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(FormModuleOptionsDTO formModuleDTO)
        {

            var existingFormModule = await _formModuleData.GetByIdAsync(formModuleDTO.Id);
            if (existingFormModule == null)
            {
                throw new EntityNotFoundException("FormModule", formModuleDTO.Id);
            }

            ValidateFormModule(formModuleDTO);


            try
            {
                existingFormModule.Active = formModuleDTO.Status;
                existingFormModule.FormId = formModuleDTO.FormId;
                existingFormModule.ModuleId = formModuleDTO.ModuleId;

                return await _formModuleData.UpdateAsync(existingFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación FormModule con ID: {FormModuleId}", formModuleDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar la relación FormModule.", ex);
            }
        }


        /// <summary>
        /// Elimina un FormModule. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del FormModule</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún formModule con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el formModule en la base de datos.
        /// </exception>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del FormModule debe ser un número mayor a cero.", nameof(id));
            }

            var existingForm = await _formModuleData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("FormModule", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _formModuleData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el FormModule con ID: {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el FormModule.", ex);
            }
        }


        /// <summary>
        /// Valida los datos de la relación FormModule.
        /// </summary>
        private void ValidateFormModule(FormModuleOptionsDTO formModuleDTO)
        {
            if (formModuleDTO == null)
            {
                throw new ValidationException("El objeto FormModule no puede ser nulo.");
            }

            if (formModuleDTO.FormId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un FormModule con FormId inválido.");
                throw new ValidationException("FormId", "El FormId es obligatorio y debe ser mayor que cero.");
            }

            if (formModuleDTO.ModuleId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un FormModule con ModuleId inválido.");
                throw new ValidationException("ModuleId", "El ModuleId es obligatorio y debe ser mayor que cero.");
            }
        }
    }
}
