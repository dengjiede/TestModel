using System;
using CityMakerBuilder.AddIn.Core;
using CityMakerBuilder.WorkSpace;

namespace CityMakerBuilder.AddIn.Example
{
    /// <summary>
    /// 关闭高程
    /// </summary>
    class ShowDEMCommand : AbstractCommand
    {
        public override void RestoreEnv()
        {
            RenderControlServices.Instance().AxRenderControl.Terrain.DemAvailable = true;
            WorkSpaceServices.Instance().NeedSaveProject = true;
        }
        public override void Run(object sender, EventArgs e)
        {
            RenderControlServices.Instance().AxRenderControl.Terrain.DemAvailable = false;
            WorkSpaceServices.Instance().NeedSaveProject = true;
        }
    }
}
