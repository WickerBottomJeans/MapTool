using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTool {

    internal class MapLayer {
        public string Name { get; set; }
        public Byte[,] Data { get; set; }

        public int mapPixelHeight { get; set; }
        public int mapPixelWidth { get; set; }

        public int mapStorageWidth { get; set; }
        public int mapStorageHeight {  get; set; }

        public int mapContentWidth { get; set; }
        public int mapContentHeight { get; set; }
        public MapLayer(string name, int mapHeight, int mapWidth, bool usingPixel) {
            if (usingPixel)
            {
                Name = name;
                this.mapPixelHeight = mapHeight;
                this.mapPixelWidth = mapWidth;
                setMapDimensions(this.mapPixelHeight, this.mapPixelWidth);
                Data = new Byte[this.mapStorageHeight, this.mapStorageWidth];
            } else
            {
                Name = name;
                Data = new byte[mapHeight, mapWidth];
            }
        }

        public void setMapDimensions(int mapPixelHeight, int mapPixelWidth)
        {
            if ((mapPixelHeight == 0) || (mapPixelWidth == 0))
            {
                return;
            } else
            {
                int MapGridWidth = 20;
                int MapGridHeight = 20;


                /// Số hàng và cột
                int numCols = (mapPixelHeight - 1) / MapGridWidth + 1;
                int numRows = (mapPixelWidth - 1) / MapGridHeight + 1;
                //set content dimesnion trc
                this.mapContentHeight = numRows;
                this.mapContentWidth = numCols;

                numCols = (int)Math.Ceiling(Math.Log(numCols, 2));
                numCols = (int)Math.Pow(2, numCols);

                numRows = (int)Math.Ceiling(Math.Log(numRows, 2));
                numRows = (int)Math.Pow(2, numRows);
                
                //set storage
                this.mapStorageHeight = numRows;
                this.mapStorageWidth = numCols;
            }
        }
        public void Rotate(int degrees) {
            if (Data == null) {
                return;
            }

            int originalWidth = Data.GetLength(1);
            int originalHeight = Data.GetLength(0);

            byte[,] rotatedMap = Data;

            if (degrees == 90) {
                rotatedMap = new byte[originalWidth, originalHeight];
                for (int y = 0; y < originalHeight; y++) {
                    for (int x = 0; x < originalWidth; x++) {
                        rotatedMap[originalWidth - 1 - x, y] = Data[y, x];
                    }
                }
            } else if (degrees == 180) {
                rotatedMap = new byte[originalHeight, originalWidth];
                for (int y = 0; y < originalHeight; y++) {
                    for (int x = 0; x < originalWidth; x++) {
                        rotatedMap[originalHeight - 1 - y, originalWidth - 1 - x] = Data[y, x];
                    }
                }
            } else if (degrees == 270 || degrees == -90) {
                rotatedMap = new byte[originalWidth, originalHeight];
                for (int y = 0; y < originalHeight; y++) {
                    for (int x = 0; x < originalWidth; x++) {
                        rotatedMap[x, originalHeight - 1 - y] = Data[y, x];
                    }
                }
            } else if (degrees != 0) {
                MessageBox.Show($"Rotation by {degrees} degrees is not supported. Please enter 0, 90, 180, or 270.", "Invalid Rotation Angle", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Data = rotatedMap;
        }

        public void Crop(int newHeight, int newWidth) {
            if (newWidth <= 0 || newHeight <= 0) {
                MessageBox.Show("Invalid dimensions for cropping.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int originalWidth = Data.GetLength(1);
            int originalHeight = Data.GetLength(0);
            byte[,] newData = new byte[newHeight, newWidth];

            int startY = originalHeight - newHeight;
            int startX = 0;

            int endY = originalHeight;
            int endX = newWidth;

            int newStartY = 0;
            int newStartX = 0;

            for (int y = newStartY; y < newHeight; y++) {
                for (int x = newStartX; x < newWidth; x++) {
                    int originalY = startY + y;
                    int originalX = startX + x;

                    if (originalX >= 0 && originalX < originalWidth && originalY >= 0 && originalY < originalHeight) {
                        newData[y, x] = Data[originalY, originalX];
                    } else {
                        newData[y, x] = 0;
                    }
                }
            }

            Data = newData;
        }

        public bool IsValidCoor(int x, int y) {
            return (x >= 0 && y >= 0 && y < Data.GetLength(0) && x < Data.GetLength(1));
        }

        public bool IsValidCoor(Point p) {
            return (p.X >= 0 && p.Y >= 0 && p.Y < Data.GetLength(0) && p.X < Data.GetLength(1));
        }
    }
}