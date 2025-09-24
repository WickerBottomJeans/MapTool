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
        public int LayerWidth
        {
            get
            {
                if (int.TryParse(tbWidth.Text, out int width))
                {
                    return width;
                }
                return 0;
            }
        }

        public int LayerHeight
        {
            get
            {
                if (int.TryParse(tbHeight.Text, out int height))    
                {
                    return height;
                }
                return 0;
            }
        }

        public CreateNewLayerForm() {
            InitializeComponent();
        }

        public string InitialLayerName
        {
            set { tbName.Text = value; }
        }

        private void label2_Click(object sender, EventArgs e) {
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
        }

        private void asaasd_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }
    }
}