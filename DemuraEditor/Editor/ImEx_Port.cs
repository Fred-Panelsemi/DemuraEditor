using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DemuraEditor.Editor
{
    // 將 Demura Table 匯入程式中
    internal class ImEx_Port
    {

        public ImEx_Port()
        {

            // 如果目標資料夾不存在，則建立
            if (!Directory.Exists(Backup_floder_path))
            {
                Directory.CreateDirectory(Backup_floder_path);
            }
            if (!Directory.Exists(Export_floder_path))
            {
                Directory.CreateDirectory(Export_floder_path);
            }
        }

        private List<string> mTestString = new List<string>();
        // Table 原始檔案 不可更改 用來回覆上一動
        private string[] arrayStr;
        // Table 原始檔案 可更改 
        private string[] arrayStr_CanChange;
        private string Original_Path = "";
        private string Original_folder_path = "";
        private string Original_UDMR_Name = "";
        private string Backup_Path = "";
        private static string Backup_floder_path = @"C:\DemuraEditor_BackUp";
        private static string Export_floder_path = @"C:\DemuraEditor_Export";
        private static string UID_Path = @"C:\UID";
        private string UID_Target_Path = "";
        private string UID_Target_Table = "";

        // 選取要進行更改的Table並將其資料備份於 【C:\DemuraEditor_BackUp】中
        public void OpenDemuraTable()
        {
            // 建立 OpenFileDialog 實例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 如果只需要特定類型檔案，可以設定過濾條件
            openFileDialog.Filter = "Text files (*.txt)|*.txt;*.UDMR|All files (*.*)|*.*";

            // 顯示對話框，使用者點選確定後回傳 true
            if (openFileDialog.ShowDialog() == true)
            {
                // 取得檔案路徑
                string selectedFilePath = openFileDialog.FileName;
                Original_UDMR_Name = System.IO.Path.GetFileName(selectedFilePath);

                // 取得檔案所在的資料夾路徑

                // 有可能會有問題 出錯時可以注意
#pragma warning disable CS8600 // 正在將 Null 常值或可能的 Null 值轉換為不可為 Null 的型別。
                string directoryPath = System.IO.Path.GetDirectoryName(selectedFilePath);
#pragma warning restore CS8600 // 正在將 Null 常值或可能的 Null 值轉換為不可為 Null 的型別。



                // 這個檔名會是 Unitary
                Original_folder_path = directoryPath;
                Original_Path = selectedFilePath;
            }

            /* 建立UID中的資料夾 由選擇的檔案最外面的資料夾命名 */
            // 這邊是找取 Unitary 的外層資料夾名稱
            DirectoryInfo di = new DirectoryInfo(Original_folder_path);
            // 名稱會是 xxxxxxx_d1 這個樣式
            string folderName = di.Parent.Name;
            // 在 C槽 的UID 資料夾中 建立相應可以進行GenBin的格式檔案
            UID_Target_Path = UID_Path + $"//{folderName.Replace("_d1", "")}//{folderName}//Unitary";
            // 如果目標資料夾不存在，則建立
            if (!Directory.Exists(UID_Target_Path))
            {
                Directory.CreateDirectory(UID_Target_Path);
            }

            /* 備份選取檔案 */
            // 取得來源資料夾中所有檔案（僅第一層，不包含子資料夾）
            string[] files = Directory.GetFiles(Original_folder_path);
            // 複製檔案並貼到創立的UID內的目標資料夾
            foreach (string file in files)
            {
                // 取得檔案名稱
                string fileName = System.IO.Path.GetFileName(file);
                // 建立目標檔案完整路徑
                string destFile = System.IO.Path.Combine(UID_Target_Path, fileName);
                // 複製檔案，若目標檔案存在則覆蓋
                File.Copy(file, destFile, true);
            }
            // 將新資料夾的位置與原來Table的檔名組合 組成目標Table的
            UID_Target_Table = UID_Target_Path + $"//{Original_UDMR_Name}";
            MessageBox.Show(UID_Target_Table);
        }

        public void ImportTxt()
        {
            string path = UID_Target_Table;
            using (StreamReader sr = new StreamReader(path))
            {
                string str = sr.ReadToEnd();
                arrayStr = Regex.Split(str, "\r\n");
                arrayStr_CanChange = Regex.Split(str, "\r\n"); ;
            }
            GC.Collect();
        }

        public void ExportTxt()
        {
            using (StreamWriter writer = new StreamWriter(@"C:\UID\Primaryplus_NGK_No1_G22\Primaryplus_NGK_No1_G22_d1\Unitary\Primaryplus_NGK_No1_G22.Udmr"))
            {
                foreach (var item in arrayStr)
                {
                    writer.WriteLine(item);
                }
            }
            GC.Collect();
        }

        public void Edit(int ModeFlag, int XBp, string UpDw, int Persent)
        {
            // 這邊是更改垂直的一條Pixel
            switch (ModeFlag)
            {
                case 1:
                    for (int i = Table_Total_1[XBp][0]; i < Table_Total_1[XBp][1]; i = i + Table_Total_1[XBp][2])
                    {
                        ChangeStringValue(ref arrayStr_CanChange[i], Persent);
                    }
                    break;
                case 2:
                    for (int i = Table_MTB_1[XBp][0]; i < Table_MTB_1[XBp][1]; i = i + Table_MTB_1[XBp][2])
                    {
                        ChangeStringValue(ref arrayStr_CanChange[i], Persent);
                    }
                    break;
            }
        }

        public void ChangeStringValue(ref string ArrStr, int Persent)
        {
            string[] spl = ArrStr.Split(',');
            double R = Convert.ToDouble(spl[0]);
            double G = Convert.ToDouble(spl[4]);
            double B = Convert.ToDouble(spl[8]);
            double R_1Persent = R / 100;
            double G_1Persent = G / 100;
            double B_1Persent = B / 100;
            R = R + (Persent * R_1Persent);
            G = G + (Persent * G_1Persent);
            B = B + (Persent * B_1Persent);
            ArrStr = $"{(int)R},0,0,0,{(int)G},0,0,0,{(int)B}";
        }

        private Dictionary<int, int[]> Table_Total_1 = new Dictionary<int, int[]>
        {
            
            // 整個螢幕最上&最下的1條Pixel
            {1,new int[]{6,966,1} },
            {2,new int[]{ 517446 ,518406,1} },
            // 整個螢幕的左右兩側
            {3,new int[]{ 6, 518406 ,960} },
            {4,new int[]{ 965, 518406 ,960} }
        };

        private Dictionary<int, int[]> Table_MTB_1 = new Dictionary<int, int[]>
        {
            // 垂直
            // X1 X2 X3 X4 的左右兩側的1條Poxel
            // X1
            {1,new int[]{6,518406, 960 } },
            {2,new int[]{ 245 ,518406, 960 } },
            // X2
            {3,new int[]{246,518406, 960 } },
            {4,new int[]{ 485 ,518406, 960 } },
            // X3
            {5,new int[]{486,518406, 960 } },
            {6,new int[]{ 725 ,518406, 960 } },
            // X4
            {7,new int[]{726,518406, 960 } },
            {8,new int[]{ 965 ,518406, 960 } }
        };

        // Tip
        // 
    }
}
