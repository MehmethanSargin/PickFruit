using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picker : MonoBehaviour
{
    public float horizontalMovementSpeed;
    public float verticalMovementSpeed;
    private Rigidbody _followBody;
    private Rigidbody _body;
    private Vector3 _movementVector;
    private bool isMoving = false;
    public GameObject followObje;
    void Start()
    {
        _followBody = followObje.GetComponent<Rigidbody>();
        _body = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        switch (GameManager.instance.CurrentGameState)
        {
            case GameManager.GameState.Prepare:
                break;
            case GameManager.GameState.MainGame:
                PickerMove();
                _body.velocity = (followObje.transform.position - transform.position) * 4; 
                break;
            case GameManager.GameState.FinishGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void PickerMove()
    {
        isMoving = Input.GetMouseButton(0);
        if (isMoving)
        {
            _movementVector = new Vector3(Input.GetAxis("Mouse X") * (horizontalMovementSpeed * Time.deltaTime),0f , Input.GetAxis("Mouse Y") * (verticalMovementSpeed * Time.deltaTime));
            _followBody.position += _movementVector;
            _followBody.transform.position = new Vector3(Mathf.Clamp(_followBody.transform.position.x, -3, 3),
                _followBody.transform.position.y, _followBody.transform.position.z);
        }
    }
}
