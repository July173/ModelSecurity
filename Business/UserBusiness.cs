using Data;
using Entity.DTOautogestion;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los usuarios del sistema.
    /// </summary>
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger _logger;

        public UserBusiness(UserData userData, ILogger logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                var usersDTO = new List<UserDto>();

                foreach (var user in users)
                {
                    usersDTO.Add(new UserDto
                    {
                        id = user.id,
                        username = user.username,
                        email = user.email,
                        active = user.active //si existe la entidad
                     });
                    }

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        // Método para obtener un usuario por ID como DTO
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByidAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return new UserDto
                {
                    id = user.id,
                    username = user.username,
                    email = user.email,
                    active = user.active
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var user = new User
                {
                    username = userDto.username,
                    email = userDto.email,
                    active = userDto.active // Si existe en la entidad
                };

                var userCreado = await _userData.CreateAsync(user);

                return new UserDto
                {
                    id = user.id,
                    username = user.username,
                    email = user.email,
                    active = user.active  // Si existe en la entidad
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {UserName}", userDto?.username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Email vacío");
                throw new Utilities.Exceptions.ValidationException("Email", "El Email del usuario es obligatorio");
            }
        }
    }
}
