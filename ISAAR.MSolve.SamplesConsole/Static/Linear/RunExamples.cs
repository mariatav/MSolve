using System;
using ISAAR.MSolve.SamplesConsole.Examples.Static.Linear;

namespace ISAAR.MSolve.SamplesConsole.Static.Linear
{
    class RunExamples
    {
        public static void Main(String[] args)
        {
            new Beam2DSimpleExample().check();
            new Beam3DSimpleExample().check();
            new Hexa8SimpleExample().Check();
            new BuildingInNoSoilSmall().Check();
        }
        
    }
}

