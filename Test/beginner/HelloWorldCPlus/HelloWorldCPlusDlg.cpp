
// HelloWorldCPlusDlg.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "HelloWorldCPlus.h"
#include "HelloWorldCPlusDlg.h"
#include "AboutDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CHelloWorldCPlusDlg �Ի���

CHelloWorldCPlusDlg::CHelloWorldCPlusDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CHelloWorldCPlusDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CHelloWorldCPlusDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CHelloWorldCPlusDlg, CDialog)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
    ON_BN_CLICKED(IDC_BUTTON_FullScreen, &CHelloWorldCPlusDlg::OnBnClickedButtonFullscreen)
    ON_BN_CLICKED(IDC_CHECK_Fog, &CHelloWorldCPlusDlg::OnBnClickedCheckFog)
    ON_CBN_SELCHANGE(IDC_COMBO_Weather, &CHelloWorldCPlusDlg::OnCbnSelchangeComboWeather)
    ON_BN_CLICKED(IDC_BUTTON_Undo, &CHelloWorldCPlusDlg::OnBnClickedButtonUndo)
    ON_BN_CLICKED(IDC_BUTTON_Redo, &CHelloWorldCPlusDlg::OnBnClickedButtonRedo)
    ON_BN_CLICKED(IDC_BUTTON_CaptureScreen, &CHelloWorldCPlusDlg::OnBnClickedButtonCapturescreen)
    ON_BN_CLICKED(IDC_BUTTON_About, &CHelloWorldCPlusDlg::OnBnClickedButtonAbout)
    ON_BN_CLICKED(IDC_BUTTON_CreateFDB, &CHelloWorldCPlusDlg::OnBnClickedButtonCreateFDB)
    ON_BN_CLICKED(IDC_BUTTON_CreateMemory, &CHelloWorldCPlusDlg::OnBnClickedButtonCreateMemory)
    ON_BN_CLICKED(IDC_BUTTON_OpenFDB, &CHelloWorldCPlusDlg::OnBnClickedButtonOpenFDB)
END_MESSAGE_MAP()


// CHelloWorldCPlusDlg ��Ϣ�������

BOOL CHelloWorldCPlusDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// ���ô˶Ի����ͼ�ꡣ��Ӧ�ó��������ڲ��ǶԻ���ʱ����ܽ��Զ�
	//  ִ�д˲���
	SetIcon(m_hIcon, TRUE);			// ���ô�ͼ��
	SetIcon(m_hIcon, FALSE);		// ����Сͼ��

	// SDKSample Code
    TCHAR szPath[MAX_PATH];
    GetModuleFileName(NULL, szPath, MAX_PATH);
    m_strExePath = szPath;
    m_strExePath = m_strExePath.Left(m_strExePath.Find(L"Samples", 0));
    m_fdbName = L"SDKDEMO";

    _bstr_t strSkyboxPath = m_strExePath + "Samples/Media/skybox/";
    GetDCT3DWindowPtr()->Initialize(true,NULL);

    rootId = GetDCT3DWindowPtr()->ObjectManager->GetProjectTree()->GetRootID();

    GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0)->SetImagePath(gviSkyboxImageBack, strSkyboxPath + L"1_BK.jpg");
    GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0)->SetImagePath(gviSkyboxImageBottom, strSkyboxPath + L"1_DN.jpg");
    GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0)->SetImagePath(gviSkyboxImageFront, strSkyboxPath + L"1_FR.jpg");
    GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0)->SetImagePath(gviSkyboxImageLeft, strSkyboxPath + L"1_LF.jpg");
    GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0)->SetImagePath(gviSkyboxImageRight, strSkyboxPath + L"1_RT.jpg");
    GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0)->SetImagePath(gviSkyboxImageTop, strSkyboxPath + L"1_UP.jpg");

    GetDCT3DWindowPtr()->Camera->put_FlyTime(1.0);

    // ��������Ĭ��ѡ�����족
    ((CComboBox*)GetDlgItem(IDC_COMBO_Weather))->SetCurSel(0);

	return TRUE;  // ���ǽ��������õ��ؼ������򷵻� TRUE
}

// �����Ի��������С����ť������Ҫ����Ĵ���
//  �����Ƹ�ͼ�ꡣ����ʹ���ĵ�/��ͼģ�͵� MFC Ӧ�ó���
//  �⽫�ɿ���Զ���ɡ�

void CHelloWorldCPlusDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// ʹͼ���ڹ����������о���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// ����ͼ��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

