using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool {
    //Class này dùng để vẽ lên bitmap của panelMap

    internal class MapRenderer {
        private MapContext context;

        public MapContext Context {
            get { return context; }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value), "MapContext cannot be null.");
                }
                context = value;
            }
        }

        private Bitmap cachedBitmap;
        public CoorConverter coorConverter;

        public event EventHandler MapDrawingFinished;

        public MapRenderer(MapContext context) {
            Context = context;
            coorConverter = new CoorConverter(context);
        }


        protected virtual void OnMapDrawingFinished() {
            MapDrawingFinished?.Invoke(this, EventArgs.Empty);
        }

        public bool CanRenderAtCellSize(int cellSize, List<MapLayer> layers)
        {
            if (layers.Count == 0) return true;

            try
            {
                int width = layers.Max(l => l.Data.GetLength(1)) * cellSize;
                int height = layers.Max(l => l.Data.GetLength(0)) * cellSize;

                // Just try to create it - if it fails, we know it won't work
                using (var testBitmap = new Bitmap(width, height))
                {
                    // Success! Dispose immediately, we just needed to test
                }

                return true;
            }
            catch (ArgumentException)
            {
                return false; // Invalid dimensions
            }
            catch (OutOfMemoryException)
            {
                return false; // Too big
            }
        }

        //It render ontop of the previous bitmap
        public void RenderPreviewAt(Point pt)
        {
            if (cachedBitmap == null) return;

            using (Graphics g = Graphics.FromImage(cachedBitmap))
            {
                Color color = Context.CurrentTool == ToolMode.Eraser ? Color.Black : GetColorForLayer(Context.CurrentLayer.Name);
                using (Brush brush = new SolidBrush(color))
                {
                    for (int dy = 0; dy < Context.BrushSize; dy++)
                    {
                        for (int dx = 0; dx < Context.BrushSize; dx++)
                        {
                            int cellX = pt.X + dx;
                            int cellY = pt.Y + dy;

                            g.FillRectangle(brush,
                                cellX * Context.CellSize, cellY * Context.CellSize,
                                Context.CellSize, Context.CellSize);
                        }
                    }
                }
            }
            OnMapDrawingFinished();
        }

        public void RenderLayers(List<MapLayer> mapLayersToRender)
        {
            if (mapLayersToRender.Count == 0)
            {
                if (cachedBitmap != null)
                {
                    using (Graphics g = Graphics.FromImage(cachedBitmap))
                    {
                        g.Clear(context.PanelBackColor);
                    }
                    OnMapDrawingFinished();
                }
                return;
            }
            int bitmapWidth = mapLayersToRender.Max(l => l.Data.GetLength(1)) * Context.CellSize;
            int bitmapHeight = mapLayersToRender.Max(l => l.Data.GetLength(0)) * Context.CellSize;
            cachedBitmap?.Dispose();
            cachedBitmap = new Bitmap(bitmapWidth, bitmapHeight);
            using (Graphics g = Graphics.FromImage(cachedBitmap))
            {
                var brushCache = new Dictionary<byte, SolidBrush>();
                try
                {
                    foreach (var layer in mapLayersToRender)
                    {
                        Color layerBaseColor = GetColorForLayer(layer.Name);

                        for (int y = 0; y < layer.Data.GetLength(0); y++)
                        {
                            for (int x = 0; x < layer.Data.GetLength(1); x++)
                            {
                                byte value = layer.Data[y, x];
                                if (value != 0)
                                {
                                    if (!brushCache.ContainsKey(value))
                                    {
                                        // Generate different color for each byte value
                                        brushCache[value] = new SolidBrush(GetColorForByteValue(value, layerBaseColor));
                                    }
                                    g.FillRectangle(brushCache[value], x * Context.CellSize, y * Context.CellSize, Context.CellSize, Context.CellSize);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    foreach (var brush in brushCache.Values)
                    {
                        brush.Dispose();
                    }
                    brushCache.Clear();
                }
            }
            OnMapDrawingFinished();
        }

        public Color GetColorForByteValue(byte value, Color baseColor)
        {
            if (value == 1)
            {
                return baseColor; // Original layer color for value 1
            }

            // Generate distinct colors for other values (2-255)
            // Use HSV color wheel for maximum distinction
            float hue = (value * 137.5f) % 360; // Golden angle for good distribution
            float saturation = 0.8f;
            float brightness = 0.9f;

            return ColorFromHSV(hue, saturation, brightness);
        }

        private Color ColorFromHSV(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);
            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
        public void RenderLine() {
            Point lineLogicalStart = coorConverter.ScreenToLogical(Context.lineScreenStart);
            Point lineLogicalEnd = coorConverter.ScreenToLogical(Context.lineScreenEnd);
            var points = GeometryUtils.GetLinePoints(lineLogicalStart, lineLogicalEnd);
            foreach (var pt in points) {
                RenderPreviewAt(pt);
            }
        }

        //public void RenderLayersAt(List<MapLayer> mapLayersToRender, Point dirtyScreenPoint)
        //{
        //    if (mapLayersToRender.Count == 0 || cachedBitmap == null) return;
        //    using (Graphics g = Graphics.FromImage(cachedBitmap))
        //    {
        //        // Convert screen to logical, then logical to bitmap
        //        Point dirtyLogicalPoint = coorConverter.ScreenToLogical(dirtyScreenPoint);
        //        Point dirtyBitmapPoint = coorConverter.LogicalToBitmap(dirtyLogicalPoint);

        //        var brushes = new Dictionary<string, SolidBrush>();
        //        try
        //        {
        //            foreach (MapLayer layer in mapLayersToRender)
        //            {
        //                if (!brushes.ContainsKey(layer.Name))
        //                {
        //                    brushes[layer.Name] = new SolidBrush(GetColorForLayer(layer.Name));
        //                }
        //                SolidBrush brush = brushes[layer.Name];

        //                for (int brushY = 0; brushY < Context.BrushSize; brushY++)
        //                {
        //                    for (int brushX = 0; brushX < Context.BrushSize; brushX++)
        //                    {
        //                        int logicalY = dirtyLogicalPoint.Y + brushY;
        //                        int logicalX = dirtyLogicalPoint.X + brushX;
        //                        if (layer.IsValidCoor(logicalX, logicalY) && layer.Data[logicalY, logicalX] == 1)
        //                        {
        //                            int screenX = dirtyBitmapPoint.X + brushX * Context.CellSize;
        //                            int screenY = dirtyBitmapPoint.Y + brushY * Context.CellSize;
        //                            g.FillRectangle(brush, screenX, screenY, Context.CellSize, Context.CellSize);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        finally
        //        {
        //            foreach (var brush in brushes.Values)
        //            {
        //                brush.Dispose();
        //            }
        //            brushes.Clear();
        //        }
        //    }
        //}

        public void RenderLayersAt(List<MapLayer> mapLayersToRender, Point dirtyScreenPoint, int alphaValue = 255)
        {
            if (mapLayersToRender.Count == 0 || cachedBitmap == null) return;
            using (Graphics g = Graphics.FromImage(cachedBitmap))
            {
                Point dirtyLogicalPoint = coorConverter.ScreenToLogical(dirtyScreenPoint);
                Point dirtyBitmapPoint = coorConverter.LogicalToBitmap(dirtyLogicalPoint);
                var brushCache = new Dictionary<byte, SolidBrush>();
                try
                {
                    foreach (MapLayer layer in mapLayersToRender)
                    {
                        Color layerBaseColor = GetColorForLayer(layer.Name);

                        for (int brushY = 0; brushY < Context.BrushSize; brushY++)
                        {
                            for (int brushX = 0; brushX < Context.BrushSize; brushX++)
                            {
                                int logicalY = dirtyLogicalPoint.Y + brushY;
                                int logicalX = dirtyLogicalPoint.X + brushX;

                                if (layer.IsValidCoor(logicalX, logicalY))
                                {
                                    byte value = layer.Data[logicalY, logicalX];
                                    if (value != 0)
                                    {
                                        if (!brushCache.ContainsKey(value))
                                        {
                                            Color color = GetColorForByteValue(value, layerBaseColor);
                                            brushCache[value] = new SolidBrush(Color.FromArgb(alphaValue, color));
                                        }

                                        int screenX = dirtyBitmapPoint.X + brushX * Context.CellSize;
                                        int screenY = dirtyBitmapPoint.Y + brushY * Context.CellSize;
                                        g.FillRectangle(brushCache[value], screenX, screenY, Context.CellSize, Context.CellSize);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    foreach (var brush in brushCache.Values)
                    {
                        brush.Dispose();
                    }
                    brushCache.Clear();
                }
            }
        }

        public Bitmap GetBitmap() {
            return cachedBitmap;
        }

        public Color GetColorForLayer(string layerName) {
            int hash = layerName.GetHashCode();
            return Color.FromArgb(Math.Abs(hash) % 256, Math.Abs(hash >> 8) % 256, Math.Abs(hash >> 16) % 256);
        }
    }
}