using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doubility3D.Resource
{
    public static class FileVersion
    {
        static int magic = 0;

        public static int Magic
        {
            get { return FileVersion.magic; }
        }
        public const int version = 1;
        static FileVersion()
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes("DOUB");
            magic = System.BitConverter.ToInt32(byteArray, 0);
        }


    }
}
