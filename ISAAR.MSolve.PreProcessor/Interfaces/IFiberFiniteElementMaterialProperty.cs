using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiberFiniteElementMaterialProperty: IFiniteElementMaterialProperty
    {
        IFiberFiniteElementMaterialState BuildMaterialState(double[] coordinates);
    }
}
