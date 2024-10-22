using System.Text;
using System.Windows.Forms;
using NetProxy;

namespace profiler_interface {

    public partial class Form1 : Form
    {
        private int _counter = 0;

        BindingSource bs = new BindingSource();

        public Form1()
        {
            Profile.StartProxy();

            Queries.GetInstance.AddQuery("test 1 long text");
            Queries.GetInstance.AddQuery("test 2 long text");

            bs.DataSource = Queries.GetInstance.GetQueriesList();

            InitializeComponent();

            QueriesList.DataSource = bs;
            QueriesList.DisplayMember = "ShortSqlText";
            QueriesList.ValueMember = "Id";
        }

        /// <summary>
        /// Действие при выборе строки из списка
        /// </summary>
        private void QueriesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (QueriesList.SelectedIndex >= 0)
            {
                var query = QueriesList.Items[QueriesList.SelectedIndex] as Query;
                QueryBody.Text = query != null ? query.SqlText : "";
            }
            else 
            {
                QueryBody.Text = "";   
            }
        }

        private void QueryBody_TextChanged(object sender, EventArgs e)
        {

        }

        private void ProfilingButton_Click(object sender, EventArgs e)
        {
            bs.ResetBindings(false);
        }
    }
}