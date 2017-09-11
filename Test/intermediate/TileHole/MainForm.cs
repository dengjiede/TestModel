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

namespace TileHole
{
    public enum TreeNodeType
    {
        NT_DATASOURCE ,
        NT_DATASET ,
        NT_FEATURECLASS ,
        NT_SUBTYPE ,
        NT_CODEVALUE ,
        NT_IMAGELAYER ,
        NT_TERRAINLAYER,
        NT_TiltedLAYER,
        NT_KmlGroup,
        NT_TerrainModifier,
        NT_RenderGeomtry,
        NT_TerrainHole,
        NT_TileHole
    }

    public partial class MainForm : Form
    {
        private int flag = -1;  // 标记"Samples"文件夹在目录中的索引号
        private System.Guid rootId = new System.Guid();

        IGeometryFactory geoFactory = new GeometryFactory();
        ICRSFactory crsFactory = new CRSFactory();
        ICoordinateReferenceSystem crs = null;
        IRenderGeometry currentRenderGeometry = null;
        IGeometry currentGeometry = null;

        int order = 0;

        public MainForm()
        {
            InitializeComponent();

            // 初始化RenderControl控件
            IPropertySet ps = new PropertySet();
            ps.SetProperty("RenderSystem", gviRenderSystem.gviRenderOpenGL);
            this.axRenderControl1.Initialize(true, ps);

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

            this.axRenderControl1.Camera.FlyTime = 1;

            // 加载瓦片图层
            string tilelayerString = Path.Combine(Application.StartupPath.Substring(0, flag), @"Samples\Media\sdk.tdb");
            I3DTileLayer layer = this.axRenderControl1.ObjectManager.Create3DTileLayer(tilelayerString, "", rootId);
            this.axRenderControl1.Camera.FlyToObject(layer.Guid, gviActionCode.gviActionFlyTo);
            // 添加节点到界面控件上
            myListNode item = new myListNode("tilelayer", TreeNodeType.NT_TiltedLAYER, layer);
            item.Checked = true;
            listView1.Items.Add(item);

            // 注册事件
            this.axRenderControl1.RcObjectEditing += new Gvitech.CityMaker.Controls._IRenderControlEvents_RcObjectEditingEventHandler(axRenderControl1_RcObjectEditing);
            this.axRenderControl1.RcObjectEditFinish += new System.EventHandler(axRenderControl1_RcObjectEditFinish);

            {
                this.helpProvider1.SetShowHelp(this.axRenderControl1, true);
                this.helpProvider1.SetHelpString(this.axRenderControl1, "");
                this.helpProvider1.HelpNamespace = "TileHole.html";
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            myListNode item = (myListNode)e.Item;

            switch (item.type)
            {
                case TreeNodeType.NT_TiltedLAYER:
                    I3DTileLayer ted = item.obj as I3DTileLayer;
                    ted.VisibleMask = e.Item.Checked ? gviViewportMask.gviViewAllNormalView : gviViewportMask.gviViewNone;
                    break;
                case TreeNodeType.NT_TileHole:
                    I3DTileHole hole = item.obj as I3DTileHole;
                    hole.VisibleMask = e.Item.Checked ? gviViewportMask.gviViewAllNormalView : gviViewportMask.gviViewNone;
                    break;
                case TreeNodeType.NT_RenderGeomtry:
                    IRenderGeometry geo = item.obj as IRenderGeometry;
                    geo.VisibleMask = e.Item.Checked ? gviViewportMask.gviViewAllNormalView : gviViewportMask.gviViewNone;
                    break;
            }
        }
            
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.listView1.SelectedItems.Count == 0) return;
            myListNode item = (myListNode)this.listView1.SelectedItems[0];
            item.Checked = true;
            switch (item.type)
            {
                case TreeNodeType.NT_TiltedLAYER:
                    I3DTileLayer layer = item.obj as I3DTileLayer;
                    layer.Highlight(0xffff0000);
                    this.axRenderControl1.Camera.FlyToObject(layer.Guid, gviActionCode.gviActionFlyTo);
                    break;
                case TreeNodeType.NT_TileHole:
                    I3DTileHole hole = item.obj as I3DTileHole;
                    hole.Highlight(0xffff0000);
                    this.axRenderControl1.Camera.FlyToObject(hole.Guid, gviActionCode.gviActionFlyTo);
                    break;
                case TreeNodeType.NT_RenderGeomtry:
                    IRenderGeometry geo = item.obj as IRenderGeometry;
                    this.axRenderControl1.Camera.FlyToObject(geo.Guid, gviActionCode.gviActionFlyTo);
                    break;
            }
        }

        private void toolStripButtonLoadTileLayer_Click(object sender, System.EventArgs e)
        {
            
        }

