using System;
using System.IO;

namespace SevenZip.Compression.LZMA
{
    public static class SevenZipHelper
    {
        static int dictionary = 1 << 23;
        static bool eos = false;
        static CoderPropID[] propIDs =
                   {
                    CoderPropID.DictionarySize,
                    CoderPropID.PosStateBits,
                    CoderPropID.LitContextBits,
                    CoderPropID.LitPosBits,
                    CoderPropID.Algorithm,
                    CoderPropID.NumFastBytes,
                    CoderPropID.MatchFinder,
                    CoderPropID.EndMarker
                };
        static object[] properties =
                   {
                    (Int32)(dictionary),
                    (Int32)(2),
                    (Int32)(3),
                    (Int32)(0),
                    (Int32)(2),
                    (Int32)(128),
                    "bt4",
                    eos
                };
        public static byte[] Compress(byte[] inputBytes)
        {
            MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            Encoder encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            long fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            return outStream.ToArray();
        }
        public static byte[] Decompress(byte[] inputBytes)
        {
            MemoryStream newInStream = new MemoryStream(inputBytes);
            Decoder decoder = new Decoder();
            newInStream.Seek(0, 0);
            MemoryStream newOutStream = new MemoryStream();
            byte[] properties2 = new byte[5];
            if (newInStream.Read(properties2, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = newInStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            decoder.SetDecoderProperties(properties2);
            long compressedSize = newInStream.Length - newInStream.Position;
            decoder.Code(newInStream, newOutStream, compressedSize, outSize, null);
            byte[] b = newOutStream.ToArray();
            return b;
        }

        public static void Zip(string OriPath, string destPath)
        {
            FileStream inStream = new FileStream(OriPath, FileMode.Open);
            FileStream outStream = new FileStream(destPath, FileMode.Create);
            Encoder encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            Int64 fileSize;
            fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((Byte)(fileSize >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            //关闭文件流
            inStream.Close();
            outStream.Close();
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="OriPath">原文件地址</param>
        /// <param name="destPath">目标文件</param>
        public static void Unzip(string OriPath, string destPath)
        {
            FileStream inStream = new FileStream(OriPath, FileMode.Open);
            FileStream outStream = new FileStream(destPath, FileMode.OpenOrCreate);
            Decoder decoder = new Decoder();
            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            decoder.SetDecoderProperties(properties);
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            long compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, null);
            //关闭文件流
            inStream.Close();
            outStream.Close();
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="OriPath">原文件地址</param>
        /// <param name="destPath">目标文件</param>
        public static byte[] Unzip(byte[] inputBytes)
        {
            MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            inStream.Seek(0, 0);
            Decoder decoder = new Decoder();
            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            decoder.SetDecoderProperties(properties);
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            long compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, null);
            byte[] b = outStream.ToArray();
            return b;
        }
        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="OriPath">原文件地址</param>
        /// <param name="destPath">目标文件</param>
        public static byte[] Unzip(string filename)
        {
            FileStream inStream = new FileStream(filename, FileMode.Open);
            MemoryStream outStream = new MemoryStream();
            Decoder decoder = new Decoder();
            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw (new Exception("input .lzma is too short"));
            decoder.SetDecoderProperties(properties);
            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int v = inStream.ReadByte();
                if (v < 0)
                    throw (new Exception("Can't Read 1"));
                outSize |= ((long)(byte)v) << (8 * i);
            }
            long compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, null);
            //关闭文件流
            inStream.Close();
            //outStream.Close();
            byte[] b = outStream.ToArray();
            return b;
        }
    }
}
