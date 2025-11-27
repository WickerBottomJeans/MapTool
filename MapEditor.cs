using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool {

    //Class này chỉ dùng để edit các MapLayer.Data[,]
    internal class MapEditor {
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

        private CoorConverter coorConverter;

        public MapEditor(MapContext context) {
            Context = context;
            coorConverter = new CoorConverter(context);
        }

        public void DrawAt(Point logicalPointList) {
            for (int dy = 0; dy < Context.BrushSize; dy++) {
                for (int dx = 0; dx < Context.BrushSize; dx++) {
                    int x = logicalPointList.X + dx;
                    int y = logicalPointList.Y + dy;
                    if (Context.CurrentLayer.IsValidCoor(x, y)) {
                        Context.CurrentLayer.Data[y, x] = (byte)(Context.CurrentTool == ToolMode.Eraser ? 0 : 1);
                    }
                }
            }
        }

        public void DrawLine() {
            Point lineLogicalStart = coorConverter.ScreenToLogical(Context.lineScreenStart);
            Point lineLogicalEnd = coorConverter.ScreenToLogical(Context.lineScreenEnd);
            var points = GeometryUtils.GetLinePoints(lineLogicalStart, lineLogicalEnd);
            foreach (var pt in points) {
                DrawAt(pt);
            }
        }

        public void DrawRect() {
            Point rectStartLogical = coorConverter.ScreenToLogical(Context.rectScreenStartPoint);
            Point rectEndLogical = coorConverter.ScreenToLogical(Context.rectScreenEndPoint);

            int x1 = Math.Min(rectStartLogical.X, rectEndLogical.X);
            int y1 = Math.Min(rectStartLogical.Y, rectEndLogical.Y);
            int x2 = Math.Max(rectStartLogical.X, rectEndLogical.X);
            int y2 = Math.Max(rectStartLogical.Y, rectEndLogical.Y);

            for (int y = y1; y <= y2; y++) {
                for (int x = x1; x <= x2; x++) {
                    if (Context.CurrentLayer.IsValidCoor(x, y)) {
                        Context.CurrentLayer.Data[y, x] = 1;
                    }
                }
            }
        }

        public void FloodFill(List<MapLayer> visibleMapLayers, Point fillStartScreen) {
            if (visibleMapLayers.Count == 0) {
                return;
            }

            Byte[,] AllMapCombined = CreateCombinedBinaryMap(visibleMapLayers);
            Point fillStartLogical = coorConverter.ScreenToLogical(fillStartScreen);

            Queue<Point> byteQueue = new Queue<Point>();
            byteQueue.Enqueue(fillStartLogical);
            HashSet<Point> processed = new HashSet<Point>();

            byte fillByte = Context.CurrentLayer.Data[fillStartLogical.Y, fillStartLogical.X]; //the byte we will fill with
            byte targetByte = AllMapCombined[fillStartLogical.Y, fillStartLogical.X]; //the byte we clicked on

            while (byteQueue.Count > 0) {
                Point currentPoint = byteQueue.Dequeue();
                if (!Context.CurrentLayer.IsValidCoor(currentPoint) || processed.Contains(currentPoint) || AllMapCombined[currentPoint.Y, currentPoint.X] != targetByte) {
                    continue;
                }

                processed.Add(currentPoint);
                Context.CurrentLayer.Data[currentPoint.Y, currentPoint.X] = 1;

                byteQueue.Enqueue(new Point(currentPoint.X + 1, currentPoint.Y));
                byteQueue.Enqueue(new Point(currentPoint.X - 1, currentPoint.Y));
                byteQueue.Enqueue(new Point(currentPoint.X, currentPoint.Y + 1));
                byteQueue.Enqueue(new Point(currentPoint.X, currentPoint.Y - 1));
            }
        }

        private byte[,] CreateCombinedBinaryMap(List<MapLayer> visibleLayers) {
            if (visibleLayers == null || !visibleLayers.Any()) {
                return new byte[0, 0];
            }

            int maxWidth = visibleLayers.Max(l => l.Data.GetLength(1));
            int maxHeight = visibleLayers.Max(l => l.Data.GetLength(0));

            byte[,] allMap = new byte[maxHeight, maxWidth];
            for (int y = 0; y < maxHeight; y++) {
                for (int x = 0; x < maxWidth; x++) {
                    for (int i = 0; i < visibleLayers.Count; i++) {
                        MapLayer layer = visibleLayers[i];
                        if (layer.IsValidCoor(x, y) && layer.Data[y, x] == 1) {
                            allMap[y, x] = (byte)(i + 1);
                        }
                    }
                }
            }
            return allMap;
        }
        // Add these methods to MapEditor class:

        public void FillPolygon(List<Point> points)
        {
            if (points.Count < 3) return;

            // Get convex hull (outer boundary, orderless)
            List<Point> hull = ComputeConvexHull(points);

            // Fill inside the hull
            FillPolygonScanline(hull);
        }

        private List<Point> ComputeConvexHull(List<Point> points)
        {
            if (points.Count < 3) return points;

            // Sort points by X, then Y
            List<Point> sorted = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();

            List<Point> lower = new List<Point>();
            foreach (Point p in sorted)
            {
                while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                {
                    lower.RemoveAt(lower.Count - 1);
                }
                lower.Add(p);
            }

            List<Point> upper = new List<Point>();
            for (int i = sorted.Count - 1; i >= 0; i--)
            {
                Point p = sorted[i];
                while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                {
                    upper.RemoveAt(upper.Count - 1);
                }
                upper.Add(p);
            }

            lower.RemoveAt(lower.Count - 1);
            upper.RemoveAt(upper.Count - 1);
            lower.AddRange(upper);

            return lower;
        }

        private long Cross(Point O, Point A, Point B)
        {
            return (long)(A.X - O.X) * (B.Y - O.Y) - (long)(A.Y - O.Y) * (B.X - O.X);
        }

        private void FillPolygonScanline(List<Point> polygon)
        {
            if (polygon.Count < 3) return;

            // Find bounding box
            int minY = polygon.Min(p => p.Y);
            int maxY = polygon.Max(p => p.Y);
            int minX = polygon.Min(p => p.X);
            int maxX = polygon.Max(p => p.X);

            // For each scanline
            for (int y = minY; y <= maxY; y++)
            {
                List<int> intersections = new List<int>();

                // Find intersections with polygon edges
                for (int i = 0; i < polygon.Count; i++)
                {
                    Point p1 = polygon[i];
                    Point p2 = polygon[(i + 1) % polygon.Count];

                    if ((p1.Y <= y && p2.Y > y) || (p2.Y <= y && p1.Y > y))
                    {
                        float x = p1.X + (float)(y - p1.Y) / (p2.Y - p1.Y) * (p2.X - p1.X);
                        intersections.Add((int)x);
                    }
                }

                intersections.Sort();

                // Fill between pairs of intersections
                for (int i = 0; i < intersections.Count; i += 2)
                {
                    if (i + 1 < intersections.Count)
                    {
                        for (int x = intersections[i]; x <= intersections[i + 1]; x++)
                        {
                            if (Context.CurrentLayer.IsValidCoor(x, y))
                            {
                                Context.CurrentLayer.Data[y, x] = 1;
                            }
                        }
                    }
                }
            }
        }

    }
}