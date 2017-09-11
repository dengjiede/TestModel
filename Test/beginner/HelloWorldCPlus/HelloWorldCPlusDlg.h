
using namespace std;


// HelloWorldCPlusDlg.h : 头文件
//

#pragma once


// CHelloWorldCPlusDlg 对话框
class CHelloWorldCPlusDlg : public CDialog
{
// 构造
public:
	CHelloWorldCPlusDlg(CWnd* pParent = NULL);	// 标准构造函数

// 对话框数据
	enum { IDD = IDD_HELLOWORLDCPLUS_DIALOG };


protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支持


// 实现
protected:
	HICON m_hIcon;

	// 生成的消息映射函数
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()


// SDKSample Code
private:
    CString m_strExePath;
    CString m_fdbName;
    _bstr_t m_fdbPath;
    IEnvelopePtr env;
    GUID rootId;

    IRenderControlPtr m_spRenderControl;

    ISpatialCRSPtr GetDefaultCRS(void);
    IFieldInfoCollectionPtr GetDefaultFldCtn(void);
    IImagePtr GetDefaultImage();
    IModelPtr GetDefaultModel();

public:
    IRenderControlPtr GetDCT3DWindowPtr();

    afx_msg void OnBnClickedButtonFullscreen();
    afx_msg void OnBnClickedCheckFog();
    afx_msg void OnCbnSelchangeComboWeather();
    DECLARE_EVENTSINK_MAP()
    void RcCameraUndoRedoStatusChangedRendercontrol();
    afx_msg void OnBnClickedButtonUndo();
    afx_msg void OnBnClickedButtonRedo();
    afx_msg void OnBnClickedButtonCapturescreen();
    void RcPictureExportEndRendercontrol(double Time, BOOL IsAborted);
    afx_msg void OnBnClickedButtonAbout();

    afx_msg void OnBnClickedButtonCreateFDB();
    afx_msg void OnBnClickedButtonCreateMemory();
    afx_msg void OnBnClickedButtonOpenFDB();
};
