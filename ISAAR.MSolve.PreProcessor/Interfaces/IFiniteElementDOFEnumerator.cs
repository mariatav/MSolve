using System;
using System.Collections.Generic;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IFiniteElementDOFEnumerator//Is one instance of this contained per element??
    {
        IList<IList<DOFType>> GetDOFTypes(IFiniteElement element);//QUESTION: do we need the element argument??
        IList<IList<DOFType>> GetDOFTypesForDOFEnumeration(IFiniteElement element);//QUESTION: do we need the element argument??
        IList<Node> GetNodesForMatrixAssembly(IFiniteElement element);//QUESTION: do we need the element argument??
        IMatrix2D<double> GetTransformedMatrix(IMatrix2D<double> matrix);
        double[] GetTransformedVector(double[] vector);
    }
}
