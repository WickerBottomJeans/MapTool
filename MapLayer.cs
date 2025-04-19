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

        public MapLayer(string name, int height, int width) {
            Name = name;
            Data = new Byte[height, width];
        }

        public void Rotate(int degrees) {
            if (Data == null) {
                return;
            }

            int originalWidth = Data.GetLength(1);
            int originalHeight = Data.GetLength(0);

            // Normalize the degrees

            byte[,] rotatedMap = Data; // Default to original

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
                return; // Return the original map if the angle is invalid
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

            // Calculate the starting indices for copying from the original data
            int startY = originalHeight - newHeight;
            int startX = 0; // Bottom-left means starting from the left (x=0)

            // Calculate the ending indices for copying
            int endY = originalHeight;
            int endX = newWidth;

            // Calculate the starting indices for the new data
            int newStartY = 0;
            int newStartX = 0;

            // Iterate through the new data array and copy from the corresponding bottom-left section of the original
            for (int y = newStartY; y < newHeight; y++) {
                for (int x = newStartX; x < newWidth; x++) {
                    int originalY = startY + y;
                    int originalX = startX + x;

                    // Check if the original coordinates are within bounds
                    if (originalX >= 0 && originalX < originalWidth && originalY >= 0 && originalY < originalHeight) {
                        newData[y, x] = Data[originalY, originalX];
                    } else {
                        newData[y, x] = 0; // Default value if out of bounds (shouldn't happen in this logic if new dimensions are valid)
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