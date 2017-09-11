using Gvitech.CityMaker.FdeGeometry;
using System.Runtime.InteropServices;

namespace Flood
{
    public delegate double[] TerrainAnalyseProcess(gviTerrainAnalyseOperation op,double[] ptArray);
    [ComVisible(true)]
    public class TerrainAnalyseCallBack
    {
        public TerrainAnalyseProcess onProcessing;
        public double[] OnProcessing(gviTerrainAnalyseOperation op, double[] ptArray) {
            if (onProcessing != null)
                return onProcessing(op, ptArray);
            else
                return null;
        }
    }
}
