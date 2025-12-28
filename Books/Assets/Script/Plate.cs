using System;

namespace Script
{
    using UnityEngine;

    public class Plate : MonoBehaviour
    {
        public int floor;
        private BookSlot[] bookSlots;

        private void Awake()
        {
            if (bookSlots == null || bookSlots.Length == 0)
            {
                var parent = transform.GetChild(0);
                bookSlots = new BookSlot[parent.childCount];
                for (int i = 0; i < parent.childCount; i++)
                {
                    bookSlots[i] = parent.GetChild(i).GetComponent<BookSlot>();
                }
            }
        }

        public bool AddBook(Book book, int index)
        {
            return bookSlots[index].AddBook(book);
        }
    }
}