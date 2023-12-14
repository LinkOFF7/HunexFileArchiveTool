using System;
using System.IO;
using System.Text;

namespace HunexFileArchiveTool.Archive
{
    internal class Header
    {
        // HUNEX Global Game Engine File Archive v1.0
        public string Magic = "HUNEXGGEFA10";
        public UInt32 NumOfFiles { get; set; }
        public UInt32 InfoBlockSize => NumOfFiles * 0x80 + 0x10;
        public void Write(BinaryWriter writer) 
        {
            if (writer.BaseStream.Position != 0)
                writer.BaseStream.Position = 0;
            writer.Write(Encoding.ASCII.GetBytes(Magic));
            writer.Write(NumOfFiles);
        }
        public Header(uint numOfFiles) { NumOfFiles = numOfFiles; }
        public Header(BinaryReader reader) 
        { 
            if(reader.BaseStream.Position != 0)
                reader.BaseStream.Position = 0;
            if (Encoding.ASCII.GetString(reader.ReadBytes(12)) != Magic)
                throw new Exception("Unknown file.");
            this.NumOfFiles = reader.ReadUInt32();
        }

        public int GetVersion()
        {
            return int.Parse(Magic.Substring(10, 1));
        }
    }
}
