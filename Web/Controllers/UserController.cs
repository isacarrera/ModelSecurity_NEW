using Business;
using Business.Interfaces;
using Data;
using Entity.DTOs.UserDTOs;
using Entity.Enums;
using Entity.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Users en el sistema
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class UserController : ControllerBase    
    {
        private readonly IBusiness<UserDTO, UserCreateDTO> _userBusiness;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructor del controlador de User
        /// </summary>
        /// <param name="UserBusiness">Capa de negocio de User</param>
        /// <param name="logger">Logger para registro de User</param>   
        public UserController(IBusiness<UserDTO, UserCreateDTO> userBusiness, ILogger<UserController> logger)
        {
            _userBusiness = userBusiness;
            _logger = logger;
        }


        /// <summary>
        /// Obtiene todos los Users del sistema
        /// </summary>
        /// <returns>Lista de Useres</returns>
        /// <response code="200">Retorna la lista de Users</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var Users = await _userBusiness.GetAllAsync();
                return Ok(Users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los users");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un User específico por su ID
        /// </summary>
        /// <param name="id">ID del User</param>
        /// <returns>User solicitado</returns>
        /// <response code="200">Retorna el User solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">User no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var User = await _userBusiness.GetByIdAsync(id);
                return Ok(User);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida para user con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "User no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el user con ID: {UserId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo User en el sistema
        /// </summary>
        /// <param name="UserDTO">Datos del User a crear</param>
        /// <returns>user creado</returns>
        /// <response code="201">Retorna el User creado</response>
        /// <response code="400">Datos del User no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(UserDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userCreateDTO)
        {
            try
            {
                var createdUser = await _userBusiness.CreateAsync(userCreateDTO);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el user");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el user");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un User existente en el sistema
        /// </summary>
        /// <param name="id">ID del User a actualizar</param>
        /// <param name="userDto">Datos actualizados del User</param>
        /// <returns>User actualizado</returns>
        /// <response code="200">Retorna el User actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">User no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody] UserCreateDTO userCreateDTO)
        {
            try
            {
                var updatedUser = await _userBusiness.UpdateAsync(userCreateDTO);
                return Ok(updatedUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el user con ID: {UserId}", userCreateDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el user con ID: {UserId}", userCreateDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el user con ID: {UserId}", userCreateDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un User del sistema. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del User a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El User fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">User no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            try
            {
                await _userBusiness.DeleteAsync(id, strategy);
                return Ok(new { message = $"Eliminación con estrategy {strategy} exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de User: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el User con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el User con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