//���û��϶���С������ʱϵͳ���ô˺���ȡ�ù��
//��ʾ��
HCURSOR CHelloWorldCPlusDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


//////////////////////////////////////////////// SDKSample Code
IRenderControlPtr CHelloWorldCPlusDlg::GetDCT3DWindowPtr()
{
    if(!m_spRenderControl)
    {
        LPUNKNOWN pUnk = GetDlgItem(IDC_RENDERCONTROL)->GetControlUnknown();
        if(pUnk)
            pUnk->QueryInterface(IID_IRenderControl,(void**)&m_spRenderControl);
    }

    return m_spRenderControl;
}


BEGIN_EVENTSINK_MAP(CHelloWorldCPlusDlg, CDialog)
    ON_EVENT(CHelloWorldCPlusDlg, IDC_RENDERCONTROL, 70, CHelloWorldCPlusDlg::RcCameraUndoRedoStatusChangedRendercontrol, VTS_NONE)
    ON_EVENT(CHelloWorldCPlusDlg, IDC_RENDERCONTROL, 24, CHelloWorldCPlusDlg::RcPictureExportEndRendercontrol, VTS_R8 VTS_BOOL)
END_EVENTSINK_MAP()


void CHelloWorldCPlusDlg::OnBnClickedButtonOpenFDB()
{
    IConnectionInfoPtr ci;
    HRESULT hr=ci.CreateInstance(CLSID_ConnectionInfo);
    try
    {
        if(SUCCEEDED(hr))
        {
            //������Դ
            ci->ConnectionType = gviConnectionFireBird2x;
            m_fdbPath = m_strExePath + L"Samples/Media/" + m_fdbName + L".FDB";
            ci->Database = m_fdbPath;

            IDataSourceFactoryPtr dsFactory ;
            hr=dsFactory.CreateInstance(CLSID_DataSourceFactory);
            IDataSourcePtr ds = dsFactory->OpenDataSource(ci);

            //��ȡ���ݼ�
            SAFEARRAY* setnames = ds->GetFeatureDatasetNames();
            CComSafeArray<BSTR> sa;
            sa.Attach(setnames);
            int count =sa.GetCount();

            #pragma region �������ݼ�
            for(int i=0;i<count;i++)
            {
                IFeatureDataSetPtr dataset = ds->OpenFeatureDataset(_bstr_t(sa.GetAt(i)));
                //��ȡҪ����
                SAFEARRAY* fcnames = dataset->GetNamesByType(gviDataSetFeatureClassTable);
                CComSafeArray<BSTR> fcs;
                fcs.Attach(fcnames);
                int fccount =fcs.GetCount();

                #pragma region ����Ҫ����
                for(int n=0;n<fccount;n++)
                {
                    IFeatureClassPtr fc = dataset->OpenFeatureClass(_bstr_t(fcs.GetAt(n)));
                    //����ʸ��ͼ��
                    IFeatureLayerPtr featureLayer = m_spRenderControl->ObjectManager->CreateFeatureLayer(
                        fc, L"Geometry", NULL, NULL, rootId);
                    //��ȡ�ռ��������
                    IFieldInfoCollectionPtr fieldinfos = fc->GetFields();
                    IFieldInfoPtr fieldinfo = fieldinfos->Get(fieldinfos->IndexOf(L"Geometry"));
                    IGeometryDefPtr geometryDef = fieldinfo->GeometryDef;
                    env = geometryDef->Envelope;
                    if (env == NULL || (env->MaxX == 0 && env->MaxY == 0 && env->MaxZ == 0 &&
                        env->MinX == 0 && env->MinY == 0 && env->MinZ == 0))
                        continue;
                    //�����λ
                    IEulerAnglePtr angle ;
                    angle.CreateInstance(CLSID_EulerAngle);
                    angle->Set(0, -20, 0);
                    m_spRenderControl->Camera->LookAt(env->Center, 1000, angle);
                }
                #pragma endregion ����Ҫ����                
            }
            #pragma endregion �������ݼ�          
        }
    }
    catch (_com_error& e)
    {
    }
}
void CHelloWorldCPlusDlg::OnBnClickedButtonFullscreen()
{
    GetDCT3DWindowPtr()->FullScreen = !GetDCT3DWindowPtr()->FullScreen;
}

