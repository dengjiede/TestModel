using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;

namespace UIImageButton
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private Hashtable fcMap = null;  //IFeatureClass, List<string> 存储dataset里featureclass及对应的空间列名
        private IEnvelope env;//加载数据时，初始化的矩形范围
        private System.Guid rootId = new System.Guid();
        private IEulerAngle ang = new EulerAngle();
        private IVector3 vec = new Vector3();

        private void init()
        {
            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

            rootId = this.axRenderControl1.ObjectManager.GetProjectTree().RootID;
            this.axRenderControl1.Camera.FlyTime = 1;

            // 设置天空盒
            flag = Application.StartupPath.LastIndexOf("Samples");
            if (flag > -1)
            {
                string tmpSkyboxPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\skybox");
                ISkyBox skybox = this.axRenderControl1.ObjectManager.GetSkyBox(0);
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageBack, tmpSkyboxPath + "\\13_BK.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageBottom, tmpSkyboxPath + "\\13_DN.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageFront, tmpSkyboxPath + "\\13_FR.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageLeft, tmpSkyboxPath + "\\13_LF.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageRight, tmpSkyboxPath + "\\13_RT.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageTop, tmpSkyboxPath + "\\13_UP.jpg");
            }
            else
            {
                MessageBox.Show("请不要随意更改SDK目录名");
                return;
            }

            #region 加载FDB场景
            try
            {
                IConnectionInfo ci = new ConnectionInfo();
                ci.ConnectionType = gviConnectionType.gviConnectionFireBird2x;
                string tmpFDBPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\SDKDEMO.FDB");
                ci.Database = tmpFDBPath;
                IDataSourceFactory dsFactory = new DataSourceFactory();
                IDataSource ds = dsFactory.OpenDataSource(ci);
                string[] setnames = (string[])ds.GetFeatureDatasetNames();
                if (setnames.Length == 0)
                    return;
                IFeatureDataSet dataset = ds.OpenFeatureDataset(setnames[0]);
                string[] fcnames = (string[])dataset.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable);
                if (fcnames.Length == 0)
                    return;
                fcMap = new Hashtable(fcnames.Length);
                foreach (string name in fcnames)
                {
                    IFeatureClass fc = dataset.OpenFeatureClass(name);
                    // 找到空间列字段
                    List<string> geoNames = new List<string>();
                    IFieldInfoCollection fieldinfos = fc.GetFields();
                    for (int i = 0; i < fieldinfos.Count; i++)
                    {
                        IFieldInfo fieldinfo = fieldinfos.Get(i);
                        if (null == fieldinfo)
                            continue;
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        if (null == geometryDef)
                            continue;
                        geoNames.Add(fieldinfo.Name);
                    }
                    fcMap.Add(fc, geoNames);
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return;
            }

            // CreateFeautureLayer
            bool hasfly = false;
            foreach (IFeatureClass fc in fcMap.Keys)
            {
                List<string> geoNames = (List<string>)fcMap[fc];
                foreach (string geoName in geoNames)
                {
                    if (!geoName.Equals("Geometry"))
                        continue;

                    IFeatureLayer featureLayer = this.axRenderControl1.ObjectManager.CreateFeatureLayer(
                    fc, geoName, null, null, rootId);

                    if (!hasfly)
                    {
                        IFieldInfoCollection fieldinfos = fc.GetFields();
                        IFieldInfo fieldinfo = fieldinfos.Get(fieldinfos.IndexOf(geoName));
                        IGeometryDef geometryDef = fieldinfo.GeometryDef;
                        env = geometryDef.Envelope;
                        if (env == null || (env.MaxX == 0.0 && env.MaxY == 0.0 && env.MaxZ == 0.0 &&
                            env.MinX == 0.0 && env.MinY == 0.0 && env.MinZ == 0.0))
                            continue;
                        IEulerAngle angle = new EulerAngle();
                        angle.Set(0, -20, 0);
                        this.axRenderControl1.Camera.LookAt(env.Center, 1000, angle);
                    }
                    hasfly = true;
                }
            }
            #endregion        
            
        }

        //private IUIButton txtbtn1 = null;
        //private IUIButton txtbtn2 = null;
        //private IUIButton txtbtn3 = null;
        private IUIImageButton txtbtn1 = null;
        private IUIImageButton txtbtn2 = null;
        private IUIImageButton txtbtn3 = null;

        public MainForm()
        {
            InitializeComponent();

            init();

            #region 加载上方按钮
            IUIRect rect = new UIRect();
            rect.Init(0, 10, 0, 10, 0, 42, 0, 42);
            IUIWindowManager manager = this.axRenderControl1.UIWindowManager;
            IUIImageButton button1 = manager.CreateImageButton();
            button1.SetArea(rect);
            button1.Name = "漫游";
            button1.IsVisible = true;
            button1.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\漫游\normal.png");
            button1.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\漫游\hover.png");
            button1.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\漫游\pushed.png");
            button1.SubscribeEvent(gviUIEventType.gviUIMouseClick);
            this.axRenderControl1.RcUIWindowEvent +=
                new Gvitech.CityMaker.Controls._IRenderControlEvents_RcUIWindowEventEventHandler(axRenderControl1_RcUIWindowEvent);

            rect.Init(0, 52, 0, 10, 0, 84, 0, 42);
            IUIImageButton button2 = manager.CreateImageButton();
            button2.SetArea(rect);
            button2.Name = "点选";
            button2.IsVisible = true;
            button2.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\点选\normal.png");
            button2.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\点选\hover.png");
            button2.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\点选\pushed.png");
            button2.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0, 94, 0, 10, 0, 126, 0, 42);
            IUIImageButton button3 = manager.CreateImageButton();
            button3.SetArea(rect);
            button3.Name = "天气";
            button3.IsVisible = true;
            button3.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\天气\normal.png");
            button3.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\天气\hover.png");
            button3.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\天气\pushed.png");
            button3.SubscribeEvent(gviUIEventType.gviUIMouseEntersArea);

            //rect.Init(0, 94, 0, 52, 0, 174, 0, 83);
            //txtbtn1 = manager.CreateButton();
            //txtbtn1.SetArea(rect);
            //txtbtn1.Name = "SunShine";
            //txtbtn1.Text = "晴天";
            //txtbtn1.IsVisible = false;            
            //txtbtn1.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            //rect.Init(0, 94, 0, 83, 0, 174, 0, 114);
            //txtbtn2 = manager.CreateButton();
            //txtbtn2.SetArea(rect);
            //txtbtn2.Name = "HeavyRain";
            //txtbtn2.Text = "大雨";
            //txtbtn2.IsVisible = false;
            //txtbtn2.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            //rect.Init(0, 94, 0, 114, 0, 174, 0, 145);
            //txtbtn3 = manager.CreateButton();
            //txtbtn3.SetArea(rect);
            //txtbtn3.Name = "HeavySnow";
            //txtbtn3.Text = "大雪";
            //txtbtn3.IsVisible = false;
            //txtbtn3.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0, 94, 0, 52, 0, 174, 0, 83);
            txtbtn1 = manager.CreateImageButton();
            txtbtn1.SetArea(rect);
            txtbtn1.Name = "SunShine";
            txtbtn1.Text = "晴天";
            txtbtn1.IsVisible = false;
            txtbtn1.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\晴天\normal.png");
            txtbtn1.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\晴天\pushed.png");
            txtbtn1.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\晴天\hover.png");
            txtbtn1.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0, 94, 0, 83, 0, 174, 0, 114);
            txtbtn2 = manager.CreateImageButton();
            txtbtn2.SetArea(rect);
            txtbtn2.Name = "HeavyRain";
            txtbtn2.Text = "大雨";
            txtbtn2.IsVisible = false;
            txtbtn2.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\大雨\normal.png");
            txtbtn2.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\大雨\pushed.png");
            txtbtn2.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\大雨\hover.png");
            txtbtn2.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0, 94, 0, 114, 0, 174, 0, 145);
            txtbtn3 = manager.CreateImageButton();
            txtbtn3.SetArea(rect);
            txtbtn3.Name = "HeavySnow";
            txtbtn3.Text = "大雪";
            txtbtn3.IsVisible = false;
            txtbtn3.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\大雪\normal.png");
            txtbtn3.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\大雪\pushed.png");
            txtbtn3.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\button\大雪\hover.png");
            txtbtn3.SubscribeEvent(gviUIEventType.gviUIMouseClick);
            #endregion

            #region 加载下方按钮
            rect.Init(0, 0, 0.8, 0, 0.2, 0, 1, 0);
            IUIImageButton button4 = manager.CreateImageButton();
            button4.SetArea(rect);
            button4.Name = "location1";
            button4.Text = "location1";
            button4.IsVisible = true;
            button4.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\普通\2c495ffc-4641-447b-a5a4-636e4f3e7976.png");
            button4.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\按下\2c495ffc-4641-447b-a5a4-636e4f3e7976.png");
            button4.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\选中\2c495ffc-4641-447b-a5a4-636e4f3e7976.png");
            button4.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0.2, 0, 0.8, 0, 0.4, 0, 1, 0);
            IUIImageButton button5 = manager.CreateImageButton();
            button5.SetArea(rect);
            button5.Name = "location2";
            button5.Text = "location2";
            button5.IsVisible = true;
            button5.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\普通\2e0ca5d1-73d2-4c28-9698-2b64c89cc806.png");
            button5.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\按下\2e0ca5d1-73d2-4c28-9698-2b64c89cc806.png");
            button5.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\选中\2e0ca5d1-73d2-4c28-9698-2b64c89cc806.png");
            button5.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0.4, 0, 0.8, 0, 0.6, 0, 1, 0);
            IUIImageButton button6 = manager.CreateImageButton();
            button6.SetArea(rect);
            button6.Name = "location3";
            button6.Text = "location3";
            button6.IsVisible = true;
            button6.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\普通\76ba8729-0131-40f4-9713-9a9374a76936.png");
            button6.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\按下\76ba8729-0131-40f4-9713-9a9374a76936.png");
            button6.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\选中\76ba8729-0131-40f4-9713-9a9374a76936.png");
            button6.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0.6, 0, 0.8, 0, 0.8, 0, 1, 0);
            IUIImageButton button7 = manager.CreateImageButton();
            button7.SetArea(rect);
            button7.Name = "location4";
            button7.Text = "location4";
            button7.IsVisible = true;
            button7.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\普通\84e489db-3f82-43fa-b068-c85f95f680f1.png");
            button7.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\按下\84e489db-3f82-43fa-b068-c85f95f680f1.png");
            button7.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\选中\84e489db-3f82-43fa-b068-c85f95f680f1.png");
            button7.SubscribeEvent(gviUIEventType.gviUIMouseClick);

            rect.Init(0.8, 0, 0.8, 0, 1, 0, 1, 0);
            IUIImageButton button9 = manager.CreateImageButton();
            button9.SetArea(rect);
            button9.Name = "location5";
            button9.Text = "location5";
            button9.IsVisible = true;
            button9.NormalImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\普通\ffd10c67-373d-45d7-b901-b493ffc2741b.png");
            button9.PushedImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\按下\ffd10c67-373d-45d7-b901-b493ffc2741b.png");
            button9.HoverImage = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\location\选中\ffd10c67-373d-45d7-b901-b493ffc2741b.png");
            button9.SubscribeEvent(gviUIEventType.gviUIMouseClick);
           
            #endregion

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "UIImageButton.html";
            }
        }

        void axRenderControl1_RcUIWindowEvent(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcUIWindowEventEvent e)
        {
            IUIMouseEventArgs args = e.eventArgs as IUIMouseEventArgs;
            if (args.UIWindow == null)
                return;

            if (e.eventType == gviUIEventType.gviUIMouseClick)
            {
                gviUIWindowType winType = args.UIWindow.Type;
                if (winType == gviUIWindowType.gviUIImageButton)
                {
                    switch (args.UIWindow.Name)
                    {
                        case "漫游":
                            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;                        
                            break;
                        case "点选":
                            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
                            break;                        
                        case "location1":
                            vec.Set(15415.510188040265, 35593.117437895737, 59.003287982044796);
                            ang.Set(47.7, -30.55, 0);
                            this.axRenderControl1.Camera.SetCamera(vec, ang, gviSetCameraFlags.gviSetCameraNoFlags);
                            break;
                        case "location2":
                            vec.Set(15243.39327614102, 35593.454101290568, 19.083291340718386);
                            ang.Set(-37.01, -15.27, 0);
                            this.axRenderControl1.Camera.SetCamera(vec, ang, gviSetCameraFlags.gviSetCameraNoFlags);
                            break;
                        case "location3":
                            vec.Set(15150.53692901546, 35785.206458874149, 21.492597977763278);
                            ang.Set(-136.66, -19.68, 0);
                            this.axRenderControl1.Camera.SetCamera(vec, ang, gviSetCameraFlags.gviSetCameraNoFlags);
                            break;
                        case "location4":
                            vec.Set(15562.369345366114, 36027.787538479148, 9.5395100144668721);
                            ang.Set(-98.11, -5.3, 0);
                            this.axRenderControl1.Camera.SetCamera(vec, ang, gviSetCameraFlags.gviSetCameraNoFlags);
                            break;
                        case "location5":
                            vec.Set(15290.261360847539, 35689.443985629681, 25.558723498791508);
                            ang.Set(-34.53, -34.98, 0);
                            this.axRenderControl1.Camera.SetCamera(vec, ang, gviSetCameraFlags.gviSetCameraNoFlags);
                            break;
                        case "SunShine":
                            this.axRenderControl1.ObjectManager.GetSkyBox(0).Weather = gviWeatherType.gviWeatherSunShine;
                            txtbtn1.IsVisible = false;
                            txtbtn2.IsVisible = false;
                            txtbtn3.IsVisible = false;
                            break;
                        case "HeavyRain":
                            this.axRenderControl1.ObjectManager.GetSkyBox(0).Weather = gviWeatherType.gviWeatherHeavyRain;
                            txtbtn1.IsVisible = false;
                            txtbtn2.IsVisible = false;
                            txtbtn3.IsVisible = false;
                            break;
                        case "HeavySnow":
                            this.axRenderControl1.ObjectManager.GetSkyBox(0).Weather = gviWeatherType.gviWeatherHeavySnow;
                            txtbtn1.IsVisible = false;
                            txtbtn2.IsVisible = false;
                            txtbtn3.IsVisible = false;
                            break;
                    }
                }
                //else if (winType == gviUIWindowType.gviUIButton)
                //{
                //    switch (args.UIWindow.Name)
                //    {
                //        case "SunShine":
                //            this.axRenderControl1.ObjectManager.GetSkyBox(0).Weather = gviWeatherType.gviWeatherSunShine;
                //            txtbtn1.IsVisible = false;
                //            txtbtn2.IsVisible = false;
                //            txtbtn3.IsVisible = false;
                //            break;
                //        case "HeavyRain":
                //            this.axRenderControl1.ObjectManager.GetSkyBox(0).Weather = gviWeatherType.gviWeatherHeavyRain;
                //            txtbtn1.IsVisible = false;
                //            txtbtn2.IsVisible = false;
                //            txtbtn3.IsVisible = false;
                //            break;
                //        case "HeavySnow":
                //            this.axRenderControl1.ObjectManager.GetSkyBox(0).Weather = gviWeatherType.gviWeatherHeavySnow;
                //            txtbtn1.IsVisible = false;
                //            txtbtn2.IsVisible = false;
                //            txtbtn3.IsVisible = false;
                //            break;
                //    }
                //}
            }
            else if (e.eventType == gviUIEventType.gviUIMouseEntersArea)
            {
                gviUIWindowType winType = args.UIWindow.Type;
                if (winType == gviUIWindowType.gviUIImageButton)
                {
                    switch (args.UIWindow.Name)
                    {
                        case "天气":
                            txtbtn1.IsVisible = true;
                            txtbtn2.IsVisible = true;
                            txtbtn3.IsVisible = true;
                            break;
                    }
                }
            }
            //else if (e.eventType == gviUIEventType.gviUIMouseLeavesArea)
            //{
            //    gviUIWindowType winType = args.UIWindow.Type;
            //    if (winType == gviUIWindowType.gviUIImageButton)
            //    {
            //        switch (args.UIWindow.Name)
            //        {
            //            case "天气":
            //                txtbtn1.IsVisible = false;
            //                txtbtn2.IsVisible = false;
            //                txtbtn3.IsVisible = false;
            //                break;
            //        }
            //    }
            //}
        }
    
    }
}
