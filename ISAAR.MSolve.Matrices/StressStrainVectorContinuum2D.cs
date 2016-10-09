using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.Matrices
{
    public class StressStrainVectorContinuum2D : Vector<Double>
    {
        public StressStrainVectorContinuum2D() : base(6) { }
        public StressStrainVectorContinuum2D(double[] data) : base(data)
        {
            if (data.Length != 3) throw new ArgumentException(String.Format("input array dimension ({0}) is not 3", data.Length));
        }
    }
}
