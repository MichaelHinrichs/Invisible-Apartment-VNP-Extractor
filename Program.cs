//Written for Invisible Apartment. https://store.steampowered.com/app/351790
using System.IO;

namespace Invisible_Apartment_VNP_Extractor
{
    class Program
    {
        static BinaryReader br;
        static string path;
        static int padTo = 0x100;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            if (new string(br.ReadChars(4)) != "vnpc")
                throw new System.ArgumentException("This file is not a Invisible Apartment vni file.");

            path = Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);

            br.ReadInt32(); //end
            br.ReadInt32(); //size
            string name = new string(br.ReadChars(32)).TrimEnd('\0');

            br.ReadInt32();//Unknown. Always 2.
            br.ReadInt64();//Unknown.

            br.BaseStream.Position = 0x100;

            while(br.BaseStream.Position < br.BaseStream.Length)
            {
                string block = new string(br.ReadChars(4));
                int end = br.ReadInt32();
                int size = br.ReadInt32();
                name = new string(br.ReadChars(0xc0)).TrimEnd('\0');
                br.ReadInt64();//Unknown.

                Pad();

                switch (block)
                {
                    case "sgmt":
                        break;
                    case "file":
                        long start = br.BaseStream.Position;
                        Directory.CreateDirectory(path + "//" + Path.GetDirectoryName(name));
                        BinaryWriter bw = new(File.Create(path + "//" + name));
                        bw.Write(br.ReadBytes(size));
                        bw.Close();
                        br.BaseStream.Position = start + end;
                        break;
                    case "sign":
                        br.ReadBytes(size);
                        Pad();
                        break;
                }

            }
        }

        public static  void Pad()
        {
            while (padTo < br.BaseStream.Position)
                padTo += 0x100;
            br.BaseStream.Position = padTo;
        }
    }
}
