using System;
using System.IO;
using System.Text;

namespace HunexFileArchiveTool.Archive
{
    internal class FileEntry
    {
        public String FileName { get; set; }
        public UInt32 Offset { get; set; }
        public UInt32 Size { get; set; }
        public UInt32[] Reversed { get; set; }

        public FileEntry() { Reversed = new UInt32[6]; }
        public FileEntry(String filename, UInt32 offset, UInt32 size) 
        {
            FileName = filename;
            Offset = offset;
            Size = size;
            Reversed = new UInt32[6];
        }

        public FileEntry(BinaryReader reader)
        {
            this.FileName = Encoding.UTF8.GetString(reader.ReadBytes(0x60)).Trim('\0');
            this.Offset = reader.ReadUInt32();
            this.Size = reader.ReadUInt32();
            Reversed = new UInt32[6];
            for(int i = 0; i < 6; i++) 
                Reversed[i] = reader.ReadUInt32();
        }

        public void Write(BinaryWriter writer)
        {
            byte[] namebuf = new byte[0x60];
            byte[] namebytes = Encoding.UTF8.GetBytes(this.FileName);
            Buffer.BlockCopy(namebytes, 0, namebuf, 0, namebytes.Length);
            writer.Write(namebuf);
            writer.Write(Offset);
            writer.Write(Size);
            for (int i = 0; i < 6; i++)
                writer.Write(Reversed[i]);
        }
    }
}
