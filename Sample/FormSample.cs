using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sample
{
    public partial class FormSample : Form
    {
        public FormSample()
        {
            InitializeComponent();
        }

        private void FormSample_Load(object sender, EventArgs e)
        {
            var node1 = tgvSample.Nodes.Add("Level 1");
            var node2_1 = node1.Nodes.Add("Level 2 - 1");
            var node3_1 = node2_1.Nodes.Add("Level 3 - 1");
        }
    }
}
