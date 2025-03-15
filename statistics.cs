using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Senders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiseLib
{
    public partial class statistics : DevExpress.XtraEditors.XtraForm
    {
        public statistics()
        {
            InitializeComponent();
        }

        private void change_chart_view(object sender, EventArgs e)
        {
            if (sender is SimpleButton button)
            {
                if (button.Name.Contains("member"))
                {
                    if (button.Text.Contains("Daily"))
                    {}
                    else if (button.Text.Contains("Weekly"))
                    {}
                    else if (button.Text.Contains("Monthly"))
                    {}
                    else if (button.Text.Contains("All time"))
                    {}
                }
            }
        }

        private void statistics_Load(object sender, EventArgs e)
        {
        }
    }
}
