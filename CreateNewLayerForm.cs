using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool {

    public partial class CreateNewLayerForm : Form {
        public string LayerName => tbName.Text;
        public int LayerWidth => int.Parse(tbWidth.Text);
        public int LayerHeight => int.Parse(tbHeight.Text);

        public CreateNewLayerForm() {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e) {
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
        }

        private void asaasd_Click(object sender, EventArgs e)
        {

        }
    }
}