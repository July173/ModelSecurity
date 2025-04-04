using Data;
using Entity.DTOautogestion;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes de usuario en el sistema.
    /// </summary>
    public class UserSedeBusiness
    {
        private readonly UserSedeData _userSedeData;
        private readonly ILogger _logger;

        public UserSedeBusiness(UserSedeData userSedeData, ILogger logger)
        {
            _userSedeData = userSedeData;
            _logger = logger;
        }

        // Método para obtener todas las sedes de usuario como DTOs
        public async Task<IEnumerable<UserSedeDto>> GetAllUserSedesAsync()
        {
            try
            {
                var userSedes = await _userSedeData.GetAllAsync();
                var userSedesDTO = new List<UserSedeDto>();

                foreach (var userSede in userSedes)
                {
                    userSedesDTO.Add(new UserSedeDto
                    {
                        Id = userSede.Id,
                        UserId = userSede.UserId,
                        SedeId = userSede.SedeId,
                        StatusProcedure = userSede.StatusProcedure
                    });
                }

                return userSedesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las sedes de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de sedes de usuario", ex);
            }
        }

        // Método para obtener una sede de usuario por ID como DTO
        public async Task<UserSedeDto> GetUserSedeByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una sede de usuario con ID inválido: {Id}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la sede de usuario debe ser mayor que cero");
            }

            try
            {
                var userSede = await _userSedeData.GetByIdAsync(id);
                if (userSede == null)
                {
                    _logger.LogInformation("No se encontró ninguna sede de usuario con ID: {Id}", id);
                    throw new EntityNotFoundException("userSede", id);
                }

                return new UserSedeDto
                {
                    Id = userSede.Id,
                    UserId = userSede.UserId,
                    SedeId = userSede.SedeId,
                    StatusProcedure = userSede.StatusProcedure
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sede de usuario con ID: {Id}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la sede de usuario con ID {id}", ex);
            }
        }

        // Método para crear una sede de usuario desde un DTO
        public async Task<UserSedeDto> CreateUserSedeAsync(UserSedeDto userSedeDto)
        {
            try
            {
                ValidateUserSede(userSedeDto);

                var userSede = new UserSede
                {
                    UserId = userSedeDto.UserId,
                    SedeId = userSedeDto.SedeId,
                    StatusProcedure = userSedeDto.StatusProcedure
                };

                var userSedeCreado = await _userSedeData.CreateAsync(userSede);

                return new UserSedeDto
                {
                    Id = userSede.Id,
                    UserId = userSede.UserId,
                    SedeId = userSede.SedeId,
                    StatusProcedure = userSede.StatusProcedure
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede de usuario: {UserId}", userSedeDto?.UserId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear la sede de usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUserSede(UserSedeDto userSedeDto)
        {
            if (userSedeDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto UserSede no puede ser nulo");
            }

            if (userSedeDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede de usuario con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId de la sede de usuario es obligatorio y debe ser mayor a cero");
            }

            if (userSedeDto.SedeId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar una sede de usuario con SedeId inválido");
                throw new Utilities.Exceptions.ValidationException("SedeId", "El SedeId de la sede de usuario es obligatorio y debe ser mayor a cero");
            }
        }
    }
}
