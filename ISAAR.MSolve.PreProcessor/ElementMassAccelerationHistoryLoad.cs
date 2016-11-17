using ISAAR.MSolve.PreProcessor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor
{
    public class ElementMassAccelerationHistoryLoad
    {
        public IFiniteElement Element { get; set; }
        public MassAccelerationHistoryLoad HistoryLoad { get; set; }
    }
}
