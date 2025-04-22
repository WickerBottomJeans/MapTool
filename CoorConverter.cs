using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTool {

    // Lớp này dùng để chuyển đổi giữa các hệ tọa độ khác nhau:
    // - Tọa độ logic (logical coor): Được sử dụng để đại diện cho các điểm dữ liệu trong bản đồ (MapLayer.Data[,]).
    // - Tọa độ màn hình (screen coor): Được sử dụng để đại diện cho vị trí điểm ảnh tương đối với cửa sổ (ví dụ: từ sự kiện MouseDown).
    // - Tọa độ Bitmap (bitmap coor): Được sử dụng để vẽ lên bitmap
    internal class CoorConverter {
        public MapContext Context { get; set; }

        public CoorConverter(MapContext context) {
            Context = context;
        }

        public Point ScreenToLogical(Point screenPoint) {
            return new Point(
                (screenPoint.X - Context.PanOffset.X) / Context.CellSize,
                (screenPoint.Y - Context.PanOffset.Y) / Context.CellSize
            );
        }

        public Point LogicalToScreen(Point logicalPoint) {
            return new Point(
                logicalPoint.X * Context.CellSize + Context.PanOffset.X,
                logicalPoint.Y * Context.CellSize + Context.PanOffset.Y
            );
        }

        public Point ScreenToBitmap(Point screenPoint) {
            return new Point(
                screenPoint.X - Context.PanOffset.X,
                screenPoint.Y - Context.PanOffset.Y
            );
        }

        public Rectangle LogicalToScreen(Rectangle logicalRect) {
            return new Rectangle(
                logicalRect.X * Context.CellSize + Context.PanOffset.X,
                logicalRect.Y * Context.CellSize + Context.PanOffset.Y,
                logicalRect.Width * Context.CellSize,
                logicalRect.Height * Context.CellSize
            );
        }

        public Rectangle ScreenToLogical(Rectangle screenRect) {
            return new Rectangle(
                (screenRect.X - Context.PanOffset.X) / Context.CellSize,
                (screenRect.Y - Context.PanOffset.Y) / Context.CellSize,
                screenRect.Width / Context.CellSize,
                screenRect.Height / Context.CellSize
            );
        }
    }
}