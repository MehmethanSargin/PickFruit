using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> LevelPrefabs;
    
    public int currentLevel;
    
    public int money;
    

    private void Start()
    {
        MoneyCount();
        LevelCount();
        Instantiate(LevelPrefabs[currentLevel], transform.position, Quaternion.identity);
    }

    public void NextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("Level", currentLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        money += 10;
        PlayerPrefs.SetInt("money",money);
        if(PlayerPrefs.GetInt("Level") == 2)
        {
            PlayerPrefs.DeleteKey("Level");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void MoneyCount()
    {
        if (PlayerPrefs.HasKey("money"))
        { 
            money = PlayerPrefs.GetInt("money",money);
        }
        else
        {
            money = 0;
        }
    }

    private void LevelCount()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevel = PlayerPrefs.GetInt("Level");  
        }
        else
        {
            currentLevel = 0;
        }
    }
}
