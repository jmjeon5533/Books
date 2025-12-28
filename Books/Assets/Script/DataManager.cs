using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    [System.Serializable]
    public class BookData
    {
        public float classNumber;
        
        public string authorMark;
        public int bookMark;
        public string workMark;
        
        public int volumeMark;
        public int copyMark;
    }
    public class DataManager : MonoBehaviour
    {
        private List<BookData> bookDatabase = new();
        private void Awake()
        {
            LoadBookData();
        }

        private void LoadBookData()
        {
            TextAsset csvFile = Resources.Load<TextAsset>("library_books");
            if (csvFile == null)
            {
                Debug.Log("CSV를 찾을 수 없음");
                return;
            }
            string[] lines = csvFile.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = ParseCSVLine(lines[i]);
                if (values.Length >= 6)
                {
                    BookData bookNum = new BookData()  // class 인스턴스 생성
                    {
                        classNumber = float.Parse(values[0]),
                        authorMark = values[1],
                        bookMark = int.Parse(values[2]),
                        workMark = values[3],
                        volumeMark = int.Parse(values[4]),
                        copyMark = int.Parse(values[5])
                    };
                    bookDatabase.Add(bookNum);
                }
            }
        }
        private string[] ParseCSVLine(string line)
        {
            List<string> result = new();
            bool inQuotes = false;
            string field = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"') inQuotes = !inQuotes;
                else if (c == ',' && !inQuotes)
                {
                    result.Add(field.Trim('"'));
                    field = "";
                }
                else field += c;
            }
            result.Add(field.Trim('"'));
            return result.ToArray();
        }

        // GameManager에서 사용
        public BookData GetRandomBookData()  // BookNumberCSV → BookNumber
        {
            return bookDatabase[Random.Range(0, bookDatabase.Count)];
        }
    }
}