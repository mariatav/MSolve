using System;
using System.Collections.Generic;
using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.Solvers.Skyline;
using ISAAR.MSolve.Problems;
using ISAAR.MSolve.Analyzers;
using ISAAR.MSolve.Logging;
using ISAAR.MSolve.PreProcessor.Materials;
using ISAAR.MSolve.PreProcessor.Elements;
using ISAAR.MSolve.Matrices;
using ISAAR.MSolve.Logging.Interfaces;

namespace ISAAR.MSolve.SamplesConsole.Static.Linear
{
    /// <summary>
    /// Solve a single hexa element
    /// </summary>
    class Beam2DSimpleExample
    {
        /// <summary>
        /// Create nodes
        /// </summary>
        /// <returns></returns>
        private static IList<Node> CreateNodes()
        {
            IList<Node> nodes = new List<Node>();
            Node node1 = new Node { ID = 1, X = 0.0, Y = 0.0, Z = 0.0 };
            Node node2 = new Node { ID = 2, X = 1.0, Y = 0.0, Z = 0.0 };
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

            // Create a new elastic 3D material
            ElasticMaterial material = new ElasticMaterial()
            {
                YoungModulus = youngModulus,
                PoissonRatio = poissonRatio,
            };

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
            model.NodesDictionary[1].Constraints.Add(DOFType.RotZ);


            //Create a new Beam2D element
            var beam = new Beam2D(material)
            {
                SectionArea = 1,
                MomentOfInertia = .1
            };


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
            model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.Y });
            //model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[2], DOF = DOFType.Z });

            // Needed in order to make all the required data structures
            model.ConnectDataStructures();

            // Choose linear equation system solver
            SolverSkyline solver = new SolverSkyline(model);

            // Choose the provider of the problem -> here a structural problem
            ProblemStructural provider = new ProblemStructural(model, solver.SubdomainsDictionary);

            // Choose parent and child analyzers -> Parent: Static, Child: Linear
            Analyzers.LinearAnalyzer childAnalyzer = new LinearAnalyzer(solver, solver.SubdomainsDictionary);
            StaticAnalyzer parentAnalyzer = new StaticAnalyzer(provider, childAnalyzer, solver.SubdomainsDictionary);

            // Choose dof types X, Y, Z to log for node 5
            childAnalyzer.LogFactories[1] = new LinearAnalyzerLogFactory(new int[] {
                model.NodalDOFsDictionary[2][DOFType.X],
                model.NodalDOFsDictionary[2][DOFType.Y],
                model.NodalDOFsDictionary[2][DOFType.RotZ]});

            // Analyze the problem
            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();

            Dictionary<int, double> results = (childAnalyzer.Logs[1][0] as DOFSLog).DOFValues;
            double[] expected = new double[] { -1.25E-07, -4.16666666666667E-07, -6.25E-07 };

            for (int i = 0; i < expected.Length; i++)
            {
                if (Math.Abs(expected[i] - results[i]) > 1e-14)
                {
                    throw new SystemException("Failed beam2D test, results don't coincide for dof no: " + i + ", expected displacement: " + expected[i] + ", calculated displacement: " + results[i]);
                }
                //Console.WriteLine(results[i]);

            }
            Console.WriteLine("ran beam2d2 test");
        }
    }
}