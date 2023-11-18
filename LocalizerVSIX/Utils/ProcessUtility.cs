using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizerVSIX.Utils
{

    internal class FluentProcess
    {

        Process _process;
        private FluentProcess(Process p) { _process = p; }

        public static FluentProcess Create(string filename, string arguments = "")
        {
            var _process = new Process();
            _process.StartInfo = new ProcessStartInfo()
            {
                FileName = filename,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            return new FluentProcess(_process);
        }
        public FluentProcess WithArguments(string args)
        {
            _process.StartInfo.Arguments = args;
            return this;
        }
        public FluentProcess WithWorkingDirectory(string dir)
        {
            _process.StartInfo.WorkingDirectory = dir;
            return this;
        }
        public FluentProcess Run()
        {
            _process.Start();
            return this;
        }
    }
}