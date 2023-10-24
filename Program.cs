using dnlib.DotNet;
using dnlib.DotNet.Writer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmDotTrialRemover
{
    internal class Program
    {
        public static ModuleDefMD module;

        private static void Main(string[] args)
        {
            Console.Title = "Armdot Trial Remover";

            string path = args[0];

            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[-] File not found!");
                Console.Read();
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[+] File found!");
            module = ModuleDefMD.Load(path);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[+] Assembly loaded!");

            RemoveTrial(module);

            ModuleWriterOptions moduleWriterOptions = new ModuleWriterOptions(module);
            string filename = string.Concat(new string[]
            {
                Path.GetDirectoryName(path),
                "\\",
                Path.GetFileNameWithoutExtension(path),
                "_TrialRemoved",
                Path.GetExtension(path)
            });
            if (module.IsILOnly)
            {
                module.Write(filename, moduleWriterOptions);
            }
            else
            {
                NativeModuleWriterOptions nativeModuleWriterOptions = new NativeModuleWriterOptions(module, optimizeImageSize: true);
                module.NativeWrite(filename, nativeModuleWriterOptions);
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[+] File saved");
            Console.WriteLine(filename);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        public static void RemoveTrial(ModuleDef module)
        {
            foreach (MethodDef md in module.GlobalType.Methods)
            {
                if (md.Name != ".cctor") continue;
                module.GlobalType.Remove(md);
                break;
            }
        }
    }
}