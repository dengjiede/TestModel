using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CityMakerBuilder.AddIn.Core;
using ExampleToolkit.Dialog;

namespace ExampleToolkit.Command
{
    public class CutModelPointCommand : AbstractCommand
    {
        /// <summary>
        /// CommandName
        /// </summary>
        public override string CommandName
        {
            get
            {
                return "切割模型";
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
            CutModelPointDlg myDlg = new CutModelPointDlg();
            myDlg.Show(CityMakerBuilder.AddIn.WinForm.MainFrmService.MainFrm);
            myDlg.SetToolDescribe();
        }
    }
}
