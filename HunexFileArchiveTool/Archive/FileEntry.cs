using System;
using System.IO;
using System.Text;

namespace HunexFileArchiveTool.Archive
{
    internal class FileEntry
    {
        public string FilePath { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }

        public FileEntry(BinaryReader reader)
        {
            this.FilePath = Encoding.UTF8.GetString(reader.ReadBytes(0x60)).Trim('\0');
            this.Offset = reader.ReadUInt32();
            this.Size = reader.ReadUInt32();
            reader.BaseStream.Position += 0x18; // some king of alignment?
        }

        public void Write(BinaryWriter writer)
        {
            byte[] namebuf = new byte[0x60];
            byte[] namebytes = Encoding.UTF8.GetBytes(this.FilePath);
            Buffer.BlockCopy(namebytes, 0, namebuf, 0, namebytes.Length);
            writer.Write(namebuf);
            writer.Write(Offset);
            writer.Write(Size);
            writer.Write(new byte[0x18]);
        }
    }
}
