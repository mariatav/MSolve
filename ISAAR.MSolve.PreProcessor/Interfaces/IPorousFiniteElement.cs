using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IPorousFiniteElement : IFiniteElement
    {
        IMatrix2D<double> PermeabilityMatrix();
        IMatrix2D<double> CouplingMatrix(IFiniteElement element);
        IMatrix2D<double> SaturationMatrix(IFiniteElement element);
    }
}
