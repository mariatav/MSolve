﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;

namespace ISAAR.MSolve.PreProcessor.Embedding
{
    public class EmbeddedGrouping
    {
        private readonly Model model;
        private readonly IEnumerable<IFiniteElement> hostGroup;
        private readonly IEnumerable<IFiniteElement> embeddedGroup;
        private readonly bool hasEmbeddedRotations = false;

        public IEnumerable<IFiniteElement> HostGroup { get { return hostGroup; } }
        public IEnumerable<IFiniteElement> EmbeddedGroup { get { return embeddedGroup; } }

        public EmbeddedGrouping(Model model, IEnumerable<IFiniteElement> hostGroup, IEnumerable<IFiniteElement> embeddedGroup, bool hasEmbeddedRotations)
        {
            this.model = model;
            this.hostGroup = hostGroup;
            this.embeddedGroup = embeddedGroup;
            this.hasEmbeddedRotations = hasEmbeddedRotations;
            hostGroup.Select(e => e).Distinct().ToList().ForEach(et =>
            {
                if (!(et is IEmbeddedHostElement))
                    throw new ArgumentException("EmbeddedGrouping: One or more elements of host group does NOT implement IEmbeddedHostElement.");
            });
            embeddedGroup.Select(e => e).Distinct().ToList().ForEach(et =>
            {
                if (!(et is IEmbeddedElement))
                    throw new ArgumentException("EmbeddedGrouping: One or more elements of embedded group does NOT implement IEmbeddedElement.");
            });
            UpdateNodesBelongingToEmbeddedElements();
        }

        public EmbeddedGrouping(Model model, IEnumerable<IFiniteElement> hostGroup, IEnumerable<IFiniteElement> embeddedGroup)
            : this(model, hostGroup, embeddedGroup, false)
        {
        }

        private void UpdateNodesBelongingToEmbeddedElements()
        {
            IEmbeddedDOFInHostTransformationVector transformer;
            if (hasEmbeddedRotations)
                transformer = new Hexa8TranslationAndRotationTransformationVector();
            else
                transformer = new Hexa8TranslationTransformationVector();

            foreach (var embeddedElement in embeddedGroup)
            {
                var elType = (IEmbeddedElement)embeddedElement;
                foreach (var node in embeddedElement.Nodes)
                {
                    var embeddedNodes = hostGroup
                        .Select(e => ((IEmbeddedHostElement)e).BuildHostElementEmbeddedNode(node, transformer))
                        .Where(e => e != null);
                    foreach (var embeddedNode in embeddedNodes)
                    {
                        if (elType.EmbeddedNodes.Count(x => x.Node == embeddedNode.Node) == 0)
                            elType.EmbeddedNodes.Add(embeddedNode);

                        // Update embedded node information for elements that are not inside the embedded group but contain an embedded node.
                        foreach (var element in model.Elements.Except(embeddedGroup))
                            if (element is IEmbeddedElement && element.Nodes.Contains(embeddedNode.Node))
                            {
                                var currentElementType = (IEmbeddedElement)element;
                                if (!currentElementType.EmbeddedNodes.Contains(embeddedNode))
                                {
                                    currentElementType.EmbeddedNodes.Add(embeddedNode);
                                    element.DOFEnumerator = new ElementEmbedder(model, element, transformer);
                                }
                            }
                    }
                }

                embeddedElement.DOFEnumerator = new ElementEmbedder(model, embeddedElement, transformer);
            }
        }
    }
}
