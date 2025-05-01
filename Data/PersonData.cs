using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.Contexts;
using Entity.DTOs.Person;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PersonData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonData> _logger;

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todas las personas que no han sido eliminadas lógicamente. get
        /// </summary>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>()
                .Where(p => p.Active)
                .ToListAsync();
        }

        /// <summary>
        /// Obtener una persona por ID, si no ha sido eliminada. getById
        /// </summary>
        public async Task<Person?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Person>()
                    .FirstOrDefaultAsync(p => p.Id == id && p.DeleteDate == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener persona con ID {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Crear una nueva persona en la base de datos.post
        /// </summary>
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {

                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la persona");
                throw;
            }
        }

        /// <summary>
        /// Actualizar completamente una persona. put
        /// </summary>
        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {

                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona");
                return false;
            }
        }

        /// <summary>
        /// Actualización parcial de campos específicos de una persona. patch
        /// </summary>
        public async Task<bool> PatchPersonAsync(PersonUpdateDto dto)
        {
            try
            {
                var person = await _context.Set<Person>().FirstOrDefaultAsync(p => p.Id == dto.Id && p.DeleteDate == null);
                if (person == null)
                    return false;

                // Actualizar solo los campos enviados desde el DTO
                person.FirstName = dto.FirstName;
                person.SecondName = dto.SecondName;
                person.FirstLastName = dto.FirstLastName;
                person.SecondLastName = dto.SecondLastName;
                person.PhoneNumber = dto.PhoneNumber;
                person.NumberIdentification = dto.NumberIdentification;

                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la persona");
                return false;
            }
        }

        /// <summary>
        /// Eliminar lógicamente una persona asignando la fecha de eliminación. delete logico
        /// </summary>
        public async Task<bool> SetActiveAsync(int id, bool active)
        {
            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                    return false;


                person.Active = active; //Desactiva el rol
                _context.Entry(person).Property(p => p.Active).IsModified = true;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado activo de la persona");
                return false;
            }
        }

        /// <summary>
        /// Eliminar lógicamente una persona asignando la fecha de eliminación actual. delete
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var person = await _context.Set<Person>().FindAsync(id);
                if (person == null)
                {
                    _logger.LogWarning("Intento de eliminar persona con ID {Id} que no existe o ya fue eliminada", id);
                    return false;
                }

           

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la persona con ID {Id}", id);
                return false;
            }
        }

        public async Task<Person> GetByDocumentAsync(long numberIdentification)
        {
            return await _context.Person
                .FirstOrDefaultAsync(p => p.NumberIdentification == numberIdentification && p.DeleteDate == null);
        }




    }
}
