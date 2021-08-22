using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private Button homeButton;

    [SerializeField] private List<Button> homeButtons;

    [SerializeField] private List<Button> levelButtons;
    // Start is called before the first frame update

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void onPlay()
    {
        _animator.SetTrigger("ButtonClick");
        homeButton.gameObject.SetActive(true);

        switchPanels(levelButtons, homeButtons);
    }

    private void switchPanels(List<Button> turnOn, List<Button> turnOff)
    {
        foreach (var button in turnOn)
        {
            button.gameObject.SetActive(true);
        }
        
        foreach (var button in turnOff)
        {
            button.gameObject.SetActive(false);
        }
    }
    
    public void selectLevel(int x)
    {
        GameSettings.difficulty = x;
        GameSettings.resume = false;
        
        SceneManager.LoadScene(1);
    }

    public void returnToMain()
    {
        _animator.SetTrigger("ButtonClick");
        homeButton.gameObject.SetActive(false);
        switchPanels(homeButtons, levelButtons);
    }
    
    public void resumeLevel()
    {
        GameSettings.resume = true;
        SceneManager.LoadScene(1);
    }

    public void onClose()
    {
        Application.Quit();
    }
    
}
