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
        public IList<IList<DOFType>> GetDOFTypes(IFiniteElement element)//TODOMaria remove element argument
        {
            return element.GetElementDOFTypes();
        }

        public IList<IList<DOFType>> GetDOFTypesForDOFEnumeration(IFiniteElement element)//TODOMaria remove element argument
        {
            return element.GetElementDOFTypes();
        }

        public IMatrix2D<double> GetTransformedMatrix(IMatrix2D<double> matrix)
        {
            return matrix;
        }

        public IList<Node> GetNodesForMatrixAssembly(IFiniteElement element)//TODOMaria remove element argument
        {
            return element.Nodes;
        }

        public double[] GetTransformedVector(double[] vector)
        {
            return vector;
        }
    }
}
