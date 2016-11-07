﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiberFiniteElementMaterialState : IFiniteElementMaterialState
    {
        IList<IFiberMaterialState> FiberMaterials { get; }
    }
}
