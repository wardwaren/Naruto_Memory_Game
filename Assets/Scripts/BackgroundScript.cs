using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour
{
    [SerializeField] private List<Sprite> backgrounds;

    private Image backgroundImage;
    
    private void Start()
    {
        backgroundImage = gameObject.GetComponent<Image>();
        backgroundImage.sprite = backgrounds[GameSettings.difficulty];
    }
}
