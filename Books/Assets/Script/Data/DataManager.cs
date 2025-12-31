using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using ExcelDataReader;
using MessagePack;

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

    [MessagePackObject]
    public partial class TableData
    {
        public static TableData Ins { get; private set; }

        public static void Initialize(TableData data)
        {
            Ins = data;
        }
        
        public static void Reset()
        {
            Ins = new();
        }
    }

    public class BaseData
    {
        [IgnoreMember]
        public TableData Table => TableData.Ins;
        
        public virtual void OnCompleteRow()
        {
        }
    }

    public class DataManager : MonoBehaviour
    {
        public const string TableSavePath = "Table/Table";
        
        private void Awake()
        {
            var tableAsset = Resources.Load<TextAsset>(TableSavePath);
            var tableData = MessagePackSerializer.Deserialize<TableData>(tableAsset.bytes);
            TableData.Initialize(tableData);
        }
        
        public BookData GetBookData(int index)
        {
            return TableData.Ins.BookDatabase[index];
        }
    }
}