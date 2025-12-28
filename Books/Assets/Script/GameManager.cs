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

        public void AddBookcase(Bookcase bookcase)
        {
            _bookcases.Add(bookcase);
            AddBooks(bookcase);
        }

        private void Awake()
        {
            Instance = this;
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
            for (int j = 0; j < bookcase._plates.Count; j++)
            {
                for (int count = 0; count < 16; count++)
                {
                    var book = Instantiate(bookPrefab,bookParent);
                    if (!bookcase._plates[j].AddBook(book,count))
                    {
                        print("오류");
                    }
                }
            }
        }
    }
}