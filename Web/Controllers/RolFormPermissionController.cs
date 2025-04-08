using Business;
using Business.Interfaces;
using Entity.DTOs.RolFormPermissionDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    [Produces("application/json")]

    public class RolFormPermissionController : ControllerBase
    {
        private readonly IBusiness<RolFormPermissionDTO, RolFormPermissionOptionsDTO> _rolFormPermissionBusiness;
        private readonly ILogger<RolFormPermissionController> _logger;

        public RolFormPermissionController(IBusiness<RolFormPermissionDTO, RolFormPermissionOptionsDTO> rolFormPermissionBusiness, ILogger<RolFormPermissionController> logger)
        {
            _rolFormPermissionBusiness = rolFormPermissionBusiness;
            _logger = logger;
        }

        ///<summary>
        /// Obtener todos los rolFormPermissions del sistema
        ///<summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolFormPermissions()
        {
            try
            {
                var RolFormPermissions = await _rolFormPermissionBusiness.GetAllAsync();
                return Ok(RolFormPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los rolFormPermissions");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        ///<summary>
        /// Obtener un rolFormPermissionBusiness especificio por su ID
        /// </summary>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(RolFormPermissionDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormPermissionById(int id)
        {
            try
            {
                var RolFormPermission = await _rolFormPermissionBusiness.GetByIdAsync(id);
                return Ok(RolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida para rolFormPermissionBusiness con ID: {RolFormPermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "RolFormPermission no encontrado con ID: {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el rolFormPermissionBusiness con ID: {RolFormPermissionId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo rolFormPermissionBusiness en el sistema
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(RolFormPermissionOptionsDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRolFormPermission([FromBody] RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            try
            {
                var createdRolFormPermission = await _rolFormPermissionBusiness.CreateAsync(rolFormPermissionDTO);
                return CreatedAtAction(nameof(GetRolFormPermissionById), new { id = createdRolFormPermission.Id }, createdRolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el rolFormPermissionBusiness");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el rolFormPermissionBusiness");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un rolFormPermissionBusiness existente en el sistema
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(RolFormPermissionOptionsDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolFormPermission([FromBody] RolFormPermissionOptionsDTO rolFormPermissionDTO)
        {
            try
            {
                var updatedRolFormPermission = await _rolFormPermissionBusiness.UpdateAsync(rolFormPermissionDTO);

                return Ok(updatedRolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el rolFormPermissionBusiness con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el rolFormPermissionBusiness con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el rolFormPermissionBusiness con ID: {RolFormPermissionId}", rolFormPermissionDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un rolFormPermissionBusiness del sistema
        /// </summary>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteRolFormPermission(int id)
        {
            try
            {
                await _rolFormPermissionBusiness.DeletePersistenceAsync(id);
                return Ok(new { message = "RolFormPermission eliminado exitosamente" });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el RolFormPermission con ID: {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });

            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el rolFormPermissionBusiness con ID: {RolFormPermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina de manera logica un formModule del sistema
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Logical/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogicaRolFormPermissionAsync(int id)
        {
            try
            {
                await _rolFormPermissionBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Eliminación lógica exitosa." });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el RolFormPermission con ID: {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el FormModulRolFormPermissione de manera lógica con ID: {RolFormPermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}