using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IContinuumMaterial2DProperty: IFiniteElementMaterialProperty
    {
        IContinuumMaterial2DState BuildMaterialState(double[] coordinates);
    }
}
