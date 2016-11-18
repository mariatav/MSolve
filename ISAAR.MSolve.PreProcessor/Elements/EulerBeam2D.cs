using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.PreProcessor.Materials;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Elements
{
    public class EulerBeam2D : IStructuralFiniteElement
    {
        private static readonly DOFType[] nodalDOFTypes = new DOFType[3] { DOFType.X, DOFType.Y, DOFType.RotZ };
        private static readonly DOFType[][] dofs = new DOFType[][] { nodalDOFTypes, nodalDOFTypes };
        private readonly double youngModulus;
        private IFiniteElementDOFEnumerator dofEnumerator = new GenericDOFEnumerator();

        public double Density { get; set; }
        public double SectionArea { get; set; }
        public double MomentOfInertia { get; set; }

        public EulerBeam2D(double youngModulus)
        {
            this.youngModulus = youngModulus;
        }

        public EulerBeam2D(double youngModulus, IFiniteElementDOFEnumerator dofEnumerator)
            : this(youngModulus)
        {
            this.dofEnumerator = dofEnumerator;
        }

        public IFiniteElementDOFEnumerator DOFEnumerator
        {
            get { return dofEnumerator; }
            set { dofEnumerator = value; }
        }

        #region IElementType Members

        public ElementDimensions ElementDimensions
        {
            get { return ElementDimensions.TwoD; }
        }

        public IList<IList<DOFType>> GetElementDOFTypes()
        {
            return dofs;
        }

        public IList<Node> GetNodesForMatrixAssembly(IFiniteElement element)
        {
            return element.Nodes;
        }

        //[  c^2*E*A/L+12*s^2*E*I/L^3,  s*E*A/L*c-12*c*E*I/L^3*s,              -6*E*I/L^2*s, -c^2*E*A/L-12*s^2*E*I/L^3, -s*E*A/L*c+12*c*E*I/L^3*s,              -6*E*I/L^2*s]
        //[  s*E*A/L*c-12*c*E*I/L^3*s,  s^2*E*A/L+12*c^2*E*I/L^3,               6*E*I/L^2*c, -s*E*A/L*c+12*c*E*I/L^3*s, -s^2*E*A/L-12*c^2*E*I/L^3,               6*E*I/L^2*c]
        //[              -6*E*I/L^2*s,               6*E*I/L^2*c,                   4*E*I/L,               6*E*I/L^2*s,              -6*E*I/L^2*c,                   2*E*I/L]
        //[ -c^2*E*A/L-12*s^2*E*I/L^3, -s*E*A/L*c+12*c*E*I/L^3*s,               6*E*I/L^2*s,  c^2*E*A/L+12*s^2*E*I/L^3,  s*E*A/L*c-12*c*E*I/L^3*s,               6*E*I/L^2*s]
        //[ -s*E*A/L*c+12*c*E*I/L^3*s, -s^2*E*A/L-12*c^2*E*I/L^3,              -6*E*I/L^2*c,  s*E*A/L*c-12*c*E*I/L^3*s,  s^2*E*A/L+12*c^2*E*I/L^3,              -6*E*I/L^2*c]
        //[              -6*E*I/L^2*s,               6*E*I/L^2*c,                   2*E*I/L,               6*E*I/L^2*s,              -6*E*I/L^2*c,                   4*E*I/L]
        public IMatrix2D<double> StiffnessMatrix()
        {
            double x2 = Math.Pow(this.Nodes[1].X - this.Nodes[0].X, 2);
            double y2 = Math.Pow(this.Nodes[1].Y - this.Nodes[0].Y, 2);
            double L = Math.Sqrt(x2 + y2);
            double c = (this.Nodes[1].X - this.Nodes[0].X) / L;
            double c2 = c * c;
            double s = (this.Nodes[1].Y - this.Nodes[0].Y) / L;
            double s2 = s * s;
            double EL = this.youngModulus;
            double EAL = EL * SectionArea;
            double EIL = EL * MomentOfInertia;
            double EIL2 = EIL / L;
            double EIL3 = EIL2 / L;
            return dofEnumerator.GetTransformedMatrix(new SymmetricMatrix2D<double>(new double[] { c2*EAL+12*s2*EIL3, c*s*EAL-12*c*s*EIL3, -6*s*EIL2, -c2*EAL-12*s2*EIL3, -c*s*EAL+12*c*s*EIL3, -6*s*EIL2,
                s2*EAL+12*c2*EIL3, 6*c*EIL2, -s*c*EAL+12*c*s*EIL3, -s2*EAL-12*c2*EIL3, 6*c*EIL2,
                4*EIL, 6*s*EIL2, -6*c*EIL2, 2*EIL,
                c2*EAL+12*s2*EIL3, s*c*EAL-12*c*s*EIL3, 6*s*EIL2,
                s2*EAL+12*c2*EIL3, -6*c*EIL2,
                4*EIL }));
        }

        ////[ 140*c^2+156*s^2,         -16*c*s,         -22*s*L,   70*c^2+54*s^2,          16*c*s,          13*s*L]
        ////[         -16*c*s, 140*s^2+156*c^2,          22*c*L,          16*c*s,   70*s^2+54*c^2,         -13*c*L]
        ////[         -22*s*L,          22*c*L,           4*L^2,         -13*s*L,          13*c*L,          -3*L^2]
        ////[   70*c^2+54*s^2,          16*c*s,         -13*s*L, 140*c^2+156*s^2,         -16*c*s,          22*s*L]
        ////[          16*c*s,   70*s^2+54*c^2,          13*c*L,         -16*c*s, 140*s^2+156*c^2,         -22*c*L]
        ////[          13*s*L,         -13*c*L,          -3*L^2,          22*s*L,         -22*c*L,           4*L^2]
        //public IMatrix2D<double> MassMatrix(Element element)
        //{
        //    double x2 = Math.Pow(element.Nodes[1].X - element.Nodes[0].X, 2);
        //    double y2 = Math.Pow(element.Nodes[1].Y - element.Nodes[0].Y, 2);
        //    double L = Math.Sqrt(x2 + y2);
        //    double L2 = L * L;
        //    double c = (element.Nodes[1].X - element.Nodes[0].X) / L;
        //    double c2 = c * c;
        //    double s = (element.Nodes[1].Y - element.Nodes[0].Y) / L;
        //    double s2 = s * s;
        //    double dAL420 = Density * SectionArea * L / 420;
        //    return new SymmetricMatrix2D<double>(new double[] { dAL420*(140*c2+156*s2), -16*dAL420*c*s, -22*dAL420*s*L, dAL420*(70*c2+54*s2), 16*dAL420*c*s, 13*dAL420*s*L,
        //        dAL420*(140*s2+156*c2), 22*dAL420*c*L, 16*dAL420*c*s, dAL420*(70*s2+54*c2), -13*dAL420*c*L,
        //        4*dAL420*L2, -13*dAL420*s*L, 13*dAL420*c*L, -3*dAL420*L2,
        //        dAL420*(140*c2+156*s2), -16*dAL420*c*s, 22*dAL420*s*L,
        //        dAL420*(140*s2+156*c2), -22*dAL420*c*L,
        //        4*dAL420*L2 });
        //}

        //[ 140*c^2+156*s^2,         -16*c*s,         -22*s*L,   70*c^2+54*s^2,          16*c*s,          13*s*L]
        //[         -16*c*s, 140*s^2+156*c^2,          22*c*L,          16*c*s,   70*s^2+54*c^2,         -13*c*L]
        //[         -22*s*L,          22*c*L,           4*L^2,         -13*s*L,          13*c*L,          -3*L^2]
        //[   70*c^2+54*s^2,          16*c*s,         -13*s*L, 140*c^2+156*s^2,         -16*c*s,          22*s*L]
        //[          16*c*s,   70*s^2+54*c^2,          13*c*L,         -16*c*s, 140*s^2+156*c^2,         -22*c*L]
        //[          13*s*L,         -13*c*L,          -3*L^2,          22*s*L,         -22*c*L,           4*L^2]
        public IMatrix2D<double> MassMatrix(IFiniteElement element)
        {
            double x2 = Math.Pow(element.Nodes[1].X - element.Nodes[0].X, 2);
            double y2 = Math.Pow(element.Nodes[1].Y - element.Nodes[0].Y, 2);
            double L = Math.Sqrt(x2 + y2);
            double L2 = L * L;
            double c = (element.Nodes[1].X - element.Nodes[0].X) / L;
            double c2 = c * c;
            double s = (element.Nodes[1].Y - element.Nodes[0].Y) / L;
            double s2 = s * s;
            double dAL420 = Density * SectionArea * L / 420;

            double totalMass = Density * SectionArea * L;
            double totalMassOfDiagonalTerms = 2 * dAL420 * (140 * c2 + 156 * s2) + 2 * dAL420 * (140 * s2 + 156 * c2);
            double scale = totalMass / totalMassOfDiagonalTerms;

            return new SymmetricMatrix2D<double>(new double[] { dAL420*(140*c2+156*s2)*scale, 0, 0, 0, 0, 0,
                dAL420*(140*s2+156*c2)*scale, 0, 0, 0, 0,
                0, 0, 0, 0,
                dAL420*(140*c2+156*s2)*scale, 0, 0,
                dAL420*(140*s2+156*c2)*scale, 0,
                0 });
        }

        public IMatrix2D<double> DampingMatrix(IFiniteElement element)
        {
            throw new NotImplementedException();
        }

        public Tuple<double[], double[]> CalculateStresses(double[] localDisplacements, double[] localdDisplacements)
        {
            throw new NotImplementedException();
        }

        public double[] CalculateForcesForLogging(double[] localDisplacements)
        {
            return CalculateForces(localDisplacements, new double[localDisplacements.Length]);
        }

        public double[] CalculateForces(double[] localDisplacements, double[] localdDisplacements)
        {
            throw new NotImplementedException();
        }

        public double[] CalculateAccelerationForces(IFiniteElement element, IList<MassAccelerationLoad> loads)
        {
            Vector<double> accelerations = new Vector<double>(6);
            IMatrix2D<double> massMatrix = MassMatrix(element);

            int index = 0;
            foreach (MassAccelerationLoad load in loads)
                foreach (DOFType[] nodalDOFTypes in dofs)
                    foreach (DOFType dofType in nodalDOFTypes)
                    {
                        if (dofType == load.DOF) accelerations[index] += load.Amount;
                        index++;
                    }

            double[] forces = new double[6];
            massMatrix.Multiply(accelerations, forces);
            return forces;
        }

        public void SaveMaterialState()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IFiniteElement Members


        public bool MaterialModified
        {
            get
            {
                return false;
            }
        }

        public void ResetMaterialModified()
        {

        }

        #endregion

        #region IFiniteElement Members

        public void ClearMaterialState()
        {
        }

        public void ClearMaterialStresses()
        {
            throw new NotImplementedException();
        }

        #endregion

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
