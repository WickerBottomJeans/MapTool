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

        public void RenderLayers(List<MapLayer> mapLayersToRender) {
            if (mapLayersToRender.Count == 0) {
                if (cachedBitmap != null) {
                    using (Graphics g = Graphics.FromImage(cachedBitmap)) {
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

            using (Graphics g = Graphics.FromImage(cachedBitmap)) {
                var brushes = new Dictionary<string, SolidBrush>();
                try {
                    foreach (var layer in mapLayersToRender) {
                        if (!brushes.ContainsKey(layer.Name)) {
                            brushes[layer.Name] = new SolidBrush(GetColorForLayer(layer.Name));
                        }
                        SolidBrush brush = brushes[layer.Name];

                        for (int y = 0; y < layer.Data.GetLength(0); y++) {
                            for (int x = 0; x < layer.Data.GetLength(1); x++) {
                                if (layer.Data[y, x] == 1) {
                                    g.FillRectangle(brush, x * Context.CellSize, y * Context.CellSize, Context.CellSize, Context.CellSize);
                                }
                            }
                        }
                    }
                } finally {
                    foreach (var brush in brushes.Values) {
                        brush.Dispose();
                    }
                    brushes.Clear();
                }
            }
            OnMapDrawingFinished();
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
                var brushes = new Dictionary<string, SolidBrush>();
                try
                {
                    foreach (MapLayer layer in mapLayersToRender)
                    {
                        if (!brushes.ContainsKey(layer.Name))
                        {
                            Color baseColor = GetColorForLayer(layer.Name);
                            brushes[layer.Name] = new SolidBrush(Color.FromArgb(alphaValue, baseColor));
                        }
                        SolidBrush brush = brushes[layer.Name];
                        for (int brushY = 0; brushY < Context.BrushSize; brushY++)
                        {
                            for (int brushX = 0; brushX < Context.BrushSize; brushX++)
                            {
                                int logicalY = dirtyLogicalPoint.Y + brushY;
                                int logicalX = dirtyLogicalPoint.X + brushX;
                                if (layer.IsValidCoor(logicalX, logicalY) && layer.Data[logicalY, logicalX] == 1)
                                {
                                    int screenX = dirtyBitmapPoint.X + brushX * Context.CellSize;
                                    int screenY = dirtyBitmapPoint.Y + brushY * Context.CellSize;
                                    g.FillRectangle(brush, screenX, screenY, Context.CellSize, Context.CellSize);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    foreach (var brush in brushes.Values)
                    {
                        brush.Dispose();
                    }
                    brushes.Clear();
                }
            }
        }

        public Bitmap GetBitmap() {
            return cachedBitmap;
        }

        private Color GetColorForLayer(string layerName) {
            int hash = layerName.GetHashCode();
            return Color.FromArgb(Math.Abs(hash) % 256, Math.Abs(hash >> 8) % 256, Math.Abs(hash >> 16) % 256);
        }
    }
}