using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Providers
{
    public class ElementPoreMassProvider : IElementMatrixProvider
    {
        private readonly IElementMatrixProvider solidMassProvider;
        private readonly double massCoefficient;

        public ElementPoreMassProvider(IElementMatrixProvider solidMassProvider, double massCoefficient)
        {
            this.solidMassProvider = solidMassProvider;
            this.massCoefficient = massCoefficient;
        }

        private IMatrix2D<double> PorousMatrix(IFiniteElement element)
        {
            IPorousFiniteElement porousElement = (IPorousFiniteElement)element;
            int dofs = 0;
            foreach (IList<DOFType> dofTypes in porousElement.DOFEnumerator.GetDOFTypes(element))
                foreach (DOFType dofType in dofTypes) dofs++;
            SymmetricMatrix2D<double> poreMass = new SymmetricMatrix2D<double>(dofs);

            IMatrix2D<double> mass = solidMassProvider.Matrix(element);

            int matrixRow = 0;
            int solidRow = 0;
            foreach (IList<DOFType> dofTypesRow in porousElement.DOFEnumerator.GetDOFTypes(element))
                foreach (DOFType dofTypeRow in dofTypesRow)
                {
                    int matrixCol = 0;
                    int solidCol = 0;
                    foreach (IList<DOFType> dofTypesCol in porousElement.DOFEnumerator.GetDOFTypes(element))
                        foreach (DOFType dofTypeCol in dofTypesCol)
                        {
                            if (dofTypeCol == DOFType.Pore)
                            {
                            }
                            else
                            {
                                if (dofTypeRow != DOFType.Pore)
                                    poreMass[matrixRow, matrixCol] = mass[solidRow, solidCol] * massCoefficient;
                                solidCol++;
                            }
                            matrixCol++;
                        }

                    if (dofTypeRow != DOFType.Pore) solidRow++;
                    matrixRow++;
                }

            return poreMass;
        }

        #region IElementMatrixProvider Members

        public IMatrix2D<double> Matrix(IFiniteElement element)
        {
            if (element is IPorousFiniteElement)
                return PorousMatrix(element);
            else
            {
                IMatrix2D<double> massMatrix = solidMassProvider.Matrix(element);
                massMatrix.Scale(massCoefficient);
                return massMatrix;
            }
        }

        #endregion
    }
}
