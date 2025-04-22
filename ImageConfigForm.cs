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

    public partial class ImageConfigForm : Form {
        public int ImgWidth => int.Parse(txtWidth.Text);
        public int ImgHeight => int.Parse(txtHeight.Text);
        public int CoorX => int.Parse(txtCoorX.Text);
        public int CoorY => int.Parse(txtCoorY.Text);

        public bool HasPosition =>
    !string.IsNullOrWhiteSpace(txtCoorX.Text) &&
    !string.IsNullOrWhiteSpace(txtCoorY.Text);

        public int SuggestedWidth { get; set; }
        public int SuggestedHeight { get; set; }

        public ImageConfigForm() {
            InitializeComponent();
        }

        private void ImageConfigForm_Load(object sender, EventArgs e) {
            txtWidth.Text = SuggestedWidth.ToString();
            txtHeight.Text = SuggestedHeight.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
        }
    }
}