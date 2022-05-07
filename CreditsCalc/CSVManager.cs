using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CreditsCalc
{
    public static class CSVManager
    {
        public static void TryWriteCSV(string fileName, string writeString)
        {
            try
            {
                var file = new FileStream(fileName, FileMode.Create);
                var sw = new StreamWriter(file, Encoding.Default);
                sw.Write(writeString);
                sw.Close();
            }
            catch
            {
                MessageBox.Show("Перезаписываемый файл уже используется другой программой.\nЗакройте его и повторите попытку, либо сохраните как новый файл.", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}