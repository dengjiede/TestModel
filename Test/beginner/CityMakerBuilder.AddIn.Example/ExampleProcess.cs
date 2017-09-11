using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityMakerBuilder.AddIn.Example
{
    public class ExampleProcess
    {
        private static ExampleProcess _exampleProcess;
        public static ExampleProcess Instance()
        {
            if (_exampleProcess == null)
            {
                _exampleProcess = new ExampleProcess();
            }
            return _exampleProcess;
        }

        private string tedFilePath = "";
        private string tedPwd = "";
        /*===========================================================
        @remark:   地形文件路径
        @details:	   
        ===========================================================*/
        public string TedPath
        {
            get { return tedFilePath; }
            set { tedFilePath = value; }
        }

        /*===========================================================
        @remark:   地形密码
        @details:	   
        ===========================================================*/
        public string TedPwd
        {
            get { return tedPwd; }
            set { tedPwd = value; }
        }
    }
}
