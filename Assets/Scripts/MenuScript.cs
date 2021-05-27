using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    // Start is called before the first frame update

    public void onPlay()
    {
        _animator.SetTrigger("ButtonClick");
    }

    public void selectLevel(int x)
    {
        GameSettings.difficulty = x;
        GameSettings.level = 1;
        SceneManager.LoadScene(1);
    }

    public void returnToMain()
    {
        _animator.SetTrigger("ButtonClick");
        
    }
    
    public void resumeLevel()
    {
        
    }
    
    public void onClose()
    {
        Application.Quit();
    }
    
}
