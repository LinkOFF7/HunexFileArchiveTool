using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HunexFileArchiveTool.Archive;

namespace HunexFileArchiveTool
{
    internal class Program
    {
        static Header _hdr;
        static FileEntry[] _entries;
        static void Main(string[] args)
        {
            if (args.Length == 1)
                Extract(args[0]);
            else return;
        }

        static void Extract(string file)
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(file)))
            {
                _hdr = new Header(reader);
                _entries = new FileEntry[_hdr.NumOfFiles];
                for(int i = 0; i < _hdr.NumOfFiles; i++)
                    _entries[i] = new FileEntry(reader);
                string outDir = Path.GetFileNameWithoutExtension(file);
                foreach(var entry in _entries)
                {
                    reader.BaseStream.Position = _hdr.InfoBlockSize + entry.Offset;
                    byte[] data = reader.ReadBytes((int)entry.Size);
                    string path = outDir + Path.DirectorySeparatorChar + entry.FilePath;
                    Console.WriteLine("Extracting: {0}", path);
                    if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllBytes(path, data);
                }
            }
        }
    }
}
