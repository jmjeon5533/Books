using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class Bookcase : MonoBehaviour
    {
        public List<Plate> _plates = new();

        private void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var plate = transform.GetChild(i).GetComponent<Plate>();
                plate.floor = i;
                _plates.Add(plate);
            }
            GameManager.Instance.AddBookcase(this);
        }
    }
}