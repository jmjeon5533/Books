using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ExcelDataReader;
using UnityEngine.Pool;

namespace Script
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AliasAttribute : Attribute
    {
        public string Alias { get; }
        public AliasAttribute(string alias) => Alias = alias;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TablePathAttribute : Attribute
    {
        public string Path { get; }
        public TablePathAttribute(string path) => Path = path;
    }

    public class BaseData
    {
        public virtual void OnCompleteRow(DataManager dataMgr)
        {
        }
    }

    [Serializable]
    [TablePath("library_books")]
    public class BookData : BaseData
    {
        [Alias("Index")] public int ID;
        [Alias("title")] public string title;
        [Alias("classNumber")] public float classNumber;

        [Alias("authorMark")] public string authorMark;
        [Alias("bookMark")] public int bookMark;
        [Alias("workMark")] public string workMark;

        [Alias("volumeMark")] public int volumeMark;
        [Alias("copyMark")] public int copyMark;

        public override void OnCompleteRow(DataManager dataMgr)
        {
            base.OnCompleteRow(dataMgr);

            dataMgr.bookDatabase.Add(this);
        }
    }

    [Serializable]
    [TablePath("library_books 1")]
    public class StageData : BaseData
    {
        [Alias("Index")] public int ID;
        [Alias("BookID")] public int BookID;

        public override void OnCompleteRow(DataManager dataMgr)
        {
            base.OnCompleteRow(dataMgr);

            dataMgr.stageDataBase.Add(this);
        }
    }

    public class DataManager : MonoBehaviour
    {
        [SerializeField] public List<BookData> bookDatabase = new();
        [SerializeField] public List<StageData> stageDataBase = new();
        private void Awake()
        {
            LoadData();
        }

        //BookData뿐만 아닌 StageData, 다른 클래스 타입도 파싱하고 저장할 수 있게 유동성 강화 필요
        //- 어떤 List에 들어가야할 지 모름
        //- 데이터의 종류가 어떤게 들어올 지 모름
        private void LoadData()
        {
            //-만약 Field의 Type이 List<BaseData>와 다르면 continue
            List<(Type type, string path)> tableInfo = new();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!typeof(BaseData).IsAssignableFrom(type)) continue;
                var att = type.GetCustomAttribute<TablePathAttribute>();
                if (att == null) continue;
                tableInfo.Add((type, att.Path));
            }
            
            foreach (var tuple in tableInfo)
            {
                var curType = tuple.type;
                var path = tuple.path;
                path = $"Assets/{path}.xlsm";
                
                using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet();

                var bookTable = result.Tables[0]; // 해당 파일의 0번째 테이블만 있다고 가정
                for (int i = 1; i < bookTable.Rows.Count; i++)
                {
                    var dataRow = bookTable.Rows[i]; // 두번째 가로줄부터 시작하는 데이터 한 줄
                    if (string.IsNullOrEmpty(dataRow[0].ToString())) // 첫번째 칸이 비어있으면 없는 데이터라서 Break
                        break;

                    var newData = Activator.CreateInstance(curType) as BaseData;
                    if (newData is null)
                        Debug.LogError($"{curType.FullName}이 BaseData가 아닙니다");

                    // BookData의 모든 필드 가져오기
                    foreach (var field in curType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        var attr = field.GetCustomAttribute<AliasAttribute>();
                        if (attr == null) continue; // Attribute 없으면 건너뛰기

                        var rootRow = bookTable.Rows[0];
                        var alias = attr.Alias;
                        var findIdx = rootRow.FindAliasIdx(alias, bookTable.Columns.Count);
                        if (findIdx == -1)
                            continue;

                        var data = dataRow[findIdx].ToString(); //실제 Alias와 같은 테이블의 데이터 값 하나

                        //Attribute가 있는 각각의 field의 Type
                        Type type = field.FieldType;
                        //(type)data 는 컴파일 시 타입이 확정되어야 하기에 사용 불가
                        field.SetValue(newData, Convert.ChangeType(data, type)); //동적 타입 형변환 함수 사용
                    }

                    newData.OnCompleteRow(this);
                }
            }
        }

        public BookData GetBookData(int index)
        {
            return bookDatabase[index];
        }
    }
}