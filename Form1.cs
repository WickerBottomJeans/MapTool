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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace MapTool
{

    public partial class mainForm : Form
    {


        private MapLayerManager mapLayerManager;
        private MapRenderer mapRenderer;
        private MapEditor mapEditor;
        private CoorConverter coorConverter;
        private MapContext sharedContext;

        private int defaultBrushSize = 5;

        private Point panningStartPoint;
        private bool isPanning = false;

        private bool isDrawing = false;

        private bool isDrawingLine = false;
        private bool isDrawingRect = false;

        private Image backgroundImage = null;
        private bool autoGeneratePosBGImage = true;
        private int bgImageWidth;
        private int bgImageHeight;
        private Point bgImagePos;

        private Point hoverPosition = Point.Empty;
        private bool showHoverPreview = false;

        private List<Point> polygonPoints = new List<Point>();
        private bool isPolygonMode = false;

        private bool selectionModeEnabled = false;
        private bool isSelecting = false;
        private Point selectionStart;
        private Point selectionEnd;
        private bool hasSelection = false;
        private bool isMovingSelection = false;
        private Point moveStartPoint;
        private byte[,] copiedRegionData = null;

        private List<MapEntity> mapEntities = new List<MapEntity>();
        private MapEntity draggedEntity = null;
        private MapEntity hoveredEntity = null;

        private string currentJsonPath = "";
        private const int EntityHitRadius = 15;
        private float entityPosScale = 1f;

        public mainForm()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panelMap, new object[] { true });

            sharedContext = new MapContext
            {
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

        private void MapRenderer_MapDrawingFinished(object sender, EventArgs e)
        {
            panelMap.Invalidate();
        }

        private void MapLayerManager_LayersDictChanged(object sender, EventArgs e)
        {
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
                        int mapWidthPixel = newLayerForm.LayerWidth;
                        int mapHeightPixel = newLayerForm.LayerHeight;

                        if (mapWidthPixel == 0 || mapHeightPixel == 0)
                        {
                            int mapHeight = (int)Math.Sqrt(buffer.Length);
                            int mapWidth = (int)Math.Sqrt(buffer.Length);
                            mapLayer = new MapLayer(Path.GetFileNameWithoutExtension(filePath), mapHeight, mapWidth, false);
                        }
                        else
                        {
                            mapLayer = new MapLayer(newLayerForm.LayerName, mapHeightPixel, mapWidthPixel, true);
                        }

                        mapLayer.Data.FromBytes(buffer);
                        mapLayerToAddToMapManager.Add(mapLayer);
                    }
                }

                if (mapLayerToAddToMapManager.Any())
                {
                    mapLayerManager.AddLayers(mapLayerToAddToMapManager);
                }
            }
        }

        private void UpdateClbLayers()
        {
            clbLayers.Items.Clear();
            foreach (var mapLayer in mapLayerManager.GetAllLayers())
            {
                clbLayers.Items.Add(mapLayer.Name, true);
            }
        }

        private void UpdateByteLegend()
        {
            panelLegend.Controls.Clear();

            if (sharedContext.CurrentLayer == null) return;

            HashSet<byte> uniqueValues = new HashSet<byte>();
            for (int y = 0; y < sharedContext.CurrentLayer.Data.GetLength(0); y++)
            {
                for (int x = 0; x < sharedContext.CurrentLayer.Data.GetLength(1); x++)
                {
                    byte value = sharedContext.CurrentLayer.Data[y, x];
                    if (value != 0)
                    {
                        uniqueValues.Add(value);
                    }
                }
            }

            var sortedValues = uniqueValues.OrderBy(v => v).ToList();
            Color layerColor = mapRenderer.GetColorForLayer(sharedContext.CurrentLayer.Name);

            int yOffset = 5;
            foreach (byte value in sortedValues)
            {
                Panel colorBox = new Panel
                {
                    Width = 30,
                    Height = 20,
                    Left = 5,
                    Top = yOffset,
                    BackColor = mapRenderer.GetColorForByteValue(value, layerColor),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label label = new Label
                {
                    Text = $"Value: {value}",
                    Left = 40,
                    Top = yOffset + 2,
                    AutoSize = true
                };

                panelLegend.Controls.Add(colorBox);
                panelLegend.Controls.Add(label);

                yOffset += 25;
            }
        }

        private List<MapLayer> GetVisibleLayers()
        {
            var visibleLayers = new List<MapLayer>();
            foreach (var item in clbLayers.CheckedItems)
            {
                string name = item.ToString();
                var layer = mapLayerManager.GetLayer(name);
                visibleLayers.Add(layer);
            }
            return visibleLayers;
        }

        private void UpdatePanelMap()
        {
            var layerToDraw = GetVisibleLayers();
            mapRenderer.RenderLayers(layerToDraw);
            UpdateByteLegend();
        }

        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            List<string> layersToRemove = new List<string>();
            foreach (var item in clbLayers.SelectedItems)
            {
                string layerName = item.ToString();
                layersToRemove.Add(layerName);
            }
            foreach (string layerName in layersToRemove)
            {
                mapLayerManager.RemoveLayer(layerName);
            }
        }

        private void clbLayers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)(() => {
                UpdatePanelMap();
            }));
        }

        private Point lastDrawPoint = Point.Empty;

        private void panelMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                panningStartPoint = e.Location;
                panelMap.Capture = true;
                return;
            }

            if (e.Button == MouseButtons.Left && mapEntities.Count > 0)
            {
                // Only allow drag if hovering on the entity ring (within hit radius)
                if (hoveredEntity != null)
                {
                    draggedEntity = hoveredEntity;
                    panelMap.Capture = true;
                    return;
                }
            }

            if (clbLayers.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Select a layer first");
                return;
            }
            if (selectionModeEnabled && hasSelection && isMovingSelection && e.Button == MouseButtons.Left)
            {
                moveStartPoint = coorConverter.ScreenToLogical(e.Location);
                return;
            }
            if (selectionModeEnabled && e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                hasSelection = false;
                selectionStart = coorConverter.ScreenToLogical(e.Location);
                selectionEnd = selectionStart;
                panelMap.Invalidate();
                return;
            }
            if (sharedContext.CurrentTool == ToolMode.Rect)
            {
                isDrawingRect = true;
                sharedContext.rectScreenEndPoint = e.Location;
                sharedContext.rectScreenStartPoint = e.Location;
            }
            if (sharedContext.CurrentTool == ToolMode.Brush || sharedContext.CurrentTool == ToolMode.Eraser)
            {
                isDrawing = true;
                lastDrawPoint = e.Location;
                mapEditor.DrawAt(coorConverter.ScreenToLogical(e.Location));
                mapRenderer.RenderLayersAt(GetVisibleLayers(), e.Location);
                panelMap.Invalidate();
            }
            if (sharedContext.CurrentTool == ToolMode.Line)
            {
                isDrawingLine = true;
                sharedContext.lineScreenStart = e.Location;
                sharedContext.lineScreenEnd = e.Location;
            }
            if (sharedContext.CurrentTool == ToolMode.Fill)
            {
                mapEditor.FloodFill(GetVisibleLayers(), e.Location);
                UpdatePanelMap();
            }
        }
        private void panelMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning && e.Button == MouseButtons.Middle)
            {
                Point currentPoint = e.Location;
                int dx = currentPoint.X - panningStartPoint.X;
                int dy = currentPoint.Y - panningStartPoint.Y;
                sharedContext.PanOffset = new Point(sharedContext.PanOffset.X + dx, sharedContext.PanOffset.Y + dy);
                panningStartPoint = currentPoint;
                panelMap.Invalidate();
                return;
            }

            if (draggedEntity != null && e.Button == MouseButtons.Left)
            {
                Point logicalMouse = coorConverter.ScreenToLogical(e.Location);
                Point pixelPos = EntityLogicalToPixel(logicalMouse.X, logicalMouse.Y);
                draggedEntity.posX = pixelPos.X;
                draggedEntity.posY = pixelPos.Y;
                panelMap.Invalidate();
                return;
            }

            // Hover detection — check if mouse is within the ring of any entity
            MapEntity newHovered = null;
            if (mapEntities.Count > 0)
            {
                foreach (var entity in mapEntities)
                {
                    Point logical = EntityPixelToLogical(entity.posX, entity.posY);
                    Point screenPos = coorConverter.LogicalToScreen(logical);
                    int half = sharedContext.CellSize / 2;
                    Point center = new Point(screenPos.X + half, screenPos.Y + half);
                    int dx = e.Location.X - center.X;
                    int dy = e.Location.Y - center.Y;
                    if (Math.Abs(dx) <= half + 3 && Math.Abs(dy) <= half + 3)
                    {
                        newHovered = entity;
                        break;
                    }
                }
            }

            if (newHovered != hoveredEntity)
            {
                hoveredEntity = newHovered;
                panelMap.Cursor = hoveredEntity != null ? Cursors.Hand : Cursors.Default;
                panelMap.Invalidate();
            }

            if (isMovingSelection && e.Button == MouseButtons.Left)
            {
                Point currentPoint = coorConverter.ScreenToLogical(e.Location);
                int deltaX = currentPoint.X - moveStartPoint.X;
                int deltaY = currentPoint.Y - moveStartPoint.Y;
                selectionStart = new Point(selectionStart.X + deltaX, selectionStart.Y + deltaY);
                selectionEnd = new Point(selectionEnd.X + deltaX, selectionEnd.Y + deltaY);
                moveStartPoint = currentPoint;
                panelMap.Invalidate();
                return;
            }

            if (isSelecting)
            {
                selectionEnd = coorConverter.ScreenToLogical(e.Location);
                panelMap.Invalidate();
                return;
            }

            if (sharedContext.CurrentTool == ToolMode.Rect && isDrawingRect)
            {
                sharedContext.rectScreenEndPoint = e.Location;
                panelMap.Invalidate();
            }

            if ((sharedContext.CurrentTool == ToolMode.Brush || sharedContext.CurrentTool == ToolMode.Eraser) && isDrawing)
            {
                InterpolateAndDraw(lastDrawPoint, e.Location);
                lastDrawPoint = e.Location;
            }

            if ((sharedContext.CurrentTool == ToolMode.Brush || sharedContext.CurrentTool == ToolMode.Eraser) && !isDrawing)
            {
                hoverPosition = e.Location;
                showHoverPreview = true;
                panelMap.Invalidate();
            }

            if (sharedContext.CurrentTool == ToolMode.Line && isDrawingLine)
            {
                sharedContext.lineScreenEnd = e.Location;
                panelMap.Invalidate();
            }
        }
        private void panelMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (isPanning && e.Button == MouseButtons.Middle)
            {
                isPanning = false;
                panelMap.Capture = false;
            }

            if (draggedEntity != null)
            {
                draggedEntity = null;
                panelMap.Capture = false;
                SaveEntitiesToJSON();
                panelMap.Invalidate();
                return;
            }

            if (isDrawingRect)
            {
                isDrawingRect = false;
                sharedContext.rectScreenEndPoint = e.Location;
                panelMap.Invalidate();
                mapEditor.DrawRect();
                UpdatePanelMap();
            }

            if (isSelecting)
            {
                isSelecting = false;
                selectionEnd = coorConverter.ScreenToLogical(e.Location);
                int width = Math.Abs(selectionEnd.X - selectionStart.X);
                int height = Math.Abs(selectionEnd.Y - selectionStart.Y);
                if (width > 0 && height > 0)
                    hasSelection = true;
                panelMap.Invalidate();
                return;
            }

            if (isMovingSelection)
            {
                isMovingSelection = false;
                panelMap.Cursor = selectionModeEnabled ? Cursors.Cross : Cursors.Default;
                int x = Math.Min(selectionStart.X, selectionEnd.X);
                int y = Math.Min(selectionStart.Y, selectionEnd.Y);
                PasteRegion(x, y, copiedRegionData);
                copiedRegionData = null;
                UpdatePanelMap();
                return;
            }

            if (isDrawing)
            {
                isDrawing = false;
                mapEditor.DrawAt(coorConverter.ScreenToLogical(e.Location));
                mapRenderer.RenderPreviewAt(coorConverter.ScreenToBitmap(e.Location));
                UpdateByteLegend();
                UpdatePanelMap();
            }

            if (isDrawingLine)
            {
                isDrawingLine = false;
                mapEditor.DrawLine();
                panelMap.Invalidate();
                UpdatePanelMap();
            }
        }
        private void InterpolateAndDraw(Point start, Point end)
        {
            int dx = end.X - start.X;
            int dy = end.Y - start.Y;
            int steps = Math.Max(Math.Abs(dx), Math.Abs(dy));

            if (steps == 0)
            {
                mapEditor.DrawAt(coorConverter.ScreenToLogical(end));
                mapRenderer.RenderLayersAt(GetVisibleLayers(), end);
                panelMap.Invalidate();
                return;
            }

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Point interpolated = new Point(
                    start.X + (int)(dx * t),
                    start.Y + (int)(dy * t)
                );
                mapEditor.DrawAt(coorConverter.ScreenToLogical(interpolated));
                mapRenderer.RenderLayersAt(GetVisibleLayers(), interpolated);
            }
            panelMap.Invalidate();
        }

        private void btnBrush_Click(object sender, EventArgs e)
        {
            sharedContext.CurrentTool = ToolMode.Brush;
        }

        private void clbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbLayers.SelectedItem != null)
            {
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

        private void btnEraser_Click(object sender, EventArgs e)
        {
            sharedContext.CurrentTool = ToolMode.Eraser;
        }

        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            sharedContext.CurrentTool = ToolMode.Line;
        }

        private void nudBrushSize_ValueChanged(object sender, EventArgs e)
        {
            if (sender is NumericUpDown nud)
            {
                sharedContext.BrushSize = (int)nud.Value;
            }
        }

        private void SwapItem(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0 || index1 >= clbLayers.Items.Count || index2 >= clbLayers.Items.Count)
            {
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
            UpdateByteLegend();
            UpdatePanelMap();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            int index = clbLayers.SelectedIndex;
            if (index > 0)
            {
                SwapItem(index, index - 1);
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            int index = clbLayers.SelectedIndex;
            if (index >= 0 && index < clbLayers.Items.Count - 1)
            {
                SwapItem(index, index + 1);
            }
        }

        private void btnDrawRect_Click(object sender, EventArgs e)
        {
            sharedContext.CurrentTool = ToolMode.Rect;
        }

        private void UISetUp()
        {
            nudBrushSize.Value = defaultBrushSize;
            panelMap.BackColor = sharedContext.PanelBackColor;
            panelMap.MouseWheel += PanelMap_MouseWheel;
            this.KeyPreview = true;
            this.KeyDown += MainForm_KeyDown;
            btnUp.Enabled = false;
            btnDown.Enabled = false;
            btnLoadBG.Enabled = false;
            btnSaveLayer.Enabled = false;
            btnRemoveLayer.Enabled = false;
            btnRotate.Enabled = false;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (selectionModeEnabled && hasSelection && clbLayers.SelectedItem != null)
            {
                int x = Math.Min(selectionStart.X, selectionEnd.X);
                int y = Math.Min(selectionStart.Y, selectionEnd.Y);
                int width = Math.Abs(selectionEnd.X - selectionStart.X);
                int height = Math.Abs(selectionEnd.Y - selectionStart.Y);

                if (e.KeyCode == Keys.L)
                {
                    sharedContext.CurrentLayer.RotateRegion(x, y, width, height, 270);
                    UpdatePanelMap();
                    e.Handled = true;
                    return;
                }
                else if (e.KeyCode == Keys.R)
                {
                    sharedContext.CurrentLayer.RotateRegion(x, y, width, height, 90);
                    UpdatePanelMap();
                    e.Handled = true;
                    return;
                }
                else if (e.KeyCode == Keys.M)
                {
                    if (!isMovingSelection)
                    {
                        isMovingSelection = true;
                        panelMap.Cursor = Cursors.SizeAll;
                        copiedRegionData = CutRegion(x, y, width, height);
                        UpdatePanelMap();
                    }
                    e.Handled = true;
                    return;
                }
            }

            if (e.KeyCode == Keys.F)
            {
                if (clbLayers.SelectedItems.Count <= 0)
                {
                    MessageBox.Show("Select a layer first");
                    return;
                }

                Point mousePos = panelMap.PointToClient(Cursor.Position);
                Point logicalPoint = coorConverter.ScreenToLogical(mousePos);
                polygonPoints.Add(logicalPoint);
                isPolygonMode = true;
                panelMap.Invalidate();
                e.Handled = true;
            }

            if (e.KeyCode == Keys.G)
            {
                if (polygonPoints.Count >= 3)
                {
                    mapEditor.FillPolygon(polygonPoints);
                    polygonPoints.Clear();
                    isPolygonMode = false;
                    UpdatePanelMap();
                }
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                if (isPolygonMode)
                {
                    polygonPoints.Clear();
                    isPolygonMode = false;
                }
                if (selectionModeEnabled)
                {
                    hasSelection = false;
                    isMovingSelection = false;
                }
                panelMap.Invalidate();
                e.Handled = true;
            }
        }

        private byte[,] CutRegion(int startX, int startY, int width, int height)
        {
            if (sharedContext.CurrentLayer?.Data == null) return null;

            byte[,] cutData = new byte[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sourceY = startY + y;
                    int sourceX = startX + x;

                    if (sourceY >= 0 && sourceY < sharedContext.CurrentLayer.Data.GetLength(0) &&
                        sourceX >= 0 && sourceX < sharedContext.CurrentLayer.Data.GetLength(1))
                    {
                        cutData[y, x] = sharedContext.CurrentLayer.Data[sourceY, sourceX];
                        sharedContext.CurrentLayer.Data[sourceY, sourceX] = 0;
                    }
                }
            }

            return cutData;
        }

        private void PasteRegion(int startX, int startY, byte[,] data)
        {
            if (sharedContext.CurrentLayer?.Data == null || data == null) return;

            int height = data.GetLength(0);
            int width = data.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int targetY = startY + y;
                    int targetX = startX + x;

                    if (targetY >= 0 && targetY < sharedContext.CurrentLayer.Data.GetLength(0) &&
                        targetX >= 0 && targetX < sharedContext.CurrentLayer.Data.GetLength(1))
                    {
                        sharedContext.CurrentLayer.Data[targetY, targetX] = data[y, x];
                    }
                }
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (sharedContext.CellSize < 20)
            {
                sharedContext.CellSize++;
                UpdatePanelMap();
            }
            else
            {
                MessageBox.Show("Maximum zoom level reached (CellSize = 5).", "Zoom Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (sharedContext.CellSize > 1)
            {
                sharedContext.CellSize--;
                UpdatePanelMap();
            }
            else
            {
                MessageBox.Show("Minimum zoom level reached (CellSize = 1).", "Zoom Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PanelMap_MouseWheel(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.Location;

            int proposedCellSize = sharedContext.CellSize;
            if (e.Delta > 0 && proposedCellSize < sharedContext.maxZoomInValue)
                proposedCellSize++;
            else if (e.Delta < 0 && proposedCellSize > 1)
                proposedCellSize--;

            if (proposedCellSize != sharedContext.CellSize)
            {
                if (mapRenderer.CanRenderAtCellSize(proposedCellSize, GetVisibleLayers()))
                {
                    Point logicalBeforeZoom = coorConverter.ScreenToLogical(mousePosition);
                    sharedContext.CellSize = proposedCellSize;
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
        }

        private void btnBucket_Click(object sender, EventArgs e)
        {
            sharedContext.CurrentTool = ToolMode.Fill;
        }

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            List<MapLayer> visibleLayer = GetVisibleLayers();
            if (backgroundImage != null && visibleLayer.Count > 0)
            {
                int scaledBackgroundWidth = bgImageWidth * sharedContext.CellSize;
                int scaledBackgroundHeight = bgImageHeight * sharedContext.CellSize;

                int backgroundX;
                int backgroundY;

                if (autoGeneratePosBGImage)
                {
                    backgroundX = sharedContext.PanOffset.X;
                    backgroundY = sharedContext.PanOffset.Y + (GetVisibleLayers()[0].Data.GetLength(0) * sharedContext.CellSize) - scaledBackgroundHeight;
                }
                else
                {
                    backgroundX = sharedContext.PanOffset.X + bgImagePos.X * sharedContext.CellSize;
                    backgroundY = sharedContext.PanOffset.Y + bgImagePos.Y * sharedContext.CellSize;
                }

                e.Graphics.DrawImage(backgroundImage, backgroundX, backgroundY, scaledBackgroundWidth, scaledBackgroundHeight);
            }

            var bmp = mapRenderer.GetBitmap();
            if (bmp != null)
                e.Graphics.DrawImage(bmp, sharedContext.PanOffset);

            foreach (var entity in mapEntities)
            {
                Point logical = EntityPixelToLogical(entity.posX, entity.posY);
                Point screenPos = coorConverter.LogicalToScreen(logical);

                bool isHovered = (hoveredEntity == entity);
                bool isDragging = (draggedEntity == entity);

                int DOT_FULL = sharedContext.CellSize;
                const int RING_PAD = 3;

                Color dotColor = (isHovered || isDragging) ? Color.White : Color.Red;
                Color ringColor = (isHovered || isDragging) ? Color.White : Color.FromArgb(180, Color.Red);
                Color markerColor = (isHovered || isDragging) ? Color.Red : Color.White;

                // Dot — exactly one grid cell, screenPos is bottom-left
                using (Brush dotBrush = new SolidBrush(dotColor))
                    e.Graphics.FillRectangle(dotBrush, screenPos.X, screenPos.Y - DOT_FULL, DOT_FULL, DOT_FULL);

                // Ring around dot
                using (Pen ringPen = new Pen(ringColor, 1))
                    e.Graphics.DrawEllipse(ringPen,
                        screenPos.X - RING_PAD,
                        screenPos.Y - DOT_FULL - RING_PAD,
                        DOT_FULL + RING_PAD * 2,
                        DOT_FULL + RING_PAD * 2);

                // Single pixel marker at exact saved position (bottom-left of dot), opposite color
                using (Brush markerBrush = new SolidBrush(markerColor))
                    e.Graphics.FillRectangle(markerBrush, screenPos.X, screenPos.Y, 1, 1);

                // Label
                using (Font font = new Font("Arial", 7, FontStyle.Regular))
                {
                    e.Graphics.DrawString(entity.name, font, Brushes.Black, screenPos.X + DOT_FULL + RING_PAD + 2, screenPos.Y - DOT_FULL);
                    e.Graphics.DrawString(entity.name, font, Brushes.Yellow, screenPos.X + DOT_FULL + RING_PAD + 1, screenPos.Y - DOT_FULL - 1);
                }
            }

            if (isDrawingRect)
            {
                Rectangle rect = new Rectangle(
                    Math.Min(sharedContext.rectScreenStartPoint.X, sharedContext.rectScreenEndPoint.X),
                    Math.Min(sharedContext.rectScreenStartPoint.Y, sharedContext.rectScreenEndPoint.Y),
                    Math.Abs(sharedContext.rectScreenEndPoint.X - sharedContext.rectScreenStartPoint.X),
                    Math.Abs(sharedContext.rectScreenEndPoint.Y - sharedContext.rectScreenStartPoint.Y)
                );
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }

            if (isDrawingLine)
            {
                using (Pen pen = new Pen(Color.Red, sharedContext.BrushSize * sharedContext.CellSize))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawLine(pen, sharedContext.lineScreenStart, sharedContext.lineScreenEnd);
                }
            }

            if (showHoverPreview && (sharedContext.CurrentTool == ToolMode.Brush || sharedContext.CurrentTool == ToolMode.Eraser))
            {
                Point logicalPoint = coorConverter.ScreenToLogical(hoverPosition);
                Point screenPoint = coorConverter.LogicalToScreen(logicalPoint);
                using (Brush ghostBrush = new SolidBrush(Color.FromArgb(64, Color.White)))
                {
                    e.Graphics.FillRectangle(ghostBrush,
                        screenPoint.X, screenPoint.Y,
                        sharedContext.BrushSize * sharedContext.CellSize,
                        sharedContext.BrushSize * sharedContext.CellSize);
                }
            }

            if (isPolygonMode && polygonPoints.Count > 0)
            {
                using (Pen pen = new Pen(Color.Yellow, 2))
                {
                    foreach (Point logicalPt in polygonPoints)
                    {
                        Point screenPt = coorConverter.LogicalToScreen(logicalPt);
                        e.Graphics.FillEllipse(Brushes.Yellow, screenPt.X - 3, screenPt.Y - 3, 6, 6);
                    }
                    if (polygonPoints.Count > 1)
                    {
                        for (int i = 0; i < polygonPoints.Count - 1; i++)
                        {
                            Point p1 = coorConverter.LogicalToScreen(polygonPoints[i]);
                            Point p2 = coorConverter.LogicalToScreen(polygonPoints[i + 1]);
                            e.Graphics.DrawLine(pen, p1, p2);
                        }
                    }
                }
            }

            if ((isSelecting || hasSelection) && selectionStart != Point.Empty)
            {
                Point screenStart = coorConverter.LogicalToScreen(selectionStart);
                Point screenEnd = coorConverter.LogicalToScreen(selectionEnd);
                Rectangle rect = new Rectangle(
                    Math.Min(screenStart.X, screenEnd.X),
                    Math.Min(screenStart.Y, screenEnd.Y),
                    Math.Abs(screenEnd.X - screenStart.X),
                    Math.Abs(screenEnd.Y - screenStart.Y)
                );
                using (Pen pen = new Pen(Color.Cyan, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }
        private void btnLoadBG_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*",
                Title = "Load Background Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
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
                                    bgImageWidth = sharedContext.CurrentLayer.mapContentHeight;
                                    bgImageHeight = sharedContext.CurrentLayer.mapContentWidth;
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
                                int BeforeConsiderRatio_bgImageWidth = sharedContext.CurrentLayer.mapContentHeight;
                                int BeforeConsiderRatio_bgImageHeight = sharedContext.CurrentLayer.mapContentWidth;

                                bgImageWidth = BeforeConsiderRatio_bgImageWidth;
                                bgImageHeight = BeforeConsiderRatio_bgImageHeight;
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReloadBitmap_Click(object sender, EventArgs e)
        {
            UpdatePanelMap();
        }

        private void btnCreateNewLayer_Click(object sender, EventArgs e)
        {
            using (var inputForm = new CreateNewLayerForm())
            {
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string layerName = inputForm.LayerName;
                    int width = inputForm.LayerWidth;
                    int height = inputForm.LayerHeight;
                    if (!string.IsNullOrWhiteSpace(layerName) && width > 0 && height > 0)
                    {
                        mapLayerManager.AddLayer(layerName, width, height);
                    }
                    else
                    {
                        MessageBox.Show("Invalid layer details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            if (sharedContext.CurrentLayer != null)
            {
                sharedContext.CurrentLayer.Rotate(90);
                UpdatePanelMap();
            }
        }

        private void btnSaveLayer_Click(object sender, EventArgs e)
        {
            if (clbLayers.SelectedItem == null)
            {
                MessageBox.Show("Select a layer to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                DefaultExt = "txt",
                AddExtension = true,
                FileName = sharedContext.CurrentLayer.Name + ".txt"
            };
            Byte[] buffer = sharedContext.CurrentLayer.Data.ToBytes();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog.FileName, buffer);
                MessageBox.Show("Layer saved successfully.");
            }
        }

        private void btnXOR_Click(object sender, EventArgs e)
        {
            if (this.mapLayerManager != null)
            {
                XOR xorForm = new XOR(this.mapLayerManager);
                xorForm.ShowDialog();
            }
        }

        private void panelMap_MouseLeave(object sender, EventArgs e)
        {
            showHoverPreview = false;
            panelMap.Invalidate();
        }

        private void btnLoadFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select folder containing blur.txt, obs.txt, Size.txt, and background image";
                folderBrowserDialog.SelectedPath = @"C:\Users\admin\Downloads\38833FF26BA1D.UnigramPreview_g9c9v27vpyspw!App\Result";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowserDialog.SelectedPath;
                    string blurPath = Path.Combine(folderPath, "blur.txt");
                    string obsPath = Path.Combine(folderPath, "obs.txt");
                    string sizePath = Path.Combine(folderPath, "Size.txt");

                    if (!File.Exists(blurPath))
                    {
                        MessageBox.Show("blur.txt not found in the selected folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!File.Exists(obsPath))
                    {
                        MessageBox.Show("obs.txt not found in the selected folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (!File.Exists(sizePath))
                    {
                        MessageBox.Show("Size.txt not found in the selected folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string[] imageExtensions = { "*.bmp", "*.jpg", "*.jpeg", "*.png", "*.gif" };
                    string backgroundImagePath = null;

                    foreach (string extension in imageExtensions)
                    {
                        string[] foundFiles = Directory.GetFiles(folderPath, extension);
                        if (foundFiles.Length > 0)
                        {
                            backgroundImagePath = foundFiles[0];
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(backgroundImagePath))
                    {
                        MessageBox.Show("No background image found in the selected folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int mapWidth = 0;
                    int mapHeight = 0;
                    try
                    {
                        string sizeText = File.ReadAllText(sizePath).Trim();
                        string[] parts = sizeText.Split(new[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 2)
                        {
                            mapWidth = int.Parse(parts[0].Trim());
                            mapHeight = int.Parse(parts[1].Trim());
                        }
                        else
                        {
                            MessageBox.Show("Size.txt must be in format: width x height", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading Size.txt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (mapWidth <= 0 || mapHeight <= 0)
                    {
                        MessageBox.Show("Invalid dimensions in Size.txt.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    List<MapLayer> mapLayerToAddToMapManager = new List<MapLayer>();

                    try
                    {
                        byte[] blurBuffer = File.ReadAllBytes(blurPath);
                        MapLayer blurLayer = new MapLayer("blur", mapHeight, mapWidth, true);
                        blurLayer.Data.FromBytes(blurBuffer);
                        blurLayer.Rotate(90);
                        mapLayerToAddToMapManager.Add(blurLayer);

                        byte[] obsBuffer = File.ReadAllBytes(obsPath);
                        MapLayer obsLayer = new MapLayer("obs", mapHeight, mapWidth, true);
                        obsLayer.Data.FromBytes(obsBuffer);
                        obsLayer.Rotate(90);
                        mapLayerToAddToMapManager.Add(obsLayer);

                        mapLayerManager.AddLayers(mapLayerToAddToMapManager);

                        if (backgroundImage != null)
                            backgroundImage.Dispose();

                        bgImageWidth = mapWidth;
                        bgImageHeight = mapHeight;
                        if (clbLayers.Items.Count > 0)
                        {
                            sharedContext.CurrentLayer = mapLayerManager.GetLayer(clbLayers.Items[0].ToString());
                        }
                        sharedContext.CurrentLayer.setMapDimensions(bgImageWidth, bgImageHeight);
                        int BeforeConsiderRatio_bgImageWidth = sharedContext.CurrentLayer.mapContentHeight;
                        int BeforeConsiderRatio_bgImageHeight = sharedContext.CurrentLayer.mapContentWidth;
                        bgImageWidth = BeforeConsiderRatio_bgImageWidth;
                        bgImageHeight = BeforeConsiderRatio_bgImageHeight;
                        autoGeneratePosBGImage = true;

                        Image originalImage = Image.FromFile(backgroundImagePath);
                        backgroundImage = ResizeImage(originalImage, bgImageWidth, bgImageHeight);
                        originalImage.Dispose();

                        panelMap.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void btnSelectRegion_Click(object sender, EventArgs e)
        {
            selectionModeEnabled = !selectionModeEnabled;

            btnSelectRegion.BackColor = selectionModeEnabled ? Color.LightBlue : SystemColors.Control;
            btnSelectRegion.Text = selectionModeEnabled ? "Cancel Selection" : "Select Region to Rotate";

            panelMap.Cursor = selectionModeEnabled ? Cursors.Cross : Cursors.Default;

            if (!selectionModeEnabled)
            {
                isSelecting = false;
                hasSelection = false;
                isMovingSelection = false;
                panelMap.Invalidate();
            }
        }

        private void btnLoadJSONEntity_Click(object sender, EventArgs e)
        {
            if (backgroundImage == null)
            {
                MessageBox.Show("Please load the map folder first before loading entities.",
                                "No Background Loaded", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                Title = "Load Guild War JSON"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentJsonPath = openFileDialog.FileName;
                    string content = File.ReadAllText(currentJsonPath);
                    mapEntities = ParseManualJson(content);
                    panelMap.Invalidate();
                    MessageBox.Show($"Loaded {mapEntities.Count} entities.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading JSON: {ex.Message}");
                }
            }
        }
        private Point EntityPixelToLogical(int pixelX, int pixelY)
        {
            const int MapGridPixelWidth = 20;
            const int MapGridPixelHeight = 20;

            // Flip Y against the grid's total pixel height, not the bg image's
            int totalGridPixelHeight = sharedContext.CurrentLayer.mapContentHeight * MapGridPixelHeight;
            int flippedY = totalGridPixelHeight - pixelY;

            return new Point(
                pixelX / MapGridPixelWidth,
                flippedY / MapGridPixelHeight
            );
        }

        private Point EntityLogicalToPixel(int logicalX, int logicalY)
        {
            const int MapGridPixelWidth = 20;
            const int MapGridPixelHeight = 20;

            int totalGridPixelHeight = sharedContext.CurrentLayer.mapContentHeight * MapGridPixelHeight;
            int flippedY = totalGridPixelHeight - (logicalY * MapGridPixelHeight);

            return new Point(
                logicalX * MapGridPixelWidth,
                flippedY
            );
        }
        private void SaveEntitiesToJSON()
        {
            if (string.IsNullOrEmpty(currentJsonPath)) return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < mapEntities.Count; i++)
            {
                sb.AppendLine("  {");
                sb.AppendLine($"    \"name\": \"{mapEntities[i].name}\",");
                sb.AppendLine($"    \"posX\": {mapEntities[i].posX},");
                sb.AppendLine($"    \"posY\": {mapEntities[i].posY}");
                sb.Append("  }" + (i < mapEntities.Count - 1 ? "," : ""));
                sb.AppendLine();
            }
            sb.Append("]");
            File.WriteAllText(currentJsonPath, sb.ToString());
        }

        private List<MapEntity> ParseManualJson(string json)
        {
            List<MapEntity> list = new List<MapEntity>();
            string pattern = @"\{\s*""name""\s*:\s*""([^""]+)""\s*,\s*""posX""\s*:\s*(\d+)\s*,\s*""posY""\s*:\s*(\d+)\s*\}";
            MatchCollection matches = Regex.Matches(json, pattern);
            foreach (Match m in matches)
            {
                list.Add(new MapEntity
                {
                    name = m.Groups[1].Value,
                    posX = int.Parse(m.Groups[2].Value),
                    posY = int.Parse(m.Groups[3].Value)
                });
            }
            return list;
        }
    }

    public enum ToolMode { None, Brush, Eraser, Fill, Line, Rect };

    public class MapEntity
    {
        public string name { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
    }
}