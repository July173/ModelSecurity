using Data;
using Entity.DTOs.Rol;
using Entity.DTOs.User;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
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

        // Método para obtener todos los usuarios como DTOs
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

        // Método para crear un usuario desde un DTO
        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var user = MapToEntity(userDto);


                var userCreado = await _userData.CreateAsync(user);

                return MapToDTO(userCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {UserName}", userDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }


        //Metodo para actualizar datos parcialmente (patch)
        public async Task<bool> UpdateParcialUserAsync(UserUpdateDto dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualiazacion de user invalido");
                throw new Utilities.Exceptions.ValidationException("Id", "Datos invalidos para actualizar user");
            }
            try
            {
                var exists = await _userData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el user con Id {userId} para actualizar", dto.Id);
                    throw new EntityNotFoundException("user", dto.Id);
                }

                return await _userData.PatchRolAsync(dto.Id, dto.Username, dto.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el user con ID {dto.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el user", ex);
            }
        }

        //Metodo para actualizar los datos en su totalidad con (put)
        public async Task<bool> UpdateUserAsync(UserUpdateDto Updatedto)
        {
            if (Updatedto == null || Updatedto.Id <= 0)
            {
                _logger.LogWarning("DTO de reemplazo de user invalido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos invalidos para reemplazar user");
            }
            try
            {
                var exists = await _userData.GetByIdAsync(Updatedto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el user con ID {RolId} para reemplzar", Updatedto.Id);
                    throw new EntityNotFoundException("user", Updatedto.Id);
                }
                var entity = await _userData.GetByIdAsync(Updatedto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("user", Updatedto.Id);

                // Modifica sus campos directamente
                entity.Username = Updatedto.Username;
                entity.Email = Updatedto.Email;


                return await _userData.UpdateAsync(entity);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al reemplzar el user con ID {Updatedto.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al reemplzar el user ", ex);
            }
        }

        //Metodo para delete logico para activar y desactivar el user (delete logico con patch)
        public async Task<bool> SetUserActiveAsync(UserStatusDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El dto de estado de user no puede ser nulo ");
            }
            if (dto.Id <= 0)
            {
                _logger.LogWarning("Se intento activar/desactivar un user con ID invalido:{userId}", dto.Id);
                throw new ValidationException("Id", "El ID del user debe ser mayor a 0");
            }
            try
            {
                var exists = await _userData.GetByIdAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el user con ID {userId} para cambiar su estado activo", dto.Id);
                    throw new EntityNotFoundException("user", dto.Id);
                }
                return await _userData.SetActiveAsync(dto.Id, dto.Active);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar el esatdo activo del user con ID {userId}", dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado activo del user con ID {dto.Id}", ex);
            }
        }

        //Metodo para borrar user permanente (Delete permanente) 

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento eliminar un user con Id invalido : {userId}", id);
                throw new ValidationException("Id", "El id del user debe ser mayor a 0");
            }
            try
            {
                var exists = await _userData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el user con ID {userId} para eliminar", id);
                    throw new EntityNotFoundException("user", id);
                }
                return await _userData.DeleteAsync(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el user con ID {userid}", id);
                throw new ExternalServiceException("Base de datos", $"Error al elimiar el user con ID {id}", ex);

            }
        }

        // Método para validar el DTO
        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(userDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Email vacío");
                throw new Utilities.Exceptions.ValidationException("Email", "El Email del usuario es obligatorio");
            }
        }
        //Metodo para mapear de User a UserDTO
        private UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Active = user.Active,//si existe la entidad
                PersonId = user.PersonId,
                Password = user.Password
            
            };
        }
        //Metodo para mapear de UserDto a User 
        private User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Username = userDto.Username,
                Email = userDto.Email,
                Active = userDto.Active, //si existe la entidad
                PersonId = userDto.PersonId,
                Password = userDto.Password
              
            };
        }
        //Metodo para mapear una lista de User a una lista de UserDto
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
