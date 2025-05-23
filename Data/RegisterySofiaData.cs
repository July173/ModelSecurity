﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    /// <summary>
    /// Repository encargado de la gestión de la entidad RegisterySofia en la base de datos.
    /// </summary>
    public class RegisterySofiaData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterySofiaData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de la base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para el registro de logs.</param>
        public RegisterySofiaData(ApplicationDbContext context, ILogger<RegisterySofiaData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los registros de Sofia almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de registros de Sofia.</returns>
        public async Task<IEnumerable<RegisterySofia>> GetAllAsync()
        {
            return await _context.Set<RegisterySofia>()
                            .Where(r => r.Active)//Trae solo los activos
                            .ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro de Sofia por su ID.
        /// </summary>
        /// <param name="id">Identificador único del registro de Sofia.</param>
        /// <returns>El registro de Sofia con el ID especificado.</returns>
        public async Task<RegisterySofia?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RegisterySofia>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener registro de Sofia con ID {id}: {ex.Message}");
                throw; // Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Crea un nuevo registro de Sofia en la base de datos.
        /// </summary>
        /// <param name="registerySofia">Instancia del registro de Sofia a crear.</param>
        /// <returns>El registro de Sofia creado.</returns>
        public async Task<RegisterySofia> CreateAsync(RegisterySofia registerySofia)
        {
            try
            {
                await _context.Set<RegisterySofia>().AddAsync(registerySofia);
                await _context.SaveChangesAsync();
                return registerySofia;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el registro de Sofia: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un registro de Sofia existente en la base de datos.
        /// </summary>
        /// <param name="registerySofia">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RegisterySofia registerySofia)
        {
            try
            {
                _context.Set<RegisterySofia>().Update(registerySofia);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el registro de Sofia: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina un registro de Sofia en la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del registro de Sofia a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var registerySofia = await _context.Set<RegisterySofia>().FindAsync(id);
                if (registerySofia == null)
                    return false;

                _context.Set<RegisterySofia>().Remove(registerySofia);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el registro de Sofia: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        /// Elimina logicamente un registerySofia (desactiva o activia el registerySofia)
        /// </summary>
        /// <param name="id">Id del registerySofia</param>
        /// <returns>True si la operacion fue exitosa</returns>
        public async Task<bool> SetActiveAsync(int id, bool active)
        {
            try
            {
                var registerySofia = await _context.Set<RegisterySofia>().FindAsync(id);
                if (registerySofia == null)
                    return false;

                registerySofia.Active = active; //Desactiva el registerySofia
                _context.Entry(registerySofia).Property(r => r.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al realizar eliminacion logica del registerySofia con ID {id}");
                return false;
            }
        }


        ///<summary>
        ///Modifica datos especificos de registerySofia 
        ///</summary>
        ///<param name="id">Id del registerySofia</param>
        ///<returns> True si la actualizacion es verdadera</returns>
        public async Task<bool> PatchAsync(int id, string NewName, string newDescription, string newDocument)
        {
            try
            {
                var registerySofia = await _context.Set<RegisterySofia>().FindAsync(id);
                if (registerySofia == null)
                    return false;

                registerySofia.Name = NewName;
                registerySofia.Description = newDescription;
                registerySofia.Document = newDocument;

                _context.Entry(registerySofia).Property(r => r.Name).IsModified = true;
                _context.Entry(registerySofia).Property(r => r.Description).IsModified = true;
                _context.Entry(registerySofia).Property(r => r.Document).IsModified = true;



                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al modificar datos del registerySofia con su Id");
                return false;
            }

        }
    }
}


