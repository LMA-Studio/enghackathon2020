using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class Transform
    {
        public XYZ Origin { get; set; }
        public XYZ BasisX { get; set; }
        public XYZ BasisY { get; set; }
        public XYZ BasisZ { get; set; }
        public double Scale { get; set; }
    }
}
