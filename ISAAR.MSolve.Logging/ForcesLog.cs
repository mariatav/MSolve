﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Logging.Interfaces;
using ISAAR.MSolve.Matrices.Interfaces;
using System.Diagnostics;
using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.Matrices;
using ISAAR.MSolve.PreProcessor.Interfaces;

namespace ISAAR.MSolve.Logging
{
    public class ForcesLog : IAnalyzerLog
    {
        private readonly IFiniteElement[] elements;
        private readonly Dictionary<int, double[]> forces = new Dictionary<int, double[]>();

        public ForcesLog(IFiniteElement[] elements)
        {
            this.elements = elements;
        }

        public Dictionary<int, double[]> Forces { get { return forces; } }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (int id in forces.Keys)
            {
                s.Append(String.Format("({0}): ", id));
                for (int i = 0; i < forces[id].Length; i++)
                    s.Append(String.Format("{0:0.00000}/", forces[id][i]));
                s.Append("; ");
            }
            return s.ToString();
        }

        #region IResultStorage Members

        public void StoreResults(DateTime startTime, DateTime endTime, IVector<double> solutionVector)
        {
            StartTime = startTime;
            EndTime = endTime;
            double[] solution = ((Vector<double>)solutionVector).Data;
            foreach (IFiniteElement e in elements)
            {
                var localVector = e.Subdomain.GetLocalVectorFromGlobal(e, solution);
                forces[e.ID] = e.CalculateForcesForLogging(e, localVector);

                //for (int i = 0; i < stresses[e.ID].Length; i++)
                //    Debug.Write(stresses[e.ID][i]);
                //Debug.WriteLine("");
            }
        }

        #endregion
    }
}
