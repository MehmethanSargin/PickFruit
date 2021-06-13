using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermelon : Fruit
{
    public bool isCursor = false;
    public SpriteRenderer cursor;
    private void Start()
    {
        if (isCursor)
        {
            cursor.color = Color.green;
        }
    }
}
