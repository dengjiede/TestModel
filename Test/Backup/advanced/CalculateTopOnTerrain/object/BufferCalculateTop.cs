using System;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.RenderControl;
using System.Windows.Forms;

namespace CalculateTopOnTerrain
{
    /*==========================================================
     *Product:     CityMaker
     *Author：     Reason
     *Date：       2013/5/15
     *Module：     缓冲区域最高点计算
     *Description: 
     ===========================================================*/

    /// <summary>
    /// 计算最高点
    /// </summary>
    public class BufferCalculateTop : CalculateTopBase, ICalculateTop
    {
        #region 成员

        /// <summary>
        /// 第一次单击
        /// </summary>
        private bool _FirstClick = true;

        /// <summary>
        /// 起始点
        /// </summary>
        private IPoint _StartPoint;

        private IPoint _EndPoint;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="axRenderControl">三维控件</param>
        public BufferCalculateTop(Gvitech.CityMaker.Controls.AxRenderControl axRenderControl)
            : base(axRenderControl)
        {
           
        }

        #endregion

        #region 事件监听

        /// <summary>
        /// 选择点事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void _AxRenderControl_RcMouseClickSelect(object sender, Gvitech.CityMaker.Controls._IRenderControlEvents_RcMouseClickSelectEvent e)
        {
            try
            {
                if (e.intersectPoint == null) { return; }

                if (e.eventSender == gviMouseSelectMode.gviMouseSelectClick)
                {
                    if (_FirstClick)
                    {
                        Clear();

                        if (_StartPoint == null)
                        {
                            IGeometryFactory geometryFactory = new GeometryFactory();
                            _StartPoint = geometryFactory.CreatePoint(gviVertexAttribute.gviVertexAttributeZ);
                        }

                        _StartPoint.SetCoords(e.intersectPoint.X, e.intersectPoint.Y, e.intersectPoint.Z, 0, 0);

                        if (_StartRenderPoint != null)
                        {
                            _StartRenderPoint.SetFdeGeometry(_StartPoint);
                        }
                        else
                        {
                            _StartRenderPoint = _AxRenderControl.ObjectManager.CreateRenderPoint(_StartPoint, _PointSymbol, rootId);
                            _StartRenderPoint.MaxVisibleDistance = double.MaxValue;
                            _StartRenderPoint.MinVisiblePixels = 1;
                        }
                        _StartRenderPoint.VisibleMask = gviViewportMask.gviViewAllNormalView;
                    }
                    else
                    {
                        IPoint startPoint = _StartRenderPoint.GetFdeGeometry() as IPoint;
                        double distance = Math.Sqrt((startPoint.X - e.intersectPoint.X) * (startPoint.X - e.intersectPoint.X) + (startPoint.Y - e.intersectPoint.Y) * (startPoint.Y - e.intersectPoint.Y));
                        if (distance <= 0) { return; }

                        DrawBufferPolygon(distance);

                        GetTopValue(_RenderPolygon.GetFdeGeometry() as IPolygon);
                    }
                    _FirstClick = !_FirstClick;
                }
                else if (e.eventSender == gviMouseSelectMode.gviMouseSelectMove && false == _FirstClick)
                {
                    IPoint startPoint = _StartRenderPoint.GetFdeGeometry() as IPoint;
                    double distance = Math.Sqrt((startPoint.X - e.intersectPoint.X) * (startPoint.X - e.intersectPoint.X) + (startPoint.Y - e.intersectPoint.Y) * (startPoint.Y - e.intersectPoint.Y));
                    if (distance <= 0) { return; }

                    DrawBufferPolygon(distance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region ICalculateTop 方法

        /// <summary>
        /// 计算最高点
        /// </summary>
        public override void CalculateTop()
        {
            _AxRenderControl.RcMouseClickSelect -= _AxRenderControl_RcMouseClickSelect;
            _AxRenderControl.RcMouseClickSelect += _AxRenderControl_RcMouseClickSelect;

            _AxRenderControl.InteractMode = gviInteractMode.gviInteractSelect;
            _AxRenderControl.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectTerrain;
            _AxRenderControl.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick | gviMouseSelectMode.gviMouseSelectMove;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _AxRenderControl.RcMouseClickSelect -= _AxRenderControl_RcMouseClickSelect;

            _AxRenderControl.InteractMode = gviInteractMode.gviInteractNormal;
            _AxRenderControl.MouseSelectObjectMask = gviMouseSelectObjectMask.gviSelectNone;
            _AxRenderControl.MouseSelectMode = gviMouseSelectMode.gviMouseSelectClick;
        }

        #endregion
        
        #region 私有方法

        /// <summary>
        /// 进行缓冲
        /// </summary>
        /// <param name="distance">缓冲距离</param>
        /// <returns>缓冲形成的多边形</returns>
        private IRenderPolygon DrawBufferPolygon(double distance)
        {
            IPolygon bufferPolygon = GetBufferPolygon(distance);

            if (_RenderPolygon != null)
            {
                _RenderPolygon.SetFdeGeometry(bufferPolygon);
            }
            else
            {
                _RenderPolygon = _AxRenderControl.ObjectManager.CreateRenderPolygon(bufferPolygon, _SurfaceSymbol, rootId);
                _RenderPolygon.MaxVisibleDistance = double.MaxValue;
                _RenderPolygon.MinVisiblePixels = 3;
                _RenderPolygon.HeightStyle = gviHeightStyle.gviHeightOnTerrain;
            }

            _RenderPolygon.VisibleMask = gviViewportMask.gviViewAllNormalView;
            
            return _RenderPolygon;
        }

        /// <summary>
        /// 获取缓冲多边形
        /// </summary>
        /// <param name="distance">缓冲半径</param>
        /// <returns>缓冲多边形</returns>
        private IPolygon GetBufferPolygon(double distance)
        {
            IPoint startPoint = _StartRenderPoint.GetFdeGeometry() as IPoint;
            if (startPoint == null) { return null; }

            IPoint drawSource = startPoint.Clone2(gviVertexAttribute.gviVertexAttributeZ) as IPoint;
            ITopologicalOperator2D drawTopo = drawSource as ITopologicalOperator2D;
            IPolygon bufferPolygon = drawTopo.Buffer2D(distance, gviBufferStyle.gviBufferCapround) as IPolygon;

            return bufferPolygon;
        }

        #endregion
    }
}
