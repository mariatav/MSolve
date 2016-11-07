using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IContinuumMaterial3DState : IFiniteElementMaterialState
    {
        StressStrainVectorContinuum3D Stresses { get; }
        ElasticityTensorContinuum3D ConstitutiveMatrix { get; }
        void UpdateMaterial(StressStrainVectorContinuum3D strains);
        void ClearState();
        void SaveState();
        void ClearStresses();
    }
}
