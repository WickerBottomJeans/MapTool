    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    namespace MapTool
    {
        public partial class XOR : Form
        {
            private List<string> _layers = new List<string>();
            public XOR()
            {
                InitializeComponent();
            }
            
            private MapLayerManager _layerManager;
            public XOR( MapLayerManager mapLayerManager)
            {
                InitializeComponent();

                this._layerManager = mapLayerManager;

                _layers = mapLayerManager.GetAllLayerNames().ToList();
                PopulateLayerLists();

                clb_xorTarget.ItemCheck += clb_xorTarget_ItemCheck;
                clb_xorSource.ItemCheck += clb_xorSource_ItemCheck;
            }

            private void PopulateLayerLists()
            {
                foreach (string layerName in _layers)
                {
                    clb_xorTarget.Items.Add(layerName);
                    clb_xorSource.Items.Add(layerName);
                }
            }

            private void clb_xorTarget_ItemCheck(object sender, ItemCheckEventArgs e)
            {
                CheckedListBox currentCLB = (CheckedListBox)sender;

                if (e.NewValue == CheckState.Checked)
                {
                    for (int i = 0; i < currentCLB.Items.Count; i++)
                    {
                        if (i != e.Index)
                        {
                            currentCLB.SetItemChecked(i, false);
                        }
                    }
                }
            }

            private void clb_xorSource_ItemCheck(object sender, ItemCheckEventArgs e)
            {
                CheckedListBox currentCLB = (CheckedListBox)sender;

                if (e.NewValue == CheckState.Checked)
                {
                    for (int i = 0; i < currentCLB.Items.Count; i++)
                    {
                        if (i != e.Index)
                        {
                            currentCLB.SetItemChecked(i, false);
                        }
                    }
                }
            }

        private void xorBtn1_Click(object sender, EventArgs e)
        {
            if (clb_xorTarget.CheckedItems.Count != 1) return;
            if (clb_xorSource.CheckedItems.Count != 1) return;

            string target = clb_xorTarget.CheckedItems[0].ToString();
            string source = clb_xorSource.CheckedItems[0].ToString();

            if (target == source) return;

            var targetLayer = _layerManager.GetLayer(target);
            var sourcelayer = _layerManager.GetLayer(source);
            if (targetLayer == null || sourcelayer == null) return;

            targetLayer.XOR_1(sourcelayer);
        }
    }
    }
