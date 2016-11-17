using System;
using System.Collections.Generic;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiniteElementDOFEnumerator
    {
        IList<IList<DOFType>> GetDOFTypes(IFiniteElement element);
        IList<IList<DOFType>> GetDOFTypesForDOFEnumeration(IFiniteElement element);
        IList<Node> GetNodesForMatrixAssembly(IFiniteElement element);
        IMatrix2D<double> GetTransformedMatrix(IMatrix2D<double> matrix);
        double[] GetTransformedVector(double[] vector);
    }
}
