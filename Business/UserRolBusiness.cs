﻿using Data;
using Entity.DTOs.UserRol;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios y sus roles en el sistema.
    /// </summary>
    public class UserRolBusiness
    {
        private readonly UserRolData _rolUserData;
        private readonly ILogger<UserRolBusiness> _logger;

        public UserRolBusiness(UserRolData rolUserData, ILogger<UserRolBusiness> logger)
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

        // Método para actualizar una relación entre rol y usuario existente.
        public async Task<bool> UpdateRolUserAsync(UserRolDto rolUserDto)
        {
            ValidateRolUser(rolUserDto);

            try
            {
                var rolUser = MapToEntity(rolUserDto);
                var result = await _rolUserData.UpdateAsync(rolUser);

                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar la relación rol-usuario con ID {RolUserId}", rolUserDto.Id);
                    throw new EntityNotFoundException("RolUser", rolUserDto.Id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación rol-usuario con ID {RolUserId}", rolUserDto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar la relación rol-usuario con ID {rolUserDto.Id}", ex);
            }
        }

        // Método para actualizar parcialmente una relación entre rol y usuario.
        public async Task<bool> UpdatePartialRolUserAsync(int id, Dictionary<string, object> updatedFields)
        {
            if (id <= 0)
            {
                throw new ValidationException("id", "El ID de la relación rol-usuario debe ser mayor que cero");
            }

            if (updatedFields == null || updatedFields.Count == 0)
            {
                throw new ValidationException("updatedFields", "Debe proporcionar al menos un campo para actualizar.");
            }

            try
            {
                var result = await _rolUserData.UpdatePartialAsync(id, updatedFields);

                if (!result)
                {
                    throw new EntityNotFoundException("RolUser", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la relación rol-usuario con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la relación rol-usuario con ID {id}", ex);
            }
        }

        // Método para eliminar una relación entre rol y usuario.
        public async Task<bool> DeleteRolUserAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException("id", "El ID de la relación rol-usuario debe ser mayor que cero");
            }

            try
            {
                var result = await _rolUserData.DeleteAsync(id);

                if (!result)
                {
                    throw new EntityNotFoundException("RolUser", id);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la relación rol-usuario con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la relación rol-usuario con ID {id}", ex);
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

        public async Task<bool> AssignRolesAsync(UserRolAssignDto dto)
        {
            if (dto.RolIds == null || dto.RolIds.Count == 0)
                throw new ArgumentException("Debe asignar al menos un rol.");

            return await _rolUserData.AssignRolesAsync(dto.UserId, dto.RolIds);
        }



        public async Task<List<UserRol>> GetRolesByUserIdAsync(int userId)
        {
            var roles = await _rolUserData.GetByUserIdAsync(userId);

            if (roles == null || !roles.Any())
                throw new EntityNotFoundException("No se encontraron roles para el usuario.");

            return roles; // Ya vienen con la propiedad Rol incluida (gracias al Include del Data)
        }


        // Método para mapear de UserRol a UserRolDto
        private UserRolDto MapToDTO(UserRol rolUser)
        {
            return new UserRolDto
            {
                Id = rolUser.Id,
                UserId = rolUser.UserId,
                RolId = rolUser.RolId
            };
        }

        // Método para mapear de UserRolDto a UserRol
        private UserRol MapToEntity(UserRolDto rolUserDto)
        {
            return new UserRol
            {
                Id = rolUserDto.Id,
                UserId = rolUserDto.UserId,
                RolId = rolUserDto.RolId
            };
        }

        // Método para mapear una lista de UserRol a una lista de UserRolDto
        private IEnumerable<UserRolDto> MapToDTOList(IEnumerable<UserRol> rolUsers)
        {
            var rolUsersDTO = new List<UserRolDto>();
            foreach (var rolUser in rolUsers)
            {
                rolUsersDTO.Add(MapToDTO(rolUser));
            }
            return rolUsersDTO;
        }
    }
}
