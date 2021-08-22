using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private List<Button> levelButtons;
    
    private StorageManager storageManager;
    
    private int selectedLevel = 1;
    
    private readonly String LEVEL_DATA_KEY = GameSettings.difficulty + "levelData";
    
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Start()
    {
        storageManager = new StorageManager();
        levelButtons[0].onClick.AddListener(delegate { selectLevel(0);});
        levelButtons[1].onClick.AddListener(delegate { selectLevel(1);});
        levelButtons[2].onClick.AddListener(delegate { selectLevel(2);});
        levelButtons[3].onClick.AddListener(delegate { selectLevel(3);});
        selectLevel(0);
    }

    public void selectLevel(int x)
    {
        GameSettings.difficulty = x;
        GameSettings.resume = false;

        for (int i = 0; i < levelButtons.Count; i++)
        {
            levelButtons[i].gameObject.GetComponent<CanvasGroup>().alpha = i == x ? 1f : 0.5f;
        }
    }

    public void onPlay()
    {
        GameSettings.resume = storageManager.LoadData(LEVEL_DATA_KEY) != null;
        SceneManager.LoadScene(1);
    }
    
}
