using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Script
{
    
    public class Book : MonoBehaviour
    {
        private BookData _bookData;
        public TextMeshPro frontTitle;
        public TextMeshPro sideTitle;
        public TextMeshPro description;

        private void Start()
        {
            _bookData = new BookData();
        }

        public void Init(BookData newData)
        {
            StringBuilder st = new StringBuilder();
            _bookData = newData;
            frontTitle.text = _bookData.title;
            sideTitle.text = _bookData.title;
            st.Append($"{_bookData.classNumber}\n");
            st.Append($"{_bookData.authorMark}{_bookData.bookMark}.{_bookData.workMark}\n");
            if (!_bookData.volumeMark.Equals(0)) st.Append($"v.{_bookData.volumeMark}\n");
            if (!_bookData.copyMark.Equals(0)) st.Append($"c.{_bookData.copyMark}");
            description.text = st.ToString();


        }
    }
}