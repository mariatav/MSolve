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
        int ID { get; }//QUESTION: do I need to add the Element public methods/properties to the interface or are element's methods only needed by the ElementTypes??
        IList<Node> Nodes { get; }
        Dictionary<int, Node> NodesDictionary { get; }
        Subdomain Subdomain { get; set; }
        ElementDimensions ElementDimensions { get; }
        IFiniteElementDOFEnumerator DOFEnumerator { get; set; }
        IList<IList<DOFType>> GetElementDOFTypes();
        bool MaterialModified { get; }
        IMatrix2D<double> StiffnessMatrix();
        IMatrix2D<double> MassMatrix();
        IMatrix2D<double> DampingMatrix();
        void ResetMaterialModified();
        Tuple<double[], double[]> CalculateStresses(double[] localDisplacements, double[] localdDisplacements);
        double[] CalculateForces(double[] localDisplacements, double[] localdDisplacements);
        double[] CalculateForcesForLogging(double[] localDisplacements);
        double[] CalculateAccelerationForces(IList<MassAccelerationLoad> loads);
        void SaveMaterialState();
        void ClearMaterialState();

        void ClearMaterialStresses();
    }
}
