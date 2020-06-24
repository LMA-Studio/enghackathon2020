using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Models
{
    public class FamilyInstance: Element
    {
        public string FamilyId { get; set; }

        public XYZ BoundingBoxMin { get; set; }
        public XYZ BoundingBoxMax { get; set; }

        public bool IsFlipped { get; set; }
        public Transform Transform { get; set; }
    }
}
