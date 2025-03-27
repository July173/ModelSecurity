using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    internal class personDTO
    {
   
            public int Id { get; set; }
            public string Name { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public string FirstLastName { get; set; }
            public string SecondLastName { get; set; }
            public short PhoneNumber { get; set; }
            public string Email { get; set; }
            public string TypeIdentification { get; set; }
            public short NumberIdentification { get; set; }
            public string Signing { get; set; }

}
}
