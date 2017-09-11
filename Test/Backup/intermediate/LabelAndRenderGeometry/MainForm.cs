using System.Windows.Forms;
using System.IO;
//****Gvitech.CityMaker****
using Gvitech.CityMaker.RenderControl;
using Gvitech.CityMaker.Math;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Common;
using Gvitech.CityMaker.Resource;

namespace LabelAndRenderGeometry
{
    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private ISpatialCRS crs = null;

        private IGeometryFactory gfactory = null;

        private IModelPoint fde_modelpoint = null;
        private IRenderModelPoint rmodelpoint = null;
        
        private IPoint fde_point = null;
        private IRenderPoint rpoint = null;
        
        private IPolyline fde_polyline = null;
        private IRenderPolyline rpolyline = null;
        
        private IPolygon fde_polygon = null;
        private IRenderPolygon rpolygon = null;

        private IPOI fde_poi = null;
        private IRenderPOI rpoi = null;
        private int poiCount = 0;

        private ISimplePointSymbol pointSymbol = null;
        private ISurfaceSymbol surfaceSymbol = null;
        private ICurveSymbol lineSymbol = null;

        private ILabel label = null;
        private ITextSymbol textSymbol = null;
        private TextAttribute textAttribute = null;

        private CheckBox check = null;
        private CheckBox checkShowOutline = null;

        private System.Guid rootId = new System.Guid();
        private gviObjectType TYPE = gviObjectType.gviObjectNone;

        public MainForm()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

            rootId = this.axRenderControl1.ObjectManager.GetProjectTree().RootID;
            this.axRenderControl1.Camera.FlyTime = 1;

            crs = (new CRSFactory()).CreateFromWKT(this.axRenderControl1.GetCurrentCrsWKT()) as ISpatialCRS;
            if (crs.CrsType == gviCoordinateReferenceSystemType.gviCrsGeographic)
                TYPE = gviObjectType.gviObjectTerrain;
            else if (crs.CrsType == gviCoordinateReferenceSystemType.gviCrsProject
                || crs.CrsType == gviCoordinateReferenceSystemType.gviCrsUnknown)
                TYPE = gviObjectType.gviObjectReferencePlane;

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

            // 下拉框控件默认选中第一项
            this.toolStripComboBoxObjectManager.SelectedIndex = 0;
            this.toolStripComboBoxColor.SelectedIndex = 0;

