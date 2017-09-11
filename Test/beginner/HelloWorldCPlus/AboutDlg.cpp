// AboutDlg.cpp : ʵ���ļ�
//

#include "stdafx.h"
#include "HelloWorldCPlus.h"
#include "AboutDlg.h"


// AboutDlg �Ի���

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


// AboutDlg ��Ϣ�������

BOOL CAboutDlg::OnInitDialog()
{
    CDialog::OnInitDialog();

    IRuntimeInfoPtr  ptrRI;
    ptrRI.CreateInstance(CLSID_RuntimeInfo);
    CString strInstallDate = ptrRI->InstallDate;
    CString strinstallVersion = ptrRI->Version;
    if(strInstallDate.GetLength()<=0)
        strInstallDate = L"��ȡ��װ�����谲װCityMakerRuntime";
    if(strinstallVersion.GetLength()<=0)
        strinstallVersion = L"��ȡ�汾��Ϣ�谲װCityMakerRuntime";
    ((CStatic*)GetDlgItem(IDC_InstallData))->SetWindowText(strInstallDate);
    ((CStatic*)GetDlgItem(IDC_SoftVersion))->SetWindowText(strinstallVersion);

    return TRUE;  
}
