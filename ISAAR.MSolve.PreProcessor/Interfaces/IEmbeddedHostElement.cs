﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Embedding;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public interface IEmbeddedHostElement
    {
        EmbeddedNode BuildHostElementEmbeddedNode(IFiniteElement element, Node node, IEmbeddedDOFInHostTransformationVector transformationVector);
        double[] GetShapeFunctionsForNode(IFiniteElement element, EmbeddedNode node); 
    }
}
