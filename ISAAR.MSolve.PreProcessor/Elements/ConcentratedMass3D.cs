using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Elements
{
    public class ConcentratedMass3D : IStructuralFiniteElement
    {
        private static readonly DOFType[] nodalDOFTypes = new DOFType[] { DOFType.X, DOFType.Y, DOFType.Z };
        private static readonly DOFType[][] dofs = new DOFType[][] { nodalDOFTypes };
        private readonly double massCoefficient;
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

        public IList<IList<DOFType>> GetElementDOFTypes()
        {
            //QUESTION: is it right to comment out the code below???
            //if (element == null) return dofs;

            var d = new List<IList<DOFType>>();
            foreach (var node in this.Nodes)
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

        public ConcentratedMass3D(double massCoefficient)
        {
            this.massCoefficient = massCoefficient;
        }

        public ConcentratedMass3D(double massCoefficient, IFiniteElementDOFEnumerator dofEnumerator)
            : this(massCoefficient)
        {
            this.dofEnumerator = dofEnumerator;
        }

        public IMatrix2D<double> MassMatrix()
        {
            return new SymmetricMatrix2D<double>(new double[] { massCoefficient, 0, 0,
                massCoefficient, 0,
                massCoefficient
            });
        }

        public IMatrix2D<double> StiffnessMatrix()
        {
            return new SymmetricMatrix2D<double>(new double[6]);
        }

        public IMatrix2D<double> DampingMatrix()
        {
            return new SymmetricMatrix2D<double>(new double[6]);
        }

        public void ResetMaterialModified()
        {
        }

        public Tuple<double[], double[]> CalculateStresses(double[] localDisplacements, double[] localdDisplacements)
        {
            return new Tuple<double[], double[]>(new double[6], new double[6]);
        }

        public double[] CalculateForcesForLogging(double[] localDisplacements)
        {
            return CalculateForces(localDisplacements, new double[localDisplacements.Length]);
        }

        public double[] CalculateForces(double[] localDisplacements, double[] localdDisplacements)
        {
            return new double[6];
        }

        public double[] CalculateAccelerationForces(IFiniteElement element, IList<MassAccelerationLoad> loads)
        {
            Vector<double> accelerations = new Vector<double>(3);
            IMatrix2D<double> massMatrix = this.MassMatrix();

            foreach (MassAccelerationLoad load in loads)
            {
                int index = 0;
                foreach (DOFType[] nodalDOFTypes in dofs)
                    foreach (DOFType dofType in nodalDOFTypes)
                    {
                        if (dofType == load.DOF) accelerations[index] += load.Amount;
                        index++;
                    }
            }
            double[] forces = new double[3];
            massMatrix.Multiply(accelerations, forces);
            return forces;
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
