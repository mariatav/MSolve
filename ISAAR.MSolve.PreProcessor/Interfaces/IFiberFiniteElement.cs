﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiberFiniteElement : IFiniteElement
    {
        IFiberFiniteElementMaterialState Material { get; }
        IList<IFiber> Fibers { get; }
    }
}
