using CityMakerBuilder.AddIn.Core;
using CityMakerBuilder.WorkSpace;

namespace CityMakerBuilder.AddIn.Example
{
    // AddTerrainLayer
    public class AddTerrainConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            if (WorkSpaceServices.Instance().HasInitialRenderControl)
            {
                if (RenderControlServices.Instance().AxRenderControl.Terrain.IsRegistered)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }
    }

    // ShowTerrain
    public class ShowTerrainConditionEvaluator : IConditionEvaluator
    {
        public bool IsValid(object caller, Condition condition)
        {
            if (RenderControlServices.Instance().AxRenderControl.Terrain != null)
            {
                if (RenderControlServices.Instance().AxRenderControl.Terrain.IsRegistered)
                    return true;
            }
            return false;
        }
    }
}
