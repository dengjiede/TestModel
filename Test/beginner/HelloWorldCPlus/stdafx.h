
// stdafx.h : ��׼ϵͳ�����ļ��İ����ļ���
// ���Ǿ���ʹ�õ��������ĵ�
// �ض�����Ŀ�İ����ļ�

#pragma once

#ifndef _SECURE_ATL
#define _SECURE_ATL 1
#endif

#ifndef VC_EXTRALEAN
#define VC_EXTRALEAN            // �� Windows ͷ���ų�����ʹ�õ�����
#endif

#include "targetver.h"

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS      // ĳЩ CString ���캯��������ʽ��

// �ر� MFC ��ĳЩ�����������ɷ��ĺ��Եľ�����Ϣ������
#define _AFX_ALL_WARNINGS

#include <afxwin.h>         // MFC ��������ͱ�׼���
#include <afxext.h>         // MFC ��չ

#include <afxdisp.h>        // MFC �Զ�����

#ifndef _AFX_NO_OLE_SUPPORT
#include <afxdtctl.h>           // MFC �� Internet Explorer 4 �����ؼ���֧��
#endif
#ifndef _AFX_NO_AFXCMN_SUPPORT
#include <afxcmn.h>             // MFC �� Windows �����ؼ���֧��
#endif // _AFX_NO_AFXCMN_SUPPORT

#include <afxcontrolbars.h>     // �������Ϳؼ����� MFC ֧��

#ifdef _UNICODE
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_IA64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='ia64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#endif


// SDKSample Code

#include <atlsafe.h>
#include <stdlib.h>
#include <afxdlgs.h>
#include <io.h>

//GcmCommon
#import "libid:B037CE21-208A-477e-A307-0BB72611F55C" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

//GcmMath
#import "libid:210A14BF-0F96-400A-BC6A-04572FE3E2BB" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

//GviSymbol
#import "libid:BB54FCF7-F6C7-4588-9EC6-95C098E522DD" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

//GcmFdeGeometry
#import "libid:DD663C14-6AFC-47d0-AECE-7FE1DB27FB98" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids rename("ISegment", "IFdeSegment")

//GcmFdeCore
#import "libid:52B162AE-72E4-47ec-9E4B-E25671B70B9B" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

//GcmFdeUndoRedo
#import "libid:40672E54-89C1-485c-BD76-EA16D8FD31D5" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

//GcmFdeDataInterop
#import "libid:A28E1802-F3CC-4c22-8DD1-C434E90FEAB2" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

//GcmRenderControl
#import "libid:2B31D54F-48C9-445b-8BDC-32A06BAC38FF" /*raw_interfaces_only raw_native_types*/ no_namespace named_guids

