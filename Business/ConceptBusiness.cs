using Data;
using Entity.DTOs.Concept;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los conceptos en el sistema.
    /// </summary>
    public class ConceptBusiness
    {
        private readonly ConceptData _conceptData;
        private readonly ILogger<ConceptData> _logger;

        public ConceptBusiness(ConceptData conceptData, ILogger<ConceptData> logger)
        {
            _conceptData = conceptData;
            _logger = logger;
        }

        // Método para obtener todos los conceptos como DTOs
        public async Task<IEnumerable<ConceptDto>> GetAllConceptsAsync()
        {
            try
            {
                var concepts = await _conceptData.GetAllAsync();
               
                return MapToDTOList(concepts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los conceptos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de conceptos", ex);
            }
        }

        // Método para obtener un concepto por ID como DTO
        public async Task<ConceptDto> GetConceptByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un concepto con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del concepto debe ser mayor que cero");
            }

            try
            {
                var concept = await _conceptData.GetByIdAsync(id);
                if (concept == null)
                {
                    _logger.LogInformation("No se encontró ningún concepto con ID: {Id}", id);
                    throw new EntityNotFoundException("concept", id);
                }

                return MapToDTO(concept);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el concepto con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el concepto con ID {id}", ex);
            }
        }

        // Método para crear un concepto desde un DTO
        public async Task<ConceptDto> CreateConceptAsync(ConceptDto conceptDto)
        {
            try
            {
                ValidateConcept(conceptDto);

                var concept = MapToEntity(conceptDto);
                concept.CreateDate = DateTime.Now;

                var conceptCreado = await _conceptData.CreateAsync(concept);

                return MapToDTO (conceptCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo concepto: {Name}", conceptDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el concepto", ex);
            }
        }


        public async Task<bool> SetConceptActiveAsync(ConceptStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de concepto no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de concepto: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID del concepto debe ser mayor a 0");
            }

            try
            {
                var entity = await _conceptData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("Concepto no encontrado con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("Concept", dto.Id);
                }

                // Establecer DeleteDate si se va a desactivar (borrado lógico)
                if (!dto.Active)
                {
                    entity.DeleteDate = DateTime.Now;
                }
                else
                {
                    entity.DeleteDate = null; // Reactivación: eliminamos la marca de eliminación
                }

                return await _conceptData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de concepto con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de concepto con ID {dto.Id}", ex);
            }
        }


        public async Task<bool> DeleteConceptAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un concepto con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _conceptData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("Concepto no encontrado con ID {Id} para eliminar", id);
                    throw new EntityNotFoundException("Concept", id);
                }

                return await _conceptData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar concepto con ID {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar concepto con ID {id}", ex);
            }
        }

        public async Task<bool> UpdateParcialConceptAsync(ConceptUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar concepto");
            }

            try
            {
                var exists = await _conceptData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("Concepto no encontrado con ID: {Id}", dto.Id);
                    throw new EntityNotFoundException("Concept", dto.Id);
                }

                return await _conceptData.PatchAsync(dto.Id, dto.Name, dto.Observation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el concepto con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar concepto con ID {dto.Id}", ex);
            }
        }


        public async Task<bool> UpdateConceptAsync(ConceptUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos inválidos para actualizar concepto");
            }

            try
            {
                var entity = await _conceptData.GetByIdAsync(dto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Concept", dto.Id);

                // Modifica sus campos directamente
                entity.Name = dto.Name;
                entity.Observation = dto.Observation;
                entity.UpdateDate = DateTime.Now;

                return await _conceptData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el concepto con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar concepto con ID {dto.Id}", ex);
            }
        }


        // Método para validar el DTO
        private void ValidateConcept(ConceptDto conceptDto)
        {
            if (conceptDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Concept no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(conceptDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un concepto con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del concepto es obligatorio");
            }
        }


        //Metodo para mapear de Concept a ConceptDto
        private ConceptDto MapToDTO(Concept concept)
        {
            return new ConceptDto
            {
                Id = concept.Id,
                Name = concept.Name,
                Observation = concept.Observation,
                Active = concept.Active,
            };
        }
        //Metodo para mapear de ConceptDto a Concept 
        private Concept MapToEntity(ConceptDto conceptDto)
        {
            return new Concept
            {   
                Id = conceptDto.Id,
                Name = conceptDto.Name,
                Observation = conceptDto.Observation,
                Active = conceptDto.Active,
            };
        }
        //Metodo para mapear una lista de Concept a una lista de ConceptDto
        private IEnumerable<ConceptDto> MapToDTOList(IEnumerable<Concept> concepts)
        {
            var conceptsDto = new List<ConceptDto>();
            foreach (var concept in concepts)
            {
                conceptsDto.Add(MapToDTO(concept));
            }
            return conceptsDto;
        }
    }
}
