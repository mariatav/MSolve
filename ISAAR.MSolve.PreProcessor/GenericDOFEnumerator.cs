using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor
{
    public class GenericDOFEnumerator : IFiniteElementDOFEnumerator//QUESTION: Why would an element want to override this?
    {
        public IList<IList<DOFType>> GetDOFTypes(Element element)
        {
            return element.ElementType.GetElementDOFTypes(element);
        }

        public IList<IList<DOFType>> GetDOFTypesForDOFEnumeration(Element element)//QUESTION: Why is the input not IFiniteElement instead of element? only the ElementType inside the element can modify DOFEnumerator! 
        {
            return element.ElementType.GetElementDOFTypes(element);
        }

        public IMatrix2D<double> GetTransformedMatrix(IMatrix2D<double> matrix)
        {
            return matrix;
        }

        public IList<Node> GetNodesForMatrixAssembly(Element element)
        {
            return element.Nodes;
        }

        public double[] GetTransformedVector(double[] vector)
        {
            return vector;
        }
    }
}
