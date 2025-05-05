using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.DTOs.RolFormPermission;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad RolFormPermission en la base de datos.
    /// </summary>
    public class RolFormPermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolFormPermissionData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{RolFormPermissionData}"/> para el registro de logs.</param>
        public RolFormPermissionData(ApplicationDbContext context, ILogger<RolFormPermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de RolFormPermission almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de RolFormPermission.</returns>
        public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                    .Include(rfp => rfp.Permission)
                    .Include(rfp => rfp.Rol)
                    .Include(rfp => rfp.Form)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener todos los registros de RolFormPermission: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un registro de RolFormPermission por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro.</param>
        /// <returns>El registro si existe, o null si no se encuentra.</returns>
        public async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                    .Include(rfp => rfp.Permission)
                    .Include(rfp => rfp.Rol)
                    .Include(rfp => rfp.Form)
                    .FirstOrDefaultAsync(rfp => rfp.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el registro de RolFormPermission con ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo registro en la tabla RolFormPermission.
        /// </summary>
        /// <param name="rolFormPermission">Instancia de RolFormPermission a insertar.</param>
        /// <returns>El registro creado.</returns>
        public async Task<RolFormPermission> CreateAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                await _context.Set<RolFormPermission>().AddAsync(rolFormPermission);
                await _context.SaveChangesAsync();
                return rolFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de RolFormPermission: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro existente en la tabla RolFormPermission.
        /// </summary>
        /// <param name="rolFormPermission">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                _context.Set<RolFormPermission>().Update(rolFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de RolFormPermission: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro de la tabla RolFormPermission.
        /// </summary>
        /// <param name="id">Identificador único del registro a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolFormPermission = await _context.Set<RolFormPermission>().FindAsync(id);
                if (rolFormPermission == null)
                    return false;

                _context.Set<RolFormPermission>().Remove(rolFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de RolFormPermission con ID {id}: {ex.Message}");
                return false;
            }
        }
        public async Task AssignPermissionsAsync(int rolId, List<FormPermissionDto> formPermissions)
        {
            foreach (var fp in formPermissions)
            {
                foreach (var permId in fp.PermissionIds)
                {
                    var entity = new RolFormPermission
                    {
                        RolId = rolId,
                        FormId = fp.FormId,
                        PermissionId = permId
                    };

                    _context.RolFormPermission.Add(entity);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<FormPermissionDto>> GetFormPermissionsByRolIdAsync(int idRol)
        {
            try
            {
                var grouped = await _context.RolFormPermission
                    .Where(rfp => rfp.RolId == idRol)
                    .GroupBy(rfp => rfp.FormId)
                    .Select(g => new FormPermissionDto
                    {
                        FormId = g.Key,
                        PermissionIds= g.Select(rfp => rfp.PermissionId).ToList()
                    })
                    .ToListAsync();

                return grouped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener permisos por formulario para el rol con ID {idRol}");
                throw;
            }
        }


        public async Task<List<RolFormDto>> ObtenerPermisosAgrupadosPorUsuario(int userId)
        {
            var datos = await (
                from ru in _context.UserRol
                where ru.UserId == userId
                join r in _context.Rol on ru.RolId equals r.Id
                join rfp in _context.RolFormPermission on ru.RolId equals rfp.RolId
                join f in _context.Form on rfp.FormId equals f.Id
                join p in _context.Permission on rfp.PermissionId equals p.Id
                select new
                {
                    Rol = r.TypeRol,
                    Formulario = f.Name,
                    Permiso = p.Name
                }
            ).ToListAsync();

            var resultado = datos
                .GroupBy(d => d.Rol)
                .Select(grupoRol => new RolFormDto
                {
                    Rol = grupoRol.Key,
                    Form = grupoRol
                        .GroupBy(g => g.Formulario)
                        .Select(gf => new FormWithPermissionDto
                        {
                            Name = gf.Key,
                            Permission = gf.Select(p => p.Permiso).Distinct().ToList()
                        }).ToList()
                }).ToList();

            return resultado;
        }

        public async Task<List<MenuDto>> ObtenerMenu(int userId)
        {
            var datos = await (
                from ru in _context.UserRol
                where ru.UserId == userId
                join r in _context.Rol on ru.RolId equals r.Id
                join rfp in _context.RolFormPermission on ru.RolId equals rfp.RolId
                join fm in _context.FormModule on rfp.FormId equals fm.FormId
                join m in _context.Module on fm.ModuleId equals m.Id
                join f in _context.Form on rfp.FormId equals f.Id
                select new
                {
                    Rol = r.TypeRol,
                    Module = m.Name,
                    Formulario = f.Name

                }
            ).ToListAsync();

            var resultado = datos
                .GroupBy(d => d.Rol)
                .Select(grupoRol => new MenuDto
                {
                    Rol = grupoRol.Key,
                    ModuleForm = grupoRol
                        .GroupBy(g => g.Module)
                        .Select(gf => new MenuFormDto
                        {
                            Name = gf.Key,
                            Form = gf.Select(f => f.Formulario).Distinct().ToList()
                        }).ToList()
                }).ToList();

            return resultado;
        }

    }
}
