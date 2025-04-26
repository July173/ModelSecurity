using Data;
using Entity.DTOs.State;
using Entity.DTOs.Verification;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los estados en el sistema.
    /// </summary>
    public class StateBusiness
    {
        private readonly StateData _stateData;
        private readonly ILogger<StateData> _logger;

        public StateBusiness(StateData stateData, ILogger<StateData> logger)
        {
            _stateData = stateData;
            _logger = logger;
        }

        // Método para obtener todos los estados como DTOs
        public async Task<IEnumerable<StateDto>> GetAllStatesAsync()
        {
            try
            {
                var states = await _stateData.GetAllAsync();
                return MapToDTOList(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los estados");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de estados", ex);
            }
        }

        // Método para obtener un estado por ID como DTO
        public async Task<StateDto> GetStateByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un estado con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del estado debe ser mayor que cero");
            }

            try
            {
                var state = await _stateData.GetByIdAsync(id);
                if (state == null)
                {
                    _logger.LogInformation("No se encontró ningún estado con ID: {Id}", id);
                    throw new EntityNotFoundException("state", id);
                }

                return MapToDTO(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el estado con ID {id}", ex);
            }
        }

        // Método para crear un estado desde un DTO
        public async Task<StateDto> CreateStateAsync(StateDto stateDto)
        {
            try
            {
                ValidateState(stateDto);

                var state = MapToEntity(stateDto);
                state.CreateDate = DateTime.Now;

                var stateCreado = await _stateData.CreateAsync(state);

                return MapToDTO(stateCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo estado: {Name}", stateDto?.TypeState ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el estado", ex);
            }
        }

        // Método para actualizar un estado completamente
        public async Task<bool> UpdateStateAsync(StateUpdateDto stateUpdateDto)
        {
            if (stateUpdateDto == null || stateUpdateDto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización inválido");
                throw new ValidationException("id", "Datos inválidos para actualizar estado");
            }

            try
            {
                var exists = await _stateData.GetByIdAsync(stateUpdateDto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró estado con ID: {Id} para actualizar", stateUpdateDto.Id);
                    throw new EntityNotFoundException("state", stateUpdateDto.Id);
                }

                // Mapear el DTO a la entidad
                var entity = await _stateData.GetByIdAsync(stateUpdateDto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("state", stateUpdateDto.Id);

                entity.TypeState = stateUpdateDto.TypeState;
                entity.Description = stateUpdateDto.Description;
                entity.UpdateDate = DateTime.Now;

                return await _stateData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado con ID: {Id}", stateUpdateDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado con ID {stateUpdateDto.Id}", ex);
            }
        }

        // Método para actualizar parcialmente un estado
        public async Task<bool> UpdatePartialStateAsync(StateUpdateDto stateUpdateDto)
        {
            if (stateUpdateDto == null || stateUpdateDto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial inválido");
                throw new ValidationException("id", "Datos inválidos para actualizar estado");
            }

            try
            {
                var exists = await _stateData.GetByIdAsync(stateUpdateDto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró estado con ID: {Id} para actualizar parcialmente", stateUpdateDto.Id);
                    throw new EntityNotFoundException("state", stateUpdateDto.Id);
                }

                // Actualización parcial (puede que solo se actualicen algunos campos)
                return await _stateData.PatchAsync(stateUpdateDto.Id, stateUpdateDto.TypeState, stateUpdateDto.Description);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el estado con ID: {Id}", stateUpdateDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el estado con ID {stateUpdateDto.Id}", ex);
            }
        }

        // Método para eliminar un estado
        public async Task<bool> DeleteStateAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un estado con ID inválido: {Id}", id);
                throw new ValidationException("Id", "El ID debe ser mayor a 0");
            }

            try
            {
                var exists = await _stateData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró estado con ID: {Id} para eliminar", id);
                    throw new EntityNotFoundException("state", id);
                }

                return await _stateData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el estado con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el estado con ID {id}", ex);
            }
        }
        public async Task<bool> SetStateActiveAsync(StateStatusDto dto)
        {
            if (dto == null)
                throw new ValidationException("El DTO de estado de State no puede ser nulo");

            if (dto.Id <= 0)
            {
                _logger.LogWarning("ID inválido para cambiar estado activo de State: {Id}", dto.Id);
                throw new ValidationException("Id", "El ID de la State debe ser mayor a 0");
            }

            try
            {
                var entity = await _stateData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("state no encontrada con ID {Id} para cambiar estado", dto.Id);
                    throw new EntityNotFoundException("state", dto.Id);
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

                return await _stateData.SetActiveAsync(dto.Id, dto.Active); // Usamos UpdateAsync porque modificamos el objeto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de state con ID {Id}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar estado activo de state con ID {dto.Id}", ex);
            }
        }



        // Método para validar el DTO
        private void ValidateState(StateDto stateDto)
        {
            if (stateDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto State no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(stateDto.TypeState))
            {
                _logger.LogWarning("Se intentó crear/actualizar un estado con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del estado es obligatorio");
            }
        }

        // Método para mapear de State a StateDto
        private StateDto MapToDTO(State state)
        {
            return new StateDto
            {
                Id = state.Id,
                TypeState = state.TypeState,
                Description = state.Description,
                Active = state.Active,
            };
        }

        // Método para mapear de StateDto a State
        private State MapToEntity(StateDto stateDto)
        {
            return new State
            {
                Id = stateDto.Id,
                TypeState = stateDto.TypeState,
                Description = stateDto.Description,
                Active = stateDto.Active,
            };
        }

        // Método para mapear una lista de State a una lista de StateDto
        private IEnumerable<StateDto> MapToDTOList(IEnumerable<State> states)
        {
            var statesDTO = new List<StateDto>();
            foreach (var state in states)
            {
                statesDTO.Add(MapToDTO(state));
            }
            return statesDTO;
        }
    }
}
