using Data;
using Entity.DTOs.User;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<UserData> _logger;

        public UserBusiness(UserData userData, ILogger<UserData> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                return MapToDTOList(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return MapToDTO(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var existingUser = await _userData.GetByEmailOrUsernameAsync(userDto.Email, userDto.Username);
                if (existingUser != null)
                {
                    throw new ValidationException("Usuario", "El correo electrónico o nombre de usuario ya está registrado.");
                }

                var user = MapToEntity(userDto);
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                user.Active = true;

                var userCreado = await _userData.CreateAsync(user);
                return MapToDTO(userCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {UserName}", userDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }





        public async Task<bool> UpdateParcialUserAsync(UserUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualización parcial de usuario inválido");
                throw new ValidationException("Id", "Datos inválidos para actualizar el usuario");
            }

            try
            {
                var exists = await _userData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró el usuario con Id {UserId} para actualizar", dto.Id);
                    throw new EntityNotFoundException("user", dto.Id);
                }

                return await _userData.PatchRolAsync(dto.Id, dto.Username, dto.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el usuario con ID {UserId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el usuario", ex);
            }
        }

        public async Task<bool> UpdateUserAsync(UserUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de reemplazo de usuario inválido");
                throw new ValidationException("id", "Datos inválidos para reemplazar el usuario");
            }

            try
            {
                var entity = await _userData.GetByIdAsync(dto.Id);
                if (entity == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para reemplazar", dto.Id);
                    throw new EntityNotFoundException("user", dto.Id);
                }

                entity.Username = dto.Username;
                entity.Email = dto.Email;

                return await _userData.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reemplazar el usuario con ID {UserId}", dto.Id);
                throw new ExternalServiceException("Base de datos", "Error al reemplazar el usuario", ex);
            }
        }

        public async Task<bool> SetUserActiveAsync(UserStatusDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El DTO de estado de usuario no puede ser nulo");
            }

            if (dto.Id <= 0)
            {
                _logger.LogWarning("Se intentó activar/desactivar un usuario con ID inválido: {UserId}", dto.Id);
                throw new ValidationException("Id", "El ID del usuario debe ser mayor a 0");
            }

            try
            {
                var exists = await _userData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para cambiar su estado activo", dto.Id);
                    throw new EntityNotFoundException("user", dto.Id);
                }

                return await _userData.SetActiveAsync(dto.Id, dto.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el estado activo del usuario con ID {UserId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado activo del usuario con ID {dto.Id}", ex);
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario con ID inválido: {UserId}", id);
                throw new ValidationException("Id", "El ID del usuario debe ser mayor a 0");
            }

            try
            {
                var exists = await _userData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontró el usuario con ID {UserId} para eliminar", id);
                    throw new EntityNotFoundException("user", id);
                }

                return await _userData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
            }
        }

        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con nombre de usuario vacío");
                throw new ValidationException("Username", "El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con correo electrónico vacío");
                throw new ValidationException("Email", "El correo electrónico es obligatorio");
            }
        }

        public async Task<UserDto> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userData.GetByEmailAsync(email);
            if (user == null ||!user.Active || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return MapToDTO(user);
        }

        private UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Active = user.Active,
                PersonId = user.PersonId,
                Password = user.Password
            };
        }

        private User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Username = userDto.Username,
                Email = userDto.Email,
                Active = userDto.Active,
                PersonId = userDto.PersonId,
                Password = userDto.Password
            };
        }

        private IEnumerable<UserDto> MapToDTOList(IEnumerable<User> users)
        {
            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                usersDto.Add(MapToDTO(user));
            }
            return usersDto;
        }
    }
}
