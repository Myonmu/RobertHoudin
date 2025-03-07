using System;
using Unity.Mathematics;
namespace RobertHoudin.Utils.RuntimeCompatible
{
    public static class DataPacking
    {
        private static byte[] _mergeBuffer = new byte[4];
        public static float PackToFloat(in ushort high, in ushort low)
        {
            BitConverter.GetBytes(high).CopyTo(_mergeBuffer,0);
            BitConverter.GetBytes(low).CopyTo(_mergeBuffer,2);
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