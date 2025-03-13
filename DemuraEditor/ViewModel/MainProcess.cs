using CommunityToolkit.Mvvm.Input;
using DemuraEditor.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DemuraEditor.ViewModel
{
    internal class MainProcess : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Version mVersion;
        private ImEx_Port mImEx_Port;
        public string DemuraEditor_Version { get; }

        public ICommand Test { get; set; }
        public ICommand Save { get; set; }
        public ICommand Edit { get; set; }
        public ICommand ValueButton { get; set; }
        public ICommand ModeChoice { get; set; }
        public ICommand ChoiceBounding { get; set; }

        private string mChangeValue = "0";
        public string ChangeValue 
        { 
            get=> mChangeValue;
            set
            {
                mChangeValue = value;
                OnPropertyChanged();
            } 
        }

        private Visibility mTotalBounding = Visibility.Visible;
        public Visibility TotalBounding
        {
            get => mTotalBounding;
            set
            {
                mTotalBounding = value;
                OnPropertyChanged();
            }
        }

        private Visibility mMTBBounding = Visibility.Hidden;
        public Visibility MTBBounding
        {
            get => mMTBBounding;
            set
            {
                mMTBBounding = value;
                OnPropertyChanged();
            }
        }

        public MainProcess()
        {
            mVersion = new Version();
            mImEx_Port = new ImEx_Port();
            DemuraEditor_Version = $"v{mVersion.GetVersion()}";
            Test = new RelayCommand(Test_Action);
            Save = new RelayCommand(Save_Action);
            Edit = new RelayCommand<string>(Edit_Action);
            ValueButton = new RelayCommand<string>(ValueButton_Action);
            ModeChoice = new RelayCommand<string>(ModeChoice_Action);
            ChoiceBounding = new RelayCommand<string>(ChoiceBounding_Action);
        }

        private int XBChoiceFlag = 0;
        private int ModeChoiceFlag = 0;
        private void ChoiceBounding_Action(string? obj)
        {
            switch (obj)
            {
                case "Top":
                    XBChoiceFlag = 1;
                    break;
                case "Bottom":
                    XBChoiceFlag = 2;
                    break;
                case "Left":
                    XBChoiceFlag = 3;
                    break;
                case "Right":
                    XBChoiceFlag = 4;
                    break;
                case "X1L":
                    XBChoiceFlag = 1;
                    break;
                case "X1R":
                    XBChoiceFlag = 2;
                    break;
                case "X2L":
                    XBChoiceFlag = 3;
                    break;
                case "X2R":
                    XBChoiceFlag = 4;
                    break;
                case "X3L":
                    XBChoiceFlag = 5;
                    break;
                case "X3R":
                    XBChoiceFlag = 6;
                    break;
                case "X4L":
                    XBChoiceFlag = 7;
                    break;
                case "X4R":
                    XBChoiceFlag = 8;
                    break;
            }
            //MessageBox.Show(XBChoiceFlag.ToString());
        }

        private void ModeChoice_Action(string? obj)
        {
            if(obj == "Total")
            {
                TotalBounding = Visibility.Visible;
                MTBBounding = Visibility.Hidden;
                ModeChoiceFlag = 1;
            }
            else
            {
                TotalBounding = Visibility.Hidden;
                MTBBounding = Visibility.Visible;
                ModeChoiceFlag = 2;
            }
        }

        private void ValueButton_Action(string? obj)
        {
            ChangeValue = $"{obj}";
        }

        private void Test_Action()
        {
            //mImEx_Port.ImportTxt();
            //mImEx_Port.Demo();
        }
        private void Edit_Action(string? UpDw)
        {
            //mImEx_Port.Edit(UpDw,40);
            //mImEx_Port.OpenDemuraTable();
            
        }
        private void Save_Action()
        {
            //mImEx_Port.ExportTxt();
        }
    }
}
