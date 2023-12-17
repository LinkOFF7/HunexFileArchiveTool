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
            if(args.Length == 2)
            {
                switch (args[0])
                {
                    case "--extract": Extract(args[1]); break;
                    case "--build": Build(args[1], args[1] + ".hfa");  break;
                    case "--patch": break;
                    default: PrintUsage(); break;
                }
            }
            else
            {
                PrintUsage();
                return;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("WITCH ON THE HOLY NIGHT HFA (Hunex) Archive Tool by LinkOFF");
            Console.WriteLine("");
            Console.WriteLine("Usage: <mode> <file/folder>");
            Console.WriteLine("  --extract\tExtracts all contents of given file archive.");
            Console.WriteLine("  --build\tBuild a new archive from given directory.");
            //Console.WriteLine("  --patch\tReplace existed file inside archive.");
        }

        static void Build(string inputDir, string hfaFile)
        {
            string[] files = Directory.GetFiles(inputDir, "*.*", SearchOption.TopDirectoryOnly);
            if(files.Length == 0)
            {
                Console.WriteLine("Directory ({0}) is empty.", inputDir);
                return;
            }
            using(BinaryWriter writer = new BinaryWriter(File.Create(hfaFile)))
            {
                _hdr = new Header((uint)files.Length);
                _hdr.Write(writer);
                _entries = new FileEntry[_hdr.FileCount];
                writer.BaseStream.Position = _hdr.DataStartOffset;
                for(int i = 0; i < _hdr.FileCount; i++)
                {
                    Console.WriteLine("Writing: {0}", files[i]);
                    byte[] data = File.ReadAllBytes(files[i]);
                    FileEntry fileEntry = new FileEntry();
                    fileEntry.FileName = Path.GetFileName(files[i]).Substring(5); // trim index
                    fileEntry.Size = (uint)data.Length;
                    fileEntry.Offset = (uint)(writer.BaseStream.Position - _hdr.DataStartOffset);
                    _entries[i] = fileEntry;
                    writer.Write(AlignData(data));
                }
                writer.BaseStream.Position = _hdr.HEADER_SIZE;
                foreach(var entry in _entries)
                    entry.Write(writer);
            }
        }

        static void Extract(string file)
        {
            using(BinaryReader reader = new BinaryReader(File.OpenRead(file)))
            {
                _hdr = new Header(reader);
                _entries = new FileEntry[_hdr.FileCount];
                for(int i = 0; i < _hdr.FileCount; i++)
                    _entries[i] = new FileEntry(reader);
                string outDir = Path.GetFileNameWithoutExtension(file);
                int index = 0; // There are files with the same names in the archive. And I want to keep the original sorting.
                foreach (var entry in _entries)
                {
                    reader.BaseStream.Position = _hdr.DataStartOffset + entry.Offset;
                    byte[] data = reader.ReadBytes((int)entry.Size);
                    string path = outDir + Path.DirectorySeparatorChar + $"{index++.ToString("D4")}_" + entry.FileName;
                    Console.WriteLine("Extracting: {0}", path);
                    if (!Directory.Exists(Path.GetDirectoryName(path))) Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllBytes(path, data);
                }
            }
        }

        static byte[] AlignData(byte[] data, int align = 0x8)
        {
            // Check if data must be aligned.
            if(data.Length % align != 0)
            {
                int len = align - (data.Length % align);
                byte[] buf = new byte[data.Length + len];
                Buffer.BlockCopy(data, 0, buf, 0, data.Length);
                for (int i = data.Length; i < len; i++) 
                    buf[i] = 0xFF;
            }
            return data;
        }
    }
}
