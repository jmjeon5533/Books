using System;

namespace Script
{
    public class ReturnShelf : Plate
    {
        private void Start()
        {
            GameManager.Instance.returnShelf ??= this;
        }
    }
}