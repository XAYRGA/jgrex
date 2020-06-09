using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace jgrex
{
    //JSETSET / JETGRIND AFS
    // 
    //
    /*
        AFS File
            0x00 int32 AFSHeader = 0x00534641;
            0x04 int32 sectionCount; 
            0x08 AFSSection[] sections;
            // This is actually sections[0].offset - 8  -- but in JSR / JGR, it seems to just always sit at 0x7FFF8
            0x7FF8 int32 fileTable;
            0x7FFC int32 fileTableLength; 
            [section data]
            @fileTable fileTable. 
            // FileTable length can be determined by dividing by 0x30, each file entry is 48 bytes long
            
        AFSSection
            0x00 int32 offset;
            0x04 int32 size; 

        AFSFileDescriptor 
            0x00 byte[16] name; // 16 bytes
            0x10 long un0;
            0x18 long un1;
            0x20 int32 un2;
            0x24 short un3;
            0x26 short un4;
            0x28 int32 un5;
            0x2C int32 length;
        

    */
    public class AFSFile
    {

        public int header;
        public int sectionCount;
        public AFSSection[] sections;
        public AFSFileDescriptor[] files;

        // End Binary Identical Struct
        private const int magic = 0x534641;
        public static AFSFile fromStream(Stream data)
        {

            // Reading AFS file header
            var br = new BinaryReader(data);
            var nf = new AFSFile()
            {
                header = br.ReadInt32(),
                sectionCount = br.ReadInt32(),
            };

            if (nf.header != magic)
                throw new InvalidDataException("Data is not an AFS file");

            nf.sections = new AFSSection[nf.sectionCount];

            for (int i=0; i < nf.sectionCount;i++)
            {
                nf.sections[i] = new AFSSection()
                {
                    offset = br.ReadInt32(),
                    length = br.ReadInt32()
                };
            }

            var w = nf.sections[0].offset;
            br.BaseStream.Position = w - 0x8; // hacks 
            var filetableOffset = br.ReadInt32();
            var filetableLength = br.ReadInt32();
            var filetableCount = filetableLength / 0x30; // The AFSFileDescriptor structure is 48 bytes long. 
            if (filetableOffset > 0 & filetableLength > 0)
            {
                cbug.write($"Filetable is at {filetableOffset:X} with {filetableCount:X} entries.");
                nf.files = new AFSFileDescriptor[filetableCount];
                br.BaseStream.Position = filetableOffset;
                for (int i=0; i < filetableCount; i++)
                {
                    nf.files[i] = new AFSFileDescriptor()
                    {
                        name = br.ReadBytes(16),
                        un0 = br.ReadInt64(),
                        un1 = br.ReadInt64(),
                        un2 = br.ReadInt32(),
                        un3 = br.ReadInt16(),
                        un4 = br.ReadInt16(),
                        un5 = br.ReadInt32(),
                        length = br.ReadInt32()
                    };
                }
            }
            return nf;
        }


    }

    public struct AFSSection
    {
        public int offset;
        public int length;
    }

    //@ Last Section Offset + Section Length
    public struct AFSFileDescriptor
    {
        public byte[] name; // 16 bytes
        public long un0;
        public long un1;
        public int un2;
        public short un3;
        public short un4;
        public int un5;
        public int length;
    }


}
