using System;
using System.Collections.Generic;

using FlatBuffers;

using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource
{
    static public class FileHeader
    {
        static int magic = 0;
        const int version = 1;
        const int numberOfBytes = 12;

        public static int Magic
        {
            get { return FileHeader.magic; }
        }
        static FileHeader()
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes("DOUB");
            magic = System.BitConverter.ToInt32(byteArray, 0);
        }

        static public byte[] GetBytes(Schema.Context context)
        {
            byte[] bytes = new byte[numberOfBytes];
            ByteBuffer bf = new ByteBuffer(bytes);
            bf.PutInt(0, magic);
            bf.PutInt(4, version);
            bf.PutInt(8, (int)context);
            return bytes;
        }

        static public int NumberOfBytes
        {
            get { return numberOfBytes; }
        }

        static public Schema.Context FromBytes(byte[] bytes)
        {
			if (bytes.Length >= numberOfBytes) {
				ByteBuffer bf = new ByteBuffer (bytes);

				int _magic = bf.GetInt (0);
				int _version = bf.GetInt (4);
				Schema.Context _context = (Schema.Context)bf.GetInt (8);
				if(_magic == magic && _version == version){
					return _context;
				}
			}
			return Schema.Context.Unknown;
        }
    }
}
