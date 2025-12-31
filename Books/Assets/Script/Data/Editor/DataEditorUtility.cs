using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExcelDataReader;
using MessagePack;
using Script;
using UnityEditor;
using UnityEngine;

public static class DataEditorUtility
{ 
    [MenuItem("Tools/Create MessagePack")]
    public static void CreateMessagePackSerialize()
    {
        LoadDataFromXlsm();
    }

    private static void LoadDataFromXlsm()
    {
        TableData.Reset();
        
        List<(Type type, string path)> tableInfo = new();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) foreach (var type in assembly.GetTypes())
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

            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
                {
                    Debug.LogError($"{curType.FullName}이 BaseData가 아닙니다");
                    continue;
                }

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

                newData.OnCompleteRow();
            }
        }

        var editorTableSavePath = $"{Application.dataPath}/Resources/{DataManager.TableSavePath}.bytes";
        var serializeData = MessagePackSerializer.Serialize(TableData.Ins);
        File.WriteAllBytes(editorTableSavePath, serializeData);
        
        TableData.Reset();
    }
}