using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.PlayerLoop;

public class OrderController : MonoBehaviour
{
    public static OrderController instance;
    
    public List<GameObject> humans;
    
    public GameObject startCanvas;
    public GameObject gameplayCanvas;
    public GameObject glass;
    
    private Camera _mainOrderCamera;
    public Transform orderCameraPos;
    public Transform humanOrderPost;
    public Transform humanExitPost;
    
    private bool isTrigger = false;
    private bool isStart = false;
    public bool isGlassActive = false;
    
    
    public LevelManager levelManager;

    public Blender blender;
    public enum GameState
    {
        Prepare,
        MainGame,
        FinishGame
    }

    private void Start()
    {
        _mainOrderCamera = Camera.main;
    }

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

    private void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Prepare:
                if (!isStart && levelManager.currentLevel ==0)
                {
                    isStart = true;
                    startCanvas.SetActive(true);  
                }

                if (!isStart && levelManager.currentLevel !=0)
                {
                    StartCoroutine(HumanTransfer());
                }
                break;
            case GameState.MainGame: 
                GlassActive();
               CameraOrderTransfer();
               AnimationTrigger();
               break;
            case GameState.FinishGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OrderNextButton()
    {
        startCanvas.SetActive(false);
        gameplayCanvas.SetActive(true);
        GameManager.instance.CurrentGameState = GameManager.GameState.MainGame;
        gameObject.SetActive(false);
    }
    
    private void CameraOrderTransfer()
    {
       _mainOrderCamera.transform.SetParent(orderCameraPos);
       _mainOrderCamera.transform.localPosition =
           Vector3.Lerp(_mainOrderCamera.transform.localPosition, Vector3.zero, Time.deltaTime * 3);
       _mainOrderCamera.transform.localRotation = Quaternion.Lerp(_mainOrderCamera.transform.localRotation,  quaternion.Euler(0,0,0),Time.deltaTime * 3);
    }

    private void AnimationTrigger()
    {
        if (!isTrigger)
        {
            humans[levelManager.currentLevel].GetComponentInChildren<Animator>().SetTrigger("Drink");
            isTrigger = true;
        }
    }

    private void GlassActive()
    {
        if (!isGlassActive)
        {
            glass.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = blender.finishColor;
            glass.SetActive(true);
            isGlassActive = true;
        }
    }
    
    private IEnumerator HumanTransfer()
    {
        yield return new WaitForSeconds(1f);
        isStart = true;
        humans[0].transform.localRotation = Quaternion.Lerp( humans[0].transform.localRotation,quaternion.Euler(0,-0.4f,0),Time.deltaTime *10);
        humans[0].transform.localPosition = Vector3.Lerp(humans[0].transform.localPosition,humanExitPost.position,
            Time.deltaTime);
        humans[levelManager.currentLevel].transform.localPosition = Vector3.Lerp(humans[levelManager.currentLevel].transform.localPosition,humanOrderPost.position,
            Time.deltaTime);
        startCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        humans[0].SetActive(false);
    }
}
