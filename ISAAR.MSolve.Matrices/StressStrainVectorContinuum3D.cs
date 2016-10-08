using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.Matrices
{
    public class StressStrainVectorContinuum3D : Vector<Double>
    {
        public StressStrainVectorContinuum3D() : base(6) { }
        public StressStrainVectorContinuum3D(double[] data) : base(data)
        {
            if (data.Length != 6) throw new ArgumentException(String.Format("input array dimension ({0}) is not 6", data.Length));
        }
    }
}
