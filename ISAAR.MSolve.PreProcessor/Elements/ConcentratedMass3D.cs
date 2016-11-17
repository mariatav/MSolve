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

        public int ID { get; set; }

        public ElementDimensions ElementDimensions
        {
            get { return ElementDimensions.ThreeD; }
        }

        public IFiniteElementDOFEnumerator DOFEnumerator
        {
            get { return dofEnumerator; }
            set { dofEnumerator = value; }
        }

        public IList<IList<DOFType>> GetElementDOFTypes()//QUESTION: Isn't a concentrated mass applied on a node?? Why do we pass an element argument???
        {
            //if (element == null) return dofs;

            var d = new List<IList<DOFType>>();
            foreach (var node in this.Nodes)//QUESTION: Is this right?? Should a concentrated mass contain more than one node??
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

        public IMatrix2D<double> StiffnessMatrix()//QUESTION: why does a mass element return a stiffness matrix??
        {
            return new SymmetricMatrix2D<double>(new double[6]);
        }

        public IMatrix2D<double> DampingMatrix()//QUESTION: why does a mass element return a damping matrix??
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

        public double[] CalculateAccelerationForces(IList<MassAccelerationLoad> loads)
        {
            Vector<double> accelerations = new Vector<double>(3);
            IMatrix2D<double> massMatrix = MassMatrix();

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

        #region Element Members
        private readonly Dictionary<int, Node> nodesDictionary = new Dictionary<int, Node>();
        private readonly Dictionary<DOFType, AbsorptionType> absorptions = new Dictionary<DOFType, AbsorptionType>();
        private readonly IList<Node> embeddedNodes = new List<Node>();

        public Dictionary<int, Node> NodesDictionary
        {
            get { return nodesDictionary; }
        }

        public Dictionary<DOFType, AbsorptionType> Absorptions
        {
            get { return absorptions; }
        }

        public IList<Node> Nodes
        {
            get { return nodesDictionary.Values.ToList<Node>(); }
        }

        public IList<Node> EmbeddedNodes
        {
            get { return embeddedNodes; }
        }

        //public IFiniteElementMaterial MaterialType { get; set; }
        public Subdomain Subdomain { get; set; }
        public int[] DOFs { get; set; }

        public void AddNode(Node node)//QUESTION: should this class contain more than one node??
        {
            nodesDictionary.Add(node.ID, node);
        }

        public void AddNodes(IList<Node> nodes)
        {
            foreach (Node node in nodes) AddNode(node);
        }

        //public IMatrix2D<double> K
        //{
        //    get { return ElementType.StiffnessMatrix(this); }
        //}

        //public IMatrix2D<double> M
        //{
        //    get { return ElementType.MassMatrix(this); }
        //}
        #endregion Element Members
    }
}
