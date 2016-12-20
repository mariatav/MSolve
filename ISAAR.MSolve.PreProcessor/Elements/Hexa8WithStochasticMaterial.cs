﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.PreProcessor.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.Matrices;

namespace ISAAR.MSolve.PreProcessor.Elements
{
    public class Hexa8Memoizer
    {
        private readonly Dictionary<int, Tuple<double[], double[,,]>> integrationDictionary = new Dictionary<int, Tuple<double[], double[,,]>>();

        public Tuple<double[], double[,,]> GetIntegrationData(int element)
        {
            if (integrationDictionary.ContainsKey(element))
                return integrationDictionary[element];
            else
                return new Tuple<double[], double[,,]>(null, null);
        }

        public void SetIntegrationData(int element, Tuple<double[], double[,,]> integrationData)
        {
            integrationDictionary.Add(element, integrationData);
        }
    }

    public class Hexa8WithStochasticMaterial : Hexa8
    {
        protected readonly new IStochasticContinuumMaterial3DState[] materialStatesAtGaussPoints;
        protected readonly Hexa8Memoizer memoizer;

        private static double[][] integrationPoints = new double[][]
        {
            new double[] { },
            new double[] { 0 },
            new double[] { -0.5773502691896, 0.5773502691896 },
            new double[] { -0.774596669, 0, 0.774596669 },
            new double[] { -0.8611363115941, -0.3399810435849, 0.3399810435849, 0.8611363115941 }
        };

        public Hexa8WithStochasticMaterial(IStochasticContinuumMaterial3DProperty materialProperty)
        {
            materialStatesAtGaussPoints = new IStochasticContinuumMaterial3DState[iInt3];
            for (int i = 0; i < iInt3; i++)
                materialStatesAtGaussPoints[i] = materialProperty.BuildMaterialState(gaussPointsCoords[i]);
        }

        public Hexa8WithStochasticMaterial(IStochasticContinuumMaterial3DProperty materialProperty, Hexa8Memoizer memoizer) : this(materialProperty)
        {
            this.memoizer = memoizer;
        }

        //public Hexa8WithStochasticMaterial(IFiniteElementMaterial3D material, IStochasticCoefficientsProvider coefficientsProvider)
        //    : this(material)
        //{
        //    this.coefficientsProvider = coefficientsProvider;
        //}

        public override IMatrix2D<double> StiffnessMatrix()
        {
            double[,,] afE = new double[iInt3, 6, 6];
            int iPos = 0;
            for (int i1 = 0; i1 < iInt; i1++)
                for (int i2 = 0; i2 < iInt; i2++)
                    for (int i3 = 0; i3 < iInt; i3++)
                    {
                        iPos = i1 * iInt2 + i2 * iInt + i3;
                        var e = ((Matrix2D<double>)materialStatesAtGaussPoints[iPos].GetConstitutiveMatrix(this.GetStochasticPoints(i1, i2, i3)));
                        for (int j = 0; j < 6; j++)
                            for (int k = 0; k < 6; k++)
                                afE[iPos, j, k] = e[j, k];
                        //afE[i, j, k] = ((Matrix2D<double>)materialsAtGaussPoints[i].GetConstitutiveMatrix(GetStochasticPoints(element, i / iInt2, (i % iInt2) / iInt, i % iInt)))[j, k];
                    }

            double[,,] faB;
            double[] faWeight;
            Tuple<double[], double[,,]> integrationData = new Tuple<double[], double[,,]>(null, null);
            if (memoizer != null)
                integrationData = memoizer.GetIntegrationData(this.ID);
            if (integrationData.Item1 == null)
            {
                faB = new double[iInt3, 24, 6];
                faWeight = new double[iInt3];
                double[,] faXYZ = this.GetCoordinates();
                double[,] faDS = new double[iInt3, 24];
                double[,] faS = new double[iInt3, 8];
                double[] faDetJ = new double[iInt3];
                double[,,] faJ = new double[iInt3, 3, 3];
                CalcH8GaussMatrices(ref iInt, faXYZ, faWeight, faS, faDS, faJ, faDetJ, faB);
                //for (int i = 0; i < iInt; i++)
                //    for (int j = 0; j < iInt; j++)
                //        for (int k = 0; k < iInt; k++)
                //        {
                //            faWeight[i * iInt * iInt + j * iInt + k] *= coefficientsProvider.GetCoefficient(GetStochasticPoints(element, i, j, k));
                //            //faWeight[i * iInt * iInt + j * iInt + k] *= coefficientsProvider.GetCoefficient(new double[] { integrationPoints[iInt][i], integrationPoints[iInt][j], integrationPoints[iInt][k] });
                //        }
                if (memoizer != null)
                    memoizer.SetIntegrationData(this.ID, new Tuple<double[], double[,,]>(faWeight, faB));
            }
            else
            {
                faB = integrationData.Item2;
                faWeight = integrationData.Item1;
            }
            double[] faK = new double[300];
            CalcH8K(ref iInt, afE, faB, faWeight, faK);
            return dofEnumerator.GetTransformedMatrix(new SymmetricMatrix2D<double>(faK));
        }

