using System;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

namespace Script
{
    public partial class TableData
    {
        [Key("stageDataBase")]
        [SerializeField] public List<StageData> StageDataBase = new();
    }
    
    [Serializable]
    [MessagePackObject]
    [TablePath("library_books 1")]
    public class StageData : BaseData
    {
        [Alias("Index"), Key(0)] 
        public int ID;
        
        [Alias("BookID"), Key(1)] 
        public int BookID;

        public override void OnCompleteRow()
        {
            base.OnCompleteRow();

            Table.StageDataBase.Add(this);
        }
    }
}