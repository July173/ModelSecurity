using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios y sus roles en el sistema.
    /// </summary>
    public class RolUserBusiness
    {
        private readonly RolUserData _rolUserData;
        private readonly ILogger _logger;

        public RolUserBusiness(RolUserData rolUserData, ILogger logger)
        {
            _rolUserData = rolUserData;
            _logger = logger;
        }

        // Método para obtener todos los roles de usuario como DTOs
        public async Task<IEnumerable<RolUserDto>> GetAllRolUsersAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();
                var rolUsersDTO = new List<RolUserDto>();

                foreach (var rolUser in rolUsers)
                {
                    rolUsersDTO.Add(new RolUserDto
                    {
                        Id = rolUser.Id,
                        UserId = rolUser.UserId,
                        RolId = rolUser.RolId
                    });
                }

                return rolUsersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de usuario", ex);
            }
        }

        // Método para obtener un rol de usuario por ID como DTO
        public async Task<RolUserDto> GetRolUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de usuario con ID inválido: {RolUserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol de usuario debe ser mayor que cero");
            }

            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("No se encontró ningún rol de usuario con ID: {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }

                return new RolUserDto
                {
                    Id = rolUser.Id,
                    UserId = rolUser.UserId,
                    RolId = rolUser.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de usuario con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol de usuario con ID {id}", ex);
            }
        }

        // Método para crear un rol de usuario desde un DTO
        public async Task<RolUserDto> CreateRolUserAsync(RolUserDto rolUserDto)
        {
            try
            {
                ValidateRolUser(rolUserDto);

                var rolUser = new RolUser
                {
                    UserId = rolUserDto.UserId,
                    RolId = rolUserDto.RolId
                };

                var rolUserCreado = await _rolUserData.CreateAsync(rolUser);

                return new RolUserDto
                {
                    Id = rolUserCreado.Id,
                    UserId = rolUserCreado.UserId,
                    RolId = rolUserCreado.RolId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de usuario: {UserId}, {RolId}", rolUserDto?.UserId ?? 0, rolUserDto?.RolId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear el rol de usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolUser(RolUserDto rolUserDto)
        {
            if (rolUserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol de usuario no puede ser nulo");
            }

            if (rolUserDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de usuario con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del rol de usuario es obligatorio y debe ser mayor que cero");
            }

            if (rolUserDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol de usuario con RolId inválido");
                throw new Utilities.Exceptions.ValidationException("RolId", "El RolId del rol de usuario es obligatorio y debe ser mayor que cero");
            }
        }
    }
}

