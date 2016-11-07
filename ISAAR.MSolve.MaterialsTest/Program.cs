using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISAAR.MSolve.MaterialsTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            #region Maria tests
            new AmbroseMaterialProperty(0, 0, 0, 0).BuildMaterialState(new double[] { 0, 0 }).ClearStresses();
            //IMaterialState state = new AmbroseMaterialProperty.AmbroseMaterialState();//produces error
            IMaterialState state2 = new AmbroseMaterialProperty(0, 0, 0, 0).BuildMaterialState(new double[] { 0, 0 });
            state2.UpdateMaterial(new double[] { 0, 0 });
            Console.WriteLine(state2.ConstitutiveMatrix[0]);
            #endregion
        }
    }
}
