using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IStochasticIsotropicContinuumMaterial3DProperty
    {
        IStochasticIsotropicContinuumMaterial3DState BuildStochasticIsotropicContinuumMaterial3DState(double[] coordinates);
    }
}
