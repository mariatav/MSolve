using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiniteElementMaterialProperty //QUESTION: Maybe it's useless. Could we define a material class that actually implements this?
    {
        IFiniteElementMaterialState BuildFiniteElementMaterialState(double[] coordinates);
    }
}
