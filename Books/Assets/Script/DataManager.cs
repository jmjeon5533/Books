using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ExcelDataReader;

namespace Script
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        public string Alias { get; }
        public AliasAttribute(string alias) => Alias = alias;
    }

    [System.Serializable]
    public class BookData
    {
        [Alias("Index")] public int ID;
        [Alias("title")] public string title;
        [Alias("classNumber")] public float classNumber;

        [Alias("authorMark")] public string authorMark;
        [Alias("bookMark")] public int bookMark;
        [Alias("workMark")] public string workMark;

        [Alias("volumeMark")] public int volumeMark;
        [Alias("copyMark")] public int copyMark;
    }
    [System.Serializable]
    public class StageData
    {
        [Alias("Index")] public int ID;
        [Alias("BookID")] public int BookID;
    }

    public class DataManager : MonoBehaviour
    {
        [SerializeField] private List<BookData> bookDatabase = new();

        private string _path = "Assets/library_books.xlsm";

        private void Awake()
        {
            LoadBookData();
        }

        //BookData뿐만 아닌 StageData, 다른 클래스 타입도 파싱하고 저장할 수 있게 유동성 강화 필요
        private void LoadBookData()
        {
            using var stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();

            List<BookData> tempList = new();
            var bookTable = result.Tables[0]; // 해당 파일의 0번째 테이블만 있다고 가정

            for(int i = 1; i <bookTable.Rows.Count; i++)
            {
                var dataRow = bookTable.Rows[i]; // 두번째 가로줄부터 시작하는 데이터 한 줄
                if (string.IsNullOrEmpty(dataRow[0].ToString())) // 첫번째 칸이 비어있으면 없는 데이터라서 Break
                    break;

                var newData = new BookData();

                // BookData의 모든 필드 가져오기
                foreach (var field in typeof(BookData).GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    var attr = field.GetCustomAttribute<AliasAttribute>();
                    if (attr == null) continue; // Attribute 없으면 건너뛰기

                    var rootRow = bookTable.Rows[0];
                    var alias = attr.Alias;
                    var findIdx = rootRow.FindAliasIdx(alias,bookTable.Columns.Count);
                    if (findIdx == -1)
                        continue;

                    var data = dataRow[findIdx].ToString(); //실제 Alias와 같은 테이블의 데이터 값 하나

                    //Attribute가 있는 각각의 field의 Type
                    Type type = field.FieldType;
                    //(type)data 는 컴파일 시 타입이 확정되어야 하기에 사용 불가
                    field.SetValue(newData, Convert.ChangeType(data, type)); //동적 타입 형변환 함수 사용
                }

                tempList.Add(newData);
            }

            bookDatabase = tempList;
        }

        public BookData GetBookData(int index)
        {
            return bookDatabase[index];
        }
    }
}