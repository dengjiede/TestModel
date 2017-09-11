
using namespace std;


// HelloWorldCPlusDlg.h : ͷ�ļ�
//

#pragma once


// CHelloWorldCPlusDlg �Ի���
class CHelloWorldCPlusDlg : public CDialog
{
// ����
public:
	CHelloWorldCPlusDlg(CWnd* pParent = NULL);	// ��׼���캯��

// �Ի�������
	enum { IDD = IDD_HELLOWORLDCPLUS_DIALOG };


protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV ֧��


// ʵ��
protected:
	HICON m_hIcon;

	// ���ɵ���Ϣӳ�亯��
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
