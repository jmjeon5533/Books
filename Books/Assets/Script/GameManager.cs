using System;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

namespace Script
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public List<Bookcase> _bookcases = new();
        [Header("참조")]
        public Plate returnShelf;
        public Transform bookParent;

        private Book bookPrefab;
        private DataManager _dataManager;

        public void AddBookcase(Bookcase bookcase)
        {
            _bookcases.Add(bookcase);
            AddBooks(bookcase);
        }

        private void Awake()
        {
            Instance = this;
            _dataManager = GetComponent<DataManager>();
            bookPrefab = Resources.Load<Book>("Prefab/Book");
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
        }

        private void AddBooks(Bookcase bookcase)
        {
            for (int i = 0; i < bookcase._plates.Count; i++)
            {
                for (int count = 0; count < bookcase._plates[i].GetBookSlotCount(); count++)
                {
                    var data = _dataManager.GetBookData(i * bookcase._plates.Count + count);
                    var book = Instantiate(bookPrefab, bookParent);
                    book.Init(data);
                    if (!bookcase._plates[i].AddBook(book,count))
                    {
                        print("오류");
                    }
                }
            }
        }
    }
}