using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Person
{
    public class PersonDto
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string FirstLastName { get; set; }
        public string SecondLastName { get; set; }
        public long PhoneNumber { get; set; }
        public string TypeIdentification { get; set; }
        public long NumberIdentification { get; set; }

    }
}
