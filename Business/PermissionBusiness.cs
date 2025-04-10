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
    ///Clase de negocio encargada de la logica relacionada con Permission en el sistema;
    ///</summary>
    public class PermissionBusiness : IBusiness<PermissionDTO, PermissionDTO>
    {
        private readonly IData<Permission> _permissionData;
        private readonly IDeleteStrategyResolver<Permission> _strategyResolver;
        private readonly ILogger<PermissionBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PermissionBusiness"/>.
        /// </summary>
        /// <param name="permissionData">Capa de acceso a datos para Permission.</param>
        /// <param name="logger">Logger para registro de Permission</param>
        public PermissionBusiness(IData<Permission> permissionData, IDeleteStrategyResolver<Permission> strategyResolver,ILogger<PermissionBusiness> logger, IMapper mapper)
        {
            _permissionData = permissionData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los Permissions y los mapea a objetos <see cref="PermissionDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="PermissionDTO"/> que representan todos los Permissions existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<PermissionDTO>> GetAllAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                return _mapper.Map<IEnumerable<PermissionDTO>>(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Permissions.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Permissions.", ex);
            }
        }


        /// <summary>
        /// Obtiene un Permission especifico por su identificador y lo mapea a un objeto <see cref="PermissionDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del permission a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="PermissionDTO"/> que representa el permission solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún permission con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el permission desde la base de datos.
        /// </exception>
        public async Task<PermissionDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un Permission con ID inválido: {PermissionId}", id);
                throw new ValidationException("id", "El ID del Permission debe ser mayor que cero.");
            }
            var permission = await _permissionData.GetByIdAsync(id);
            if (permission == null)
            {
                _logger.LogInformation("No se encontró ningún Permission con ID: {PermissionId}", id);
                throw new EntityNotFoundException("Permission", id);
            }
            try
            {
                return _mapper.Map<PermissionDTO>(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el Permission con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo Permission en la base de datos a partir de un objeto <see cref="PermissionDTO"/>.
        /// </summary>
        /// <param name="PermissionDto">Objeto <see cref="PermissionDTO"/> que contiene la inpermissionación del permission a crear.</param>
        /// <returns>El objeto <see cref="PermissionDTO"/> que representa el Permission recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del permission no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el permission en la base de datos.
        /// </exception>
        public async Task<PermissionDTO> CreateAsync(PermissionDTO permissionDTO)
        {
            ValidatePermission(permissionDTO);

            try
            {
                var permission = _mapper.Map<Permission>(permissionDTO);
                var createdPermission = await _permissionData.CreateAsync(permission);

                return _mapper.Map<PermissionDTO>(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo Permission.");
                throw new ExternalServiceException("Base de datos", "Error al crear el Permission.", ex);
            }
        }


        /// <summary>
        /// Actualiza un Permission existente en la base de datos con los datos proporcionados en el <see cref="PermissionDTO"/>.
        /// </summary>
        /// <param name="permissionDTO">Objeto <see cref="PermissionDTO"/> con la inpermissionación actualizada del Permission. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del permission no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún permission con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el permission en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(PermissionDTO permissionDTO)
        {
            ValidatePermission(permissionDTO);

            var existingPermission = await _permissionData.GetByIdAsync(permissionDTO.Id);
            if (existingPermission == null)
            {
                throw new EntityNotFoundException("Permission", permissionDTO.Id);
            }
            try
            {

                existingPermission.Active = permissionDTO.Status;
                existingPermission.Name = permissionDTO.Name;
                existingPermission.Description = permissionDTO.Description;

                return await _permissionData.UpdateAsync(existingPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el Permission con ID: {PermissionId}", permissionDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el Permission.", ex);
            }
        }


        /// <summary>
        /// Elimina un Permission. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del Permission</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún permission con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el permission en la base de datos.
        /// </exception>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del Permission debe ser un número mayor a cero.", nameof(id));
            }

            var existingForm = await _permissionData.GetByIdAsync(id);
            if (existingForm == null)
            {
                throw new EntityNotFoundException("Permission", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _permissionData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el Permission con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Permission.", ex);
            }
        }


        /// <summary>
        /// Valida los datos del permiso.
        /// </summary>
        private void ValidatePermission(PermissionDTO permissionDTO)
        {
            if (permissionDTO == null)
            {
                throw new ValidationException("El objeto Permission no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(permissionDTO.Name))
            {
                _logger.LogWarning("Intento de crear/actualizar un Permission con Name vacío.");
                throw new ValidationException("Name", "El nombre del Permission es obligatorio.");
            }
        }
    }
}