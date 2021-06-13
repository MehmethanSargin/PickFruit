using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blender : MonoBehaviour
{
    public Color finishColor;
    
    public Material blendermMaterial;
    public GameObject blenderWave;
   
    private bool waveTrigger = false; 
    
    private int waveTotal = 100;
    private int waveAmount = 100;
    private int waveFinish;

    public LevelManager levelManager;
    public UIController _uıController;
    private void Start()
    {
        waveTotal = waveTotal /GameManager.instance.OrderFruits.Count;
        waveFinish = GameManager.instance.OrderFruits.Count;
    }

    private void Update()
    {
        switch (GameManager.instance.CurrentGameState)
        {
            case GameManager.GameState.Prepare:
                break;
            case GameManager.GameState.MainGame:
                RotateWave();
                break;
            case GameManager.GameState.FinishGame:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var fruit = other.GetComponent<Fruit>();
        if (fruit)
        {
            waveFinish--;
            waveAmount -= waveTotal;
            waveTrigger = true;
            GameManager.instance.collectedFruits.Add(fruit);
            for (int i = 0; i < GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList.Count; i++)
            {
                if (fruit.GetType() == GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].fruit.GetType())
                {
                    GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].amount--;
                  _uıController.GamePlayTextList[i].text = GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].amount.ToString();
                    
                }
            }
            blenderWave.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(2, waveAmount);
                blendermMaterial.color = (blendermMaterial.color + fruit.GetComponent<MeshRenderer>().materials[0].color) /2;
                fruit.transform.SetParent(blenderWave.transform);
                if (waveFinish==0)
                {
                    finishColor = blendermMaterial.color;
                   OrderFinish();
                }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        other.transform.localPosition = Vector3.Lerp(other.transform.localPosition, Vector3.zero, Time.deltaTime * 5);
        other.transform.Rotate(0,1,0);
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.SetActive(false);
    }

    private void RotateWave()
    {
        if (waveTrigger)
        {
           blenderWave.transform.Rotate(0,1,0); 
        }
    }

    private void OrderFinish()
    {
        GameManager.instance.CurrentGameState = GameManager.GameState.FinishGame;
        OrderController.instance.gameObject.SetActive(true);
        OrderController.instance.CurrentGameState = OrderController.GameState.MainGame;
    }
    
}
