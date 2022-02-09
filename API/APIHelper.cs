using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Web.API
{
    public static class APIHelper
    {
        public static APIResponse<ResponseDecompiler> Decompile(byte[] luacFile)
        {
            APIResponse<ResponseDecompiler> result = new APIResponse<ResponseDecompiler>();
            result.status = "Ok";
            if(luacFile == null)
            {
                result.message = "Error";
                result.message = "Error, input file empty.";
                return result;
            }

            //luacFile = System.IO.File.ReadAllBytes(@"L:\Projects\LuaBytcodeInterpreter\lua_installer\files\upvalues.luac");
            try
            {
                string filename = $"tmp_{DateTime.UtcNow.Ticks}";
                File.WriteAllBytes(filename + ".luac", luacFile);

                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "luadec";
                    proc.StartInfo.Arguments = $"{filename}.luac";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.Start();

                    string output = proc.StandardOutput.ReadToEnd();
                    Console.WriteLine(output);

                    proc.WaitForExit(7 * 1000); // timeout after 7 sec?
                    if (!proc.HasExited)
                        proc.Kill();

                    result.data.decompiled = "-- Decompiled online using https://Lua-Decompiler.ferib.dev/ (luadec 2.0.2)\n";
                    result.data.decompiled += output;

                    File.Delete(filename);

                    if (proc.ExitCode != 0)
                    {
                        result.status = "Error";
                        result.message = "Unknown error during decompilation!";
                        return result;
                    }
                }
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                result.status = "Error";
                result.message = "Unknown error!";
                result.data.decompiled = "";
                return result;
            }
            return null;
        }
        
        public static APIResponse<ResponseBeautifier> Beautifie()
        {
            APIResponse<ResponseBeautifier> result = new APIResponse<ResponseBeautifier>();
            result.status = "N/A";
            result.message = "Error, not yet implemented!";
            return result;
        }
        
        public static APIResponse<ResponseHighlighter> Highlight()
        {
            APIResponse<ResponseHighlighter> result = new APIResponse<ResponseHighlighter>();
            result.status = "N/A";
            result.message = "Error, not yet implemented!";
            return result;
        }
    }
}
