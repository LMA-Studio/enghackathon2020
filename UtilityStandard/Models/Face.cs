using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class Face
    {
        public string MaterialId { get; set; }
        public XYZ Origin { get; set; }
        public XYZ XVector { get; set; }
        public XYZ YVector { get; set; }
        public XYZ Normal { get; set; }

        public List<int> Indices{ get; set; }
        public List<XYZ> Vertices { get; set; }
    }
}
