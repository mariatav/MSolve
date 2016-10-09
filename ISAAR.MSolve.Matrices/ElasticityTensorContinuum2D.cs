using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.Matrices
{
    public class ElasticityTensorContinuum2D : Matrix2D<Double>
    {
        public ElasticityTensorContinuum2D() : base(3, 3) { }

        public ElasticityTensorContinuum2D(double[,] data) : base(data)
        {
            if (data.GetLength(0) != 3 || data.GetLength(1) != 3) throw new ArgumentException(String.Format("input array (dimensions: {0} by {1}) is not 3 by 3", data.GetLength(0), data.GetLength(1)));
        }
    }
}