        private double[] GetStochasticPoints(int iX, int iY, int iZ)
        {
            // Calculate for element centroid
            double X = 0;
            double Y = 0;
            double Z = 0;
            double minX = this.Nodes[0].X;
            double minY = this.Nodes[0].Y;
            double minZ = this.Nodes[0].Z;

            for (int i = 0; i < 8; i++)
            {
                minX = minX > this.Nodes[i].X ? this.Nodes[i].X : minX;
                minY = minY > this.Nodes[i].Y ? this.Nodes[i].Y : minY;
                minZ = minZ > this.Nodes[i].Z ? this.Nodes[i].Z : minZ;
                for (int j = i + 1; j < 8; j++)
                {
                    X = X < Math.Abs(this.Nodes[j].X - this.Nodes[i].X) ? Math.Abs(this.Nodes[j].X - this.Nodes[i].X) : X;
                    Y = Y < Math.Abs(this.Nodes[j].Y - this.Nodes[i].Y) ? Math.Abs(this.Nodes[j].Y - this.Nodes[i].Y) : Y;
                    Z = Z < Math.Abs(this.Nodes[j].Z - this.Nodes[i].Z) ? Math.Abs(this.Nodes[j].Z - this.Nodes[i].Z) : Z;
                }
            }

            double pointX = minX + X / 2;
            double pointY = minY + Y / 2;
            double pointZ = minZ + Z / 2;

            return new double[] { pointX, pointY, pointZ };

            //// Calculate for individual gauss point
            ////if (iInt != 2) throw new ArgumentException("Stochastic provided functions with integration order of 2.");

            //double X = 0;
            //double Y = 0;
            //double Z = 0;
            //double minX = element.Nodes[0].X;
            //double minY = element.Nodes[0].Y;
            //double minZ = element.Nodes[0].Z;

            //for (int i = 0; i < 8; i++)
            //{
            //    minX = minX > element.Nodes[i].X ? element.Nodes[i].X : minX;
            //    minY = minY > element.Nodes[i].Y ? element.Nodes[i].Y : minY;
            //    minZ = minZ > element.Nodes[i].Z ? element.Nodes[i].Z : minZ;
            //    for (int j = i + 1; j < 8; j++)
            //    {
            //        X = X < Math.Abs(element.Nodes[j].X - element.Nodes[i].X) ? Math.Abs(element.Nodes[j].X - element.Nodes[i].X) : X;
            //        Y = Y < Math.Abs(element.Nodes[j].Y - element.Nodes[i].Y) ? Math.Abs(element.Nodes[j].Y - element.Nodes[i].Y) : Y;
            //        Z = Z < Math.Abs(element.Nodes[j].Z - element.Nodes[i].Z) ? Math.Abs(element.Nodes[j].Z - element.Nodes[i].Z) : Z;
            //    }
            //}

            //double pointX = minX + X / 2 * (integrationPoints[iInt][iX] + 1);
            //double pointY = minY + Y / 2 * (integrationPoints[iInt][iY] + 1);
            //double pointZ = minZ + Z / 2 * (integrationPoints[iInt][iZ] + 1);

            //return new double[] { pointX, pointY, pointZ };
        }
    }
}
