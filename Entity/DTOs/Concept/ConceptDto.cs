using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Concept
{
    public class ConceptDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Observation { get; set; }
        public bool Active { get; set; }

    }
}
