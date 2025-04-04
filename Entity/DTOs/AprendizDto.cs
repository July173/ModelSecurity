using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOautogestion
{
    public class AprendizDto
    {
        public int Id { get; set; }
        public string PreviuosProgram { get; set; }
        public bool Active { get; set; }
        public int UserId { get; set; }
    }
}
