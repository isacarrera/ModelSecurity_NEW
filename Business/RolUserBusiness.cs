using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs.RolUserDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con RolUser en el sistema;
    ///</summary>
    public class RolUserBusiness : IBusiness<RolUserDTO, RolUserOptionsDTO>
    {
        private readonly IData<RolUser> _rolUserData;
        private readonly IDeleteStrategyResolver<RolUser> _strategyResolver;
        private readonly ILogger<RolUserBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RolUserBusiness"/>.
        /// </summary>
        /// <param name="rolUserData">Capa de acceso a datos para RolUser.</param>
        /// <param name="logger">Logger para registro de RolUser</param>
        public RolUserBusiness(IData<RolUser> rolUserData, IDeleteStrategyResolver<RolUser> strategyResolver, ILogger<RolUserBusiness> logger, IMapper mapper)
        {
            _rolUserData = rolUserData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los RolUsers y los mapea a objetos <see cref="RolUserDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="RolUserDTO"/> que representan todos los RolUsers existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<RolUserDTO>> GetAllAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                return _mapper.Map<IEnumerable<RolUserDTO>>(rolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los registros de RolUser.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de RolUser.", ex);
            }
        }


        /// <summary>
        /// Obtiene un RolUser especifico por su identificador y lo mapea a un objeto <see cref="RolUserDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del rolUser a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="RolUserDTO"/> que representa el rolUser solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún rolUser con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el rolUser desde la base de datos.
        /// </exception>
        public async Task<RolUserDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un RolUser con ID inválido: {RolUserId}", id);
                throw new ValidationException("id", "El ID de RolUser debe ser mayor que cero.");
            }

            var rolUser = await _rolUserData.GetByIdAsync(id);
            if (rolUser == null)
            {
                _logger.LogInformation("No se encontró ningún RolUser con ID: {RolUserId}", id);
                throw new EntityNotFoundException("RolUser", id);
            }

            try
            {
                return _mapper.Map<RolUserDTO>(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el RolUser con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo RolUser en la base de datos a partir de un objeto <see cref="RolUserDTO"/>.
        /// </summary>
        /// <param name="RolUserDto">Objeto <see cref="RolUserDTO"/> que contiene la inrolUseración del rolUser a crear.</param>
        /// <returns>El objeto <see cref="RolUserDTO"/> que representa el RolUser recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del rolUser no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el rolUser en la base de datos.
        /// </exception>
        public async Task<RolUserOptionsDTO> CreateAsync(RolUserOptionsDTO rolUserDTO)
        {
            ValidateRolUser(rolUserDTO);
            try
            {
                var rolUser = _mapper.Map<RolUser>(rolUserDTO);
                var createdRolUser = await _rolUserData.CreateAsync(rolUser);

                return _mapper.Map<RolUserOptionsDTO>(createdRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo RolUser.");
                throw new ExternalServiceException("Base de datos", "Error al crear la asignación de RolUser.", ex);
            }
        }


        /// <summary>
        /// Actualiza un RolUser existente en la base de datos con los datos proporcionados en el <see cref="RolUserDTO"/>.
        /// </summary>
        /// <param name="rolUserDTO">Objeto <see cref="RolUserDTO"/> con la inrolUseración actualizada del RolUser. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del rolUser no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún rolUser con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el rolUser en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(RolUserOptionsDTO rolUserDTO)
        {
            ValidateRolUser(rolUserDTO);

            var existingRolUser = await _rolUserData.GetByIdAsync(rolUserDTO.Id);
            if (existingRolUser == null)
            {
                throw new EntityNotFoundException("RolUser", rolUserDTO.Id);
            }
            try
            {
                existingRolUser.Active = rolUserDTO.Status;
                existingRolUser.UserId = rolUserDTO.UserId;
                existingRolUser.RolId = rolUserDTO.RolId;

                return await _rolUserData.UpdateAsync(existingRolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el RolUser con ID: {RolUserId}", rolUserDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el RolUser.", ex);
            }
        }


        /// <summary>
        /// Elimina un RolUser. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del RolUser</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún rolUser con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el rolUser en la base de datos.
        /// </exception>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del RolUser debe ser un número mayor a cero.", nameof(id));
            }

            var existingRolUser = await _rolUserData.GetByIdAsync(id);
            if (existingRolUser == null)
            {
                throw new EntityNotFoundException("RolUser", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _rolUserData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolUser con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser.", ex);
            }
        }


        /// <summary>
        /// Valida los datos de RolUser.
        /// </summary>
        private void ValidateRolUser(RolUserOptionsDTO rolUserDTO)
        {
            if (rolUserDTO == null)
            {
                throw new ValidationException("El objeto RolUser no puede ser nulo.");
            }

            if (rolUserDTO.UserId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolUser con UserId inválido.");
                throw new ValidationException("UserId", "El ID del usuario debe ser mayor que cero.");
            }

            if (rolUserDTO.RolId <= 0)
            {
                _logger.LogWarning("Intento de crear/actualizar un RolUser con RoleId inválido.");
                throw new ValidationException("RoleId", "El ID del rol debe ser mayor que cero.");
            }
        }
    }
}