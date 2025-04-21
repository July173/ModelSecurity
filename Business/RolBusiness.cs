
using Data;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Utilities.Exceptions;
using System.ComponentModel.Design;
using Entity.DTOs.Rol;
using ValidationException = Utilities.Exceptions.ValidationException;


namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// </summary>
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<RolData> _logger;

        public RolBusiness(RolData rolData, ILogger<RolData> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }
        // Método para obtener todos los roles como DTOs (get)
        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                return MapToDTOList(roles);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rolez");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }
        // Método para obtener un rol por su ID como DTO (getByid)
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con un ID invalido: {RolId}",id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor a 0");
            }
            try
            {
                var rol = await _rolData.GetByidAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró el rol con ID {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }
                return MapToDTO(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error al obtener el rol con ID {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }
        // Método para crear un rol desde un DTO (post)
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var rol = MapToEntity(RolDto);

                var rolCreado = await _rolData.CreateAsync(rol);

                return MapToDTO(rolCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo rol: {RolNombre}", RolDto?.TypeRol?? "null");
                throw new ExternalServiceException("Base de datos", $"Error al crear el rol", ex);
            }
        }

        //Metodo para actualizar datos parcialmente (patch)
        public async Task<bool> UpdateParcialRolAsync(RolUpdateDto dto)
        {
            if(dto== null || dto.Id <= 0)
            {
                _logger.LogWarning("DTO de actualiazacion de rol invalido");
                throw new Utilities.Exceptions.ValidationException("Id", "Datos invalidos para actualizar rol");
            }
            try
            {
                var exists = await _rolData.GetByidAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el rol con Id {RolId} para actualizar", dto.Id);
                    throw new EntityNotFoundException("Rol", dto.Id);
                }

                return await _rolData.PatchRolAsync(dto.Id, dto.TypeRol, dto.Description);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el rol con ID {dto.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el rol", ex );
            }
        }


        //Metodo para actualizar los datos en su totalidad con (put)
        public async Task<bool> UpdateRolAsync(RolUpdateDto Updatedto)
        {
            if(Updatedto == null || Updatedto.Id <= 0)
            {
                _logger.LogWarning("DTO de reemplazo de rol invalido");
                throw new Utilities.Exceptions.ValidationException("id", "Datos invalidos para reemplazar rol");
            }
            try
            {
                var exists = await _rolData.GetByidAsync(Updatedto.Id);
                if(exists == null)
                {
                    _logger.LogInformation("No se encontro el rol con ID {RolId} para reemplzar", Updatedto.Id);
                    throw new EntityNotFoundException("Rol", Updatedto.Id);
                }
                var entity = await _rolData.GetByidAsync(Updatedto.Id);
                if (entity == null)
                    throw new EntityNotFoundException("Verification", Updatedto.Id);

                // Modifica sus campos directamente
                entity.TypeRol = Updatedto.TypeRol;
                entity.Description = Updatedto.Description;
                

                return await _rolData.UpdateAsync(entity);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error al reemplzar el rol con ID {Updatedto.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al reemplzar el rol ", ex);
            }
        }

        //Metodo para delete logico para activar y desactivar el rol (delete logico con patch)
        public async Task<bool> SetRolActiveAsync(RolStatusDto dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El dto de estado de rol no puede ser nulo ");
            }
            if (dto.Id <= 0)
            {
                _logger.LogWarning("Se intento activar/desactivar un rol con ID invalido:{RolId}", dto.Id);
                throw new ValidationException("Id", "El ID del rol debe ser mayor a 0");
            }
            try
            {
                var exists= await _rolData.GetByidAsync(dto.Id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el rol con ID {RolId} para cambiar su estado activo", dto.Id);
                    throw new EntityNotFoundException("Rol", dto.Id);
                }
                return await _rolData.SetActiveAsync(dto.Id, dto.Active);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al cambiar el esatdo activo del rol con ID {RolId}",dto.Id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el estado activo del rol con ID {dto.Id}", ex );
            }
        }
        //Metodo para borrar roles permanente (Delete permanente) 

        public async Task<bool> DeleteRolAsync (int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento eliminar un rol con Id invalido : {RolId}", id);
                throw new ValidationException("Id", "El id del rol debe ser mayor a 0");
            }
            try
            {
                var exists = await _rolData.GetByidAsync (id);
                if (exists == null)
                {
                    _logger.LogInformation("No se encontro el rol con ID {RolId} para eliminar", id);
                    throw new EntityNotFoundException("Rol", id);
                }
                return await _rolData.DeleteAsync(id);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID {Rolid}", id);
                throw new ExternalServiceException("Base de datos",$"Error al elimiar el rol con ID {id}", ex);

            }
        }


        // Método para validar el DTO 
        private void ValidateRol(RolDto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException( "El objeto rol no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(RolDto.TypeRol))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con nombre vacio");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre del rol nes obligatorio");
            }
        }

        //Metodo para mapear de Rol a RolDTO
        private RolDto MapToDTO(Rol rol)
        {
            return new RolDto
            {
                Id = rol.Id,
                TypeRol = rol.TypeRol,
                Description = rol.Description,
                Active = rol.Active, //si existe la entidad
               
            };
        }

        //Metodo para mapear de RolDto a Rol 
        private Rol MapToEntity(RolDto rolDto)
        {
            return new Rol
            {
                Id = rolDto.Id,
                TypeRol = rolDto.TypeRol,
                Description = rolDto.Description,
                Active = rolDto.Active, //si existe la entidad
                
            };
        }


        //Metodo para mapear una lista de Rol a una lista de RolDto
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
        {
            var rolesDto = new List<RolDto>();
            foreach (var rol in roles)
            {
                rolesDto.Add(MapToDTO(rol));
            }
            return rolesDto;
        }

        

    }
}
