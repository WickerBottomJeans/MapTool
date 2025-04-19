using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTool {

    internal static class ByteConverter {

        public static void FromBytes<T>(this T[,] array, byte[] buffer) where T : struct {
            var len = Math.Min(array.GetLength(0) * array.GetLength(1) * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), buffer.Length);
            Buffer.BlockCopy(buffer, 0, array, 0, len);
        }

        public static byte[] ToBytes<T>(this T[,] array) where T : struct {
            var buffer = new byte[array.GetLength(0) * array.GetLength(1) * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T))];
            Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}