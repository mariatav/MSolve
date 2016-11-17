using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor
{
    public class GenericDOFEnumerator : IFiniteElementDOFEnumerator
    {
        public IList<IList<DOFType>> GetDOFTypes(IFiniteElement element)
        {
            return element.GetElementDOFTypes(element);
        }

        public IList<IList<DOFType>> GetDOFTypesForDOFEnumeration(IFiniteElement element)
        {
            return element.GetElementDOFTypes(element);
        }

        public IMatrix2D<double> GetTransformedMatrix(IMatrix2D<double> matrix)
        {
            return matrix;
        }

        public IList<Node> GetNodesForMatrixAssembly(IFiniteElement element)
        {
            return element.Nodes;
        }

        public double[] GetTransformedVector(double[] vector)
        {
            return vector;
        }
    }
}
