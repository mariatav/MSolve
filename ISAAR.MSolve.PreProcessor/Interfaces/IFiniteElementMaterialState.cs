﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiniteElementMaterialState : ICloneable
    {
        int ID { get; }
        bool Modified { get; }
        void ResetModified();
        double[] Coordinates { get; }
    }
}