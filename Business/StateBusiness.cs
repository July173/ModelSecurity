using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los estados en el sistema.
    /// </summary>
    public class StateBusiness
    {
        private readonly StateData _stateData;
        private readonly ILogger _logger;

        public StateBusiness(StateData stateData, ILogger logger)
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
                var statesDTO = new List<StateDto>();

                foreach (var state in states)
                {
                    statesDTO.Add(new StateDto
                    {
                        Id = state.Id,
                        TypeState = state.TypeState,
                        Description = state.Description,
                        Active = state.Active // si existe la entidad
                    });
                }

                return statesDTO;
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

                return new StateDto
                {
                    Id = state.Id,
                    TypeState = state.TypeState,
                    Description = state.Description,
                    Active = state.Active // si existe la entidad
                };
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

                var state = new State
                {
                    TypeState = stateDto.TypeState,
                    Description = stateDto.Description,
                    Active = stateDto.Active // si existe la entidad
                };

                var stateCreado = await _stateData.CreateAsync(state);

                return new StateDto
                {
                    Id = state.Id,
                    TypeState = state.TypeState,
                    Description = state.Description,
                    Active = state.Active // si existe la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo estado: {Name}", stateDto?.TypeState?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el estado", ex);
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
    }
}
