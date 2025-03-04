using CommunityToolkit.Mvvm.Input;
using DemuraEditor.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DemuraEditor.ViewModel
{
    internal class MainProcess
    {
        private Version mVersion;
        private ImEx_Port mImEx_Port;
        public string DemuraEditor_Version { get; }

        public ICommand Test { get; set; }
        public ICommand Save { get; set; }
        public ICommand Edit { get; set; }
        public MainProcess()
        {
            mVersion = new Version();
            mImEx_Port = new ImEx_Port();
            DemuraEditor_Version = $"v{mVersion.GetVersion()}";
            Test = new RelayCommand(Test_Action);
            Save = new RelayCommand(Save_Action);
            Edit = new RelayCommand<string>(Edit_Action);
        }

       
        private void Test_Action()
        {
            mImEx_Port.ImportTxt();
           
        }
        private void Edit_Action(string UpDw)
        {
            mImEx_Port.Edit(UpDw,40);
        }
        private void Save_Action()
        {
            mImEx_Port.ExportTxt();
        }
    }
}
