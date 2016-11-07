using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiberMaterialState : IFiniteElementMaterialState
    {
        double Stress { get; }
        double Strain { get; }
        void UpdateMaterial(double dStrain);
        void SaveState();
        void ClearStresses();
        IFiberMaterialState Clone(IFiberFiniteElementMaterialState parent);

        double YoungModulus { get; set; }
        double PoissonRatio { get; set; } //It might be useless

        //double YoungModulusElastic { get; set; }
    }
}
