
// HelloWorldCPlus.cpp : ����Ӧ�ó��������Ϊ��
//

#include "stdafx.h"
#include "HelloWorldCPlus.h"
#include "HelloWorldCPlusDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CHelloWorldCPlusApp

BEGIN_MESSAGE_MAP(CHelloWorldCPlusApp, CWinAppEx)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp) 
END_MESSAGE_MAP()


// CHelloWorldCPlusApp ����

CHelloWorldCPlusApp::CHelloWorldCPlusApp()
{
	// TODO: �ڴ˴���ӹ�����룬
	// ��������Ҫ�ĳ�ʼ�������� InitInstance ��
}


// Ψһ��һ�� CHelloWorldCPlusApp ����

CHelloWorldCPlusApp theApp;


// CHelloWorldCPlusApp ��ʼ��

BOOL CHelloWorldCPlusApp::InitInstance()
{
    // SDKSample Code
    TCHAR szPath[MAX_PATH];
    GetModuleFileName(NULL, szPath, MAX_PATH);
    CString strExePath(szPath);
    if(strExePath.Find(L"Samples", 0) == -1)
    {
        AfxMessageBox(L"�벻Ҫ�������SDKĿ¼��", MB_ICONERROR, 0);
        return  FALSE;  // �˴�ֱ���˳�����ֹ��������ʾ
    }


	// ���һ�������� Windows XP �ϵ�Ӧ�ó����嵥ָ��Ҫ
	// ʹ�� ComCtl32.dll �汾 6 ����߰汾�����ÿ��ӻ���ʽ��
	//����Ҫ InitCommonControlsEx()�����򣬽��޷��������ڡ�
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// ��������Ϊ��������Ҫ��Ӧ�ó�����ʹ�õ�
	// �����ؼ��ࡣ
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinAppEx::InitInstance();

	AfxEnableControlContainer();

	// ��׼��ʼ��
	// ���δʹ����Щ���ܲ�ϣ����С
	// ���տ�ִ���ļ��Ĵ�С����Ӧ�Ƴ�����
	// ����Ҫ���ض���ʼ������
	// �������ڴ洢���õ�ע�����
	// TODO: Ӧ�ʵ��޸ĸ��ַ�����
	// �����޸�Ϊ��˾����֯��
	SetRegistryKey(_T("Gvitech SDK HelloWorldCPlusApp"));

	CHelloWorldCPlusDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: �ڴ˷��ô����ʱ��
		//  ��ȷ�������رնԻ���Ĵ���
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: �ڴ˷��ô����ʱ��
		//  ��ȡ�������رնԻ���Ĵ���
	}

	// ���ڶԻ����ѹرգ����Խ����� FALSE �Ա��˳�Ӧ�ó���
	//  ����������Ӧ�ó������Ϣ�á�
	return FALSE;
}