void CHelloWorldCPlusDlg::OnBnClickedCheckFog()
{
    int fogCheck = ((CButton*)GetDlgItem(IDC_CHECK_Fog))->GetCheck();
    ISkyBoxPtr skybox = GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0);
    if(fogCheck == 1)
    {
        skybox->FogStartDistance = 0;
        skybox->FogEndDistance = 500;
        skybox->FogMode = gviFogLinear;
        IEulerAnglePtr angle;
        angle.CreateInstance(CLSID_EulerAngle);
        angle->Set(0, -20, 0);
        m_spRenderControl->Camera->LookAt(env->Center, 500, angle);
    }
    else if(fogCheck == 0)
    {
        skybox->FogMode = gviFogNone;
    }
}

void CHelloWorldCPlusDlg::OnCbnSelchangeComboWeather()
{
    int index = ((CComboBox*)GetDlgItem(IDC_COMBO_Weather))->GetCurSel();
    ISkyBoxPtr skybox = GetDCT3DWindowPtr()->ObjectManager->GetSkyBox(0);
    switch(index)
    {
    case 0:
        skybox->Weather = gviWeatherSunShine;
        break;
    case 1:
        skybox->Weather = gviWeatherLightRain;
        break;
    case 2:
        skybox->Weather = gviWeatherModerateRain;
        break;
    case 3:
        skybox->Weather = gviWeatherHeavyRain;
        break;
    case 4:
        skybox->Weather = gviWeatherLightSnow;
        break;
    case 5:
        skybox->Weather = gviWeatherModerateSnow;
        break;
    case 6:
        skybox->Weather = gviWeatherHeavySnow;
        break;
    }
}

void CHelloWorldCPlusDlg::RcCameraUndoRedoStatusChangedRendercontrol()
{
    if(GetDCT3DWindowPtr()->Camera->CanRedo)
        ((CButton*)GetDlgItem(IDC_BUTTON_Redo))->EnableWindow(true);
    else
        ((CButton*)GetDlgItem(IDC_BUTTON_Redo))->EnableWindow(false);
    if(GetDCT3DWindowPtr()->Camera->CanUndo)
        ((CButton*)GetDlgItem(IDC_BUTTON_Undo))->EnableWindow(true);
    else
        ((CButton*)GetDlgItem(IDC_BUTTON_Undo))->EnableWindow(false);
}

void CHelloWorldCPlusDlg::OnBnClickedButtonUndo()
{
    GetDCT3DWindowPtr()->Camera->Undo();
}

void CHelloWorldCPlusDlg::OnBnClickedButtonRedo()
{
    GetDCT3DWindowPtr()->Camera->Redo();
}

void CHelloWorldCPlusDlg::OnBnClickedButtonCapturescreen()
{
    CString strFilePath;
    CString strFilter = L"Image Files(*.BMP)|*.BMP|Image Files(*.PNG)|*.PNG|Image Files(*.JPG)|*.JPG||";
    CFileDialog dlg(false, L".bmp", NULL, OFN_OVERWRITEPROMPT, strFilter);
    if(dlg.DoModal() == IDOK)
    {
        strFilePath = dlg.GetPathName();
        VARIANT_BOOL b = GetDCT3DWindowPtr()->ExportManager->ExportImage((_bstr_t)strFilePath, 1024, 1024, VARIANT_FALSE);
        if(!b)
        {
            wchar_t a[10];  //LPCWSTR 
            _itow_s(GetDCT3DWindowPtr()->GetLastError(), a, 10, 10);
            ::MessageBox(m_hWnd, a, L"��ͼ����ʱ��ʾ", MB_ICONERROR);
        }
    }
}

void CHelloWorldCPlusDlg::RcPictureExportEndRendercontrol(double Time, BOOL IsAborted)
{
    ::MessageBox(m_hWnd, L"Congratulation! Done Exported!", L"��ͼ���ʱ��ʾ", MB_ICONINFORMATION);
}

void CHelloWorldCPlusDlg::OnBnClickedButtonAbout()
{
    CAboutDlg dlg;
    dlg.DoModal();
}

