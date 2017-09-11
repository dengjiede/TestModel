using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CityMakerBuilder.AddIn.Core;
using ExampleToolkit.Dialog;

namespace ExampleToolkit.Command
{
    public class AddImageAsResourceCommand : AbstractCommand
    {
        /// <summary>
        /// CommandName
        /// </summary>
        public override string CommandName
        {
            get
            {
                return "往资源库里加贴图";
            }
        }
        /// <summary>
        /// 重置操作
        /// </summary>
        public override void RestoreEnv()
        {

        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Run(object sender, EventArgs e)
        {
            if (CityMakerBuilder.AddIn.WinForm.MainFrmService.OpenForms.Count > 1)
                return;
            CommandManager.Push(this);
            AddImageAsResourceDlg myDlg = new AddImageAsResourceDlg();
            myDlg.Show(CityMakerBuilder.AddIn.WinForm.MainFrmService.MainFrm);
            myDlg.SetToolDescribe();
        }
    }
}
