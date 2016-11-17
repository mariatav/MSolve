using ISAAR.MSolve.PreProcessor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor
{
    public class ElementMassAccelerationLoad
    {
        public IFiniteElement Element { get; set; }
        public DOFType DOF { get; set; }
        public double Amount { get; set; }
    }
}
