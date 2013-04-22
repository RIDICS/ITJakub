using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using Jewelery;

namespace AdvLib
{
    public class AdvTool
    {

        /// <summary>
        /// Unpacks the archive and creates AdvFile object structure from it.
        /// </summary>
        /// <param name="stream">Stream with the ADV archive content.</param>
        /// <returns>Object representation of the ADV file</returns>
        public AdvFile Unpack(Stream stream)
        {
            using (ZipInputStream s = new ZipInputStream(stream))
            {
                AdvFile advFile = new AdvFile();

                ZipEntry entry;
                while ((entry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(entry.Name);
                    string fileName = Path.GetFileName(entry.Name);

                    // we dont use directories at ADV format for now
                    if (entry.IsFile)
                        if (fileName != String.Empty)
                        {
                            int size = 4096;
                            byte[] buffer = new byte[size];
                            Stream content = new MemoryStream();
                            while (size > 0)
                            {
                                size = s.Read(buffer, 0, buffer.Length);
                                if (size > 0)
                                    content.Write(buffer, 0, size);
                            }
                            content.Position = 0;
                            advFile.PutFile(fileName, content.StreamToByteArray());
                        }
                }

                return advFile;
            }
        }

        /// <summary>
        /// Unpacks the archive and creates AdvFile object structure from it.
        /// </summary>
        /// <param name="advContent">Byte array with the ADV archive content.</param>
        /// <returns>Object representation of the ADV file</returns>
        public AdvFile Unpack(byte[] advContent)
        {
            Stream inputStream = new MemoryStream(advContent);
            return Unpack(inputStream);
        }

        /// <summary>
        /// Packs the object representation to the archive in stream representation.
        /// </summary>
        /// <param name="advFile">ADV file to be packed</param>
        /// <returns>Stream with packaged (ZIP archived) ADV file</returns>
        public Stream PackToStream(AdvFile advFile)
        {
            // 0 - store only to 9 - means best compression
            const int COMPRESS_LEVEL = 7;

            Stream stream = new MemoryStream();
            try
            {
                ZipOutputStream zipOutput = null;

                Crc32 crc = new Crc32();
                zipOutput = new ZipOutputStream(stream);
                zipOutput.SetLevel(COMPRESS_LEVEL);

                string[] filenames = advFile.GetFileNames();

                foreach (string filename in filenames)
                {
                    // get content
                    byte[] buffer = advFile.GetFile(filename);

                    //StringUtil.GetBytes(file);

                    // generate zip header
                    ZipEntry entry = new ZipEntry(filename);
                    entry.DateTime = DateTime.Now;
                    entry.Size = buffer.Length;
                    entry.CompressionMethod = CompressionMethod.Deflated;

                    //crc.Reset();
                    //crc = new Crc32();
                    //crc.Update(buffer);
                    //entry.Crc = crc.Value;

                    // write header
                    zipOutput.PutNextEntry(entry);

                    // write content
                    zipOutput.Write(buffer, 0, buffer.Length);
                }
                zipOutput.Finish();
                //zipOutput.Close();
                zipOutput = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Packs the object representation to the archive in byte array representation.
        /// </summary>
        /// <param name="advFile">ADV file to be packed</param>
        /// <returns>Byte array with packaged (ZIP archived) ADV file</returns>
        public byte[] PackToBytes(AdvFile advFile)
        {
            Stream stream = PackToStream(advFile);
            return stream.StreamToByteArray();
        }
    }
}
