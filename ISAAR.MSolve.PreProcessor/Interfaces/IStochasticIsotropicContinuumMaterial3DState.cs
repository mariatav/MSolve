using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IStochasticIsotropicContinuumMaterial3DState : IIsotropicContinuumMaterial3DState
    {
        IStochasticMaterialCoefficientsProvider CoefficientsProvider { get; set; }
        ElasticityTensorContinuum3D GetConstitutiveMatrix(double[] coordinates);
    }
}
