using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class ImagePart : MonoBehaviour
{
    [SerializeField] private int ID;
    [SerializeField] private SpriteRenderer sprite;
    
    public int id => ID;
    public SpriteRenderer Sprite => sprite;
    
    public ImagePart(int ID, SpriteRenderer sprite)
    {
        this.ID = ID;
        this.sprite = sprite;
    }
    
}
