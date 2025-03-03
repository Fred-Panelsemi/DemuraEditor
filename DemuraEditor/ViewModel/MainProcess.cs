using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DemuraEditor.ViewModel
{
    internal class MainProcess
    {
        private Version mVersion;
        public string DemuraEditor_Version { get; }
        public MainProcess()
        {
            mVersion = new Version();
            DemuraEditor_Version = $"v{mVersion.GetVersion()}";
            
        }
    }
}