            // 注册控件拾取事件
            this.axRenderControl1.RcMouseClickSelect += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEventHandler(axRenderControl1_RcMouseClickSelect);
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
            this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
            this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "LabelAndRenderGeometry.html";
            }    
        }

        void axRenderControl1_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            if (e.pickResult.Type == gviObjectType.gviObjectLabel)
            {
                ILabelPickResult tlpr = e.pickResult as ILabelPickResult;
                gviObjectType type = tlpr.Type;
                ILabel fl = tlpr.Label;
                MessageBox.Show("拾取到" + type + "类型，内容为" + fl.Text);
            }
            else if (e.pickResult.Type == gviObjectType.gviObjectRenderModelPoint)
            {
                IRenderModelPointPickResult tlpr = e.pickResult as IRenderModelPointPickResult;
                gviObjectType type = tlpr.Type;
                IRenderModelPoint fl = tlpr.ModelPoint;
                MessageBox.Show("拾取到" + type + "类型，模型名称为" + fl.ModelName);
            }
            else if (e.pickResult.Type == gviObjectType.gviObjectRenderPoint)
            {
                IRenderPointPickResult tlpr = e.pickResult as IRenderPointPickResult;
                gviObjectType type = tlpr.Type;
                IRenderPoint fl = tlpr.Point;
                MessageBox.Show("拾取到" + type + "类型，大小为" + fl.Symbol.Size);
            }
            else if (e.pickResult.Type == gviObjectType.gviObjectRenderPolyline)
            {
                IRenderPolylinePickResult tlpr = e.pickResult as IRenderPolylinePickResult;
                gviObjectType type = tlpr.Type;
                IRenderPolyline fl = tlpr.Polyline;
                MessageBox.Show("拾取到" + type + "类型，GUID为" + fl.Guid);
            }
            else if (e.pickResult.Type == gviObjectType.gviObjectRenderPolygon)
            {
                IRenderPolygonPickResult tlpr = e.pickResult as IRenderPolygonPickResult;
                gviObjectType type = tlpr.Type;
                IRenderPolygon fl = tlpr.Polygon;
                MessageBox.Show("拾取到" + type + "类型，GUID为" + fl.Guid);
            }
            else if (e.pickResult.Type == gviObjectType.gviObjectRenderPOI)
            {
                IRenderPOIPickResult tlpr = e.pickResult as IRenderPOIPickResult;
                gviObjectType type = tlpr.Type;
                IRenderPOI fl = tlpr.POI;
                MessageBox.Show("拾取到" + type + "类型，名称为" + ((IPOI)fl.GetFdeGeometry()).Name);
            }
            else if (e.pickResult.Type == TYPE)
            {
                switch (this.toolStripComboBoxObjectManager.Text)
                {
                    case "CreateLabel":
                        {
                            label = this.axRenderControl1.ObjectManager.CreateLabel(rootId);
                            label.Text = "我是testlabel";
                            label.Position = e.intersectPoint;
                            textSymbol = new TextSymbol();
                            textAttribute = new TextAttribute();
                            textAttribute.TextColor = 0xffffff00;
                            textAttribute.TextSize = 20;
                            textAttribute.Underline = true;
                            textAttribute.Font = "楷体";
                            textSymbol.TextAttribute = textAttribute;
                            textSymbol.VerticalOffset = 10;
                            textSymbol.DrawLine = true;
                            textSymbol.MarginColor = 0x8800ffff;
                            label.TextSymbol = textSymbol;
                            this.axRenderControl1.Camera.FlyToObject(label.Guid, gviActionCode.gviActionFlyTo);
                        }
                        break;
                    case "CreateRenderModelPoint":
                        {
                            if (gfactory == null)
                                gfactory = new GeometryFactoryClass();

                            string tmpOSGPath = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\osg\Buildings\Apartment\Apartment.osg");
                            fde_modelpoint = gfactory.CreateGeometry(gviGeometryType.gviGeometryModelPoint,
                                gviVertexAttribute.gviVertexAttributeZ) as IModelPoint;
                            fde_modelpoint.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_modelpoint.SpatialCRS = crs;
                            fde_modelpoint.ModelName = tmpOSGPath;
                            rmodelpoint = this.axRenderControl1.ObjectManager.CreateRenderModelPoint(fde_modelpoint, null, rootId);
                            rmodelpoint.MaxVisibleDistance = double.MaxValue;
                            rmodelpoint.MinVisiblePixels = 0;
                            rmodelpoint.ShowOutline = checkShowOutline.Checked;
                            rmodelpoint.ToolTipText = "Apartment";
                            IEulerAngle angle = new EulerAngle();
                            angle.Set(0, -20, 0);
                            this.axRenderControl1.Camera.LookAt2(e.intersectPoint, 100, angle);
                        }
                        break;
                    case "CreateRenderPoint":
                        {
                            if (gfactory == null)
                                gfactory = new GeometryFactoryClass();

                            fde_point = (IPoint)gfactory.CreateGeometry(gviGeometryType.gviGeometryPoint,
                                gviVertexAttribute.gviVertexAttributeZ);
                            fde_point.SpatialCRS = crs;
                            fde_point.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);

                            pointSymbol = new SimplePointSymbolClass();
                            pointSymbol.FillColor = 0xff0000ff;
                            pointSymbol.Size = 10;
                            rpoint = this.axRenderControl1.ObjectManager.CreateRenderPoint(fde_point, pointSymbol, rootId);
                            rpoint.ShowOutline = checkShowOutline.Checked;
                            rpoint.ToolTipText = "point";
                            this.axRenderControl1.Camera.FlyToObject(rpoint.Guid, gviActionCode.gviActionFlyTo);
                        }
                        break;
                    case "CreateRenderPolyline":
                        {
                            if (gfactory == null)
                                gfactory = new GeometryFactoryClass();

                            fde_polyline = (IPolyline)gfactory.CreateGeometry(gviGeometryType.gviGeometryPolyline,
                                gviVertexAttribute.gviVertexAttributeZ);
                            fde_polyline.SpatialCRS = crs;
                            fde_point = (IPoint)gfactory.CreateGeometry(gviGeometryType.gviGeometryPoint,
                                gviVertexAttribute.gviVertexAttributeZ);
                            fde_point.SpatialCRS = crs;
                            fde_point.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_polyline.AppendPoint(fde_point);
                            fde_point.SetCoords(e.intersectPoint.X + 20, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_polyline.AppendPoint(fde_point);
                            fde_point.SetCoords(e.intersectPoint.X + 20, e.intersectPoint.Y + 20, e.intersectPoint.Z, 0, 0);
                            fde_polyline.AppendPoint(fde_point);
                            fde_point.SetCoords(e.intersectPoint.X + 20, e.intersectPoint.Y + 20, e.intersectPoint.Z + 20, 0, 0);
                            fde_polyline.AppendPoint(fde_point);

                            lineSymbol = new CurveSymbolClass();
                            lineSymbol.Color = 0xffff00ff;  // 紫红色
                            rpolyline = this.axRenderControl1.ObjectManager.CreateRenderPolyline(fde_polyline, lineSymbol, rootId);
                            rpolyline.ShowOutline = checkShowOutline.Checked;
                            rpolyline.ToolTipText = "polyline";
                            this.axRenderControl1.Camera.FlyToObject(rpolyline.Guid, gviActionCode.gviActionFlyTo);
                        }
                        break;
                    case "CreateRenderPolygon":
                        {
                            if (gfactory == null)
                                gfactory = new GeometryFactoryClass();

                            fde_polygon = (IPolygon)gfactory.CreateGeometry(gviGeometryType.gviGeometryPolygon,
                                gviVertexAttribute.gviVertexAttributeZ);
                            fde_polygon.SpatialCRS = crs;

                            fde_point = (IPoint)gfactory.CreateGeometry(gviGeometryType.gviGeometryPoint,
                                gviVertexAttribute.gviVertexAttributeZ);
                            fde_point.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_polygon.ExteriorRing.AppendPoint(fde_point);
                            fde_point.SetCoords(e.intersectPoint.X + 10, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_polygon.ExteriorRing.AppendPoint(fde_point);
                            fde_point.SetCoords(e.intersectPoint.X + 10, e.intersectPoint.Y + 10, e.intersectPoint.Z, 0, 0);
                            fde_polygon.ExteriorRing.AppendPoint(fde_point);
                            fde_point.SetCoords(e.intersectPoint.X, e.intersectPoint.Y + 10, e.intersectPoint.Z, 0, 0);
                            fde_polygon.ExteriorRing.AppendPoint(fde_point);

                            surfaceSymbol = new SurfaceSymbolClass();
                            surfaceSymbol.Color = 0xFF0000FF;  // 蓝色
                            rpolygon = this.axRenderControl1.ObjectManager.CreateRenderPolygon(fde_polygon, surfaceSymbol, rootId);
                            rpolygon.ShowOutline = checkShowOutline.Checked;
                            rpolygon.ToolTipText = "polygon";
                            this.axRenderControl1.Camera.FlyToObject(rpolygon.Guid, gviActionCode.gviActionFlyTo);
                        }
                        break;
                    case "CreateRenderPOI":
                        {
                            if (gfactory == null)
                                gfactory = new GeometryFactoryClass();

                            fde_poi = (IPOI)gfactory.CreateGeometry(gviGeometryType.gviGeometryPOI, gviVertexAttribute.gviVertexAttributeZ);
                            fde_poi.SpatialCRS = crs;
                            fde_poi.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_poi.ImageName = "#(1)";
                            fde_poi.Name = (++poiCount).ToString();
                            fde_poi.Size = 50;
                            rpoi = this.axRenderControl1.ObjectManager.CreateRenderPOI(fde_poi);
                            rpoi.ShowOutline = checkShowOutline.Checked;
                            this.axRenderControl1.Camera.FlyToObject(rpoi.Guid, gviActionCode.gviActionFlyTo);
                        }
                        break;
                    case "CreateFixedBillboard":
                        {
                            TextAttribute ta = new TextAttribute();
                            ta.TextSize = 10;
                            ta.TextColor = 0xffff0000;
                            IImage image = null;
                            IModel model = null;
                            string imageName = "";
                            this.axRenderControl1.Utility.CreateFixedBillboard("I'm fixed billboard!", ta, 50, 100, true, out model, out image, out imageName);
                            this.axRenderControl1.ObjectManager.AddModel("fixedModel", model);
                            this.axRenderControl1.ObjectManager.AddImage(imageName, image);    

                            if (gfactory == null)
                                gfactory = new GeometryFactoryClass();
                            fde_modelpoint = gfactory.CreateGeometry(gviGeometryType.gviGeometryModelPoint,
                                gviVertexAttribute.gviVertexAttributeZ) as IModelPoint;
                            fde_modelpoint.SpatialCRS = crs;
                            fde_modelpoint.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);
                            fde_modelpoint.ModelName = "fixedModel";
                            rmodelpoint = this.axRenderControl1.ObjectManager.CreateRenderModelPoint(fde_modelpoint, null, rootId);
                            rmodelpoint.MaxVisibleDistance = double.MaxValue;
                            rmodelpoint.MinVisiblePixels = 0;
                            rmodelpoint.ShowOutline = checkShowOutline.Checked;
                            IEulerAngle angle = new EulerAngle();
                            angle.Set(0, -20, 0);
                            this.axRenderControl1.Camera.LookAt2(e.intersectPoint, 100, angle);
                        }
                        break;
                }
            }
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            check = new CheckBox();
            check.Text = "进入漫游模式";
            check.Width = 80;
            check.Checked = false;
            check.CheckedChanged += new System.EventHandler(check_CheckedChanged);
            ToolStripControlHost host = new ToolStripControlHost(check);
            toolStrip1.Items.Add(host);

            checkShowOutline = new CheckBox();
            checkShowOutline.Text = "显示外轮廓线";
            checkShowOutline.Width = 80;
            checkShowOutline.Checked = false;
            checkShowOutline.CheckedChanged += new System.EventHandler(checkShowOutline_CheckedChanged);
            ToolStripControlHost hostShowOutline = new ToolStripControlHost(checkShowOutline);
            toolStrip1.Items.Add(hostShowOutline);
        }

        void check_CheckedChanged(object sender, System.EventArgs e)
        {
            if (check.Checked)
            {
                this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
            }
            else
            {
                this.axRenderControl1.InteractMode = gviInteractMode.gviInteractSelect;
                this.axRenderControl1.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectAll;
                this.axRenderControl1.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
            }
        }

        void checkShowOutline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkShowOutline == null)
                return;

            if (checkShowOutline.Checked)
            {
                switch (toolStripComboBoxColor.SelectedIndex)
                {
                    case 0:
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamOutlineColor, 0xffff0000);
                        break;
                    case 1:
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamOutlineColor, 0xffffff00);
                        break;
                    case 2:
                        this.axRenderControl1.SetRenderParam(gviRenderControlParameters.gviRenderParamOutlineColor, 0xff0000ff);
                        break;
                }
            }

            if (rmodelpoint != null)
                rmodelpoint.ShowOutline = checkShowOutline.Checked;
            if (rpoint != null)
                rpoint.ShowOutline = checkShowOutline.Checked;
            if (rpolyline != null)
                rpolyline.ShowOutline = checkShowOutline.Checked;
            if (rpolygon != null)
                rpolygon.ShowOutline = checkShowOutline.Checked;
            if (rpoi != null)
                rpoi.ShowOutline = checkShowOutline.Checked;
        }

    }
}
