using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Signals
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public TabControl TabControl
        {
            get { return tcDocuments; }
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.Instance.NewDocumentWithView();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.Instance.OpenDocumentWithView();
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.Instance.CloseActiveView();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.Instance.SaveActiveDocument();
        }

        private void tcDocuments_SelectedIndexChanged(object sender, EventArgs e)
        {
            App.Instance.UpdateActiveView();
        }

        private void NewViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.Instance.CreateViewForActiveDocument();
        }

        private void LiveDataSourceModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Document activeDocument = App.Instance.ActiveDocument;
            if (activeDocument != null)
                activeDocument.SwitchLiveDataSource();
            
        }
    }
}
