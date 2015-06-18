using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Collections;
using ICSharpCode.SharpZipLib.Checksums;
using AdvLib;

namespace ServicesLayer
{
    [ServiceContract]
    public class AdvFilesService
    {
        private AdvTool tool = new AdvTool();

        [OperationContract]
        public AdvFile Unpack(byte[] advContent)
        {
            return tool.Unpack(advContent);
        }

        [OperationContract]
        public AdvFile Unpack(Stream advStream)
        {
            return tool.Unpack(advStream);
        }

        [OperationContract]
        public byte[] PackToBytes(AdvFile advFile)
        {
            return tool.PackToBytes(advFile);
        }

        [OperationContract]
        public Stream PackToStream(AdvFile advFile)
        {
            return tool.PackToStream(advFile);
        }
    }
}
