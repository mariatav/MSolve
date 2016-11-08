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

namespace ISAAR.MSolve.SamplesConsole.Examples.Static.Linear
{
    class Hexa8SimpleExample
    {
        private static IList<Node> CreateNodes()
        {
            IList<Node> nodes = new List<Node>();
            Node node1 = new Node { ID = 1, X = 0.0, Y = 0.0, Z = 0.0 };
            Node node2 = new Node { ID = 2, X = 1.0, Y = 0.0, Z = 0.0 };
            Node node3 = new Node { ID = 3, X = 1.0, Y = 1.0, Z = 0.0 };
            Node node4 = new Node { ID = 4, X = 0.0, Y = 1.0, Z = 0.0 };
            Node node5 = new Node { ID = 5, X = 0.0, Y = 0.0, Z = 1.0 };
            Node node6 = new Node { ID = 6, X = 1.0, Y = 0.0, Z = 1.0 };
            Node node7 = new Node { ID = 7, X = 1.0, Y = 1.0, Z = 1.0 };
            Node node8 = new Node { ID = 8, X = 0.0, Y = 1.0, Z = 1.0 };
            nodes.Add(node1);
            nodes.Add(node2);
            nodes.Add(node3);
            nodes.Add(node4);
            nodes.Add(node5);
            nodes.Add(node6);
            nodes.Add(node7);
            nodes.Add(node8);
            return nodes;
        }

        public void Check()
        {
            VectorExtensions.AssignTotalAffinityCount();
            double youngModulus = 200.0e06;
            double poissonRatio = 0.3;
            double nodalLoad = 25.0;

            // Create a new elastic 3D material
            ElasticMaterial3DState material = new ElasticMaterial3DState(youngModulus, poissonRatio, new double[] { 0, 0, 0 });

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

            // Constraint bottom nodes of the model
            model.NodesDictionary[1].Constraints.Add(DOFType.X);
            model.NodesDictionary[1].Constraints.Add(DOFType.Y);
            model.NodesDictionary[1].Constraints.Add(DOFType.Z);
            model.NodesDictionary[2].Constraints.Add(DOFType.X);
            model.NodesDictionary[2].Constraints.Add(DOFType.Y);
            model.NodesDictionary[2].Constraints.Add(DOFType.Z);
            model.NodesDictionary[3].Constraints.Add(DOFType.X);
            model.NodesDictionary[3].Constraints.Add(DOFType.Y);
            model.NodesDictionary[3].Constraints.Add(DOFType.Z);
            model.NodesDictionary[4].Constraints.Add(DOFType.X);
            model.NodesDictionary[4].Constraints.Add(DOFType.Y);
            model.NodesDictionary[4].Constraints.Add(DOFType.Z);


            // Create a new Hexa8 element
            var element = new Element()
            {
                ID = 1,
                ElementType = new Hexa8(material)
            };

            // Add nodes to the created element
            element.AddNode(model.NodesDictionary[1]);
            element.AddNode(model.NodesDictionary[2]);
            element.AddNode(model.NodesDictionary[3]);
            element.AddNode(model.NodesDictionary[4]);
            element.AddNode(model.NodesDictionary[5]);
            element.AddNode(model.NodesDictionary[6]);
            element.AddNode(model.NodesDictionary[7]);
            element.AddNode(model.NodesDictionary[8]);

            // Add Hexa element to the element and subdomains dictionary of the model
            model.ElementsDictionary.Add(element.ID, element);
            model.SubdomainsDictionary[1].ElementsDictionary.Add(element.ID, element);

            // Add nodal load values at the top nodes of the model
            model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[5], DOF = DOFType.Z });
            model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[6], DOF = DOFType.Z });
            model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[7], DOF = DOFType.Z });
            model.Loads.Add(new Load() { Amount = -nodalLoad, Node = model.NodesDictionary[8], DOF = DOFType.Z });

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
                model.NodalDOFsDictionary[5][DOFType.X],
                model.NodalDOFsDictionary[5][DOFType.Y],
                model.NodalDOFsDictionary[5][DOFType.Z],
                model.NodalDOFsDictionary[6][DOFType.X],
                model.NodalDOFsDictionary[6][DOFType.Y],
                model.NodalDOFsDictionary[6][DOFType.Z],
                model.NodalDOFsDictionary[7][DOFType.X],
                model.NodalDOFsDictionary[7][DOFType.Y],
                model.NodalDOFsDictionary[7][DOFType.Z],
                model.NodalDOFsDictionary[8][DOFType.X],
                model.NodalDOFsDictionary[8][DOFType.Y],
                model.NodalDOFsDictionary[8][DOFType.Z]});

            // Analyze the problem
            parentAnalyzer.BuildMatrices();
            parentAnalyzer.Initialize();
            parentAnalyzer.Solve();


            Dictionary<int, double> results = (childAnalyzer.Logs[1][0] as DOFSLog).DOFValues;
            double[] expected = new double[] { -9.7500000000003E-08, -9.75000000000031E-08, -4.55000000000003E-07, 9.75000000000031E-08, -9.7500000000003E-08, -4.55000000000002E-07, 9.7500000000003E-08, 9.75000000000032E-08, -4.55000000000003E-07, -9.75000000000031E-08, 9.75000000000029E-08, -4.55000000000002E-07 };

            for (int i = 0; i < expected.Length; i++)
            {
                if (Math.Abs(expected[i] - results[i]) > 1e-14)
                {
                    throw new SystemException("Failed hexa8 test, results don't coincide for dof no: " + i + ", expected displacement: " + expected[i] + ", calculated displacement: " + results[i]);
                }
                //Console.Write(results[i] + ",");

            }
            Console.WriteLine("ran hexa8 test");

            // Write results to console
            //Console.WriteLine("Writing results for node 5");
            //Console.WriteLine("Dof and Values for Displacement X, Y, Z");
            //Console.WriteLine(childAnalyzer.Logs[1][0]);


        }
    }
}