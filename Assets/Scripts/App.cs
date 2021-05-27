using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }


    public void onPlay()
    {
        _animator.SetTrigger("ButtonClick");
    }
    
    public void onClose()
    {
        Application.Quit();
    }
    

}
