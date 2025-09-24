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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace MapTool {

    public enum ToolMode { None, Brush, Eraser, Fill, Line, Rect };

    public partial class mainForm : Form {
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

        //For previewing.
        private bool isDrawing = false; //Dùng cho cả Brush và Eraser

        private bool isDrawingLine = false;
        private bool isDrawingRect = false;

        //Background image
        private Image backgroundImage = null;

        private bool autoGeneratePosBGImage = true;
        private int bgImageWidth;
        private int bgImageHeight;
        private Point bgImagePos;

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
                maxZoomInValue = 20
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

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            List<MapLayer> mapLayerToAddToMapManager = new List<MapLayer>();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string filePath in openFileDialog1.FileNames)
                {
                    byte[] buffer = File.ReadAllBytes(filePath);
                    CreateNewLayerForm newLayerForm = new CreateNewLayerForm();
                    newLayerForm.InitialLayerName = Path.GetFileNameWithoutExtension(filePath);

                    MapLayer mapLayer;

                    if (newLayerForm.ShowDialog() == DialogResult.OK)
                    {
                        // Get the user's input from the form
                        int mapWidthPixel = newLayerForm.LayerWidth;
                        int mapHeightPixel = newLayerForm.LayerHeight;

                        // Case 1: User did not fill width and height
                        if (mapWidthPixel == 0 || mapHeightPixel == 0)
                        {
                            int mapHeight = (int)Math.Sqrt(buffer.Length);
                            int mapWidth = (int)Math.Sqrt(buffer.Length);
                            mapLayer = new MapLayer(Path.GetFileNameWithoutExtension(filePath), mapHeight, mapWidth, false);
                        }
                        // Case 2: User filled width and height
                        else
                        {
                            mapLayer = new MapLayer(newLayerForm.LayerName, mapHeightPixel, mapWidthPixel, true);
                        }

                        mapLayer.Data.FromBytes(buffer);
                        mapLayerToAddToMapManager.Add(mapLayer);
                    }
                }

                // Check if the list of layers is not empty before adding
                if (mapLayerToAddToMapManager.Any())
                {
                    mapLayerManager.AddLayers(mapLayerToAddToMapManager);
                }
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

        private void clbLayers_ItemCheck(object sender, ItemCheckEventArgs e) {
            this.BeginInvoke((MethodInvoker)(() => {
                UpdatePanelMap();
            }));
        }

        private void panelMap_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Middle) {
                isPanning = true;
                panningStartPoint = e.Location;
                panelMap.Capture = true;
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

            if (sharedContext.CurrentTool == ToolMode.Brush && isDrawing || sharedContext.CurrentTool == ToolMode.Eraser && isDrawing) {
                mapEditor.DrawAt(coorConverter.ScreenToLogical(e.Location));
                mapRenderer.RenderLayersAt(GetVisibleLayers(), e.Location);
                panelMap.Invalidate();
            }

            if (sharedContext.CurrentTool == ToolMode.Line && isDrawingLine) {
                sharedContext.lineScreenEnd = e.Location;
                panelMap.Invalidate();
            }
        }

        private void panelMap_MouseUp(object sender, MouseEventArgs e) {
            if (isPanning && e.Button == MouseButtons.Middle) {
                isPanning = false;
                panelMap.Capture = false;
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
                panelMap.Invalidate();
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
            int index = clbLayers.SelectedIndex;
            if (index == -1) return;

            btnUp.Enabled = index > 0;
            btnDown.Enabled = index >= 0 && index < clbLayers.Items.Count - 1;

            bool hasSelection = clbLayers.SelectedItem != null;
            btnSaveLayer.Enabled = hasSelection;
            btnLoadBG.Enabled = hasSelection;
            btnRemoveLayer.Enabled = hasSelection;
            btnRotate.Enabled = hasSelection;
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
            btnUp.Enabled = false;
            btnDown.Enabled = false;
            btnLoadBG.Enabled = false;
            btnSaveLayer.Enabled = false;
            btnRemoveLayer.Enabled = false;
            btnRotate.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e) {
        }

        private void btnZoomIn_Click(object sender, EventArgs e) {
            if (sharedContext.CellSize < 20) {
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

        private void PanelMap_MouseWheel(object sender, MouseEventArgs e) {
            Point mousePosition = e.Location;

            Point logicalBeforeZoom = coorConverter.ScreenToLogical(mousePosition);
            int previousCellSize = sharedContext.CellSize;

            if (e.Delta > 0 && sharedContext.CellSize < sharedContext.maxZoomInValue)
                sharedContext.CellSize++;
            else if (e.Delta < 0 && sharedContext.CellSize > 1)
                sharedContext.CellSize--;

            if (sharedContext.CellSize != previousCellSize) {
                Point screenAfterZoom = coorConverter.LogicalToScreen(logicalBeforeZoom);

                int deltaX = mousePosition.X - screenAfterZoom.X;
                int deltaY = mousePosition.Y - screenAfterZoom.Y;

                sharedContext.PanOffset = new Point(
                    sharedContext.PanOffset.X + deltaX,
                    sharedContext.PanOffset.Y + deltaY
                );

                UpdatePanelMap();
            }
        }

        private void btnBucket_Click(object sender, EventArgs e) {
            sharedContext.CurrentTool = ToolMode.Fill;
        }

        private void panelMap_Paint(object sender, PaintEventArgs e) {
            List<MapLayer> visibleLayer = GetVisibleLayers();
            if (backgroundImage != null && visibleLayer.Count > 0) {
                int scaledBackgroundWidth = (bgImageWidth * sharedContext.CellSize);
                int scaledBackgroundHeight = (bgImageHeight * sharedContext.CellSize);

                int backgroundX;
                int backgroundY;

                if (autoGeneratePosBGImage) {
                    backgroundX = sharedContext.PanOffset.X;
                    backgroundY = sharedContext.PanOffset.Y + (GetVisibleLayers()[0].Data.GetLength(0) * sharedContext.CellSize) - scaledBackgroundHeight;
                } else {
                    backgroundX = sharedContext.PanOffset.X + (bgImagePos.X * sharedContext.CellSize);
                    backgroundY = sharedContext.PanOffset.Y + (bgImagePos.Y * sharedContext.CellSize);
                }

                e.Graphics.DrawImage(backgroundImage,
                                    backgroundX,
                                    backgroundY,
                                    scaledBackgroundWidth,
                                    scaledBackgroundHeight);
            }
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

        }

        private void btnLoadBG_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*",
                Title = "Load Background Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    if (backgroundImage != null)
                        backgroundImage.Dispose();

                    using (var configForm = new ImageConfigForm())
                    {
                        DialogResult result = configForm.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            if (configForm.ImgWidth == 0 || configForm.ImgHeight == 0)
                            {
                                if (!(sharedContext.CurrentLayer.mapContentWidth == 0 || sharedContext.CurrentLayer.mapContentHeight == 0))
                                {
                                   bgImageWidth  = sharedContext.CurrentLayer.mapContentWidth;
                                  bgImageHeight    = sharedContext.CurrentLayer.mapContentHeight;
                                }
                                else
                                {
                                    MessageBox.Show("file layer chưa có pixel dimension, hãy tự điền", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                            }
                            else
                            {
                                bgImageWidth = configForm.ImgWidth;
                                bgImageHeight = configForm.ImgHeight;
                                sharedContext.CurrentLayer.setMapDimensions(bgImageWidth, bgImageHeight);
                                bgImageWidth = sharedContext.CurrentLayer.mapContentWidth;
                                bgImageHeight = sharedContext.CurrentLayer.mapContentHeight;
                            }

                            if (configForm.HasPosition)
                            {
                                bgImagePos = new Point(configForm.CoorX, configForm.CoorY);
                                autoGeneratePosBGImage = false;
                            }
                            else
                            {
                                autoGeneratePosBGImage = true;
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    backgroundImage = Image.FromFile(openFileDialog.FileName);

                    panelMap.Invalidate();
                } catch (Exception ex) {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReloadBitmap_Click(object sender, EventArgs e) {
            UpdatePanelMap();
        }

        private void btnCreateNewLayer_Click(object sender, EventArgs e) {
            using (var inputForm = new CreateNewLayerForm()) 
            {
                if (inputForm.ShowDialog() == DialogResult.OK) {
                    string layerName = inputForm.LayerName;
                    int width = inputForm.LayerWidth;
                    int height = inputForm.LayerHeight;
                    if (!string.IsNullOrWhiteSpace(layerName) && width > 0 && height > 0) {
                        mapLayerManager.AddLayer(layerName, width, height);
                    } else {
                        MessageBox.Show("Invalid layer details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnRotate_Click(object sender, EventArgs e) {
            if (sharedContext.CurrentLayer != null) {
                sharedContext.CurrentLayer.Rotate(90);
                UpdatePanelMap();
            }
        }

        private void btnSaveLayer_Click(object sender, EventArgs e) {
            if (clbLayers.SelectedItem == null) {
                MessageBox.Show("Select a layer to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = "Text files (*.txt)|*.txt",
                DefaultExt = "txt",
                AddExtension = true,
                FileName = sharedContext.CurrentLayer.Name + ".txt"
            };
            Byte[] buffer = sharedContext.CurrentLayer.Data.ToBytes();
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                File.WriteAllBytes(saveFileDialog.FileName, buffer);
                MessageBox.Show("Layer saved successfully.");
            }
        }
    }
}