using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IStochasticContinuumMaterial3DProperty
    {
        IStochasticContinuumMaterial3DState BuildStochasticContinuumMaterial3DState(double[] coordinates);
    }
}
