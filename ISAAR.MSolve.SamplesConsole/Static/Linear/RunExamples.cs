using System;
using ISAAR.MSolve.SamplesConsole.Static.Linear;

namespace ISAAR.MSolve.SamplesConsole.Static.Linear
{
    class RunExamples
    {
        public static void Main(String[] args)
        {
            new Applied_Forces.Beam2DSimpleExample().check();
            //new Applied_Forces.Beam3DSimpleExample().check();
            //new Applied_Forces.Hexa8SimpleExample().Check();
            //new Applied_Forces.BuildingInNoSoilSmall().Check();
            //new Applied_Displacements.Beam2DSimpleExample().check();
        }

    }
}

