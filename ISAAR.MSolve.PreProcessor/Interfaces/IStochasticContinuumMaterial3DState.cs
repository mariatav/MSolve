using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IStochasticContinuumMaterial3DState : IContinuumMaterial3DState
    {
        IStochasticMaterialCoefficientsProvider CoefficientsProvider { get; }
        ElasticityTensorContinuum3D GetConstitutiveMatrix(double[] coordinates);
    }
}
