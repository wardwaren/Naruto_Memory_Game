using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private List<GameObject> rows;
    
    private List<List<ImagePart>> gameImages;
    private int SCREEN_WIDTH;
    
    private List<ImagePart> initialOrder;
    private List<Vector3> initialPositions;

    private Stopwatch gameTime;
    private bool levelCompleted = false;
    
    void Start()
    {
        initializeGameImages();
        
        gameTime = new Stopwatch();
        initialOrder = new List<ImagePart>();
        initialPositions = new List<Vector3>();
        
        SCREEN_WIDTH = Screen.width;
        
        createInitialLists();
        ShuffleImages();
        gameTime.Start();
    }
    
    private void initializeGameImages()
    {
        gameImages = new List<List<ImagePart>>();
        
        foreach (var row in rows)
        {
            List<ImagePart> imageList = new List<ImagePart>();
            foreach (var gamePart in row.GetComponentsInChildren<ImagePart>())
            {
                imageList.Add(gamePart);
            }
            gameImages.Add(imageList);
        }
    }
    
    private void createInitialLists()
    {
        foreach (var imageRow in gameImages)
        {
            foreach (var image in imageRow)
            {
                initialOrder.Add(new ImagePart(image.id, image.Sprite));
                initialPositions.Add(image.transform.position);
            }
        }
    }
    
    private void ShuffleImages()
    {
        List<ImagePart> flatList = new List<ImagePart>();

        foreach (var row in gameImages)
        {
            foreach (var imagePart in row)
            {
                flatList.Add(imagePart);
            }
        }

        flatList = flatList.OrderBy(x => Random.value).ToList();
        
        for (int i = 0; i < gameImages.Count; i++)
        {
            for (int j = 0; j < gameImages[i].Count; j++)
            {
                gameImages[i][j] = flatList[i * gameImages.Count + j];
            }
        }

        for (int i = 0; i < gameImages.Count; i++)
        {
            for (int j = 0; j < gameImages[i].Count; j++)
            {
                gameImages[i][j].transform.position = initialPositions[i * gameImages.Count + j];
            }
        }
        
    }

    
    public bool IsOrdered()
    {
        for (int i = 0; i < gameImages.Count; i++)
        {
            for (int j = 0; j < gameImages[i].Count - 1; j++)
            {
                if (gameImages[i][j].id > gameImages[i][j+1].id) return false;
            }

            if (i >= gameImages.Count - 1) continue;
            
            if (gameImages[i][gameImages[i].Count - 1].id > gameImages[i + 1][0].id) return false;
        }

        return true;
    }

    private void FixedUpdate()
    {
        TimeSpan timeTaken = gameTime.Elapsed;
        timerText.text = timeTaken.ToString(@"mm\:ss");
    }

    private void LevelCompleted()
    {
        gameTime.Stop();
    }
}