        #region 创建地形编辑
        private void toolStripButtonCreateTileHole_Click(object sender, System.EventArgs e)
        {
            TileHoleSettingForm form = new TileHoleSettingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                order = form.Order;

                currentGeometry = geoFactory.CreateGeometry(gviGeometryType.gviGeometryPolygon, gviVertexAttribute.gviVertexAttributeZ) as IPolygon;
                currentGeometry.SpatialCRS = crs as ISpatialCRS;

                ISurfaceSymbol sfbottom = new SurfaceSymbol();
                sfbottom.Color = 0x550000FF;
                currentRenderGeometry = this.axRenderControl1.ObjectManager.CreateRenderPolygon(currentGeometry as IPolygon, sfbottom, rootId);
                (currentRenderGeometry as IRenderPolygon).HeightStyle = gviHeightStyle.gviHeightOnTerrain;
                this.axRenderControl1.InteractMode = gviInteractMode.gviInteractEdit;
                this.axRenderControl1.ObjectEditor.StartEditRenderGeometry(currentRenderGeometry, gviGeoEditType.gviGeoEditCreator);
            }
        }

        void axRenderControl1_RcObjectEditFinish(object sender, System.EventArgs e)
        {
            I3DTileHole hole = this.axRenderControl1.ObjectManager.Create3DTileHole(currentGeometry as IPolygon, System.Guid.Empty);
            if (hole != null)
            {
                hole.DrawOrder = order;

                // 添加节点到界面控件上
                myListNode item = new myListNode(string.Format("TileHole_{0}", hole.Guid), TreeNodeType.NT_TileHole, hole);
                item.Checked = true;
                listView1.Items.Add(item);

                // 添加节点到界面控件上
                //item = new myListNode(string.Format("RenderPolygon_{0}", hole.Guid), TreeNodeType.NT_RenderGeomtry, currentRenderGeometry);
                //item.Checked = true;
                //listView1.Items.Add(item);
            }

            // 恢复漫游模式
            this.axRenderControl1.InteractMode = gviInteractMode.gviInteractNormal;
        }

        void axRenderControl1_RcObjectEditing(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcObjectEditingEvent e)
        {
            currentGeometry = e.geometry;
        }
        #endregion

        #region 右键弹出菜单
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.listView1.GetItemAt(e.X, e.Y) == null)
            {
                属性ToolStripMenuItem.Enabled = false;
                删除ToolStripMenuItem.Enabled = false;
                生成瓦片挖洞ToolStripMenuItem.Enabled = false;
                return;
            }                
            if (e.Button == MouseButtons.Right)
            {
                myListNode selectNode = this.listView1.GetItemAt(e.X, e.Y) as myListNode;                
                if (selectNode.type == TreeNodeType.NT_TileHole)
                {
                    属性ToolStripMenuItem.Enabled = true;
                    删除ToolStripMenuItem.Enabled = true;
                    生成瓦片挖洞ToolStripMenuItem.Enabled = false;
                }
                else if (selectNode.type == TreeNodeType.NT_RenderGeomtry)
                {
                    属性ToolStripMenuItem.Enabled = false;
                    删除ToolStripMenuItem.Enabled = true;
                    生成瓦片挖洞ToolStripMenuItem.Enabled = true;
                }
                else
                {
                    属性ToolStripMenuItem.Enabled = false;
                    删除ToolStripMenuItem.Enabled = false;
                    生成瓦片挖洞ToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void 属性ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            myListNode selectNode = this.listView1.SelectedItems[0] as myListNode;
            if (selectNode != null)
            {
                I3DTileHole hole = selectNode.obj as I3DTileHole;
                if (hole == null)
                    return;
                int index = 0;
                TileHoleSettingForm form = new TileHoleSettingForm(hole.DrawOrder, index);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    order = form.Order;
                    hole.DrawOrder = order;
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            myListNode selectNode = this.listView1.SelectedItems[0] as myListNode;
            if (selectNode != null)
            {
                IRenderable rgeo = selectNode.obj as IRenderable;
                this.axRenderControl1.ObjectManager.DeleteObject(rgeo.Guid);
                this.listView1.Items.Remove(selectNode);
            }
        }

        private void 生成瓦片挖洞ToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            myListNode selectNode = this.listView1.SelectedItems[0] as myListNode;
            if (selectNode != null)
            {
                IRenderPolygon rgeo = selectNode.obj as IRenderPolygon;
                IPolygon polygon = rgeo.GetFdeGeometry() as IPolygon;
                
                // 生成带洞polygon，可注释掉
                IEnvelope env = rgeo.Envelope;
                IRing ring = (new GeometryFactory()).CreateGeometry(gviGeometryType.gviGeometryRing, gviVertexAttribute.gviVertexAttributeZ) as IRing;
                IPoint center = (new GeometryFactory()).CreatePoint(gviVertexAttribute.gviVertexAttributeZ);
                center.Position = env.Center;
                center.SpatialCRS = crs as ISpatialCRS;
                ring.AppendPoint(center);
                center.Y = env.Center.Y - 50;
                ring.AppendPoint(center);
                center.X = env.Center.X + 50;
                ring.AppendPoint(center);
                center.Y = env.Center.Y;
                ring.AppendPoint(center);
                polygon.AddInteriorRing(ring);
                // To here

                ISurfaceSymbol sfbottom = new SurfaceSymbol();
                sfbottom.Color = 0x5500FF00;
                IRenderPolygon rgeoNew = this.axRenderControl1.ObjectManager.CreateRenderPolygon(polygon, sfbottom, rootId);

                TileHoleSettingForm form = new TileHoleSettingForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    order = form.Order;

                    I3DTileHole hole = this.axRenderControl1.ObjectManager.Create3DTileHole(polygon, System.Guid.Empty);
                    if (hole != null)
                    {
                        hole.DrawOrder = order;

                        // 添加节点到界面控件上
                        myListNode item = new myListNode(string.Format("TileHole_{0}", hole.Guid), TreeNodeType.NT_TerrainHole, hole);
                        item.Checked = true;
                        listView1.Items.Add(item);

                        // 添加节点到界面控件上
                        item = new myListNode(string.Format("RenderPolygon_{0}", hole.Guid), TreeNodeType.NT_RenderGeomtry, rgeoNew);
                        item.Checked = true;
                        listView1.Items.Add(item);
                    }
                }
            }
        }
        #endregion
    }

    class myListNode : ListViewItem
    {
        public string name;
        public TreeNodeType type;
        public IRObject obj;

        public myListNode(string n, TreeNodeType t, IRObject o)
        {
            name = n;
            type = t;
            obj = o;
            this.Text = n;
        }
    }
}
