﻿using Data;
using Entity.DTOs.UserSede;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using ValidationException = Utilities.Exceptions.ValidationException;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con las sedes de usuario en el sistema.
    /// </summary>
    public class UserSedeBusiness
    {
        private readonly UserSedeData _userSedeData;
        private readonly ILogger<UserSedeData> _logger;

        public UserSedeBusiness(UserSedeData userSedeData, ILogger<UserSedeData> logger)
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
                return MapToDTOList(userSedes);

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

                return MapToDTO(userSede);
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

                var userSede = MapToEntity(userSedeDto);

                var userSedeCreado = await _userSedeData.CreateAsync(userSede);

                return MapToDTO(userSedeCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva sede de usuario: {UserId}", userSedeDto?.UserId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al crear la sede de usuario", ex);
            }
        }



        //Metodo para borrar roles permanente (Delete permanente) 

        public async Task<bool> DeleteUserSedeAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento eliminar un userSede con Id invalido : {userSedeId}", id);
                throw new ValidationException("Id", "El id del userSede debe ser mayor a 0");
            }
            try
            {
                var exists = await _userSedeData.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el userSede con ID {userSedeId} para eliminar", id);
                    throw new EntityNotFoundException("userSede", id);
                }
                return await _userSedeData.DeleteAsync(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el userSede con ID {userSedeid}", id);
                throw new ExternalServiceException("Base de datos", $"Error al elimiar el userSede con ID {id}", ex);

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

        //Metodo para mapear de UserSede a UserSedeDto
        private UserSedeDto MapToDTO(UserSede userSede)
        {
            return new UserSedeDto
            {
                Id = userSede.Id,
                UserId = userSede.UserId,
                SedeId = userSede.SedeId,
                
            };
        }
        //Metodo para mapear de UserSedeDto a UserSede
        private UserSede MapToEntity(UserSedeDto userSedeDto)
        {
            return new UserSede
            {
                Id = userSedeDto.Id,
                UserId = userSedeDto.UserId,
                SedeId = userSedeDto.SedeId,
            
            };
        }
        //Metodo para mapear una lista de UserSede a una lista de UserSedeDto
        private IEnumerable<UserSedeDto> MapToDTOList(IEnumerable<UserSede> userSedes)
        {
            var userSedesDto = new List<UserSedeDto>();
            foreach (var userSede in userSedes)
            {
                userSedesDto.Add(MapToDTO(userSede));
            }
            return userSedesDto;
        }
    }
}
