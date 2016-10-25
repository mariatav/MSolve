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

    public interface IFiniteElement //TODOMaria elemental loads should be calculated like this: the one who calls CalculateStresses et.c. should provide nodal displacements compliant to the constraints
        //QUESTION: Should we maybe rename it to IFiniteElementType??
    {
        int ID { get; }
        ElementDimensions ElementDimensions { get; }
        IFiniteElementDOFEnumerator DOFEnumerator { get; set; }
        IList<IList<DOFType>> GetElementDOFTypes(Element element);
        bool MaterialModified { get; }
        IMatrix2D<double> StiffnessMatrix(Element element);
        IMatrix2D<double> MassMatrix(Element element);
        IMatrix2D<double> DampingMatrix(Element element);
        void ResetMaterialModified();
        Tuple<double[], double[]> CalculateStresses(Element element, double[] localDisplacements, double[] localdDisplacements);//TODOMaria: The algorithm, as it stands, should first calculate stresses and then calculate the stiffness matrix. If I did it, I would have two sets of functions for the stresses, stiffnessMatrix and nodalforces: one that takes as input argument the element and returns the stiffness matrix at the current state and one that takes as input argument the element and the current displacement and incremental vector which mutates the material and returns the forces and stiffness matrix at that state
        double[] CalculateForces(Element element, double[] localDisplacements, double[] localdDisplacements);
        double[] CalculateForcesForLogging(Element element, double[] localDisplacements);
        double[] CalculateAccelerationForces(Element element, IList<MassAccelerationLoad> loads);
        void SaveMaterialState();
        void ClearMaterialState();

        void ClearMaterialStresses();
    }
}
