using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IContinuumMaterial3DProperty: IFiniteElementMaterialProperty
    {
        IContinuumMaterial3DState BuildMaterialState(double[] coordinates);
    }
}
