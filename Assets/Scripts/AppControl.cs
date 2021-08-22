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
    [SerializeField] private Button winButton;
    [SerializeField] private Animator victoryScreenAnimator;
    
    private GameManager gameManager;
   
    private int level;
    private Vector3 scale;
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
        gameManager.SetScreenAnimator(victoryScreenAnimator);
        gameManager.setTimeText(timeText);
        gameManager.setRecordText(recordText);
        winButton.onClick.AddListener(delegate {mainLevel.GetComponent<GameManager>().winGame();});
    }

    public void onRestartClick()
    {
        if (gameManager.onPause())
        {
            victoryScreenAnimator.SetTrigger("RestartGame");
        }
        Destroy(gameManager.gameObject);
        winButton.onClick.RemoveListener(delegate {mainLevel.GetComponent<GameManager>().winGame();});
        GameSettings.resume = false;
        InitLevel(level);
    }
    
    public void onMainMenuClick()
    {
        gameManager.saveGameState();
        SceneManager.LoadScene(0);
    }
}
