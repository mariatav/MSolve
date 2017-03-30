using System;
using System.Collections.Generic;
using ISAAR.MSolve.Analyzers;
using ISAAR.MSolve.FEM.Entities;
using ISAAR.MSolve.Numerical.LinearAlgebra;
using ISAAR.MSolve.FEM.Problems.Structural.Constitutive;
using ISAAR.MSolve.FEM.Problems.Structural.Elements;
using ISAAR.MSolve.Solvers.Skyline;
using ISAAR.MSolve.FEM.Problems.Structural;
using ISAAR.MSolve.FEM.Logging;

namespace ISAAR.MSolve.SamplesConsole.Static.Linear.Applied_Forces
{
    /// <summary>
    /// Solve a single hexa element
    /// </summary>
    class Beam3DSimpleExample
    {
        /// <summary>
        /// Create nodes
        /// </summary>
        /// <returns></returns>
        private static IList<Node> CreateNodes()
        {
            IList<Node> nodes = new List<Node>();
            Node node1 = new Node { ID = 1, X = 0.0, Y = 0.0, Z = 0.0 };
            Node node2 = new Node { ID = 2, X = 1.0, Y = 2.0, Z = 3.0 };
            nodes.Add(node1);
            nodes.Add(node2);
            return nodes;
        }

        public void check()
        {
            VectorExtensions.AssignTotalAffinityCount();
            double youngModulus = 200.0e06;
            double poissonRatio = 0.3;
            double nodalLoad = 25.0;

            // Node creation
            IList<Node> nodes = CreateNodes();

            // Model creation
            Model model = new Model();

            // Add a single subdomain to the model
            model.SubdomainsDictionary.Add(1, new Subdomain() { ID = 1 });

            // Add nodes to the nodes dictonary of the model
            for (int i = 0; i < nodes.Count; ++i)
            {
                model.NodesDictionary.Add(i + 1, nodes[i]);
            }

            // Constrain bottom nodes of the model
            model.NodesDictionary[1].Constraints.Add(DOFType.X);
            model.NodesDictionary[1].Constraints.Add(DOFType.Y);
            model.NodesDictionary[1].Constraints.Add(DOFType.Z);
            model.NodesDictionary[1].Constraints.Add(DOFType.RotX);
            model.NodesDictionary[1].Constraints.Add(DOFType.RotY);
            model.NodesDictionary[1].Constraints.Add(DOFType.RotZ);


            //Create a new Beam2D element
            var beam = new Beam3D(youngModulus, poissonRatio)
            {
                SectionArea = 1,
                MomentOfInertiaY = .1,
                MomentOfInertiaZ = .1,
                MomentOfInertiaPolar = .1
            };

            //var beam = new EulerBeam3D(youngModulus, poissonRatio)
            //{
            //    SectionArea = 1,
            //    MomentOfInertiaY = .1,
            //    MomentOfInertiaZ = .1,
            //    MomentOfInertiaPolar = .1
            //};

            var element = new Element()
            {
                ID = 1,
                ElementType = beam
            };

            //// Add nodes to the created element
            element.AddNode(model.NodesDictionary[1]);
            element.AddNode(model.NodesDictionary[2]);

            //var a = beam.StiffnessMatrix(element);

            // Add Hexa element to the element and subdomains dictionary of the model
            model.ElementsDictionary.Add(element.ID, element);
            model.SubdomainsDictionary[1].ElementsDictionary.Add(element.ID, element);

            // Add nodal load values at the top nodes of the model
            model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.X });
            model.Loads.Add(new Load() { Amount = -2 * nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.Y });
            model.Loads.Add(new Load() { Amount = -3 * nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.Z });
            model.Loads.Add(new Load() { Amount = 1.5 * nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.RotX });
            model.Loads.Add(new Load() { Amount = 2.5 * nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.RotY });
            model.Loads.Add(new Load() { Amount = 3.5 * nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.RotZ });

            // Needed in order to make all the required data structures
            model.ConnectDataStructures();

            // Choose linear equation system solver
            var linearSystem = new SkylineLinearSystem(1, model.SubdomainsDictionary[1].Forces);
            SolverFBSubstitution solver = new SolverFBSubstitution(linearSystem);

            // Choose the provider of the problem -> here a structural problem
            ProblemStructural provider = new ProblemStructural(model);

            // Choose parent and child analyzers -> Parent: Static, Child: Linear
            LinearAnalyzer childAnalyzer = new LinearAnalyzer(solver, linearSystem);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, childAnalyzer, linearSystem);

            // Choose dof types X, Y, Z to log for node 5
            childAnalyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] {
                model.NodalDOFsDictionary[2][DOFType.X],
                model.NodalDOFsDictionary[2][DOFType.Y],
                model.NodalDOFsDictionary[2][DOFType.Z],
                model.NodalDOFsDictionary[2][DOFType.RotX],
                model.NodalDOFsDictionary[2][DOFType.RotY],
                model.NodalDOFsDictionary[2][DOFType.RotZ]});

            // Analyze the problem
            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();

            Dictionary<int, double> results = (childAnalyzer.Logs[1][0] as DOFSLog).DOFValues;

            double[] expected = new double[] { 7.0156076002012693E-07, -3.2739502134272092E-06, -2.3385358667336872E-07, 1.6102489825223587E-05, 2.986644378371346E-05, 4.3630397742203316E-05 };

            for (int i = 0; i < expected.Length; i++)
            {
                if (Math.Abs(expected[i] - results[i]) > 1e-14)
                {
                    throw new SystemException("Failed beam3D test, results don't coincide for dof no: " + i + ", expected displacement: " + expected[i] + ", calculated displacement: " + results[i]);
                }
                //Console.WriteLine(results[i]);

            }
            Console.WriteLine("ran beam3d test");
        }
    }
}