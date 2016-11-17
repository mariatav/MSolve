using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISAAR.MSolve.Logging.Interfaces;
using ISAAR.MSolve.PreProcessor;
using ISAAR.MSolve.PreProcessor.Interfaces;

namespace ISAAR.MSolve.Logging
{
    public class LinearAnalyzerLogFactory
    {
        private readonly int[] dofs;
        private readonly IFiniteElement[] stressElements, forceElements;

        public LinearAnalyzerLogFactory(int[] dofs, IFiniteElement[] stressElements, IFiniteElement[] dofElements)
        {
            this.dofs = dofs;
            this.stressElements = stressElements;
            this.forceElements = dofElements;
        }

        public LinearAnalyzerLogFactory(int[] dofs) : this(dofs, new IFiniteElement[0], new IFiniteElement[0])
        {
        }

        public IAnalyzerLog[] CreateLogs()
        {
            var l = new List<IAnalyzerLog>();
            l.Add(new DOFSLog(dofs));
            if (stressElements.Length > 0)
                l.Add(new StressesLog(stressElements));
            if (forceElements.Length > 0)
                l.Add(new ForcesLog(forceElements));
            return l.ToArray();
        }
    }
}
