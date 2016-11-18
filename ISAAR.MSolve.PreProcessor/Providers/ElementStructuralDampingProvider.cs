using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor.Providers
{
    public class ElementStructuralDampingProvider : IElementMatrixProvider
    {
        #region IElementMatrixProvider Members

        public IMatrix2D<double> Matrix(IFiniteElement element)
        {
            //return element.M;
            return element.DampingMatrix();
        }

        #endregion
    }
}
