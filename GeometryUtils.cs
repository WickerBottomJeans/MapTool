using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTool {

    //Tạo class riêng dù bây giờ chỉ dùng mỗi GetLinePoints, nhỡ sau này có GetCirclePoints ...
    internal static class GeometryUtils {

        public static List<Point> GetLinePoints(Point p0, Point p1) {
            List<Point> points = new List<Point>();

            int x0 = p0.X;
            int y0 = p0.Y;
            int x1 = p1.X;
            int y1 = p1.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true) {
                points.Add(new Point(x0, y0));

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy) {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx) {
                    err += dx;
                    y0 += sy;
                }
            }

            return points;
        }
    }
}