using System;
using TMPro;
using UnityEngine;

namespace Script
{
    
    public class Book : MonoBehaviour
    {
        private BookData _bookData;
        public TextMeshPro title;
        public TextMeshPro description;

        private void Start()
        {
            _bookData = new BookData();
        }
    }
}