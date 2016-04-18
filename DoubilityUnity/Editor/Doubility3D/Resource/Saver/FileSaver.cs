using System;
using System.Text;
using FlatBuffers;

using Doubility3D.Resource;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Saver
{
	static public class FileSaver
	{
		static public void Save (ByteBuffer buf, Context context, string filePath)
		{
			FlatBufferBuilder builder = new FlatBufferBuilder (16);
			Offset<Header> hdr = Header.CreateHeader (builder, FileVersion.Magic, context, FileVersion.version);
			builder.Finish (hdr.Value);
            
			ByteBuffer bufHeader = builder.DataBuffer;
			int lengthHeader = bufHeader.Length - bufHeader.Position;
			byte[] byteHeader = new byte[lengthHeader];
			Array.Copy (bufHeader.Data, bufHeader.Position, byteHeader, 0, lengthHeader);

			int length = buf.Length - buf.Position;
			byte[] bytes = new byte[length];
			Array.Copy (buf.Data, buf.Position, bytes, 0, length);

			System.IO.FileStream fs = System.IO.File.OpenWrite (filePath);
			if (fs != null) {
				fs.Write (byteHeader, 0, lengthHeader);
				fs.Write (bytes, 0, length);
				fs.Close ();
				fs = null;
			}
		}
	}
}
