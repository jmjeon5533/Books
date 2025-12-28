using UnityEngine;

namespace Script
{
    public class BookSlot : MonoBehaviour
    {
        public Book currentBook;
        public bool IsOccupied => currentBook != null;
    
        public bool AddBook(Book book)
        {
            if (IsOccupied) return false;
            currentBook = book;
            book.transform.SetPositionAndRotation(transform.position, transform.rotation);
            return true;
        }
    
        public void RemoveBook()
        {
            currentBook = null;
        }
    
        public Book TakeBook()
        {
            var book = currentBook;
            currentBook = null;
            return book;
        }
    }

}