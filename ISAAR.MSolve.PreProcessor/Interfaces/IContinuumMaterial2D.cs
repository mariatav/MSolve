using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IContinuumMaterial2D : IFiniteElementMaterial
    {
        StressStrainVectorContinuum2D Stresses { get; }
        ElasticityTensorContinuum2D ConstitutiveMatrix { get; }
        void UpdateMaterial(StressStrainVectorContinuum2D strains);
        void ClearState();
        void SaveState();
        void ClearStresses();
    }
}
