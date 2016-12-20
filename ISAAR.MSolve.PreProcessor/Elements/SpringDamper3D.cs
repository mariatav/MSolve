﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Elements
{
    public enum SpringDirections
    {
        X = 0,
        Y,
        Z,
        XY,
        YZ,
        XZ,
        XYZ
    }

    public class SpringDamper3D : IStructuralFiniteElement
    {
        private static readonly DOFType[] nodalDOFTypes = new DOFType[] { DOFType.X, DOFType.Y, DOFType.Z };
        private static readonly DOFType[][] dofs = new DOFType[][] { nodalDOFTypes, nodalDOFTypes };
        private readonly double springCoefficient, dampingCoefficient;
        private readonly SpringDirections springDirections, dampingDirections;
        private IFiniteElementDOFEnumerator dofEnumerator = new GenericDOFEnumerator();
        
        public ElementDimensions ElementDimensions
        {
            get { return ElementDimensions.ThreeD; }
        }

        public IFiniteElementDOFEnumerator DOFEnumerator
        {
            get { return dofEnumerator; }
            set { dofEnumerator = value; }
        }

        public IList<IList<DOFType>> GetElementDOFTypes(IFiniteElement element)
        {
            if (element == null) return dofs;

            var d = new List<IList<DOFType>>();
            foreach (var node in element.Nodes)
            {
                var nodeDofs = new List<DOFType>();
                nodeDofs.AddRange(nodalDOFTypes);
                d.Add(nodeDofs);
            }
            return d;
        }

        public bool MaterialModified
        {
            get { return false; }
        }

        public SpringDamper3D(double springCoefficient, double dampingCoefficient, SpringDirections springDirections, SpringDirections dampingDirections)
        {
            this.springCoefficient = springCoefficient;
            this.dampingCoefficient = dampingCoefficient;
            this.springDirections = springDirections;
            this.dampingDirections = dampingDirections;
        }

        public SpringDamper3D(double springCoefficient, double dampingCoefficient, SpringDirections springDirections, SpringDirections dampingDirections, IFiniteElementDOFEnumerator dofEnumerator)
            : this(springCoefficient, dampingCoefficient, springDirections, dampingDirections)
        {
            this.dofEnumerator = dofEnumerator;
        }

        public IMatrix2D<double> StiffnessMatrix(IFiniteElement element)
        {
            double x = (springDirections == SpringDirections.X || springDirections == SpringDirections.XY || springDirections == SpringDirections.XZ || springDirections == SpringDirections.XYZ) ? springCoefficient : 0;
            double y = (springDirections == SpringDirections.Y || springDirections == SpringDirections.XY || springDirections == SpringDirections.YZ || springDirections == SpringDirections.XYZ) ? springCoefficient : 0;
            double z = (springDirections == SpringDirections.Z || springDirections == SpringDirections.XZ || springDirections == SpringDirections.YZ || springDirections == SpringDirections.XYZ) ? springCoefficient : 0;
            return new SymmetricMatrix2D<double>(new double[] { x, 0, 0, -x, 0, 0,
                y, 0, 0, -y, 0, 
                z, 0, 0, -z,
                x, 0, 0,
                y, 0,
                z
            });
        }

        public IMatrix2D<double> MassMatrix(IFiniteElement element)
        {
            return new SymmetricMatrix2D<double>(new double[] { 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 
                0, 0, 0, 0,
                0, 0, 0,
                0, 0,
                0
            });
        }

        public IMatrix2D<double> DampingMatrix(IFiniteElement element)
        {
            double x = (dampingDirections == SpringDirections.X || dampingDirections == SpringDirections.XY || dampingDirections == SpringDirections.XZ || dampingDirections == SpringDirections.XYZ) ? dampingCoefficient : 0;
            double y = (dampingDirections == SpringDirections.Y || dampingDirections == SpringDirections.XY || dampingDirections == SpringDirections.YZ || dampingDirections == SpringDirections.XYZ) ? dampingCoefficient : 0;
            double z = (dampingDirections == SpringDirections.Z || dampingDirections == SpringDirections.XZ || dampingDirections == SpringDirections.YZ || dampingDirections == SpringDirections.XYZ) ? dampingCoefficient : 0;
            return new SymmetricMatrix2D<double>(new double[] { x, 0, 0, -x, 0, 0,
                y, 0, 0, -y, 0, 
                z, 0, 0, -z,
                x, 0, 0,
                y, 0,
                z
            });
        }

        public void ResetMaterialModified()
        {
        }

        public Tuple<double[], double[]> CalculateStresses(IFiniteElement element, double[] localDisplacements, double[] localdDisplacements)
        {
            return new Tuple<double[], double[]>(new double[6], new double[6]);
        }

        public double[] CalculateForcesForLogging(IFiniteElement element, double[] localDisplacements)
        {
            return CalculateForces(element, localDisplacements, new double[localDisplacements.Length]);
        }

        public double[] CalculateForces(IFiniteElement element, double[] localDisplacements, double[] localdDisplacements)
        {
            IMatrix2D<double> stiffnessMatrix = StiffnessMatrix(element);
            Vector<double> disps = new Vector<double>(localDisplacements);
            double[] forces = new double[localDisplacements.Length];
            stiffnessMatrix.Multiply(disps, forces);
            return forces;
        }

        public double[] CalculateAccelerationForces(IFiniteElement element, IList<MassAccelerationLoad> loads)
        {
            return new double[6];
        }

        public void ClearMaterialState()
        {
        }

        public void SaveMaterialState()
        {
        }

        public void ClearMaterialStresses()
        {
        }

        #region Element Class members
        public int ID { get; set; }
        private readonly Dictionary<int, Node> nodesDictionary = new Dictionary<int, Node>();
        public Dictionary<int, Node> NodesDictionary
        {
            get { return nodesDictionary; }
        }
        public IList<Node> Nodes
        {
            get { return nodesDictionary.Values.ToList<Node>(); }
        }
        public Subdomain Subdomain { get; set; }

        #region Possibly useless code
        private readonly Dictionary<DOFType, AbsorptionType> absorptions = new Dictionary<DOFType, AbsorptionType>();
        //private readonly IList<Node> embeddedNodes = new List<Node>();

        public Dictionary<DOFType, AbsorptionType> Absorptions
        {
            get { return absorptions; }
        }

        //public IList<Node> EmbeddedNodes
        //{
        //    get { return embeddedNodes; }
        //}

        public int[] DOFs { get; set; }

        public void AddNode(Node node)
        {
            nodesDictionary.Add(node.ID, node);
        }

        public void AddNodes(IList<Node> nodes)
        {
            foreach (Node node in nodes) AddNode(node);
        }
        #endregion
        #endregion
    }
}
