using Business;
using Business.Interfaces;
using Data;
using Entity.DTOs.FormModuleDTOs;
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

    public class FormModuleController : ControllerBase
    {


        private readonly IBusiness<FormModuleDTO,FormModuleOptionsDTO> _formModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
        {
            _formModuleBusiness = formModuleBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los formModules del sistema
        /// </summary>
        [HttpGet("GetAll/")]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFormModules()
        {
            try
            {
                var FormModules = await _formModuleBusiness.GetAllAsync();
                return Ok(FormModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los formModules");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        ///<summary>
        /// Obtener un form especificio por su ID
        /// </summary>
        [HttpGet("GetByiId/{id}/")]
        [ProducesResponseType(typeof(FormModuleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormModuleById(int id)
        {
            try
            {
                var Form = await _formModuleBusiness.GetByIdAsync(id);
                return Ok(Form);
            }
            catch (ValidationException ex)
            {
                _logger.LogInformation(ex, "Validacion fallida para formModule con ID: {FormModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {

                _logger.LogInformation(ex, "FormModule no encontrado con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el formModule con ID: {FormMOduleId}", id);
                throw;
            }
        }


        /// <summary>
        /// Crea un nuevo formModule en el sistema
        /// </summary>
        [HttpPost("Create/")]
        [ProducesResponseType(typeof(FormModuleOptionsDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateFormModule([FromBody] FormModuleOptionsDTO formModuleOptionsDTO)
        {
            try
            {
                var createdFormModule = await _formModuleBusiness.CreateAsync(formModuleOptionsDTO);
                return CreatedAtAction(nameof(GetFormModuleById), new { id = createdFormModule.Id }, createdFormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear el formModule");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el formModule");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza un formModule existente en el sistema
        /// </summary>
        [HttpPut("Update/")]
        [ProducesResponseType(typeof(FormModuleDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFormModule([FromBody] FormModuleOptionsDTO formModuleDTO)
        {
            try
            {
                var updatedFormModule = await _formModuleBusiness.UpdateAsync(formModuleDTO);
                return Ok(updatedFormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar el formModule con ID: {FormModuleId}",formModuleDTO.Id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el formModule con ID: {FormModuleId}",formModuleDTO.Id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el formModule con ID: {FormModuleId}",formModuleDTO.Id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Elimina un formModule del sistema
        /// </summary>
        [HttpDelete("Delete/{id}/")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteFormModule(int id)
        {
            try
            {
                await _formModuleBusiness.DeletePersistenceAsync(id);
                return Ok(new { message = "FormModule eliminado exitosamente" });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el formModule con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });

            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el formModule con ID: {FormModuleId}", id);
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
        public async Task<IActionResult> DeleteLogicalFormModuleAsync(int id)
        {
            try
            {
                await _formModuleBusiness.DeleteLogicAsync(id);
                return Ok(new { message = "Eliminación lógica exitosa." });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "No se encontró el FormModule con ID: {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el FormModule de manera lógica con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
