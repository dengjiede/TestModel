
// HelloWorldCPlus.h : PROJECT_NAME Ӧ�ó������ͷ�ļ�
//

#pragma once

#ifndef __AFXWIN_H__
	#error "�ڰ������ļ�֮ǰ������stdafx.h�������� PCH �ļ�"
#endif

#include "resource.h"		// ������


// CHelloWorldCPlusApp:
// �йش����ʵ�֣������ HelloWorldCPlus.cpp
//

class CHelloWorldCPlusApp : public CWinAppEx
{
public:
	CHelloWorldCPlusApp();

// ��д
	public:
	virtual BOOL InitInstance();

// ʵ��

	DECLARE_MESSAGE_MAP()
};

extern CHelloWorldCPlusApp theApp;