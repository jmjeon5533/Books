using System;
using System.Collections.Generic;
using MessagePack;

namespace Script
{
    public partial class TableData
    {
        [Key("BookDatabase")]
        public List<BookData> BookDatabase = new();
    }
    
    [Serializable]
    [MessagePackObject]
    [TablePath("library_books")]
    public class BookData : BaseData
    {
        [Alias("Index"), Key(0)] 
        public int ID;

        [Alias("title"), Key(1)] 
        public string title;

        [Alias("classNumber"), Key(2)] 
        public float classNumber;

        [Alias("authorMark"), Key(3)] 
        public string authorMark;

        [Alias("bookMark"), Key(4)] 
        public int bookMark;

        [Alias("workMark"), Key(5)] 
        public string workMark;

        [Alias("volumeMark"), Key(6)] 
        public int volumeMark;

        [Alias("copyMark"), Key(7)] 
        public int copyMark;

        public override void OnCompleteRow()
        {
            base.OnCompleteRow();

            Table.BookDatabase.Add(this);
        }
    }
}