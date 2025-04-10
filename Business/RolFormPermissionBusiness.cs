using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs.RolFormPermissionDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con RolFormPermission en el sistema;
    ///</summary>
    public class RolFormPermissionBusiness : IBusiness<RolFormPermissionDTO, RolFormPermissionOptionsDTO>
    {
        private readonly IData<RolFormPermission> _rolFormPermissionData;
        private readonly IDeleteStrategyResolver<RolFormPermission> _strategyResolver;
        private readonly ILogger<RolFormPermissionBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RolFormPermissionBusiness"/>.
        /// </summary>
        /// <param name="rolFormPermissionData">Capa de acceso a datos para RolFormPermission.</param>
        /// <param name="logger">Logger para registro de RolFormPermission</param>
        public RolFormPermissionBusiness(IData<RolFormPermission> rolFormPermissionData, IDeleteStrategyResolver<RolFormPermission> strategyResolver, ILogger<RolFormPermissionBusiness> logger, IMapper mapper)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los RolFormPermissions y los mapea a objetos <see cref="RolFormPermissionDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="RolFormPermissionDTO"/> que representan todos los RolFormPermissions existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<RolFormPermissionDTO>> GetAllAsync()
        {
            try
            {
                var rolFormPermissions = await _rolFormPermissionData.GetAllAsync();
                return _mapper.Map<IEnumerable<RolFormPermissionDTO>>(rolFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de RolFormPermission.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Obtiene un RolFormPermission especifico por su identificador y lo mapea a un objeto <see cref="RolFormPermissionDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del rolFormPermission a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="RolFormPermissionDTO"/> que representa el rolFormPermission solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún rolFormPermission con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el rolFormPermission desde la base de datos.
        /// </exception>
        public async Task<RolFormPermissionDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un RolFormPermission con ID inválido: {RolFormPermissionId}", id);
                throw new ValidationException("id", "El ID de RolFormPermission debe ser mayor que cero.");
            }
            var rolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
            if (rolFormPermission == null)
            {
                _logger.LogInformation("No se encontró ningún RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new EntityNotFoundException("RolFormPermission", id);
            }
            try
            {
                return _mapper.Map<RolFormPermissionDTO>(rolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolFormPermission con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo RolFormPermission en la base de datos a partir de un objeto <see cref="RolFormPermissionDTO"/>.
        /// </summary>
        /// <param name="RolFormPermissionDto">Objeto <see cref="RolFormPermissionDTO"/> que contiene la inrolFormPermissionación del rolFormPermission a crear.</param>
        /// <returns>El objeto <see cref="RolFormPermissionDTO"/> que representa el RolFormPermission recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del rolFormPermission no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el rolFormPermission en la base de datos.
        /// </exception>
        public async Task<RolFormPermissionOptionsDTO> CreateAsync(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            ValidateRolFormPermission(rolFormPermissionDTO);

            try
            {
                var rolFormPermission = _mapper.Map<RolFormPermission>(rolFormPermissionDTO);
                var createdRolFormPermission = await _rolFormPermissionData.CreateAsync(rolFormPermission);

                return _mapper.Map<RolFormPermissionOptionsDTO>(createdRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo RolFormPermission.");
                throw new ExternalServiceException("Base de datos", "Error al crear el RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Actualiza un RolFormPermission existente en la base de datos con los datos proporcionados en el <see cref="RolFormPermissionDTO"/>.
        /// </summary>
        /// <param name="rolFormPermissionDTO">Objeto <see cref="RolFormPermissionDTO"/> con la inrolFormPermissionación actualizada del RolFormPermission. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del rolFormPermission no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún rolFormPermission con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el rolFormPermission en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            ValidateRolFormPermission(rolFormPermissionDTO);

            var existingRolFormPermission = await _rolFormPermissionData.GetByIdAsync(rolFormPermissionDTO.Id);
            if (existingRolFormPermission == null)
            {
                throw new EntityNotFoundException("RolFormPermission", rolFormPermissionDTO.Id);
            }
            try
            {
                existingRolFormPermission.Active = rolFormPermissionDTO.Status;
                existingRolFormPermission.RolId = rolFormPermissionDTO.RolId;
                existingRolFormPermission.PermissionId = rolFormPermissionDTO.PermissionId;
                existingRolFormPermission.FormId = rolFormPermissionDTO.FormId;

                return await _rolFormPermissionData.UpdateAsync(existingRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolFormPermission con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Elimina un RolFormPermission. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del RolFormPermission</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún rolFormPermission con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el rolFormPermission en la base de datos.
        /// </exception>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del RolFormPermission debe ser un número mayor a cero.", nameof(id));
            }

            var existingRolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
            if (existingRolFormPermission == null)
            {
                throw new EntityNotFoundException("RolFormPermission", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _rolFormPermissionData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolFormPermission con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolFormPermission.", ex);
            }
        }


        /// <summary>
        /// Valida los datos de RolFormPermission.
        /// </summary>
        private void ValidateRolFormPermission(RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            if (rolFormPermissionDTO == null)
            {
                throw new ValidationException("El objeto RolFormPermission no puede ser nulo.");
            }

            if (rolFormPermissionDTO.RolId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolFormPermission con RoleId inválido.");
                throw new ValidationException("RoleId", "El ID del rol debe ser mayor que cero.");
            }

            if (rolFormPermissionDTO.PermissionId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolFormPermission con PermissionId inválido.");
                throw new ValidationException("PermissionId", "El ID del permiso debe ser mayor que cero.");
            }

            if (rolFormPermissionDTO.FormId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolFormPermission con FormId inválido.");
                throw new ValidationException("FormId", "El ID del formulario debe ser mayor que cero.");
            }
        }
    }
}