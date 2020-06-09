using System;
using System.IO;
using System.Text;

namespace jgrex
{
    public static class root
    {
        static void Main(string[] args)
        {
            Console.WriteLine("jgrex");
            var w = File.OpenRead("DEMO30.afs");
            var afs = AFSFile.fromStream(w);
            for (int i = 0; i < afs.sections.Length; i++)
            {
                cbug.write($"Entry {i} is at 0x{afs.sections[i].offset:X} of length {afs.sections[i].length:X}");
            }
            for (int i = 0; i < afs.files.Length; i++)
            {
                var fileName = Encoding.ASCII.GetString(afs.files[i].name);
                cbug.write($"FileEntry {i}({fileName}) is length {afs.files[i].length:X}");
            }


        }

    }
}
