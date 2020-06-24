using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class Ceiling: Element
    {
        public string MaterialId { get; set; }
        public List<Face> Faces { get; set; }
    }
}
