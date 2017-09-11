using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using CityMakerBuilder.AddIn.Core;

namespace CityMakerBuilder.AddIn.Example
{
    public partial class TerrainServerForm : DevExpress.XtraEditors.XtraForm
    {
        private string ip;
        private string port;
        private string datasource;
        private string dataset;
        private string tedPath;
        public TerrainServerForm()
        {
            InitializeComponent();
        }
        public string TedPath
        {
            get { return this.tedPath; }
        }
        void te_IP_TextChanged(object sender, System.EventArgs e)
        {
            ip = this.te_IP.Text;
        }

        void te_Port_TextChanged(object sender, System.EventArgs e)
        {
            port = this.te_Port.Text;
        }

        void te_DSrc_TextChanged(object sender, System.EventArgs e)
        {
            datasource = this.te_DSrc.Text;
        }

        void te_DSet_TextChanged(object sender, System.EventArgs e)
        {
            dataset = this.te_DSet.Text;
        }

        private void sbtn_OK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(datasource))
            {
                XtraMessageBox.Show(StringParser.Parse("${res:View_ParameterNotNull}"));
                te_DSrc.Focus();
                return;
            }
            if (string.IsNullOrEmpty(dataset))
            {
                XtraMessageBox.Show(StringParser.Parse("${res:View_ParameterNotNull}"));
                te_DSet.Focus();
                return;
            }
            if (string.IsNullOrEmpty(ip))
            {
                XtraMessageBox.Show(StringParser.Parse("${res:View_ParameterNotNull}"));
                te_IP.Focus();
                return;
            }
            if (string.IsNullOrEmpty(port))
            {
                XtraMessageBox.Show(StringParser.Parse("${res:View_ParameterNotNull}"));
                te_Port.Focus();
                return;
            }
            tedPath = string.Format("{0}:{1}@{2}:{3}", datasource, dataset, ip, port);
            this.DialogResult = DialogResult.OK;
        }

        private void sbtn_FromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "TED File(*.ted)|*.ted";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tedPath = dlg.FileName;
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}