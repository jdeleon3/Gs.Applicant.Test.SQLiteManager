using Gs.Applicant.Test.SQLite.Manager.Presenter;
using Gs.Applicant.Test.SQLite.Manager.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gs.Applicant.Test.SQLite.Manager
{
    public partial class SQLiteManagerForm : Form, ISQLiteManagerView
    {
        private Presenter.SQLiteManagerPresenter presenter;
        public SQLiteManagerForm()
        {
            InitializeComponent();
            presenter = new SQLiteManagerPresenter(this, new SQLiteConnector());
            Reset();
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //TODO: Finish Adding table funcionality, then add this tab back in.
            tabControl1.TabPages.Remove(tabPage2);
            tabPage2.Dispose();
        }

        #region View Interface

        public DataTable CurrentTableRows
        {
            get;
            set;
        }

        public string DBConnectionString
        {
            get
            {
                return string.Format("Data Source={0}", DBName);
            }

        }

        public string DBName
        {
            get;
            set;
        }

        public IList<Column> NewTableColumns
        {
            get;
            set;
        }

        public string NewTableName
        {
            get
            {
                return this.tbNewTableName.Text;
            }

            set
            {
                this.tbNewTableName.Text = value;
            }
        }

        public int SelectedRow
        {
            get;
            set;
        }

        public string SelectedTableName
        {
            get;
            set;
        }

        public string[] tableList
        {
            get;
            set;
        }

        public void Reset()
        {
            this.CurrentTableRows = new DataTable();
            this.SelectedTableName = string.Empty;
            this.SelectedRow = -1;
            this.NewTableColumns = new List<Column>();
            this.DBName = string.Empty;
        }

        #endregion


        #region From Methods

        private void SQLiteManagerForm_Load(object sender, EventArgs e)
        {

        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab == tabPage1)
            {
                presenter.LoadDatabase();
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var results = openFileDialog1.ShowDialog();
            if (results == DialogResult.OK)
            {
                this.DBName = openFileDialog1.FileName;
                presenter.LoadDatabase();
            }

            BindTableList();
            BindNewTableColumns();
            BindCurrentTableRows();
        }
        private void cbTableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTableName = ((ComboBox)sender).SelectedValue.ToString();
            presenter.LoadTable();
            BindCurrentTableRows();
        }
        

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var results = saveFileDialog1.ShowDialog();
            if(results == DialogResult.OK)
            {
                this.DBName = saveFileDialog1.FileName;
                presenter.CreateAndLoadDatabase();
            }
            BindTableList();
            BindNewTableColumns();
            BindCurrentTableRows();
        }

        private void BindCurrentTableRows()
        {
            gvCreateTableColumns.DataSource = null;
            gvCreateTableColumns.DataSource = NewTableColumns;
        }

        private void BindNewTableColumns()
        {
            gvTableRows.DataSource = CurrentTableRows;
            
        }


        #endregion

        #region Helper Methods


        private void BindTableList()
        {
            cbTableList.DataSource = this.tableList;
        }


        #endregion

        
    }
}
