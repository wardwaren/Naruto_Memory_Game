using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    private int level;
    private int difficulty;

    private void Start()
    {
        level = GameSettings.level;
        difficulty = GameSettings.difficulty;
        InitLevel(level);
    }

    private void InitLevel(int lvl)
    {
        
    }

    private void MemoryLevel()
    {
        
    }

    private void GuessLevel()
    {
        
    }
}
