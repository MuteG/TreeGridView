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
            var node1 = tgvSample.Nodes.Add("Node 1");
            var node1_1 = node1.Nodes.Add("Node 1 - 1");
            var node1_1_1 = node1_1.Nodes.Add("Node 1 - 1 - 1");
            var node1_1_2 = node1_1.Nodes.Add("Node 1 - 1 - 2");
            var node1_2 = node1.Nodes.Add("Node 1 - 2");
            var node2 = tgvSample.Nodes.Add("Node 2");
        }
    }
}