void CHelloWorldCPlusDlg::OnBnClickedButtonCreateFDB()
{
    IConnectionInfoPtr ci;
    HRESULT hr = ci.CreateInstance(CLSID_ConnectionInfo);
    try
    {
        if(SUCCEEDED(hr))
        {
            //������Դ
            ci->ConnectionType = gviConnectionFireBird2x;
            m_fdbName = L"TESTCreateFDB";
            m_fdbPath = m_strExePath + L"Samples/Media/" + m_fdbName + L".FDB";
            ci->Database = m_fdbPath;

            IDataSourceFactoryPtr dsFactory ;
            hr = dsFactory.CreateInstance(CLSID_DataSourceFactory);
            IDataSourcePtr ds = NULL;
            if (!dsFactory->HasDataSource(ci))
            {
                ds = dsFactory->CreateDataSource(ci, _T(""));
            }
            else
            {
                ds = dsFactory->OpenDataSource(ci);
            }

            _bstr_t dsName = _T("testfds");
            IFeatureDataSetPtr dataset;
            ISpatialCRSPtr pCrs = GetDefaultCRS();
            BSTR fixedName = NULL;
            if (ds->ValidateName(gviNameFeatureDataSet, dsName, &fixedName))
            {
                dataset = ds->CreateFeatureDataset(dsName, pCrs);
            }
            else
            {
                dataset = ds->OpenFeatureDataset(dsName);
            }

            IResourceManager* rcmng = NULL;
            dataset->QueryInterface(IID_IResourceManager, (void**)&rcmng);
            if(rcmng)
            {
                IModelPtr model = GetDefaultModel();
                IImagePtr img = GetDefaultImage();
                if(rcmng->CheckResourceName(L"TestModel"))
                {
                    rcmng->AddImage(L"TestImage", img);
                    HRESULT hr = rcmng->AddModel(L"TestModel", model, NULL);
                }

                IFieldInfoCollectionPtr fic = GetDefaultFldCtn();
                IFeatureClassPtr fc = dataset->CreateFeatureClass(L"TestFeatureClass", fic);
                if (fc)
                {
                    IGridIndexInfoPtr index;
                    index.CreateInstance(CLSID_GridIndexInfo);

                    //index.Name���ܴ�. ���Ȳ��ܳ���160
                    index->GeoColumnName= "TestGridIndex"; 
                    index->L1 = 500;
                    index->L2 = 2000;
                    index->L3 = 10000;
                    index->GeoColumnName= "Geometry";
                    fc->AddSpatialIndex(index);

                    IRenderIndexInfoPtr renderInx;
                    renderInx.CreateInstance(CLSID_RenderIndexInfo);

                    renderInx->GeoColumnName= "Geometry"; //����ָ��GeoColumnName������ᱨ��_FDE_INVALID_PARAMETER����
                    renderInx->L1 = 500;
                    fc->AddRenderIndex(renderInx);
                    fc->LockType= gviLockSharedSchema;

                    IFdeCursorPtr cursor = fc->Insert();
                    if (cursor)
                    {
                        IGeometryFactoryPtr gFac ;
                        gFac.CreateInstance(CLSID_GeometryFactory);
                        IGeometryPtr geom = gFac->CreateGeometry(gviGeometryModelPoint, gviVertexAttributeZ);

                        IModelPointPtr mdlPoint;
                        geom.QueryInterface(IID_IModelPoint, &mdlPoint);

                        mdlPoint->ModelName = L"TestModel";
                        mdlPoint->ModelEnvelope = model->Envelope->Clone();
                        mdlPoint->X = 200;
                        mdlPoint->Y = 200;
                        mdlPoint->Z = 200;

                        IRowBufferPtr rb = fc->CreateRowBuffer();

                        rb->SetValue(1, _T("name1"));
                        rb->SetValue(2, 1);
                        rb->SetValue(3, mdlPoint.GetInterfacePtr());

                        cursor->InsertRow(rb);
                    }
                }
            }
        }
    }
    catch (_com_error& e)
    {

    }
}

void CHelloWorldCPlusDlg::OnBnClickedButtonCreateMemory()
{
    IImagePtr image = GetDefaultImage();
    image->ConvertFormat(gviImageDDS); //703�ڴ���ֻ֧��dds��ʽ
    GetDCT3DWindowPtr()->ObjectManager->AddImage(L"TestImage", image);	

    IModelPtr model = GetDefaultModel();
    GetDCT3DWindowPtr()->ObjectManager->AddModel(L"TestModel", model);				

    IGeometryFactoryPtr gFac ;
    gFac.CreateInstance(CLSID_GeometryFactory);
    IGeometryPtr geom = gFac->CreateGeometry(gviGeometryModelPoint, gviVertexAttributeZ);

    IModelPointPtr mdlPoint;
    geom.QueryInterface(IID_IModelPoint, &mdlPoint);

    mdlPoint->ModelName = L"TestModel";
    mdlPoint->ModelEnvelope = model->Envelope->Clone();
    mdlPoint->X = 200;
    mdlPoint->Y = 200;
    mdlPoint->Z = 200;

    IRenderModelPointPtr rmd = GetDCT3DWindowPtr()->ObjectManager->CreateRenderModelPoint(mdlPoint, NULL, rootId);
    GetDCT3DWindowPtr()->Camera->FlyToObject(rmd->GetGuid(), gviActionFlyTo);

}

