using System;
using CityMakerBuilder.AddIn.Core;
using CityMakerBuilder.WorkSpace;
using Gvitech.CityMaker.RenderControl;

namespace CityMakerBuilder.AddIn.Example
{
    /// <summary>
    /// 隐藏地形
    /// </summary>
    class ShowTerrainCommand : AbstractCommand
    {
        public override void RestoreEnv()
        {
            RenderControlServices.Instance().AxRenderControl.Terrain.VisibleMask = gviViewportMask.gviViewAllNormalView;
            ProjectTreeServices.SetTreeNodeChecked(true);
            WorkSpaceServices.Instance().NeedSaveProject = true;
        }
        public override void Run(object sender, EventArgs e)
        {
            RenderControlServices.Instance().AxRenderControl.Terrain.VisibleMask = gviViewportMask.gviViewNone;
            ProjectTreeServices.SetTreeNodeChecked(false);
            WorkSpaceServices.Instance().NeedSaveProject = true;
        }
    }
}
