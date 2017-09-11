using System;
using CityMakerBuilder.AddIn.Core;
using CityMakerBuilder.WorkSpace;

namespace CityMakerBuilder.AddIn.Example
{
    class AddTerrainCommand : AbstractCommand
    {
        public override void RestoreEnv()
        {
                
        }
        public override void Run(object sender, EventArgs e)
        {
            CommandManager.Push(this);
            TerrainSettingForm form = new TerrainSettingForm();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProjectTreeServices.CreateTerrainLayer(true, form.TedName, String.Format("{0}|{1}", form.TedPath, form.TedPwd));
            }
        }
    }
}
