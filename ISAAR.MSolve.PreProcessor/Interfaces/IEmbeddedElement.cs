using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Embedding;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IEmbeddedElement
    {
        IList<EmbeddedNode> EmbeddedNodes { get; }
        Dictionary<DOFType, int> GetInternalNodalDOFs(Node node);//QUESTION: Is this right??
        double[] GetLocalDOFValues(IFiniteElement hostElement, double[] hostDOFValues);//QUESTION: Is this right??
    }
}
