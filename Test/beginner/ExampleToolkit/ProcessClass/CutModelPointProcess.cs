using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataInteropCommon.PublicClass;
using Gvitech.CityMaker.FdeCore;
using CityMakerBuilder.WorkSpace;
using System.Runtime.InteropServices;
using Gvitech.CityMaker.FdeGeometry;
using Gvitech.CityMaker.Resource;

namespace ExampleToolkit.ProcessClass
{
    public class CutModelPointProcess : DataProcessClass
    {
        //目标数据
        protected object _targetData = null;

        //源数据
        protected object _sourceData = null;

        protected string _shpFile = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sourceItems">输入数据集集合</param>
        /// <param name="targetItem">目标数据集</param>
        /// <param name="bConflictDataOverlap">数据冲突是否覆盖</param>
        public CutModelPointProcess(ToolKitFeatureDataSetNodeData sourceItems, ToolKitFeatureDataSetNodeData targetItem, string shpFile)
        {
            this._sourceData = sourceItems;
            this._targetData = targetItem;
            this._shpFile = shpFile;
        }

        /// <summary>
        /// 目标数据
        /// </summary>
        public object TargetData
        {
            set { _targetData = value; }
        }

        /// <summary>
        /// 源数据
        /// </summary>
        public object SourceData
        {
            set { _sourceData = value; }
        }

        public string ShpFile
        {
            set { _shpFile = value; }
        }

