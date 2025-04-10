using Business;
using Business.Interfaces;
using Data;
using Entity.DTOs.RolUserDTOs;
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
    /// Controlador para la gestión de RolUsers en el sistema
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class RolUserController : ControllerBase
    {
        private readonly IBusiness<RolUserDTO, RolUserOptionsDTO> _rolUserBusiness;
        private readonly ILogger<RolUserController> _logger;

        /// <summary>
        /// Constructor del controlador de RolUser
        /// </summary>
        /// <param name="RolUserBusiness">Capa de negocio de RolUser</param>
        /// <param name="logger">Logger para registro de RolUser</param>
        public RolUserController(IBusiness<RolUserDTO, RolUserOptionsDTO> rolUserBusiness, ILogger<RolUserController> logger)
        {
            _rolUserBusiness = rolUserBusiness;
            _logger = logger;
        }


        /// Obtiene todos los RolUsers del sistema
        /// </summary>
        /// <returns>Lista de RolUseres</returns>
        /// <response code="200">Retorna la lista de RolUsers</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<RolUserDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUsers()
        {
            try
            {
                var RolUsers = await _rolUserBusiness.GetAllAsync();
                return Ok(RolUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los rolUsers");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene un RolUser específico por su ID
        /// </summary>
        /// <param name="id">ID del RolUser</param>
        /// <returns>RolUser solicitado</returns>
        /// <response code="200">Retorna el RolUser solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">RolUser no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(RolUserDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserById(int id)
        {
            try
            {
                var RolUser = await _rolUserBusiness.GetByIdAsync(id);
                return Ok(RolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida para rolUserBusiness con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "RolUser no encontrado con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el rolUserBusiness con ID: {RolUserId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo RolUser en el sistema
        /// </summary>
        /// <param name="RolUserDTO">Datos del RolUser a crear</param>
        /// <returns>rolUser creado</returns>
        /// <response code="201">Retorna el RolUser creado</response>
        /// <response code="400">Datos del RolUser no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(RolUserOptionsDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolUser([FromBody] RolUserOptionsDTO rolUserDTO)
        {
            try
            {
                var createdRolUser = await _rolUserBusiness.CreateAsync(rolUserDTO);
                return CreatedAtAction(nameof(GetRolUserById), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el rolUserBusiness");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el rolUserBusiness");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un RolUser existente en el sistema
        /// </summary>
        /// <param name="id">ID del RolUser a actualizar</param>
        /// <param name="rolUserDto">Datos actualizados del RolUser</param>
        /// <returns>RolUser actualizado</returns>
        /// <response code="200">Retorna el RolUser actualizado</response>
        /// <response code="400">Datos inválidos o ID incorrecto</response>
        /// <response code="404">RolUser no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(RolUserOptionsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolUser([FromBody] RolUserOptionsDTO rolUserDTO)
        {
            try
            {

                var updatedRolUser = await _rolUserBusiness.UpdateAsync(rolUserDTO);
                return Ok(updatedRolUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el rolUserBusiness con ID: {RolUserId}", rolUserDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el rolUserBusiness con ID: {RolUserId}", rolUserDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el rolUserBusiness con ID: {RolUserId}", rolUserDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un RolUser del sistema. Eleccion si la eliminación es lógica o permanente.
        /// </summary>
        /// <param name="id">ID del RolUser a eliminar</param>
        /// <returns>Mensaje de confirmación</returns>
        /// <response code="200">El RolUser fue eliminado exitosamente</response>
        /// <response code="400">Parametro Incorrecto</response>
        /// <response code="404">RolUser no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolUser(int id, [FromQuery] DeleteType strategy = DeleteType.Logical)
        {
            try
            {
                await _rolUserBusiness.DeleteAsync(id, strategy);
                return Ok(new { message = $"Eliminación con estrategy {strategy} exitosa." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "ID inválido para eliminación de RolUser: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el RolUser con ID: {RolUserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el RolUser con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}