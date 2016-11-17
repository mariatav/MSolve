using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ISAAR.MSolve.Matrices.Interfaces;

namespace ISAAR.MSolve.PreProcessor.Interfaces
{
    public enum ElementDimensions
    {
        Unknown = 0,
        OneD = 1,
        TwoD = 2,
        ThreeD = 3
    }

    public interface IFiniteElement
    {
        int ID { get; }
        Dictionary<int, Node> NodesDictionary { get; }
        IList<Node> Nodes { get; }
        Subdomain Subdomain { get; set; }
        ElementDimensions ElementDimensions { get; }
        IFiniteElementDOFEnumerator DOFEnumerator { get; set; }
        IList<IList<DOFType>> GetElementDOFTypes();
        bool MaterialModified { get; }
        IMatrix2D<double> StiffnessMatrix();
        IMatrix2D<double> MassMatrix(IFiniteElement element);
        IMatrix2D<double> DampingMatrix(IFiniteElement element);
        void ResetMaterialModified();
        Tuple<double[], double[]> CalculateStresses(IFiniteElement element, double[] localDisplacements, double[] localdDisplacements);
        double[] CalculateForces(IFiniteElement element, double[] localDisplacements, double[] localdDisplacements);
        double[] CalculateForcesForLogging(IFiniteElement element, double[] localDisplacements);
        double[] CalculateAccelerationForces(IFiniteElement element, IList<MassAccelerationLoad> loads);
        void SaveMaterialState();
        void ClearMaterialState();

        void ClearMaterialStresses();
    }
}
