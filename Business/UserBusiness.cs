using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.DTOs.UserDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Strategy.Interfaces;
using Utilities.Exceptions;

namespace Business
{
    ///<summary>
    ///Clase de negocio encargada de la logica relacionada con User en el sistema;
    ///</summary>
    public class UserBusiness : IBusiness<UserDTO, UserCreateDTO>
    {
        private readonly IData<User> _userData;
        private readonly IDeleteStrategyResolver<User> _strategyResolver;   
        private readonly ILogger<UserBusiness> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UserBusiness"/>.
        /// </summary>
        /// <param name="userData">Capa de acceso a datos para User.</param>
        /// <param name="logger">Logger para registro de User</param>
        public UserBusiness(IData<User> userData, IDeleteStrategyResolver<User> strategyResolver, ILogger<UserBusiness> logger,IMapper mapper)
        {
            _userData = userData;
            _strategyResolver = strategyResolver;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Obtiene todos los Users y los mapea a objetos <see cref="UserDTO"/>.
        /// </summary>
        /// <returns>Una colección de objetos <see cref="UserDTO"/> que representan todos los Users existentes.</returns>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar recuperar los datos desde la base de datos.
        /// </exception>
        public async Task<IEnumerable<UserDTO>> GetAllAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return _mapper.Map<IEnumerable<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios.");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios.", ex);
            }
        }


        /// <summary>
        /// Obtiene un User especifico por su identificador y lo mapea a un objeto <see cref="UserDTO"/>.
        /// </summary>
        /// <param name="id">Identificador único del user a buscar. Debe ser mayor que cero.</param>
        /// <returns>Un objeto <see cref="UserDTO"/> que representa el user solicitado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza cuando el parámetro <paramref name="id"/> es menor o igual a cero.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza cuando no se encuentra ningún user con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al mapear o recuperar el user desde la base de datos.
        /// </exception>
        public async Task<UserDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Intento de obtener un User con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del User debe ser mayor que cero.");
            }
            var user = await _userData.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                throw new EntityNotFoundException("User", id);
            }
            try
            {
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el User con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el User con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Crea un nuevo User en la base de datos a partir de un objeto <see cref="UserDTO"/>.
        /// </summary>
        /// <param name="UserDto">Objeto <see cref="UserDTO"/> que contiene la inuseración del user a crear.</param>
        /// <returns>El objeto <see cref="UserDTO"/> que representa el User recién creado, incluyendo su identificador asignado.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del user no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error al intentar crear el user en la base de datos.
        /// </exception>
        public async Task<UserCreateDTO> CreateAsync(UserCreateDTO userCreateDTO)
        {
            ValidateUser(userCreateDTO);

            try
            {
                var user = _mapper.Map<User>(userCreateDTO);
                var createdUser = await _userData.CreateAsync(user);

                return _mapper.Map<UserCreateDTO>(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario: {Username}", userCreateDTO?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario.", ex);
            }
        }


        /// <summary>
        /// Actualiza un User existente en la base de datos con los datos proporcionados en el <see cref="UserDTO"/>.
        /// </summary>
        /// <param name="userDTO">Objeto <see cref="UserDTO"/> con la inuseración actualizada del User. Debe contener un ID válido.</param>
        /// <returns>Un valor booleano que indica si la operación de actualización fue exitosa.</returns>
        /// <exception cref="Utilities.Exceptions.ValidationException">
        /// Se lanza si el DTO del user no cumple con las reglas de validación definidas.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún user con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el user en la base de datos.
        /// </exception>
        public async Task<bool> UpdateAsync(UserCreateDTO userCreateDTO)
        {

            ValidateUser(userCreateDTO);

            var existingUser = await _userData.GetByIdAsync(userCreateDTO.Id);
            if (existingUser == null)
            {
                throw new EntityNotFoundException("User", userCreateDTO.Id);
            }

            try
            {
                existingUser.Username = userCreateDTO.Username;
                existingUser.Password = userCreateDTO.Password;
                existingUser.Active = userCreateDTO.Status;
                existingUser.PersonId = userCreateDTO.PersonId;

                return await _userData.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID: {UserId}", userCreateDTO.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el usuario.", ex);
            }
        }


        /// <summary>
        /// Elimina un User. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del User</param>
        /// <param name="strategy">Tipo de eliminación (Logical o Permanent)</param>
        /// <exception cref="EntityNotFoundException">
        /// Se lanza si no se encuentra ningún user con el ID especificado.
        /// </exception>
        /// <exception cref="ExternalServiceException">
        /// Se lanza cuando ocurre un error inesperado al intentar actualizar el user en la base de datos.
        /// </exception>
        public async Task<bool> DeleteAsync(int id, DeleteType strategyType)
        {
            if (id <= 0)
            {
                throw new ArgumentException("El ID del User debe ser un número mayor a cero.", nameof(id));
            }

            var existingUser = await _userData.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new EntityNotFoundException("User", id);
            }

            try
            {
                var strategy = _strategyResolver.Resolve(strategyType);
                return await strategy.DeleteAsync(id, _userData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el User con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el User.", ex);
            }
        }


        /// <summary>
        /// Valida los datos del usuario.
        /// </summary>
        private void ValidateUser(UserCreateDTO userDTO)
        {
            if (userDTO == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(userDTO.Username))
            {
                _logger.LogWarning("Intento de crear/actualizar un usuario con Username vacío.");
                throw new ValidationException("Username", "El nombre de usuario es obligatorio.");
            }
        }
    }
}
