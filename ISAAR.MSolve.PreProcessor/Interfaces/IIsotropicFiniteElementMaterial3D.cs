using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IIsotropicFiniteElementMaterial3D : IFiniteElementMaterial3D
    {
        double YoungModulus { get; set; }
        double PoissonRatio { get; set; }
    }
}
