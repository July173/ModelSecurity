using Data;
using Entity.DTOautogestion;
using Entity.DTOautogestion.pivote;
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
    public class UserRolBusiness
    {
        private readonly UserRolData _rolUserData;
        private readonly ILogger<UserRolData> _logger;

        public UserRolBusiness(UserRolData rolUserData, ILogger<UserRolData> logger)
        {
            _rolUserData = rolUserData;
            _logger = logger;
        }

        // Método para obtener todos los roles de usuario como DTOs
        public async Task<IEnumerable<UserRolDto>> GetAllRolUsersAsync()
        {
            try
            {
                var rolUsers = await _rolUserData.GetAllAsync();

                return MapToDTOList(rolUsers);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de usuario", ex);
            }
        }

        // Método para obtener un rol de usuario por ID como DTO
        public async Task<UserRolDto> GetRolUserByIdAsync(int id)
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

                return MapToDTO(rolUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de usuario con ID: {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol de usuario con ID {id}", ex);
            }
        }

        // Método para crear un rol de usuario desde un DTO
        public async Task<UserRolDto> CreateRolUserAsync(UserRolDto rolUserDto)
        {
            try
            {
                ValidateRolUser(rolUserDto);

                var rolUser = MapToEntity(rolUserDto);
                var rolUserCreado = await _rolUserData.CreateAsync(rolUser);

                return MapToDTO(rolUserCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de usuario: {UserId}, {RolId}", rolUserDto?.UserId ?? 0, rolUserDto?.RolId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear el rol de usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolUser(UserRolDto rolUserDto)
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

        //Metodo para mapear de UserRol a UserRolDto
        private UserRolDto MapToDTO(UserRol userRol)
        {
            return new UserRolDto
            {
                Id = userRol.Id,
                UserId = userRol.UserId,
                RolId = userRol.RolId
            };
        }
        //Metodo para mapear de UserRolDto a UserRol
        private UserRol MapToEntity(UserRolDto userRolDto)
        {
            return new UserRol
            {
                Id = userRolDto.Id,
                UserId = userRolDto.UserId,
                RolId = userRolDto.RolId
            };
        }
        //Metodo para mapear una lista de UserRol a una lista de UserRolDto
        private IEnumerable<UserRolDto> MapToDTOList(IEnumerable<UserRol> userRoles)
        {
            var userRolesDto = new List<UserRolDto>();
            foreach (var userRol in userRoles)
            {
                userRolesDto.Add(MapToDTO(userRol));
            }
            return userRolesDto;
        }
    }
}

