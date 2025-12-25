namespace Script
{
    using UnityEngine;

    public class Plate : MonoBehaviour
    {
        public int floor;
        private BookSlot[] bookSlots = new BookSlot[8];

        public bool AddBook(Book book, int index)
        {
            var slot = bookSlots[index];
            if (slot.IsOccupied) return false;

            slot.currentBook = book;
            return true;
        }

        public void RemoveBook(int index)
        {
            var slot = bookSlots[index];
            if (!slot.IsOccupied) return;

            slot.currentBook = null;
        }

        public Book TakeBook(int index)
        {
            var slot = bookSlots[index];
            if (!slot.IsOccupied) return null;

            var take = slot.currentBook;
            slot.currentBook = null;
            return take;
        }
    }
}