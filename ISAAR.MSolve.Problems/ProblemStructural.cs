﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Matrices.Interfaces;
using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.Solvers.Interfaces;
using ISAAR.MSolve.Matrices;
using ISAAR.MSolve.Analyzers.Interfaces;
using System.Threading.Tasks;
using ISAAR.MSolve.PreProcessor.Providers;
using ISAAR.MSolve.Analyzers;

namespace ISAAR.MSolve.Problems
{
    public class ProblemStructural : IImplicitIntegrationProvider, IStaticProvider, INonLinearProvider
    {
        private Dictionary<int, IMatrix2D<double>> ms, cs, ks;
        private readonly Model model;
        private IDictionary<int, ISolverSubdomain> subdomains;
        private ElementStructuralStiffnessProvider stiffnessProvider = new ElementStructuralStiffnessProvider();
        private ElementStructuralMassProvider massProvider = new ElementStructuralMassProvider();

        public ProblemStructural(Model model, IDictionary<int, ISolverSubdomain> subdomains)
        {
            this.model = model;
            this.subdomains = subdomains;
        }

        public double AboserberE { get; set; }
        public double Aboseberv { get; set; }

        private IDictionary<int, IMatrix2D<double>> Ms
        {
            get
            {
                if (ms == null) BuildMs();
                return ms;
            }
        }

        private IDictionary<int, IMatrix2D<double>> Cs
        {
            get
            {
                if (cs == null) BuildCs();
                return cs;
            }
        }

        private IDictionary<int, IMatrix2D<double>> Ks
        {
            get
            {
                if (ks == null)
                    BuildKs();
                else
                    RebuildKs();
                return ks;
            }
        }

        private void BuildKs()
        {
            ks = new Dictionary<int, IMatrix2D<double>>(model.SubdomainsDictionary.Count);
            //ks.Add(1, new SkylineMatrix2D<double>(new double[,] { { 6, -2 }, { -2, 4 } }));
            ElementStructuralStiffnessProvider s = new ElementStructuralStiffnessProvider();
            //foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
            //    ks.Add(subdomain.ID, GlobalMatrixAssemblerSkyline.CalculateGlobalMatrix(subdomain, s));

            //var kks = new Dictionary<int, IMatrix2D<double>>(model.SubdomainsDictionary.Count);
            int procs = VectorExtensions.AffinityCount;
            var k = model.SubdomainsDictionary.Keys.Select(x => x).ToArray<int>();
            var internalKs = new Dictionary<int, IMatrix2D<double>>[procs];
            Parallel.ForEach(k.PartitionLimits(procs), limit =>
            {
                if (limit.Item3 - limit.Item2 > 0)
                {
                    internalKs[limit.Item1] = new Dictionary<int, IMatrix2D<double>>(limit.Item3 - limit.Item2);
                    for (int i = limit.Item2; i < limit.Item3; i++)
                        internalKs[limit.Item1].Add(k[i], GlobalMatrixAssemblerSkyline.CalculateGlobalMatrix(model.SubdomainsDictionary[k[i]], s));
                }
                else
                    internalKs[limit.Item1] = new Dictionary<int, IMatrix2D<double>>();
            });
            for (int i = 0; i < procs; i++)
                foreach (int key in internalKs[i].Keys)
                    ks.Add(key, internalKs[i][key]);
        }

        private void RebuildKs()
        {
            foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
            //Parallel.ForEach(model.SubdomainsDictionary.Values, subdomain =>
            {
                if (subdomain.MaterialsModified)
                {
                    ks[subdomain.ID] = GlobalMatrixAssemblerSkyline.CalculateGlobalMatrix(subdomain, stiffnessProvider);
                    subdomain.ResetMaterialsModifiedProperty();
                }
            }//);
        }

        private void BuildMs()
        {
            ms = new Dictionary<int, IMatrix2D<double>>(model.SubdomainsDictionary.Count);
            //ms.Add(1, new SkylineMatrix2D<double>(new double[,] { { 2, 0 }, { 0, 1 } }));
            ElementStructuralMassProvider s = new ElementStructuralMassProvider();
            foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
                ms.Add(subdomain.ID, GlobalMatrixAssemblerSkyline.CalculateGlobalMatrix(subdomain, s));
        }

        private void BuildCs()
        {
            cs = new Dictionary<int, IMatrix2D<double>>(model.SubdomainsDictionary.Count);
            //foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
            //    cs.Add(subdomain.ID, SkylineMatrix2D<double>.Empty(subdomain.TotalDOFs));
            ElementStructuralDampingProvider s = new ElementStructuralDampingProvider();
            foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
                cs.Add(subdomain.ID, GlobalMatrixAssemblerSkyline.CalculateGlobalMatrix(subdomain, s));
        }

