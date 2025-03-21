using CommunityToolkit.Mvvm.Input;
using DemuraEditor.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static System.Windows.Forms.AxHost;

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
        public ICommand Open_C_UID_Folder { get; set; }
        public ICommand CopyGenBinName { get; set; }
        public ICommand OpenGenBinExe { get; set; }
        public ICommand OpenOrigTable { get; set; }
        public ICommand OpenPrimaryTool { get; set; }
        public ICommand Edit_DrawPreview { get; set; }
        public ICommand Recover { get; set; }

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

        private Visibility mCustomsizeBounding = Visibility.Hidden;
        public Visibility CustomsizeBounding
        {
            get => mCustomsizeBounding;
            set
            {
                mCustomsizeBounding = value;
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

        private string mGenBinName = "";
        public string GenBinName
        {
            get=> mGenBinName;
            set
            {
                mGenBinName= value;
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
            Open_C_UID_Folder = new RelayCommand(Open_C_UID_Folder_Action);
            CopyGenBinName = new RelayCommand(CopyGenBinName_Action);
            OpenGenBinExe = new RelayCommand(OpenGenBinExe_Action);
            OpenOrigTable = new RelayCommand(OpenOrigTable_Action);
            OpenPrimaryTool = new RelayCommand(OpenPrimaryTool_Action);
            Edit_DrawPreview = new RelayCommand(Edit_DrawPreview_Action);
            Recover = new RelayCommand(Recover_Action);
        }

        private void Recover_Action()
        {
            mImEx_Port.RecoverChangeTable();
            secondWindow.Recover();
        }

        private void OpenPrimaryTool_Action()
        {
            Process.Start(@"Primary.exe");
        }

        private void OpenOrigTable_Action()
        {
            mImEx_Port.OpenDemuraTable();
            GenBinName = mImEx_Port.GenBinName;

            // 取得所有螢幕
            var screens = System.Windows.Forms.Screen.AllScreens;

            if (screens.Length > 1) // 確保有延伸螢幕
            {
                var secondScreen = screens[1]; // 取得第二個螢幕

                var dpi = VisualTreeHelper.GetDpi(System.Windows.Application.Current.MainWindow);   // 獲取主螢幕的縮放倍率
                double dpiScaleX = dpi.DpiScaleX; // 水平縮放（1.0 = 100%）
                double dpiScaleY = dpi.DpiScaleY; // 垂直縮放

                secondWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                secondWindow.Left = secondScreen.Bounds.Left / dpiScaleX;
                secondWindow.Top = secondScreen.Bounds.Top / dpiScaleY;
                secondWindow.Width = 960;
                secondWindow.Height = 540;
            }


            secondWindow.Show();
            secondWindow.ImageChange();
        }

        private void OpenGenBinExe_Action()
        {
            Process.Start(@"GenBin_Primary.exe");
        }

        private void CopyGenBinName_Action()
        {
            if (!string.IsNullOrEmpty(GenBinName))
            {
                System.Windows.Clipboard.SetText(GenBinName);
                System.Windows.MessageBox.Show("文字已複製！");
            }
        }

        private void Open_C_UID_Folder_Action()
        {
            mImEx_Port.Open_C_UID_Folder();
        }

        private int XBChoiceFlag = 0;
        private int ModeChoiceFlag = 0;
        private string HrV = "";
        private void ChoiceBounding_Action(string? obj)
        {
            switch (obj)
            {
                case "Top":
                    XBChoiceFlag = 1;
                    StartX = 0;
                    StartY = 0;
                    End = 959;
                    HrV = "H";
                    break;
                case "Bottom":
                    XBChoiceFlag = 2;
                    HrV = "H";
                    StartX = 0;
                    StartY = 539;
                    End = 959;
                    break;
                case "Left":
                    XBChoiceFlag = 3;
                    HrV = "V";
                    StartX = 0;
                    StartY = 0;
                    End = 539;
                    break;
                case "Right":
                    XBChoiceFlag = 4;
                    HrV = "V";
                    StartX = 959;
                    StartY = 0;
                    End = 539;
                    break;
                case "X1L":
                    XBChoiceFlag = 1;
                    HrV = "V";
                    StartX = 0;
                    StartY = 0;
                    End = 539;
                    break;
                case "X1R":
                    XBChoiceFlag = 2;
                    HrV = "V";
                    StartX = 239;
                    StartY = 0;
                    End = 539;
                    break;
                case "X2L":
                    XBChoiceFlag = 3;
                    HrV = "V";
                    StartX = 240;
                    StartY = 0;
                    End = 539;
                    break;
                case "X2R":
                    XBChoiceFlag = 4;
                    HrV = "V";
                    StartX = 479;
                    StartY = 0;
                    End = 539;
                    break;
                case "X3L":
                    XBChoiceFlag = 5;
                    HrV = "V";
                    StartX = 480;
                    StartY = 0;
                    End = 539;
                    break;
                case "X3R":
                    XBChoiceFlag = 6;
                    HrV = "V";
                    StartX = 719;
                    StartY = 0;
                    End = 539;
                    break;
                case "X4L":
                    XBChoiceFlag = 7;
                    HrV = "V";
                    StartX = 720;
                    StartY = 0;
                    End = 539;
                    break;
                case "X4R":
                    XBChoiceFlag = 8;
                    HrV = "V";
                    StartX = 959;
                    StartY = 0;
                    End = 539;
                    break;
                case "H":
                    HrV = "H";
                    break;
                case "V":
                    HrV = "V";
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
                CustomsizeBounding = Visibility.Hidden;
                ModeChoiceFlag = 1;
            }
            else if(obj == "MTB")
            {
                TotalBounding = Visibility.Hidden;
                MTBBounding = Visibility.Visible;
                CustomsizeBounding = Visibility.Hidden;
                ModeChoiceFlag = 2;
            }
            else
            {
                TotalBounding = Visibility.Hidden;
                MTBBounding = Visibility.Hidden;
                CustomsizeBounding = Visibility.Visible;
                ModeChoiceFlag = 3;
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
            mImEx_Port.ImportTxt();
            mImEx_Port.Edit(ModeChoiceFlag, XBChoiceFlag, Convert.ToInt32(ChangeValue));
            mImEx_Port.ExportTxt();
        }


        //---------------------------------------------------------------------------------------------
        private int mStartX = 0;
        public int StartX
        {
            get => mStartX;
            set
            {
                mStartX = value;
                OnPropertyChanged();
            }
        }

        private int mStartY = 0;
        public int StartY
        {
            get => mStartY;
            set
            {
                mStartY = value;
                OnPropertyChanged();
            }
        }

        private int mEnd = 0;
        public int End
        {
            get => mEnd;
            set
            {
                mEnd = value;
                OnPropertyChanged();
            }
        }
        private void Edit_DrawPreview_Action()
        {
            if(HrV == "H")
            {
                secondWindow.Adjust(HrV, StartX, End, StartY,Convert.ToInt16(ChangeValue));
            }
            else
            {
                secondWindow.Adjust(HrV, StartY, End, StartX, Convert.ToInt16(ChangeValue));
            }
            
        }

        HDMIView secondWindow = new HDMIView();
        private void Save_Action()
        {


            

            // 取得所有螢幕
            var screens = System.Windows.Forms.Screen.AllScreens;

            if (screens.Length > 1) // 確保有延伸螢幕
            {
                var secondScreen = screens[1]; // 取得第二個螢幕
              
                var dpi = VisualTreeHelper.GetDpi(System.Windows.Application.Current.MainWindow);   // 獲取主螢幕的縮放倍率
                double dpiScaleX = dpi.DpiScaleX; // 水平縮放（1.0 = 100%）
                double dpiScaleY = dpi.DpiScaleY; // 垂直縮放

                secondWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                secondWindow.Left = secondScreen.Bounds.Left/ dpiScaleX;
                secondWindow.Top = secondScreen.Bounds.Top/ dpiScaleY;
                secondWindow.Width = 960;
                secondWindow.Height = 540;
            }

            
            secondWindow.Show();
            secondWindow.ImageChange();
            //secondWindow.Adjust("H",240,960,405);
            //secondWindow.Adjust("V",270,540,750);

        }
    }
}
