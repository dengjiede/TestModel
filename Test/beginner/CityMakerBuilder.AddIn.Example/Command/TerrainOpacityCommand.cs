using System;
using CityMakerBuilder.AddIn.Core;
using CityMakerBuilder.WorkSpace;
using DevExpress.XtraEditors;

namespace CityMakerBuilder.AddIn.Example
{
    class TerrainOpacityCommand : AbstractCommand
    {
        public override void RestoreEnv()
        {

        }
        public override void Run(object sender, EventArgs e)
        {
            TrackBarControl ac = sender as TrackBarControl;
            RenderControlServices.Instance().AxRenderControl.Terrain.Opacity = double.Parse(ac.EditValue.ToString()) / 100.0;
        }
    }
}
