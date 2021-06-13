using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
   
    public float movementSpeed;
    private bool pickerCheck = false;
    public SpriteRenderer cursor;
   
    void Update()
    {
        switch (GameManager.instance.CurrentGameState)
        {
            case GameManager.GameState.Prepare:
                break;
            case GameManager.GameState.MainGame:
                if (!pickerCheck)
                {
                    transform.Translate(0,0,-1 * movementSpeed * Time.deltaTime);  
                }
                break;
            case GameManager.GameState.FinishGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Picker>())
        {
            pickerCheck = true;
        }
    }
}
