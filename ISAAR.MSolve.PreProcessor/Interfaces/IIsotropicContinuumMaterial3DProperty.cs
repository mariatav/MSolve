using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IIsotropicContinuumMaterial3DProperty: IContinuumMaterial3DProperty
    {
        IIsotropicContinuumMaterial3DState BuildMaterialState(double[] coordinates);
    }
}
