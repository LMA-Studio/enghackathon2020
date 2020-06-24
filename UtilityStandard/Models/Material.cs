using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class Material: Element
    {
        public Color Color { get; set; }
        public int Transparency { get; set; }
        public int Shininess { get; set; }
        public int Smoothness { get; set; }
    }
}
