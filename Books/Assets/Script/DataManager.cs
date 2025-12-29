using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    [System.Serializable]
    public class BookData
    {
        [Tooltip("제목")] public string title;
        [Tooltip("분류기호")] public float classNumber;
        
        [Tooltip("저자기호")] public string authorMark;
        [Tooltip("도서기호")] public int bookMark;
        [Tooltip("저작기호")] public string workMark;
        
        [Tooltip("권호기호")] public int volumeMark;
        [Tooltip("복본기호")] public int copyMark;
    }
    public class DataManager : MonoBehaviour
    {
        [SerializeField]
        private List<BookData> bookDatabase = new();

        public string path = "library_books";
        private void Awake()
        {
            LoadBookData();
        }

        private void LoadBookData()
        {
            TextAsset csvFile = Resources.Load<TextAsset>(path);
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
                        copyMark = int.Parse(values[5]),
                        title = values[6]
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

        public BookData GetBookData(int index)
        {
            return bookDatabase[index];
        }
    }
}