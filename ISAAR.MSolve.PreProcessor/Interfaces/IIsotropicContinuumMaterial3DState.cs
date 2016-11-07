using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IIsotropicContinuumMaterial3DState : IContinuumMaterial3DState
    {
        double YoungModulus { get; set; }//QUESTION: maybe rename to TangentYoungModulus?
        double PoissonRatio { get; set; }//QUESTION: maybe rename to TangentPoissonRatio?
    }
}
