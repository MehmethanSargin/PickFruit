using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
   public List<Image> Images = new List<Image>();
   public List<Text> TextList= new List<Text>();

   public List<Image> GamePlayImages = new List<Image>();
   public List<Text> GamePlayTextList = new List<Text>();
   
   public LevelManager levelManager;

   public Text scoreText;
   
   private void Start()
   {
      for (int i = 0; i < GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList.Count; i++)
      {
         Images[i].gameObject.SetActive(true);
         Images[i].sprite = GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].fruitImage;
         TextList[i].text = GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].amount.ToString();
      }

      for (int i = 0; i < GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList.Count; i++)
      {
         GamePlayImages[i].gameObject.SetActive(true);
         GamePlayImages[i].sprite = GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].fruitImage;
         GamePlayTextList[i].text = GameManager.instance.OrderLevelList[levelManager.currentLevel].orderManagerList[i].amount.ToString();
      }
      
      scoreText.text = levelManager.money.ToString();
   }
   
}
