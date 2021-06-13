using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[System.Serializable]
public class OrderManager
{
    public Sprite fruitImage;
    public Fruit fruit;
    public int amount;
}

[System.Serializable]
public class Test
{
    public List<OrderManager> orderManagerList = new List<OrderManager>();

}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public enum GameState
    {
        Prepare,
        MainGame,
        FinishGame
    }

    public List<Test> OrderLevelList = new List<Test>();
    public List<OrderManager> orderList = new List<OrderManager>();
    public List<Fruit> OrderFruits = new List<Fruit>();
    public List<Fruit> collectedFruits = new List<Fruit>();
   
    public LevelManager levelManager;
    private Camera _mainCamera;
    
    public Transform CollectCamerPos;
    
    public int level = 0;
    private int match;
    
    public bool gameOver = false;
    
    public GameObject winCanvas;
    public GameObject loseCanvas;

    
    
    private GameState _currentGameState;

    public GameState CurrentGameState
    {
        get
        {
            return _currentGameState;
        }
        set
        {
            switch (value)
            {
                case GameState.Prepare: 
                    break;
                case GameState.MainGame:
                    break;
                case GameState.FinishGame:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            _currentGameState = value;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        ListAdder();
        CursorGreen();
    }

    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Prepare:
                TouchThePlay();
                break;
            case GameState.MainGame:
                CameraTransfer();
               CursorRed();
                break;
            case GameState.FinishGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

  
    public IEnumerator FinishMethod()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < OrderFruits.Count; i++)
        {
            foreach (Fruit collect in collectedFruits)
            {
                if (collect.GetType() == OrderFruits[i].GetType())
                {
                        match++;
                }
            }
        }
        if (match/ OrderFruits.Count * 100 <= 25)
        {
            gameOver = true;
            StartCoroutine(CanvasActivator(loseCanvas));
            OrderController.instance.humans[levelManager.currentLevel].GetComponentInChildren<Animator>().SetBool("Lose", true);
        }

        if (match / OrderFruits.Count * 100 >= 60)
        {
            gameOver = false;
            StartCoroutine(CanvasActivator(winCanvas));
            OrderController.instance.humans[levelManager.currentLevel].GetComponentInChildren<Animator>().SetBool("HighFun", true);
        }

        if (match / OrderFruits.Count * 100 <= 60 && match / OrderFruits.Count * 100>25)
        {
            gameOver = false;
            StartCoroutine(CanvasActivator(winCanvas));
            OrderController.instance.humans[levelManager.currentLevel].GetComponentInChildren<Animator>().SetBool("LowFun", true);  
        }
    }

    private IEnumerator CanvasActivator(GameObject canvas)
    {
        yield return new WaitForSeconds(1.5f);
        canvas.SetActive(true);
    }

    private void TouchThePlay()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentGameState = GameState.MainGame; 
        }
    }

    public void NextButtonMethod()
    {
        CurrentGameState = GameState.FinishGame;
        OrderController.instance.gameObject.SetActive(true);
        OrderController.instance.CurrentGameState = OrderController.GameState.MainGame;
    }

    private void ListAdder()
    {
        for (int i = 0; i < OrderLevelList[levelManager.currentLevel].orderManagerList.Count; i++)
        {
            int x = 1;
            while (x<=OrderLevelList[levelManager.currentLevel].orderManagerList[i].amount)
            {
                OrderFruits.Add(OrderLevelList[levelManager.currentLevel].orderManagerList[i].fruit);
                x++;
            }
        }
    }


    private void CursorGreen()
    {
        for (int i = 0; i < OrderFruits.Count; i++)
        {
            var fruits = FindObjectsOfType<Fruit>();
            for (int j = 0; j < fruits.Length; j++)
            {
                if (OrderFruits[i].GetType() == fruits[j].GetType())
                {
                    fruits[j].cursor.color = Color.green;
                }
            }
        }
    }

    private void CursorRed()
    {
        if (collectedFruits.Count>0)
        {
            for (int i = 0; i < OrderLevelList[levelManager.currentLevel].orderManagerList.Count; i++)
            {
                var fruits = FindObjectsOfType<Fruit>();
                if (OrderLevelList[levelManager.currentLevel].orderManagerList[i].amount == 0)
                {
                    for (int j = 0; j < fruits.Length; j++)
                    {
                        if (OrderLevelList[levelManager.currentLevel].orderManagerList[i].fruit.GetType() == fruits[j].GetType())
                        {
                            fruits[j].cursor.color = Color.red;
                        }  
                    }
                
                }
           
            }  
        }
    }
    private void CameraTransfer()
    {
       _mainCamera.transform.SetParent(CollectCamerPos);
       _mainCamera.transform.localPosition =
           Vector3.Lerp(_mainCamera.transform.localPosition, Vector3.zero, Time.deltaTime);
       _mainCamera.transform.localRotation = Quaternion.Lerp(_mainCamera.transform.localRotation, Quaternion.Euler(0,0,0),Time.deltaTime);
    }

  
}
