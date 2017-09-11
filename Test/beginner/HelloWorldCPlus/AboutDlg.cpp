// AboutDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "HelloWorldCPlus.h"
#include "AboutDlg.h"


// AboutDlg 对话框

IMPLEMENT_DYNAMIC(CAboutDlg, CDialog)

CAboutDlg::CAboutDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CAboutDlg::IDD, pParent)
{
}

CAboutDlg::~CAboutDlg()
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// AboutDlg 消息处理程序

BOOL CAboutDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    IRuntimeInfoPtr  ptrRI;
    ptrRI.CreateInstance(CLSID_RuntimeInfo);
    CString strInstallDate = ptrRI->InstallDate;
    CString strinstallVersion = ptrRI->Version;
    if(strInstallDate.GetLength()<=0)
        strInstallDate = L"获取安装日期需安装CityMakerRuntime";
    if(strinstallVersion.GetLength()<=0)
        strinstallVersion = L"获取版本信息需安装CityMakerRuntime";
    ((CStatic*)GetDlgItem(IDC_InstallData))->SetWindowText(strInstallDate);
    ((CStatic*)GetDlgItem(IDC_SoftVersion))->SetWindowText(strinstallVersion);

    return TRUE;  
}
