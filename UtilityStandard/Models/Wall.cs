using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class Wall: Element
    {
        public string MaterialId { get; set; }
        public double Depth { get; set; }
        public XYZ Orientation { get; set; }
        public XYZ Endpoint0 { get; set; }
        public XYZ Endpoint1 { get; set; }
        public List<Face> Faces { get; set; }
    }
}
