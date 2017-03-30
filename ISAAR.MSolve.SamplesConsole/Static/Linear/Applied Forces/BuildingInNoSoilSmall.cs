using ISAAR.MSolve.Analyzers;
using ISAAR.MSolve.FEM.Entities;
using ISAAR.MSolve.FEM.Logging;
using ISAAR.MSolve.FEM.Problems.Structural;
using ISAAR.MSolve.Numerical.LinearAlgebra;
using ISAAR.MSolve.Solvers.Skyline;
using System;
using System.Collections.Generic;

namespace ISAAR.MSolve.SamplesConsole.Static.Linear.Applied_Forces
{
    class BuildingInNoSoilSmall
    {
        public void Check()
        {
            VectorExtensions.AssignTotalAffinityCount();
            Model model = new Model();
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });
            BeamBuildingBuilder.MakeBeamBuilding(model, 20, 20, 20, 5, 4, model.NodesDictionary.Count + 1,
                model.ElementsDictionary.Count + 1, 1, 4, false, false);
            model.Loads.Add(new Load() { Amount = -100, Node = model.Nodes[21], DOF = DOFType.X });
            model.ConnectDataStructures();

            var linearSystem = new SkylineLinearSystem(1, model.SubdomainsDictionary[1].Forces);
            SolverFBSubstitution solver = new SolverFBSubstitution(linearSystem);

            ProblemStructural provider = new ProblemStructural(model);
            LinearAnalyzer analyzer = new LinearAnalyzer(solver, linearSystem);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, analyzer, linearSystem);

            analyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] { 420 });

            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();
            Dictionary<int, double> results = (analyzer.Logs[1][0] as DOFSLog).DOFValues;
            double expected = -0.189287317648762;
            if (Math.Abs(expected - results[420]) > 1e-15)
            {
                throw new SystemException("Failed BuildingInNoSoilSmall test, expected displacement: " + expected + ", calculated displacement: " + results[0]);
            }
            Console.WriteLine("ran BuildingInNoSoilSmall test");
        }
    }
}
