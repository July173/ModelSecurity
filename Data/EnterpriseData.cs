using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad Enterprise en la base de datos.
    /// </summary>
    public class EnterpriseData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EnterpriseData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public EnterpriseData(ApplicationDbContext context, ILogger<EnterpriseData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los Enterprise almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de Enterprise.</returns>
        public async Task<IEnumerable<Enterprise>> GetAllAsync()
        {
            return await _context.Set<Enterprise>()
                         .Where(e => e.Active)//Trae solo los activos
                         .ToListAsync();
        }

        /// <summary>
        /// Obtiene un Enterprise por su ID.
        /// </summary>
        /// <param name="id">Identificador único del Enterprise.</param>
        /// <returns>El Enterprise con el ID especificado.</returns>
        public async Task<Enterprise?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Enterprise>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener Enterprise con ID {id}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo Enterprise en la base de datos.
        /// </summary>
        /// <param name="enterprise">Instancia del Enterprise a crear.</param>
        /// <returns>El Enterprise creado.</returns>
        public async Task<Enterprise> CreateAsync(Enterprise enterprise)
        {
            try
            {
                await _context.Set<Enterprise>().AddAsync(enterprise);
                await _context.SaveChangesAsync();
                return enterprise;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el Enterprise {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un Enterprise existente en la base de datos.
        /// </summary>
        /// <param name="enterprise">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Enterprise enterprise)
        {
            try
            {
                _context.Set<Enterprise>().Update(enterprise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el Enterprise {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un Enterprise en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del Enterprise a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var enterprise = await _context.Set<Enterprise>().FindAsync(id);
                if (enterprise == null)
                    return false;

                _context.Set<Enterprise>().Remove(enterprise);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el Enterprise {ex.Message}");
                return false;
            }
        }

        ///<summary>
        /// Elimina logicamente un enterprise (desactiva o activia el enterprise)
        /// </summary>
        /// <param name="id">Id del enterprise</param>
        /// <returns>True si la operacion fue exitosa</returns>
        public async Task<bool> SetActiveAsync(int id, bool active)
        {
            try
            {
                var enterprise = await _context.Set<Enterprise>().FindAsync(id);
                if (enterprise == null)
                    return false;

                enterprise.Active = active; //Desactiva enterprise 
                _context.Entry(enterprise).Property(e => e.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al realizar eliminacion logica del enterprise con ID {id}");
                return false;
            }
        }


        ///<summary>
        ///Modifica datos especificos de enterprise 
        ///</summary>
        ///<param name="id">Id del enterprise</param>
        ///<returns> True si la actualizacion es verdadera</returns>
        public async Task<bool> PatchAsync(int id, string NewName, string newObservation, string NewPhone, string NewLocate, string NewEmail)
        {
            try
            {
                var enterprise = await _context.Set<Enterprise>().FindAsync(id);
                if (enterprise == null)
                    return false;


                enterprise.NameEnterprise = NewName;
                enterprise.Observation = newObservation;
                enterprise.PhoneEnterprise = NewPhone;
                enterprise.EmailEnterprise = NewEmail;
                enterprise.Locate = NewLocate;

            
                _context.Entry(enterprise).Property(e => e.NameEnterprise).IsModified = true;
                _context.Entry(enterprise).Property(e => e.Observation).IsModified = true;
                _context.Entry(enterprise).Property(e => e.EmailEnterprise).IsModified = true;
                _context.Entry(enterprise).Property(e => e.PhoneEnterprise).IsModified = true;
                _context.Entry(enterprise).Property(e => e.Locate).IsModified = true;




                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar datos del enterprise con su Id");
                return false;
            }

        } 
    }
}



