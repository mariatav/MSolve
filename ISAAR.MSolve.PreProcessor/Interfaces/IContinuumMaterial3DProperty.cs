using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IContinuumMaterial3DProperty
    {
        IContinuumMaterial3DState BuildContinuumMaterial3DState(double[] coordinates);
    }
}
