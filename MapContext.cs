using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool {

    internal class MapContext {
        public int CellSize { get; set; }
        public Point PanOffset { get; set; }
        public int BrushSize { get; set; }
        public ToolMode CurrentTool { get; set; }

        public Point lineScreenStart { get; set; }
        public Point lineScreenEnd { get; set; }
        public MapLayer CurrentLayer { get; set; }

        public Point rectScreenEndPoint { get; set; }
        public Point rectScreenStartPoint { get; set; }
        public Color PanelBackColor { get; set; }

        public int maxZoomInValue { get; set; }
    }
}