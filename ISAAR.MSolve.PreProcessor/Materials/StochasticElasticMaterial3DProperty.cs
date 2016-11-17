﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Materials
{
    public class StochasticElasticMaterial3DProperty : IStochasticIsotropicContinuumMaterial3DProperty, IIsotropicContinuumMaterial3DProperty
    {
        public double YoungModulus { get; }
        public double PoissonRatio { get; }
        private readonly IStochasticMaterialCoefficientsProvider coefficientsProvider;//QUESTION: why is this private??
        public StochasticElasticMaterial3DProperty(double youngModulus, double poissonRatio, IStochasticMaterialCoefficientsProvider coefficientsProvider)
        {
            this.YoungModulus = youngModulus;
            this.PoissonRatio = poissonRatio;
            this.coefficientsProvider = coefficientsProvider;
        }
        public IStochasticIsotropicContinuumMaterial3DState BuildMaterialState(double[] coordinates)
        {
            return new StochasticElasticMaterial3DState(this.YoungModulus, this.PoissonRatio, this.coefficientsProvider, coordinates);
        }
        IIsotropicContinuumMaterial3DState IIsotropicContinuumMaterial3DProperty.BuildMaterialState(double[] coordinates)
        {
            return BuildMaterialState(coordinates);
        }
        IContinuumMaterial3DState IContinuumMaterial3DProperty.BuildMaterialState(double[] coordinates)
        {
            return BuildMaterialState(coordinates);
        }
        IFiniteElementMaterialState IFiniteElementMaterialProperty.BuildMaterialState(double[] coordinates)
        {
            return BuildMaterialState(coordinates);
        }
        private class StochasticElasticMaterial3DState : IStochasticIsotropicContinuumMaterial3DState, IIsotropicContinuumMaterial3DState
        {
            private IStochasticMaterialCoefficientsProvider coefficientsProvider;
            private readonly double[] strains = new double[6];
            private readonly double[] stresses = new double[6];
            //private double[,] constitutiveMatrix = null;
            public double YoungModulus { get; }
            public double PoissonRatio { get; }
            public double[] Coordinates { get; }

            public StochasticElasticMaterial3DState(double youngModulus, double poissonRatio, IStochasticMaterialCoefficientsProvider coefficientsProvider, double[] coordinates)
            {
                this.YoungModulus = youngModulus;
                this.PoissonRatio = poissonRatio;
                this.coefficientsProvider = coefficientsProvider;
                this.Coordinates = new double[coordinates.Length];
                System.Array.Copy(coordinates, this.Coordinates, coordinates.Length);
            }

            private double[,] GetConstitutiveMatrixInternal(double[] coordinates)
            {
                //double variation = coefficientsProvider.GetCoefficient(YoungModulus, coordinates);
                //double fE1 = variation * YoungModulus / (1.0 + PoissonRatio);
                double stochasticYoungModulus = coefficientsProvider.GetCoefficient(YoungModulus, coordinates);
                double fE1 = stochasticYoungModulus / (1.0 + PoissonRatio);
                double fE2 = fE1 * PoissonRatio / (1.0 - 2.0 * PoissonRatio);
                double fE3 = fE1 + fE2;
                double fE4 = fE1 * 0.5;
                double[,] afE = new double[6, 6];
                afE[0, 0] = fE3;
                afE[0, 1] = fE2;
                afE[0, 2] = fE2;
                afE[1, 0] = fE2;
                afE[1, 1] = fE3;
                afE[1, 2] = fE2;
                afE[2, 0] = fE2;
                afE[2, 1] = fE2;
                afE[2, 2] = fE3;
                afE[3, 3] = fE4;
                afE[4, 4] = fE4;
                afE[5, 5] = fE4;

                //Vector<double> s = (new Matrix2D<double>(afE)) * (new Vector<double>(strains));
                //s.Data.CopyTo(stresses, 0);

                return afE;
            }

            #region IFiniteElementMaterial Members

            public int ID
            {
                get { return 1; }
            }

            public bool Modified
            {
                get { return false; }
            }

            public void ResetModified()
            {
            }

            #endregion

            #region IFiniteElementMaterial3D Members

            public StressStrainVectorContinuum3D Stresses { get { return new StressStrainVectorContinuum3D(new double[6]); } }

            public ElasticityTensorContinuum3D ConstitutiveMatrix
            {
                get
                {
                    throw new NotImplementedException();
                    //if (constitutiveMatrix == null) UpdateMaterial(new double[6]);
                    //return new Matrix2D<double>(constitutiveMatrix);
                }
            }

            public void UpdateMaterial(StressStrainVectorContinuum3D strains)
            {
                throw new NotImplementedException();

                //strains.CopyTo(this.strains, 0);
                //constitutiveMatrix = GetConstitutiveMatrix();
            }

            public void ClearState()
            {
            }

            public void SaveState()
            {
                throw new NotImplementedException();
            }

            public void ClearStresses()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region ICloneable Members

            public object Clone()
            {
                return new StochasticElasticMaterial3DState(this.YoungModulus, this.PoissonRatio, this.coefficientsProvider, this.Coordinates);
            }

            #endregion

            #region IStochasticContinuumMaterial3D Members
            public IStochasticMaterialCoefficientsProvider CoefficientsProvider
            {
                get { return coefficientsProvider; }
            }

            public ElasticityTensorContinuum3D GetConstitutiveMatrix(double[] coordinates)
            {
                return new ElasticityTensorContinuum3D(GetConstitutiveMatrixInternal(coordinates));
            }

            #endregion
        }
    }
}