        /// <summary>
        /// 执行数据合并过程
        /// </summary>
        /// <returns></returns>
        public override bool ExecDataProcess()
        {
            Logger.WriteMsg(LogLevel.Message, "***************Begin CutModelPoint Toolkit***************", DateTime.Now);
            ToolKitFeatureDataSetNodeData sourceItem = this._sourceData as ToolKitFeatureDataSetNodeData;
            ToolKitFeatureDataSetNodeData targetItem = this._targetData as ToolKitFeatureDataSetNodeData;
            object[] obj = null;
            string toolTip = "";
            
            IDataSource targetDs = null;
            IFeatureDataSet targetFds = null;
            IFeatureClass targetFc = null;
            IFdeCursor targetCursor = null;
            List<string> targetFcNames = new List<string>();
            List<string> targetModelList = new List<string>();
            List<string> targetImageList = new List<string>();

            //记录要素类中引用的模型名称
            List<string> usedModels = new List<string>();
            //记录要素类中引用的贴图的名称
            List<string> usedImages = new List<string>();
	                
	        IDataSource sourceDs = null;
	        IFeatureDataSet sourceFds = null;	
	        Dictionary<string, IFeatureClass> dicFcName = new Dictionary<string, IFeatureClass>();
	        IFdeCursor sourceCursor = null;

            IMultiPolygon multiPolygon = null;

            try
            {
                #region 打开目标数据集
                targetDs = new DataSourceFactory().OpenDataSourceByString(targetItem.ConnectionInfoStr);
                if (targetDs == null)
                {
                    //第一个进度条
                    toolTip = "执行出错，打开目标数据源失败！";
                    obj = new object[] { true, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    //第二个进度条
                    toolTip = " ";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Open Target DataSource <" + targetItem.DataSourceName + "> Failed!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                targetFds = targetDs.OpenFeatureDataset(targetItem.DataSetName);
                if (targetFds == null)
                {
                    //第一个进度条
                    toolTip = "执行出错，打开目标数据集失败！";
                    obj = new object[] { true, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    //第二个进度条
                    toolTip = " ";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);

                    Marshal.ReleaseComObject(targetFds);
                    targetFds = null;
                    Marshal.ReleaseComObject(targetDs);
                    targetDs = null;
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Open Target FeatureDataset <" + targetItem.DataSetName + "> Failed!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                if (targetFds.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable).Length != 0)
                {
                    //第一个进度条
                    toolTip = "目标数据集不能包含要素类！";
                    obj = new object[] { true, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    //第二个进度条
                    toolTip = " ";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);

                    Marshal.ReleaseComObject(targetFds);
                    targetFds = null;
                    Marshal.ReleaseComObject(targetDs);
                    targetDs = null;
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Target FeatureDataset <" + targetItem.DataSetName + "> contains featureclass!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                #endregion
                IResourceManager targetResource = targetFds as IResourceManager;

                #region 打开输入数据集
                sourceDs = new DataSourceFactory().OpenDataSourceByString(sourceItem.ConnectionInfoStr);
                if (sourceDs == null)
                {
                    toolTip = "打开输入数据源" + sourceItem.ParentDataSourceItem.DataSourceName + "失败";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    if (targetFds != null)
                    {
                        Marshal.ReleaseComObject(targetFds);
                        targetFds = null;
                    }
                    if (targetDs != null)
                    {
                        Marshal.ReleaseComObject(targetDs);
                        targetDs = null;
                    }
                    if (sourceDs != null)
                    {
                        Marshal.ReleaseComObject(sourceDs);
                        sourceDs = null;
                    }
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Open DataSource <" + sourceItem.DataSourceName + "> Failed!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                string sourceFdsName = sourceItem.DataSetName;
                sourceFds = sourceDs.OpenFeatureDataset(sourceFdsName);
                if (sourceFds == null)
                {
                    toolTip = "打开输入数据集" + sourceItem.ParentDataSourceItem.DataSourceName + "\\" + sourceItem.DataSetName + "失败";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    if (targetFds != null)
                    {
                        Marshal.ReleaseComObject(targetFds);
                        targetFds = null;
                    }
                    if (targetDs != null)
                    {
                        Marshal.ReleaseComObject(targetDs);
                        targetDs = null;
                    }
                    if (sourceFds != null)
                    {
                        Marshal.ReleaseComObject(sourceFds);
                        sourceFds = null;
                    }
                    if (sourceDs != null)
                    {
                        Marshal.ReleaseComObject(sourceDs);
                        sourceDs = null;
                    }
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Open FeatureDataset <" + sourceItem.DataSetName + "> Failed!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                string[] sourceFcNames = sourceFds.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable);
                if (sourceFcNames == null || sourceFcNames.Length == 0)
                {
                    toolTip = "输入数据集不存在要素类！";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    if (targetFds != null)
                    {
                        Marshal.ReleaseComObject(targetFds);
                        targetFds = null;
                    }
                    if (targetDs != null)
                    {
                        Marshal.ReleaseComObject(targetDs);
                        targetDs = null;
                    }
                    if (sourceFds != null)
                    {
                        Marshal.ReleaseComObject(sourceFds);
                        sourceFds = null;
                    }
                    if (sourceDs != null)
                    {
                        Marshal.ReleaseComObject(sourceDs);
                        sourceDs = null;
                    }
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:FeatureDataset <" + sourceItem.DataSetName + "> has no featureclass!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                #endregion
                IResourceManager sourceResource = sourceFds as IResourceManager;

                #region 获取导入输入数据集总记录数量
                //获取所有FeatureClass内数据总记录
                int totalCount = 0;
                //保存当前记录数
                int iCurRecordIndex = 0;

                dicFcName.Clear();
                foreach (string fcName in sourceFcNames)
                {
                    ManualResult.WaitOne();
                    if (BgWorker.CancellationPending)
                    {
                        if (!DoWorkEvent.Cancel)
                            DoWorkEvent.Cancel = true;
                        break;
                    }
                    IFeatureClass myFc = sourceFds.OpenFeatureClass(fcName);
                    dicFcName[fcName] = myFc;
                    totalCount += myFc.GetCount(null);
                }

                if (!DoWorkEvent.Cancel && dicFcName.Count == 0 && totalCount == 0)
                {
                    toolTip = "无有效输入要素类！";
                    obj = new object[] { false, toolTip };
                    BgWorker.ReportProgress(0, obj);
                    if (targetFds != null)
                    {
                        Marshal.ReleaseComObject(targetFds);
                        targetFds = null;
                    }
                    if (targetDs != null)
                    {
                        Marshal.ReleaseComObject(targetDs);
                        targetDs = null;
                    }
                    if (sourceFds != null)
                    {
                        Marshal.ReleaseComObject(sourceFds);
                        sourceFds = null;
                    }
                    if (sourceDs != null)
                    {
                        Marshal.ReleaseComObject(sourceDs);
                        sourceDs = null;
                    }
                    Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:FeatureDataset <" + sourceItem.DataSetName + "> has no valid featureclass!", DateTime.Now);
                    Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                    return false;
                }
                #endregion

                #region 获取MultiPolygon
                IDataSourceFactory dsf = new DataSourceFactory();
                IConnectionInfo conn = new ConnectionInfo();
                conn.ConnectionType = gviConnectionType.gviConnectionShapeFile;
                conn.Database = _shpFile;
                IDataSource ds = dsf.OpenDataSource(conn);
                int polygonIndex = -1;
                if (ds != null)
                {
                    IFeatureDataSet fds = ds.OpenFeatureDataset(ds.GetFeatureDatasetNames()[0]);
                    IFeatureClass shpFc = fds.OpenFeatureClass(fds.GetNamesByType(gviDataSetType.gviDataSetFeatureClassTable)[0]);
                    IFieldInfoCollection fields = shpFc.GetFields();
                    for (int i = 0; i < fields.Count; i++)
                    {
                        IFieldInfo field = fields.Get(i);
                        if (field.FieldType == gviFieldType.gviFieldGeometry)
                        {
                            if (field.GeometryDef != null)
                            {
                                if (field.GeometryDef.GeometryColumnType == gviGeometryColumnType.gviGeometryColumnPolygon)
                                    polygonIndex = i;
                            }
                        }
                    }
                    IFdeCursor shpCursor = null;
                    try
                    {
                        shpCursor = shpFc.Search(null, true);
                        IRowBuffer shpRow = null;
                        while ((shpRow = shpCursor.NextRow()) != null)
                        {
                            IGeometry geo = shpRow.GetValue(polygonIndex) as IGeometry;
                            if (multiPolygon == null)
                                multiPolygon = new GeometryFactory().CreateGeometry(gviGeometryType.gviGeometryMultiPolygon, geo.VertexAttribute) as IMultiPolygon;
                            multiPolygon.AddGeometry(geo);
                        }
                    }
                    catch (COMException ex)
                    {
                        if (multiPolygon != null)
                        {
                            Marshal.ReleaseComObject(multiPolygon);
                            multiPolygon = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (multiPolygon != null)
                        {
                            Marshal.ReleaseComObject(multiPolygon);
                            multiPolygon = null;
                        }
                    }
                    finally
                    {
                        if (shpCursor != null)
                        {
                            Marshal.ReleaseComObject(shpCursor);
                            shpCursor = null;
                        }
                    }                    
                }                
                #endregion
                
                #region 遍历输入要素类
                int totalFCCount = sourceFcNames.Length;
                int iCurFCIndex = 0;
                foreach (KeyValuePair<string, IFeatureClass> myFcPair in dicFcName)
                {
                    ManualResult.WaitOne();
                    if (BgWorker.CancellationPending)
                    {
                        if (!DoWorkEvent.Cancel)
                            DoWorkEvent.Cancel = true;
                        break;
                    }

                    ProgressStepOne(totalFCCount, iCurFCIndex++, true, "正在处理第{0}个/总共{1}个要素类");

                    string strName = myFcPair.Key;
                    IFeatureClass sourceFc = myFcPair.Value;

                    IFieldInfoCollection sourceFields = sourceFc.GetFields();
                    IFieldInfoCollection cloneSourceFields = sourceFields.Clone();
                    for (int y = 0; y < cloneSourceFields.Count; y++)
                    {
                        if (cloneSourceFields.Get(y).FieldType == gviFieldType.gviFieldFID)
                        {
                            cloneSourceFields.RemoveAt(y);
                            break;
                        }
                        cloneSourceFields.Get(y).RegisteredRenderIndex = false;
                        cloneSourceFields.Get(y).Domain = null;
                    }
                    targetFc = targetFds.CreateFeatureClass(strName, cloneSourceFields);
                    targetFcNames.Add(strName);
                    targetFc.LockType = gviLockType.gviLockExclusiveSchema;

                    IFieldInfoCollection targetFields = targetFc.GetFields();
                    Dictionary<int, int> dicModelFieldInTargetPos = new Dictionary<int, int>();
                    Dictionary<int, int> dicNotModelFieldInTargetPos = new Dictionary<int, int>();
                    for (int d = 0; d < sourceFields.Count; d++)
                    {
                        IFieldInfo sourceField = sourceFields.Get(d);
                        if (sourceField.FieldType == gviFieldType.gviFieldFID)
                            continue;
                        else if (sourceField.FieldType == gviFieldType.gviFieldGeometry && sourceField.GeometryDef.GeometryColumnType == gviGeometryColumnType.gviGeometryColumnModelPoint)
                        {
                            dicModelFieldInTargetPos[d] = targetFields.IndexOf(sourceField.Name);
                        }
                        else
                            dicNotModelFieldInTargetPos[d] = targetFields.IndexOf(sourceField.Name);
                    }

                    #region 逐条处理
                    targetDs.StartEditing();
                    sourceCursor = sourceFc.Search(null, true);
                    IRowBuffer sourceRow = null;
                    targetCursor = targetFc.Insert();
                    IRowBuffer targetRow = null;
                    while ((sourceRow = sourceCursor.NextRow()) != null)
                    {
                        ManualResult.WaitOne();
                        if (BgWorker.CancellationPending)
                        {
                            if (!DoWorkEvent.Cancel)
                                DoWorkEvent.Cancel = true;
                            break;
                        }

                        ProgressStepOne(totalCount, ++iCurRecordIndex, false, "模型切割中，已完成{0}条/总共{1}条");
                        
                        foreach (KeyValuePair<int, int> pos in dicModelFieldInTargetPos)
                        {
                            if (sourceRow.IsNull(pos.Key))
                                continue;
                            IModelPoint modelPointSrc = sourceRow.GetValue(pos.Key) as IModelPoint;
                            if (modelPointSrc != null)
                            {
                                IModel modelSrc = sourceResource.GetModel(modelPointSrc.ModelName);
                                IModel modelOut = null;
                                IModelPoint mpOut = null;
                                if (new GeometryConvertor().CutModelPointByPolygon2D(multiPolygon, modelSrc, modelPointSrc, out modelOut, out mpOut))
                                {
                                    targetRow = targetFc.CreateRowBuffer();
                                    foreach (KeyValuePair<int, int> posNotModel in dicNotModelFieldInTargetPos)
                                    {
                                        if (sourceRow.IsNull(posNotModel.Key))
                                            continue;
                                        int notModelFieldTargetIndex = posNotModel.Value;
                                        object notModelFieldTargetValue = sourceRow.GetValue(posNotModel.Key);
                                        targetRow.SetValue(notModelFieldTargetIndex, notModelFieldTargetValue);
                                    }                                    

                                    targetRow.SetValue(pos.Value, mpOut);
                                    targetCursor.InsertRow(targetRow);

                                    string modelName = modelPointSrc.ModelName;
                                    MergeModelAndImage(modelName, ref usedModels, ref usedImages, sourceResource, targetResource, ref targetModelList, ref targetImageList);

                                    targetResource.AddModel(mpOut.ModelName, modelOut, null);
                                    targetResource.RebuildSimplifiedModel(mpOut.ModelName);                                   
                                }
                            }                            
                        }                        
                    }
                    targetDs.StopEditing(true);
                    #endregion

                    if (sourceCursor != null)
                    {
                        Marshal.ReleaseComObject(sourceCursor);
                        sourceCursor = null;
                    }
                    if (targetFc != null && targetFc.LockType == gviLockType.gviLockExclusiveSchema)
                        targetFc.LockType = gviLockType.gviLockSharedSchema;
                    if (targetCursor != null)
                    {
                        Marshal.ReleaseComObject(targetCursor);
                        targetCursor = null;
                    }
                    if (targetFc != null)
                    {
                        Marshal.ReleaseComObject(targetFc);
                        targetFc = null;
                    }
                }
                #endregion

                #region 释放对象
                if (dicFcName != null && dicFcName.Count > 0)
                {
                    foreach (KeyValuePair<string, IFeatureClass> myFcNamePair in dicFcName)
                    {
                        IFeatureClass myFeatureClass = myFcNamePair.Value;
                        if (myFeatureClass != null)
                        {
                            if (myFeatureClass.LockType == gviLockType.gviLockExclusiveSchema)
                                myFeatureClass.LockType = gviLockType.gviLockSharedSchema;
                            Marshal.ReleaseComObject(myFeatureClass);
                            myFeatureClass = null;
                        }
                    }
                }
                dicFcName.Clear();

                if (sourceFds != null)
                {
                    Marshal.ReleaseComObject(sourceFds);
                    sourceFds = null;
                }
                if (sourceDs != null)
                {
                    Marshal.ReleaseComObject(sourceDs);
                    sourceDs = null;
                }
                if (targetFds != null)
                {
                    Marshal.ReleaseComObject(targetFds);
                    targetFds = null;
                }
                if (targetDs != null)
                {
                    Marshal.ReleaseComObject(targetDs);
                    targetDs = null;
                }

                if (!DoWorkEvent.Cancel)
                {
                    BgWorker.ReportProgress(100, new object[] { true, "已完成处理！" });
                }
                if (DoWorkEvent.Cancel)
                    Logger.WriteMsg(LogLevel.Message, "Toolkit Msg:Operation Canceled!", DateTime.Now);
                else
                    Logger.WriteMsg(LogLevel.Message, "Toolkit Msg:Succeeded!", DateTime.Now);
                Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                return true;
                #endregion
            }
            catch (COMException e)
            {
                if (sourceCursor != null)
                {
                    Marshal.ReleaseComObject(sourceCursor);
                    sourceCursor = null;
                }
                if (dicFcName != null && dicFcName.Count > 0)
                {
                    foreach (KeyValuePair<string, IFeatureClass> myFcNamePair in dicFcName)
                    {
                        IFeatureClass myFeatureClass = myFcNamePair.Value;
                        if (myFeatureClass != null)
                        {
                            if (myFeatureClass.LockType == gviLockType.gviLockExclusiveSchema)
                                myFeatureClass.LockType = gviLockType.gviLockSharedSchema;
                            Marshal.ReleaseComObject(myFeatureClass);
                            myFeatureClass = null;
                        }
                    }
                }
                if (sourceFds != null)
                {
                    Marshal.ReleaseComObject(sourceFds);
                    sourceFds = null;
                }
                if (sourceDs != null)
                {
                    Marshal.ReleaseComObject(sourceDs);
                    sourceDs = null;
                }
                if (targetDs != null && targetDs.IsEditing)
                    targetDs.StopEditing(false);
                if (targetFc != null && targetFc.LockType == gviLockType.gviLockExclusiveSchema)
                    targetFc.LockType = gviLockType.gviLockSharedSchema;
                if (targetCursor != null)
                {
                    Marshal.ReleaseComObject(targetCursor);
                    targetCursor = null;
                }
                if (targetFc != null)
                {
                    Marshal.ReleaseComObject(targetFc);
                    targetFc = null;
                }
                if (targetFds != null)
                {
                    Marshal.ReleaseComObject(targetFds);
                    targetFds = null;
                }
                if (targetDs != null)
                {
                    Marshal.ReleaseComObject(targetDs);
                    targetDs = null;
                }

                toolTip = e.Message;
                toolTip = CityMakerBuilder.WorkSpace.Logger.GetErrorMessage(e);
                if (e.ErrorCode == -2147218890)
                    toolTip = "请检查数据是否存在非空字段而在导入的数据中出现非空字段未赋值的现象，或存在唯一值约束字段而在导入的数据中出现重复现象！";
                obj = new object[] { false, toolTip };
                BgWorker.ReportProgress(0, obj);

                Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Exception Info:" + toolTip, DateTime.Now);
                Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Failed!", DateTime.Now);
                Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                return false;
            }
            catch (Exception ex)
            {
                if (sourceCursor != null)
                {
                    Marshal.ReleaseComObject(sourceCursor);
                    sourceCursor = null;
                }
                if (dicFcName != null && dicFcName.Count > 0)
                {
                    foreach (KeyValuePair<string, IFeatureClass> myFcNamePair in dicFcName)
                    {
                        IFeatureClass myFeatureClass = myFcNamePair.Value;
                        if (myFeatureClass != null)
                        {
                            if (myFeatureClass.LockType == gviLockType.gviLockExclusiveSchema)
                                myFeatureClass.LockType = gviLockType.gviLockSharedSchema;
                            Marshal.ReleaseComObject(myFeatureClass);
                            myFeatureClass = null;
                        }
                    }
                }
                if (sourceFds != null)
                {
                    Marshal.ReleaseComObject(sourceFds);
                    sourceFds = null;
                }
                if (sourceDs != null)
                {
                    Marshal.ReleaseComObject(sourceDs);
                    sourceDs = null;
                }
                if (targetDs != null && targetDs.IsEditing)
                    targetDs.StopEditing(false);
                if (targetFc != null && targetFc.LockType == gviLockType.gviLockExclusiveSchema)
                    targetFc.LockType = gviLockType.gviLockSharedSchema;
                if (targetCursor != null)
                {
                    Marshal.ReleaseComObject(targetCursor);
                    targetCursor = null;
                }
                if (targetFc != null)
                {
                    Marshal.ReleaseComObject(targetFc);
                    targetFc = null;
                }
                if (targetFds != null)
                {
                    Marshal.ReleaseComObject(targetFds);
                    targetFds = null;
                }
                if (targetDs != null)
                {
                    Marshal.ReleaseComObject(targetDs);
                    targetDs = null;
                }

                toolTip = ex.Message;
                obj = new object[] { false, toolTip };
                BgWorker.ReportProgress(0, obj);

                Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Exception Info:" + toolTip, DateTime.Now);
                Logger.WriteMsg(LogLevel.Error, "Toolkit Msg:Failed!", DateTime.Now);
                Logger.WriteMsg(LogLevel.Message, "****************End CutModelPoint Toolkit****************", DateTime.Now);
                return false;
            }
        }

        /// <summary>
        /// 进度前进一步
        /// </summary>
        /// <param name="iTotalCount">总记录数</param>
        /// <param name="iCurCount">当前记录数</param>
        /// <param name="bFirstProgress">是否为第一个进度条的信息</param>
        protected void ProgressStepOne(int iTotalCount, int iCurCount, bool bFirstProgress, string sMessageInfo)
        {
            int percent = iCurCount * 100 / iTotalCount;
            string toolTip = string.Format(sMessageInfo, iCurCount, iTotalCount);
            object[] obj = new object[] { bFirstProgress, toolTip };
            BgWorker.ReportProgress(percent, obj);
        }

        /// <summary>
        /// 合并模型与贴图
        /// </summary>
        /// <param name="modelName">模型名称</param>
        /// <param name="usedModels">已经处理的模型</param>
        /// <param name="usedImages">已经处理的贴图</param>
        /// <param name="sourceResource">输入数据集资源</param>
        /// <param name="targetResource">目标数据集资源</param>
        protected void MergeModelAndImage(string modelName, ref List<string> usedModels,
            ref List<string> usedImages, IResourceManager sourceResource, IResourceManager targetResource,
            ref List<string> targetModels, ref List<string> targetImages)
        {
            if (!usedModels.Contains(modelName))
            {
                usedModels.Add(modelName);
                if (targetModels.Contains(modelName))
                    return;
                List<string> modelImageNames = new List<string>();

                //将精模和简模的贴图名称放入贴图集合
                IModel fineModel = sourceResource.GetModel(modelName);
                if (fineModel != null)
                {
                    string[] fineImageNames = fineModel.GetImageNames();
                    if (fineImageNames != null)
                    {
                        modelImageNames.AddRange(fineImageNames);
                    }
                }
                IModel simpleModel = sourceResource.GetSimplifiedModel(modelName);
                if (simpleModel != null)
                {
                    string[] simpleImageNames = simpleModel.GetImageNames();
                    if (simpleImageNames != null)
                    {
                        modelImageNames.AddRange(simpleImageNames);
                    }
                }
                //遍历模型所有贴图并导入
                foreach (string imageName in modelImageNames)
                {
                    if (!usedImages.Contains(imageName))
                    {
                        usedImages.Add(imageName);
                        if (targetImages.Contains(imageName))
                            continue;
                        IImage image = sourceResource.GetImage(imageName);
                        if (image != null)
                        {
                            if (targetImages.Contains(imageName))
                            {
                                targetResource.DeleteImage(imageName);
                                targetResource.AddImage(imageName, image);
                            }
                            else
                            {
                                targetImages.Add(imageName);
                                targetResource.AddImage(imageName, image);
                            }
                        }
                    }
                }
                //导入模型
                if (fineModel != null || simpleModel != null)
                {
                    if (targetModels.Contains(modelName))
                    {
                        targetResource.DeleteModel(modelName);
                        targetResource.AddModel(modelName, fineModel, simpleModel);
                    }
                    else
                    {
                        targetModels.Add(modelName);
                        targetResource.AddModel(modelName, fineModel, simpleModel);
                    }
                }
            }
        }

    }
}
