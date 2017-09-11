using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
using System;
using System.Data;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.FdeCore;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.FdeGeometry;


namespace TerrainVideo
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号

        private DataTable dt = null;
        private string locationName = "";
        private CameraProperty cp = null;
        private IVector3 vector = new Vector3();
        private IEulerAngle angle = new EulerAngle();
        private IPoint positionPoint = null;

        private ITerrainVideoConfig videoConfig = null;

        private List<ITerrainVideo> videoList = new List<ITerrainVideo>();
        private int curVideoIndex = -1;
        private ITerrainVideo curVideo = null;
        private int curRowIndex = -1;
        private StreamWriter streamWriter = null;
        private double factor = 10.0;

        private System.Guid rootId = new System.Guid();

        private VideoObject tmpTV = null;
        private PropertyFrm pf = null;

        public MainForm()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);
            this.axRenderControl1.Camera.FlyTime = 1;

            CommonUnity.RenderHelper = this.axRenderControl1;

            rootId = this.axRenderControl1.ObjectManager.GetProjectTree().RootID;

            // 设置天空盒
            flag = Application.StartupPath.LastIndexOf("Samples");
            if (flag > -1)
            {
                string tmpSkyboxPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\skybox");
                ISkyBox skybox = this.axRenderControl1.ObjectManager.GetSkyBox(0);
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageBack, tmpSkyboxPath + "\\1_BK.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageBottom, tmpSkyboxPath + "\\1_DN.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageFront, tmpSkyboxPath + "\\1_FR.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageLeft, tmpSkyboxPath + "\\1_LF.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageRight, tmpSkyboxPath + "\\1_RT.jpg");
                skybox.SetImagePath(gviSkyboxImageIndex.gviSkyboxImageTop, tmpSkyboxPath + "\\1_UP.jpg");
            }
            else
            {
                MessageBox.Show("请不要随意更改SDK目录名");
                return;
            }

            this.axRenderControl1.Camera.VerticalFieldOfView = 60;

            // 加载瓦片图层
            string tilePath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\sdk.tdb");
            I3DTileLayer layer = this.axRenderControl1.ObjectManager.Create3DTileLayer(tilePath, "", rootId);
            this.axRenderControl1.Camera.FlyToObject(layer.Guid, gviActionCode.gviActionFlyTo);

            IGeometryFactory fac = new GeometryFactory();
            positionPoint = fac.CreatePoint(gviVertexAttribute.gviVertexAttributeZ);

            videoConfig = this.axRenderControl1.TerrainVideoConfig;

            ConstructTable();
            //LoadCameraFromFile();

            // 绑定键盘事件
            this.axRenderControl1.RcKeyUp += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcKeyUpEventHandler(axRenderControl1_RcKeyUp);
            string filePath = @"C:\Users\yuanying\Desktop\adjust.txt";
            if (File.Exists(filePath))
            {
                streamWriter = new StreamWriter(filePath, true);                
            }   
     
            // 拾取摄像头显示投射线
            //this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectRenderGeometry;
            //this.axRenderControl1.RcMouseHover += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseHoverEventHandler(axRenderControl1_RcMouseHover);

            // 双击摄像头飞入
            this.axRenderControl1.RcLButtonDblClk += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcLButtonDblClkEventHandler(axRenderControl1_RcLButtonDblClk);

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "TerrainVideo.html";
            }
        }

        bool axRenderControl1_RcMouseHover(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseHoverEvent e)
        {
            IPickResult pic = this.axRenderControl1.Camera.ScreenToWorld(e.x, e.y, out positionPoint);
            if (positionPoint == null || pic == null)
            {
                for (int i = 0; i < videoList.Count; i++)
                {
                    ITerrainVideo v = videoList[i];
                    v.ShowProjectionLines = false;
                    v.ShowProjector = false;
                }
                videoConfig.SetPriorityDisplay(null);
                return false;
            }

            IRenderPointPickResult pr = pic as IRenderPointPickResult;
            if (pr != null)
            {
                curVideoIndex = int.Parse(pr.Point.Name);
                if (curVideoIndex == -1)
                    return false;
                ITerrainVideo video = videoList[curVideoIndex];                
                videoConfig.SetPriorityDisplay(video);
                video.ShowProjectionLines = true;
                video.ShowProjector = true;                
            }
            else
            {
                for (int i = 0; i < videoList.Count; i++)
                {
                    ITerrainVideo v = videoList[i];
                    v.ShowProjectionLines = false;
                    v.ShowProjector = false;
                }
                videoConfig.SetPriorityDisplay(null);
            }

            return false;
        }

        bool axRenderControl1_RcLButtonDblClk(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcLButtonDblClkEvent e)
        {
            if (curVideoIndex != -1)
            {
                ITerrainVideo video = videoList[curVideoIndex];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cp = dt.Rows[i]["Location"] as CameraProperty;
                    if (cp.index == curVideoIndex)
                    {
                        vector.Set(cp.X, cp.Y, cp.Z);
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                    }
                }
                    
            }

            return true;
        }

        bool axRenderControl1_RcKeyUp(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcKeyUpEvent e)
        {
            if (curVideoIndex < 0)
                return false;

            switch (e.@char)
            {
            #region 按键调整
                case (uint)Keys.Q:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.X += factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        positionPoint.Position = vector;
                        curVideo.Position = positionPoint;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);

                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.W:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.X -= factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        positionPoint.Position = vector;
                        curVideo.Position = positionPoint;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);

                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.E:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Y += factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        positionPoint.Position = vector;
                        curVideo.Position = positionPoint;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.R:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Y -= factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        positionPoint.Position = vector;
                        curVideo.Position = positionPoint;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.T:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Z += factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        positionPoint.Position = vector;
                        curVideo.Position = positionPoint;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.Y:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Z -= factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        positionPoint.Position = vector;
                        curVideo.Position = positionPoint;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.U:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Heading += factor;
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        curVideo.Angle = angle;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.I:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Heading -= factor;
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        curVideo.Angle = angle;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.O:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Tilt += factor;
                        vector.Set(cp.X, cp.Y, cp.Z);
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        curVideo.Angle = angle;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.P:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Tilt -= factor;
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        curVideo.Angle = angle;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.D:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Roll += factor;
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        curVideo.Angle = angle;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.F:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.Roll -= factor;
                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
                        curVideo.Angle = angle;
                        //this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.G:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.AspectRatio += factor;
                        curVideo.AspectRatio = cp.AspectRatio;
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.H:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.AspectRatio -= factor;
                        curVideo.AspectRatio = cp.AspectRatio;
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.J:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.FieldOfView += factor;
                        curVideo.FieldOfView = cp.FieldOfView;
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.K:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.FieldOfView -= factor;
                        curVideo.FieldOfView = cp.FieldOfView;
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.Z:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.VideoOpacity += factor;
                        curVideo.VideoOpacity = cp.VideoOpacity;
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.X:
                    {
                        curVideo = videoList[curVideoIndex];
                        cp.VideoOpacity -= factor;
                        curVideo.VideoOpacity = cp.VideoOpacity;
                        DataRow dr = dt.Rows[curRowIndex];
                        dr["Location"] = cp;
                    }
                    break;
                case (uint)Keys.D1:
                    {
                        factor = 10;
                    }
                    break;
                case (uint)Keys.D2:
                    {
                        factor = 1;
                    }
                    break;
                case (uint)Keys.D3:
                    {
                        factor = 0.1;
                    }
                    break;
                case (uint)Keys.D4:
                    {
                        factor = 0.01;
                    }
                    break;
            #endregion
                case (uint)Keys.S:
                    {
                        String str = cp.PropertyStrings();
                        streamWriter.WriteLine(str);
                        streamWriter.Flush();
                    }
                    break;
                case (uint)Keys.C:
                    {
                        curVideo = videoList[curVideoIndex];
                        curVideo.VisibleMask = gviViewportMask.gviViewAllNormalView;
                    }
                    break;
                case (uint)Keys.V:
                    {
                        curVideo = videoList[curVideoIndex];
                        curVideo.VisibleMask = gviViewportMask.gviViewNone;
                    }
                    break;
            }

            return true;
        }

        // 构造picture控件
        private void ConstructTable()
        {
            dt = new DataTable();
            DataColumn col_Name = new DataColumn("Name", typeof(System.String));
            DataColumn col_Location = new DataColumn("Location", typeof(System.Object));
            DataColumn col_Object = new DataColumn("Object", typeof(System.Object));
            dt.Columns.AddRange(new DataColumn[] { col_Name, col_Location, col_Object });
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns[1].Visible = false;
            this.dataGridView1.Columns[2].Visible = false;
        }

        //private void LoadCameraFromFile()
        //{
        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.Load(Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\CSharp\bin\TerrainVideo.xml"));
        //    if (xmlDoc.SelectSingleNode("CameraGroup") != null)
        //    {
        //        XmlNodeList xnl = xmlDoc.SelectSingleNode("CameraGroup").ChildNodes;
        //        if (xnl.Count > 0)
        //        {
        //            foreach (XmlNode gNode in xnl)  // gNode = "Camera"
        //            {
        //                if (gNode.Name == "Camera")
        //                {
        //                    locationName = gNode.Attributes["Name"].Value;
        //                    cp = new CameraProperty();
        //                    cp.name = locationName;
        //                    cp.X = double.Parse(gNode.Attributes["x"].Value);
        //                    cp.Y = double.Parse(gNode.Attributes["y"].Value);
        //                    cp.Z = double.Parse(gNode.Attributes["z"].Value);
        //                    cp.Heading = double.Parse(gNode.Attributes["heading"].Value);
        //                    cp.Roll = double.Parse(gNode.Attributes["roll"].Value);
        //                    cp.Tilt = double.Parse(gNode.Attributes["tilt"].Value);
        //                    cp.AspectRatio = double.Parse(gNode.Attributes["AspectRatio"].Value);
        //                    cp.FieldOfView = double.Parse(gNode.Attributes["FieldOfView"].Value);
        //                    cp.VideoOpacity = double.Parse(gNode.Attributes["VideoOpacity"].Value);  

        //                    vector.Set(cp.X, cp.Y, cp.Z);
        //                    positionPoint.Position = vector;

        //                    string videoFilePath = videoString + locationName + ".avi";
        //                    if (File.Exists(videoFilePath))
        //                    {
        //                        ITerrainVideo video = this.axRenderControl1.ObjectManager.CreateTerrainVideo(positionPoint, rootId);
        //                        video.VideoFileName = videoFilePath;
        //                        angle.Set(cp.Heading, cp.Tilt, cp.Roll);
        //                        video.Angle = angle;                                
        //                        video.AspectRatio = cp.AspectRatio;
        //                        video.FieldOfView = cp.FieldOfView;
        //                        video.VideoOpacity = cp.VideoOpacity;
        //                        video.PlayVideoOnStartup = true;
        //                        videoList.Add(video);
        //                        cp.index = videoList.Count - 1;

        //                        IImagePointSymbol sym = new ImagePointSymbol();
        //                        sym.ImageName = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\png\camera.png");
        //                        sym.Size = 30;
        //                        IRenderPoint rp = this.axRenderControl1.ObjectManager.CreateRenderPoint(positionPoint, sym, rootId);
        //                        rp.Name = cp.index.ToString();
        //                    }

        //                    DataRow dr = dt.NewRow();
        //                    dr["Name"] = locationName;
        //                    dr["Location"] = cp;
        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //        }
        //    }
        //}

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            curRowIndex = (sender as DataGridView).CurrentRow.Index;
            cp = (sender as DataGridView).CurrentRow.Cells[1].Value as CameraProperty;
            vector.Set(cp.X, cp.Y, cp.Z);
            angle.Set(cp.Heading, cp.Tilt, cp.Roll);
            this.axRenderControl1.Camera.SetCamera(vector, angle, gviSetCameraFlags.gviSetCameraNoFlags);
            curVideoIndex = cp.index;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if ((sender as DataGridView).CurrentRow == null)
                return;

            curRowIndex = (sender as DataGridView).CurrentRow.Index;
            tmpTV = (sender as DataGridView).CurrentRow.Cells[2].Value as VideoObject;

            if (pf.hasClosed())
            {
                pf = new PropertyFrm(tmpTV);
                pf.Text = tmpTV.GUIDString + "的属性";
                pf.Owner = this;
                pf.Show();
            }
            else
            {
                pf.Text = tmpTV.GUIDString + "的属性";
                pf.SetSource(tmpTV);
            }

            cp = (sender as DataGridView).CurrentRow.Cells[1].Value as CameraProperty;
            curVideoIndex = cp.index;
        }

        private void toolStripButtonCreateVideo_Click(object sender, EventArgs e)
        {
            tmpTV = new VideoObject();
            tmpTV.Create();
            tmpTV.Update();

            pf = new PropertyFrm(tmpTV);
            pf.Text = tmpTV.GUIDString + "的属性";
            pf.Owner = this;        
            pf.Show();

            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
            this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick | gviMouseSelectMode.gviMouseSelectMove;
            this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectTileLayer;
            this.axRenderControl1.RcMouseClickSelect += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);
        }

        void axRenderControl1_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            if (e.intersectPoint == null)
                return;

            if (e.eventSender.Equals(gviMouseSelectMode.gviMouseSelectClick))
            {
                this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
                this.axRenderControl1.RcMouseClickSelect -= new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);
                
                //把视频加入list，同时更新界面
                locationName = tmpTV.TerrainVideoObject.Guid.ToString();
                cp = new CameraProperty();
                cp.name = locationName;
                cp.X = tmpTV.TerrainVideoObject.Position.X;
                cp.Y = tmpTV.TerrainVideoObject.Position.Y;
                cp.Z = tmpTV.TerrainVideoObject.Position.Z;
                cp.Heading = tmpTV.TerrainVideoObject.Angle.Heading;
                cp.Roll = tmpTV.TerrainVideoObject.Angle.Roll;
                cp.Tilt = tmpTV.TerrainVideoObject.Angle.Tilt;
                cp.AspectRatio = tmpTV.TerrainVideoObject.AspectRatio;
                cp.FieldOfView = tmpTV.TerrainVideoObject.FieldOfView;
                cp.VideoOpacity = tmpTV.TerrainVideoObject.VideoOpacity;

                videoList.Add(tmpTV.TerrainVideoObject);
                cp.index = videoList.Count - 1;

                DataRow dr = dt.NewRow();
                dr["Name"] = locationName;
                dr["Location"] = cp;
                dr["Object"] = tmpTV;
                dt.Rows.Add(dr);

                this.dataGridView1.Rows[dt.Rows.Count - 1].Selected = true;

                pf.SetSource(tmpTV);
            }
            else if (e.eventSender.Equals(gviMouseSelectMode.gviMouseSelectMove))
            {
                this.axRenderControl1.Camera.GetCamera(out vector, out angle);
                tmpTV.SetAngle(angle);

                positionPoint = e.intersectPoint;
                positionPoint.Z += 10;
                tmpTV.SetPosition(positionPoint);
            }
        }

        private void toolStripButtonHideVideo_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Text.Equals("隐藏视频"))
            {
                for (int i = 0; i < videoList.Count; i++)
                {
                    ITerrainVideo v = videoList[i];
                    v.VisibleMask = gviViewportMask.gviViewNone;
                }
            }
            else
            {
                for (int i = 0; i < videoList.Count; i++)
                {
                    ITerrainVideo v = videoList[i];
                    v.VisibleMask = gviViewportMask.gviViewAllNormalView;
                }
            }

            if ((sender as ToolStripButton).Text.Equals("隐藏视频"))
                (sender as ToolStripButton).Text = "显示视频";
            else
                (sender as ToolStripButton).Text = "隐藏视频";
        }

        private void toolStripButtonHideProjectionLines_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Text.Equals("隐藏投影线"))
            {
                for (int i = 0; i < videoList.Count; i++)
                {
                    ITerrainVideo v = videoList[i];
                    v.ShowProjectionLines = false;
                    v.ShowProjector = false;
                }
            }
            else
            {
                for (int i = 0; i < videoList.Count; i++)
                {
                    ITerrainVideo v = videoList[i];
                    v.ShowProjectionLines = true;
                    v.ShowProjector = true;
                }
            }

            if ((sender as ToolStripButton).Text.Equals("隐藏投影线"))
                (sender as ToolStripButton).Text = "显示投影线";
            else
                (sender as ToolStripButton).Text = "隐藏投影线";
        }





    }

    public class CameraProperty
    {
        public double X = 0.0;
        public double Y = 0.0;
        public double Z = 0.0;
        public double Heading = 0.0;
        public double Tilt = 0.0;
        public double Roll = 0.0;
        public int index = -1;
        public double AspectRatio = 0.0;
        public double FieldOfView = 0.0;
        public double VideoOpacity = 0.0;
        public string name = "";

        public CameraProperty()
        {

        }
        public bool SetCameraProperty(String str)
        {
            try
            {
                String[] buf = str.Split(';');
                Double.TryParse(buf[0], out X);
                Double.TryParse(buf[1], out Y);
                Double.TryParse(buf[2], out Z);
                Double.TryParse(buf[3], out Heading);
                Double.TryParse(buf[4], out Tilt);
                Double.TryParse(buf[5], out Roll);
                int.TryParse(buf[6], out index);
                Double.TryParse(buf[7], out AspectRatio);
                Double.TryParse(buf[8], out FieldOfView);
                Double.TryParse(buf[9], out VideoOpacity);
                name = buf[10].ToString();
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        public String PropertyStrings()
        {
            String s = String.Format("Name=\"{0}\" x=\"{1}\" y=\"{2}\" z=\"{3}\" heading=\"{4}\" tilt=\"{5}\" roll=\"{6}\" AspectRatio=\"{7}\" FieldOfView=\"{8}\" VideoOpacity=\"{9}\"", name, X, Y, Z, Heading, Tilt, Roll, AspectRatio, FieldOfView, VideoOpacity);
            return s;
        }
    }
}
