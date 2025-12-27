using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public static class SaveInfo
    {
        public static void Save()
        {
            /*StreamWriter sw = new StreamWriter(".\\ContactBooksSaveData.txt");
            var JsonSerObj = JsonSerializer.Serialize(СontactBook.ContactBooks);
            sw.WriteLine(JsonSerObj);
            sw.Close();*/
        }
    }
    public static class LoadInfo
    {
        public static void Load()
        {
            /*StreamReader sr = new StreamReader(".\\ContactBooksSaveData.txt");
            СontactBook.ContactBooks = JsonSerializer.Deserialize<List<СontactBook>>(sr.ReadToEnd());
            sr.Close();*/
        }
    }
}
