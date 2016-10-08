using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IIsotropicContinuumMaterial3D : IContinuumMaterial3D
    {
        double YoungModulus { get; set; }
        double PoissonRatio { get; set; }
    }
}
