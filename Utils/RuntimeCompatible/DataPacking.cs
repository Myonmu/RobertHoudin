using System;
using Unity.Mathematics;
namespace RobertHoudin.Utils.RuntimeCompatible
{
    public static class DataPacking
    {
        private static byte[] _mergeBuffer = new byte[2];
        public static float PackToFloat(in ushort high, in ushort low)
        {
            _mergeBuffer[0] = (byte)(high);
            _mergeBuffer[1] = (byte)(low);
            return BitConverter.ToSingle(_mergeBuffer);
        }
        public static float PackToFloat(in half high, in half low)
        {
            return PackToFloat(high.value, low.value);
        }
        public static float PackToFloat(in float high, in float low)
        {
            return PackToFloat((half)high, (half)low);
        }
        
        public static float PackToFloat(in uint high, in uint low)
        {
            return PackToFloat((ushort)high, (ushort)low);
        }
    }
}