        #region IAnalyzerProvider Members
        public void Reset()
        {
            foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
                foreach (var element in subdomain.ElementsDictionary.Values)
                    element.ClearMaterialState();
                
            cs = null;
            ks = null;
            ms = null;
        }
        #endregion 

        #region IImplicitIntegrationProvider Members

        public void CalculateEffectiveMatrix(ISolverSubdomain subdomain, ImplicitIntegrationCoefficients coefficients)
        {
            subdomain.Matrix = this.Ks[subdomain.ID];

            //// REMOVE
            //subdomain.CloneMatrix();

            if (((SkylineMatrix2D<double>)subdomain.Matrix).IsFactorized)
                BuildKs();

            subdomain.Matrix.LinearCombination(
                new double[] 
                {
                    coefficients.Stiffness, coefficients.Mass, coefficients.Damping
                }, 
                new IMatrix2D<double>[] 
                { 
                    this.Ks[subdomain.ID], this.Ms[subdomain.ID], this.Cs[subdomain.ID] 
                });
        }

        public void ProcessRHS(ISolverSubdomain subdomain, ImplicitIntegrationCoefficients coefficients)
        {
        }

        public void GetRHSFromHistoryLoad(int timeStep)
        {
            foreach (Subdomain subdomain in model.SubdomainsDictionary.Values)
                for (int i = 0; i < subdomain.Forces.Length; i++)
                    subdomain.Forces[i] = 0;

            model.AssignLoads();
            model.AssignMassAccelerationHistoryLoads(timeStep);

            ////AMBROSIOS
            //if (model.MassAccelerationHistoryLoads.Count > 0)
            //{
            //    int[] sDOFs = new int[] { 0, 1, 2, 3, 4, 5, 36, 37, 38, 39, 40, 41 };
            //    int[] fDOFs = new int[] { model.NodalDOFsDictionary[3][DOFType.X], model.NodalDOFsDictionary[3][DOFType.Y], model.NodalDOFsDictionary[3][DOFType.Z], 
            //        model.NodalDOFsDictionary[4][DOFType.X], model.NodalDOFsDictionary[4][DOFType.Y], model.NodalDOFsDictionary[4][DOFType.Z], 
            //        model.NodalDOFsDictionary[15][DOFType.X], model.NodalDOFsDictionary[15][DOFType.Y], model.NodalDOFsDictionary[16][DOFType.Z], 
            //        model.NodalDOFsDictionary[16][DOFType.X], model.NodalDOFsDictionary[15][DOFType.Y], model.NodalDOFsDictionary[16][DOFType.Z]};
            //    int[] fDOFs = new int[] { 6, 7, 8, 9, 10, 11, 42, 43, 44, 45, 46, 47 };
            //    var msfdata = new double[sDOFs.Length, fDOFs.Length];
            //    var lines = File.ReadAllLines(@"C:\Development\vs 2010\AnalSharp\msfAmbrosios.csv");
            //    for (int i = 0; i < sDOFs.Length; i++)
            //        for (int j = 0; j < fDOFs.Length; j++)
            //            msfdata[i, j] = Double.Parse(lines[i * fDOFs.Length + j]); 
            //            msfdata[i, j] = ms[1][sDOFs[i], fDOFs[j]];
            //    var msf = new Matrix2D<double>(msfdata);
            //    var acc = new double[sDOFs.Length];

            //    List<MassAccelerationLoad> m = new List<MassAccelerationLoad>(model.MassAccelerationHistoryLoads.Count);
            //    foreach (IMassAccelerationHistoryLoad l in model.MassAccelerationHistoryLoads)
            //        for (int i = 0; i < sDOFs.Length / 3; i++)
            //            acc[3*i] = l[timeStep];

            //    var localForces = new double[sDOFs.Length];
            //    msf.Multiply(new Vector<double>(acc), localForces);

            //    for (int j = 0; j < fDOFs.Length; j++)
            //        model.SubdomainsDictionary[1].Forces[fDOFs[j]] -= localForces[j];
            //}
        }

        public void MassMatrixVectorProduct(ISolverSubdomain subdomain, IVector<double> vIn, double[] vOut)
        {
            this.Ms[subdomain.ID].Multiply(vIn, vOut);
        }

        public void DampingMatrixVectorProduct(ISolverSubdomain subdomain, IVector<double> vIn, double[] vOut)
        {
            this.Cs[subdomain.ID].Multiply(vIn, vOut);
        }

        #endregion

        #region IStaticProvider Members

        public void CalculateMatrix(ISolverSubdomain subdomain)
        {
            if (ks == null) BuildKs();
            subdomain.Matrix = this.ks[subdomain.ID];
        }

        #endregion

        #region INonLinearProvider Members

        public double RHSNorm(double[] rhs)
        {
            return (new Vector<double>(rhs)).Norm;
        }

        public void ProcessInternalRHS(ISolverSubdomain subdomain, double[] rhs, double[] solution)
        {
        }

        #endregion
    }
}
