using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IIsotropicContinuumMaterial3DProperty
    {
        IIsotropicContinuumMaterial3DState BuildIsotropicContinuumMaterial3DState(double[] coordinates);
    }
}
