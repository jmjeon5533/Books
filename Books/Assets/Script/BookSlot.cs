namespace Script
{
    public class BookSlot
    {
        public Book currentBook;
        public bool IsOccupied => currentBook != null;
    }
}