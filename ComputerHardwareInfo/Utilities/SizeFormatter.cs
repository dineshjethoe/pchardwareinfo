using System;

namespace ComputerHardwareInfo.Utilities
{
    public class BytesSizeConverter
    {
        private static readonly string[] SIZES = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public string ConvertBytesToReadableSize(long byteValue)
        {
            if (byteValue < 0)
                return "-" + ConvertBytesToReadableSize(-byteValue);
            if (byteValue == 0)
                return "0.0 bytes";

            int index = (int)Math.Log(byteValue, 1024);
            decimal size = (decimal)byteValue / (1L << (index * 10));
            return string.Format("{0:n1} {1}", size, SIZES[index]);
        }
    }
}
