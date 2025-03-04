using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DemuraEditor.Editor
{
    // 將 Demura Table 匯入程式中
    internal class ImEx_Port
    {

        public ImEx_Port()
        {

        }

        private List<string> mTestString = new List<string>();
        private string[] arrayStr;

        public void ImportTxt()
        {
            string path = @"C:\UID\NGK_No1\Primaryplus_NGK_No1_G22_d1\Unitary\Primaryplus_NGK_No1_G22.Udmr";
            using (StreamReader sr = new StreamReader(path))
            {
                string str = sr.ReadToEnd();
                arrayStr = Regex.Split(str, "\r\n");
            }
            GC.Collect();
        }

        // 色域 1 > 6 ~ 518406
        // 色域 2 > 518407 ~ 1036807
        // 色域 3 > 1036808 ~ 1555208
        // 色域 4 > 
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

        public void Edit(string UpDw, int Persent)
        {
            // 先只調整XB1 左邊最左邊第一、二、三排
            switch (UpDw)
            {
                case "Up":
                    for (int i = 6; i < 518406; i = i + 960)
                    {
                        ChangeStringValue(ref arrayStr[i], Persent);
                        ChangeStringValue(ref arrayStr[i+1], Persent);
                        ChangeStringValue(ref arrayStr[i+2], Persent);
                    }
                    for (int i = 246; i < 518406; i = i + 960)
                    {
                        ChangeStringValue(ref arrayStr[i - 1], Persent);
                        ChangeStringValue(ref arrayStr[i - 2], Persent);
                        ChangeStringValue(ref arrayStr[i], Persent);
                        ChangeStringValue(ref arrayStr[i + 1], Persent);
                        ChangeStringValue(ref arrayStr[i + 2], Persent);
                    }
                    break;
                case "Dw":
                    for (int i = 6; i < 518406; i = i + 960)
                    {
                        ChangeStringValue(ref arrayStr[i], -Persent);
                        ChangeStringValue(ref arrayStr[i+1], -Persent);
                        ChangeStringValue(ref arrayStr[i+2], -Persent);
                    }
                    for (int i = 246; i < 518406; i = i + 960)
                    {
                        ChangeStringValue(ref arrayStr[i - 1], -Persent);
                        ChangeStringValue(ref arrayStr[i - 2], -Persent);
                        ChangeStringValue(ref arrayStr[i], -Persent);
                        ChangeStringValue(ref arrayStr[i + 1], -Persent);
                        ChangeStringValue(ref arrayStr[i + 2], -Persent);
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
    }
}
