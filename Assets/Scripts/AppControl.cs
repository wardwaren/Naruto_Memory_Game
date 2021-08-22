using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppControl : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameLevels;
    [SerializeField] private Transform gameLevelAnchor;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI recordText;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button winButton;

    private GameManager gameManager;
   
    private int level;
    private GameObject mainLevel;
    
    private void Start()
    {
        level = GameSettings.difficulty;
        InitLevel(level);
    }

    private void InitLevel(int lvl)
    {
        mainLevel = Instantiate(gameLevels[lvl], gameLevelAnchor);
        gameManager = mainLevel.GetComponent<GameManager>();
        mainLevel.GetComponent<GameManager>().setTimeText(timeText);
        mainLevel.GetComponent<GameManager>().setRecordText(recordText);
        winButton.onClick.AddListener(delegate {mainLevel.GetComponent<GameManager>().winGame();});
    }

    public void onRestartClick()
    {
        Destroy(gameManager.gameObject);
        winButton.onClick.RemoveListener(delegate {mainLevel.GetComponent<GameManager>().winGame();});
        GameSettings.resume = false;
        InitLevel(level);
    }
    
    public void onSettingsClick(){
        settingsPanel.SetActive(true);
        gameManager.pauseGame(true);
    }

    public void onResumeClick()
    {
        settingsPanel.SetActive(false);
        gameManager.pauseGame(false);
    }

    public void onMainMenuClick()
    {
        gameManager.saveGameState();
        SceneManager.LoadScene(0);
    }
   
    public void onQuitClick()
    {
        Application.Quit();
    }
}
