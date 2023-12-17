using System;
using System.IO;
using System.Text;

namespace HunexFileArchiveTool.Archive
{
    internal class Header
    {
        public UInt32 HEADER_SIZE = 0x10;
        public UInt32 ENTRY_SIZE = 0x80;

        // HUNEX Global Game Engine File Archive v1.0
        public String Magic = "HUNEXGGEFA10";
        public UInt32 FileCount { get; set; }
        public UInt32 DataStartOffset => FileCount * ENTRY_SIZE + HEADER_SIZE;
        public void Write(BinaryWriter writer) 
        {
            if (writer.BaseStream.Position != 0)
                writer.BaseStream.Position = 0;
            writer.Write(Encoding.ASCII.GetBytes(Magic));
            writer.Write(FileCount);
        }
        public Header(uint numOfFiles) { FileCount = numOfFiles; }
        public Header(BinaryReader reader) 
        { 
            if(reader.BaseStream.Position != 0)
                reader.BaseStream.Position = 0;
            if (Encoding.ASCII.GetString(reader.ReadBytes(12)) != Magic)
                throw new Exception("Unknown file.");
            this.FileCount = reader.ReadUInt32();
        }

        public int GetVersion()
        {
            return int.Parse(Magic.Substring(10, 1));
        }
    }
}
