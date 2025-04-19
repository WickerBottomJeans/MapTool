using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool {

    public enum ToolMode { None, Brush, Eraser, Fill, Line, Rect };

    public partial class mainForm : Form {
        private bool TestGit;
        private MapLayerManager mapLayerManager;
        private MapRenderer mapRenderer;
        private MapEditor mapEditor;
        private CoorConverter coorConverter;
        private MapContext sharedContext;

        //UI configure
        private int defaultBrushSize = 5;

        //For panning
        private Point panningStartPoint;

        private bool isPanning = false;

        //For previewing. i tried ==ToolMode but i didnt work out well
        private bool isDrawing = false;

        private bool isDrawingLine = false;
        private bool isDrawingRect = false;
        private bool isErasing = false;

        public mainForm() {
            InitializeComponent();
            typeof(Panel).InvokeMember("DoubleBuffered",
     System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
     null, panelMap, new object[] { true });

            sharedContext = new MapContext {
                CellSize = 1,
                PanOffset = new Point(0, 0),
                BrushSize = 1,
                CurrentTool = ToolMode.None,
                PanelBackColor = Color.Black,
                maxZoomInValue = 5
            };

            mapLayerManager = new MapLayerManager();
            mapLayerManager.LayersDictChanged += MapLayerManager_LayersDictChanged;

            mapRenderer = new MapRenderer(sharedContext);
            mapRenderer.MapDrawingFinished += MapRenderer_MapDrawingFinished;
            mapRenderer.Context = sharedContext;

            mapEditor = new MapEditor(sharedContext);
            mapEditor.Context = sharedContext;

            coorConverter = new CoorConverter(sharedContext);
            coorConverter.Context = sharedContext;

            UISetUp();
        }

        private void MapRenderer_MapDrawingFinished(object sender, EventArgs e) {
            panelMap.Invalidate();
        }

        private void MapLayerManager_LayersDictChanged(object sender, EventArgs e) {
            UpdateClbLayers();
            UpdatePanelMap();
        }

        private void btnLoadFile_Click(object sender, EventArgs e) {
            List<MapLayer> mapLayerToAddToMapManager = new List<MapLayer>();
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                foreach (string filePath in openFileDialog1.FileNames) {
                    byte[] buffer = File.ReadAllBytes(filePath);
                    int mapHeight = (int)Math.Sqrt(buffer.Length);
                    int mapWidth = (int)Math.Sqrt(buffer.Length);

                    MapLayer mapLayer = new MapLayer(Path.GetFileNameWithoutExtension(filePath), mapHeight, mapWidth);
                    mapLayer.Data.FromBytes(buffer);
                    mapLayer.Rotate(90);
                    mapLayerToAddToMapManager.Add(mapLayer);
                }
                mapLayerManager.AddLayers(mapLayerToAddToMapManager);
            }
        }

        private void UpdateClbLayers() {
            clbLayers.Items.Clear();
            foreach (var mapLayer in mapLayerManager.GetAllLayers()) {
                clbLayers.Items.Add(mapLayer.Name, true);
            }
        }

        private List<MapLayer> GetVisibleLayers() {
            var visibleLayers = new List<MapLayer>();
            foreach (var item in clbLayers.CheckedItems) {
                string name = item.ToString();
                var layer = mapLayerManager.GetLayer(name);
                visibleLayers.Add(layer);
            }
            return visibleLayers;
        }

        private void UpdatePanelMap() {
            var layerToDraw = GetVisibleLayers();
            mapRenderer.RenderLayers(layerToDraw);
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e) {
            List<string> layersToRemove = new List<string>();
            foreach (var item in clbLayers.SelectedItems) {
                string layerName = item.ToString();
                layersToRemove.Add(layerName);
            }
            foreach (string layerName in layersToRemove) {
                mapLayerManager.RemoveLayer(layerName);
            }
        }

        private void panelMap_Paint(object sender, PaintEventArgs e) {
            var bmp = mapRenderer.GetBitmap();
            if (bmp != null) {
                e.Graphics.DrawImage(bmp, sharedContext.PanOffset);
            }

            if (isDrawingRect) {
                Rectangle rect = new Rectangle(
                    Math.Min(sharedContext.rectScreenStartPoint.X, sharedContext.rectScreenEndPoint.X),
                    Math.Min(sharedContext.rectScreenStartPoint.Y, sharedContext.rectScreenEndPoint.Y),
                    Math.Abs(sharedContext.rectScreenEndPoint.X - sharedContext.rectScreenStartPoint.X),
                    Math.Abs(sharedContext.rectScreenEndPoint.Y - sharedContext.rectScreenStartPoint.Y)
                );
                using (Pen pen = new Pen(Color.Red, 2)) {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }

            if (isDrawingLine) {
                using (Pen pen = new Pen(Color.Red, sharedContext.BrushSize * sharedContext.CellSize)) {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawLine(pen, sharedContext.lineScreenStart, sharedContext.lineScreenEnd);
                }
            }
            Debug.WriteLine("Panel updated");
        }

        private void clbLayers_ItemCheck(object sender, ItemCheckEventArgs e) {
            this.BeginInvoke((MethodInvoker)(() => {
                UpdatePanelMap();
            }));
        }

        private void panelMap_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Middle) {
                isPanning = true;
                panningStartPoint = e.Location;
                //panelMap.Capture = true;
                return;
            }
            if (clbLayers.SelectedItems.Count <= 0) {
                MessageBox.Show("Select a layer first");
                return;
            }

            if (sharedContext.CurrentTool == ToolMode.Rect) {
                isDrawingRect = true;
                sharedContext.rectScreenEndPoint = e.Location;
                sharedContext.rectScreenStartPoint = e.Location;
            }
            if (sharedContext.CurrentTool == ToolMode.Brush || sharedContext.CurrentTool == ToolMode.Eraser) {
                isDrawing = true;
                mapEditor.DrawAt(coorConverter.ScreenToLogical(e.Location));
                mapRenderer.RenderLayersAt(GetVisibleLayers(), e.Location);
                panelMap.Invalidate();
            }
            if (sharedContext.CurrentTool == ToolMode.Line) {
                isDrawingLine = true;
                sharedContext.lineScreenStart = e.Location;
                sharedContext.lineScreenEnd = e.Location;
            }
            if (sharedContext.CurrentTool == ToolMode.Fill) {
                mapEditor.FloodFill(GetVisibleLayers(), e.Location);
                UpdatePanelMap();
            }
        }

        private void panelMap_MouseMove(object sender, MouseEventArgs e) {
            if (isPanning && e.Button == MouseButtons.Middle) {
                Point currentPoint = e.Location;
                int dx = currentPoint.X - panningStartPoint.X;
                int dy = currentPoint.Y - panningStartPoint.Y;
                sharedContext.PanOffset = new Point(sharedContext.PanOffset.X + dx, sharedContext.PanOffset.Y + dy);
                panningStartPoint = currentPoint;
                panelMap.Invalidate();
                return;
            }

            if (sharedContext.CurrentTool == ToolMode.Rect && isDrawingRect) {
                sharedContext.rectScreenEndPoint = e.Location;
                panelMap.Invalidate();
            }

            //BRUSH AND FANCY ERASER
            if (sharedContext.CurrentTool == ToolMode.Brush && isDrawing || sharedContext.CurrentTool == ToolMode.Eraser && isDrawing) {
                mapEditor.DrawAt(coorConverter.ScreenToLogical(e.Location));
                mapRenderer.RenderLayersAt(GetVisibleLayers(), e.Location);
                panelMap.Invalidate();
            }

            if (sharedContext.CurrentTool == ToolMode.Line && isDrawingLine) {
                //To preview the line.
                sharedContext.lineScreenEnd = e.Location;
                panelMap.Invalidate();
            }
        }

        private void panelMap_MouseUp(object sender, MouseEventArgs e) {
            if (isPanning && e.Button == MouseButtons.Middle) {
                isPanning = false;
                //panelMap.Capture = false;
            }
            if (isDrawingRect) {
                isDrawingRect = false;
                sharedContext.rectScreenEndPoint = e.Location;
                panelMap.Invalidate();
                mapEditor.DrawRect();
                UpdatePanelMap();
            }
            if (isDrawing) {
                isDrawing = false;
                mapEditor.DrawAt(coorConverter.ScreenToLogical(e.Location));
                mapRenderer.RenderPreviewAt(coorConverter.ScreenToBitmap(e.Location));
                UpdatePanelMap();
            }
            if (isDrawingLine) {
                isDrawingLine = false;
                mapEditor.DrawLine();
                panelMap.Invalidate();   //Để clear line preview.
                UpdatePanelMap();
            }
        }

        private void btnBrush_Click(object sender, EventArgs e) {
            sharedContext.CurrentTool = ToolMode.Brush;
        }

        private void clbLayers_SelectedIndexChanged(object sender, EventArgs e) {
            if (clbLayers.SelectedItem != null) {
                sharedContext.CurrentLayer = mapLayerManager.GetLayer(clbLayers.SelectedItem.ToString());
            }
        }

        private void btnEraser_Click(object sender, EventArgs e) {
            sharedContext.CurrentTool = ToolMode.Eraser;
        }

        private void btnDrawLine_Click(object sender, EventArgs e) {
            sharedContext.CurrentTool = ToolMode.Line;
        }

        private void nudBrushSize_ValueChanged(object sender, EventArgs e) {
            if (sender is NumericUpDown nud) {
                sharedContext.BrushSize = (int)nud.Value;
            }
        }

        private void SwapItem(int index1, int index2) {
            if (index1 < 0 || index2 < 0 || index1 >= clbLayers.Items.Count || index2 >= clbLayers.Items.Count) {
                return;
            }
            object item1 = clbLayers.Items[index1];
            object item2 = clbLayers.Items[index2];
            bool isChecked1 = clbLayers.GetItemChecked(index1);
            bool isChecked2 = clbLayers.GetItemChecked(index2);

            clbLayers.Items[index1] = item2;
            clbLayers.Items[index2] = item1;
            clbLayers.SetItemChecked(index1, isChecked2);
            clbLayers.SetItemChecked(index2, isChecked1);
            clbLayers.SelectedIndex = index2;

            clbLayers_SelectedIndexChanged(clbLayers, EventArgs.Empty);

            UpdatePanelMap();
        }

        private void btnUp_Click(object sender, EventArgs e) {
            int index = clbLayers.SelectedIndex;
            if (index > 0) {
                SwapItem(index, index - 1);
            }
        }

        private void btnDown_Click(object sender, EventArgs e) {
            int index = clbLayers.SelectedIndex;
            if (index >= 0 && index < clbLayers.Items.Count - 1) {
                SwapItem(index, index + 1);
            }
        }

        private void btnDrawRect_Click(object sender, EventArgs e) {
            sharedContext.CurrentTool = ToolMode.Rect;
        }

        private void UISetUp() {
            nudBrushSize.Value = defaultBrushSize;
            panelMap.BackColor = sharedContext.PanelBackColor;
            panelMap.MouseWheel += PanelMap_MouseWheel;
        }

        private void label1_Click(object sender, EventArgs e) {
        }

        private void btnZoomIn_Click(object sender, EventArgs e) {
            if (sharedContext.CellSize < 5) {
                sharedContext.CellSize++;
                UpdatePanelMap();
            } else {
                MessageBox.Show("Maximum zoom level reached (CellSize = 5).", "Zoom Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e) {
            if (sharedContext.CellSize > 1) {
                sharedContext.CellSize--;
                UpdatePanelMap();
            } else {
                MessageBox.Show("Minimum zoom level reached (CellSize = 1).", "Zoom Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //PROBLEM, PANNING IS WRONG!!!
        private void PanelMap_MouseWheel(object sender, MouseEventArgs e) {
            Point mousePosition = e.Location;
            Point logicalMouseBeforeZoom = coorConverter.ScreenToLogical(mousePosition);

            int previousCellSize = sharedContext.CellSize;

            if (e.Delta > 0) { // Zoom in
                if (sharedContext.CellSize < sharedContext.maxZoomInValue) {
                    sharedContext.CellSize++;
                }
            } else if (e.Delta < 0) { // Zoom out
                if (sharedContext.CellSize > 1) {
                    sharedContext.CellSize--;
                }
            }

            if (sharedContext.CellSize != previousCellSize) {
                Point logicalMouseAfterZoom = coorConverter.ScreenToLogical(mousePosition);

                // Calculate the logical pixel that should remain under the mouse
                Point logicalAnchor = logicalMouseBeforeZoom;

                // Convert the logical anchor back to screen coordinates with the new CellSize
                Point screenAnchorAfterZoom = coorConverter.LogicalToScreen(logicalAnchor);

                // Calculate the screen-space difference needed to keep the anchor at the mouse position
                int deltaScreenX = mousePosition.X - screenAnchorAfterZoom.X;
                int deltaScreenY = mousePosition.Y - screenAnchorAfterZoom.Y;

                // Adjust the PanOffset by this screen-space difference
                sharedContext.PanOffset = new Point(
                    sharedContext.PanOffset.X + deltaScreenX,
                    sharedContext.PanOffset.Y + deltaScreenY
                );

                UpdatePanelMap();
            }
        }

        private void btnBucket_Click(object sender, EventArgs e) {
            sharedContext.CurrentTool = ToolMode.Fill;
        }

        private void btnLoadBG_Click(object sender, EventArgs e) {
        }

        private void btnReloadBitmap_Click(object sender, EventArgs e) {
            UpdatePanelMap();
        }
    }
}