ISpatialCRSPtr CHelloWorldCPlusDlg::GetDefaultCRS(void)
{
	ICRSFactoryPtr crsFac;
	crsFac.CreateInstance(CLSID_CRSFactory);

	//BSTR wktStr = L"GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";

	ICoordinateReferenceSystemPtr coorSys = crsFac->CreateCRS (gviCrsUnknown);

	ISpatialCRSPtr scr;
	coorSys.QueryInterface(IID_ISpatialCRS, &scr);

	return scr;
}

IFieldInfoCollectionPtr CHelloWorldCPlusDlg::GetDefaultFldCtn(void)
{
	IFieldInfoCollectionPtr ficoll;
	ficoll.CreateInstance(CLSID_FieldInfoCollection);

	IFieldInfoPtr field;
	field.CreateInstance(CLSID_FieldInfo);

	field->Name= "Name";
	field->FieldType= gviFieldString;
	field->Length= 255;
	ficoll->Add(field);

	IFieldInfoPtr field2;
	field2.CreateInstance(CLSID_FieldInfo);
	field2->Name= "Groupid";
	field2->FieldType= gviFieldInt32;
	//ע����Ⱦ����
	field2->RegisteredRenderIndex= true; 
	ficoll->Add(field2);

	IFieldInfoPtr field3;
	field3.CreateInstance(CLSID_FieldInfo);

	IGeometryDefPtr gd;
	gd.CreateInstance(CLSID_GeometryDef);
	gd->GeometryColumnType= gviGeometryColumnModelPoint; 

	field3->Name= "Geometry";
	field3->FieldType= gviFieldGeometry;
	field3->RegisteredRenderIndex= true; 
	field3->GeometryDef = gd;//ע����Ⱦ�����������

	ficoll->Add(field3);

	return ficoll;
}

IImagePtr CHelloWorldCPlusDlg::GetDefaultImage(void)
{
	IResourceFactoryPtr factory;
	factory.CreateInstance(CLSID_ResourceFactory);
    _bstr_t strImgPath = m_strExePath + L"Samples/Media/skybox/1_BK.jpg";

	return factory->CreateImageFromFile(strImgPath);
}

IModelPtr CHelloWorldCPlusDlg::GetDefaultModel(void)
{
	IFloatArrayPtr floatArr;
	floatArr.CreateInstance(CLSID_FloatArray);

	floatArr->Append(0.0);
	floatArr->Append(0.0);
	floatArr->Append(0.0);

	floatArr->Append(100.0);
	floatArr->Append(0.0);
	floatArr->Append(0.0);

	floatArr->Append(100.0);
	floatArr->Append(100.0);
	floatArr->Append(0.0);

    floatArr->Append(100.0);
    floatArr->Append(100.0);
    floatArr->Append(0.0);

	floatArr->Append(0.0);
	floatArr->Append(100.0);
	floatArr->Append(0.0);

	floatArr->Append(0.0);
	floatArr->Append(0.0);
	floatArr->Append(0.0);

	IDrawPrimitivePtr primitive;
	primitive.CreateInstance(CLSID_DrawPrimitive);
	primitive->VertexArray = floatArr;

    IDrawMaterialPtr material;
    material.CreateInstance(CLSID_DrawMaterial);
    material->TextureName = L"TestImage";
    material->CullMode = gviCullNone;
    material->WrapModeS = gviTextureWrapRepeat;
    material->WrapModeT = gviTextureWrapRepeat;

    IFloatArrayPtr taArr;
    taArr.CreateInstance(CLSID_FloatArray);
    taArr->Append(0);
    taArr->Append(0);

    taArr->Append(1);
    taArr->Append(0);

    taArr->Append(1);
    taArr->Append(1);

    taArr->Append(1);
    taArr->Append(1);

    taArr->Append(0);
    taArr->Append(1);

    taArr->Append(0);
    taArr->Append(0);
    primitive->TexcoordArray = taArr;
    primitive->Material = material;

	IDrawGroupPtr dgroup;
	dgroup.CreateInstance(CLSID_DrawGroup);
	dgroup->AddPrimitive(primitive);

	IResourceFactoryPtr factory;
	factory.CreateInstance(CLSID_ResourceFactory);
	IModelPtr mdl = factory->CreateModel();
	mdl->AddGroup(dgroup);

	return mdl;
}

