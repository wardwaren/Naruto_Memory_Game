using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
   
    [SerializeField] private List<GameObject> rows;
    [SerializeField] private float SWIPE_THRESHOLD = 15f;

    private List<List<ImagePart>> gameImages;
    private List<List<int>> gameImageIds;
    
    private List<ImagePart> initialOrder;
    private List<Vector3> initialPositions;

    private TextMeshProUGUI timerText;
    private TextMeshProUGUI recordText;
    private CustomStopwatch gameTime;
    private StorageManager storageManager;
    private Animator victoryScreenAnimator;
    
    private bool gameOnPause = false;
    
    private int emptyFieldX = 4;
    private int emptyFieldY = 4;
    
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    
    private readonly String RECORD_KEY = GameSettings.difficulty + "record";
    private readonly String RESUME_KEY = GameSettings.difficulty + "resume";
    private readonly String LEVEL_DATA_KEY = GameSettings.difficulty + "levelData";
    
    private enum swipeDirections{left, right, up, down}
    
    void Start()
    {
        initializeGameImages();
        
        initialOrder = new List<ImagePart>();
        initialPositions = new List<Vector3>();
        storageManager = new StorageManager();
        gameTime = new CustomStopwatch(TimeSpan.Zero);
        
        if (PlayerPrefs.HasKey(RECORD_KEY))
        {
            TimeSpan gameRecord = TimeSpan.FromMilliseconds(PlayerPrefs.GetInt(RECORD_KEY));
            recordText.text = gameRecord.ToString(@"mm\:ss") + " /";
        }
        
        createInitialLists();
        
        if (!GameSettings.resume)
        {
            ShuffleImages();
        }
        else
        {
            restoreGameState();
            gameTime = new CustomStopwatch(TimeSpan.FromMilliseconds(PlayerPrefs.GetInt(RESUME_KEY)));
        }    
        
        gameTime.Start();
    }
    
    private void initializeGameImages()
    {
        gameImages = new List<List<ImagePart>>();
        gameImageIds = new List<List<int>>();
        
        foreach (var row in rows)
        {
            List<ImagePart> imageList = new List<ImagePart>();
            List<int> imageListIds = new List<int>();
            
            foreach (var gamePart in row.GetComponentsInChildren<ImagePart>())
            {
                imageList.Add(gamePart);
                imageListIds.Add(gamePart.id);
            }
            
            gameImages.Add(imageList);
            gameImageIds.Add(imageListIds);
        }
    }
    
    private void createInitialLists()
    {
        foreach (var imageRow in gameImages)
        {
            foreach (var image in imageRow)
            {
                initialOrder.Add(image);
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
                gameImageIds[i][j] = gameImages[i][j].id;
                
                if (gameImages[i][j].Sprite.sprite == null)
                {
                    emptyFieldX = i;
                    emptyFieldY = j;
                }
            }
        }
        UpdatePositions();
        
        if(IsOrdered())
            ShuffleImages();
    }

    private void restoreFromIDs()
    {
        for (int i = 0; i < gameImages.Count; i++)
        {
            for (int j = 0; j < gameImages[i].Count; j++)
            {
                gameImages[i][j] = initialOrder[gameImageIds[i][j]];
                
                if (gameImages[i][j].Sprite.sprite == null)
                {
                    emptyFieldX = i;
                    emptyFieldY = j;
                }
            }
        }

    }
    
    private void UpdatePositions()
    {
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
        TimeSpan timeTaken = TimeSpan.FromMilliseconds(gameTime.ElapsedMilliseconds);
     
        if (PlayerPrefs.HasKey(RECORD_KEY))
        {
            TimeSpan gameRecord = TimeSpan.FromMilliseconds(PlayerPrefs.GetInt(RECORD_KEY));
            if(gameRecord < timeTaken)
                recordText.text = timeTaken.ToString(@"mm\:ss") + " /";
        }
        
        timerText.text = timeTaken.ToString(@"mm\:ss");

        if (!gameOnPause)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    fingerUp = touch.position;
                    fingerDown = touch.position;
                }
            
                if (touch.phase == TouchPhase.Ended)
                {
                    fingerDown = touch.position;
                    checkSwipe();
                }
            }
        }
    }
    
    void checkSwipe()
    {
        if (verticalMove() > SWIPE_THRESHOLD && verticalMove() > horizontalValMove())
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                RegisterSwipe(swipeDirections.up);
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                RegisterSwipe(swipeDirections.down);
            }
            fingerUp = fingerDown;
        }

        else if (horizontalValMove() > SWIPE_THRESHOLD && horizontalValMove() > verticalMove())
        {
            if (fingerDown.x - fingerUp.x > 0)
            {
                RegisterSwipe(swipeDirections.right);
            }
            else if (fingerDown.x - fingerUp.x < 0)
            {
                RegisterSwipe(swipeDirections.left);
            }
            fingerUp = fingerDown;
        }
    }
    
    float verticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    float horizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    private void RegisterSwipe(swipeDirections dir)
    {
        if (dir == swipeDirections.left)
        {
            if (emptyFieldY == gameImages.Count - 1) return;
            
            SwapImages(emptyFieldX, emptyFieldY, emptyFieldX, emptyFieldY + 1);

            emptyFieldY++;
        }
        else if (dir == swipeDirections.right)
        {
            if (emptyFieldY == 0) return;
            
            SwapImages(emptyFieldX, emptyFieldY, emptyFieldX, emptyFieldY - 1);

            emptyFieldY--;
        }
        else if (dir == swipeDirections.up) // up
        {
            if (emptyFieldX == gameImages.Count - 1) return;
            
            SwapImages(emptyFieldX, emptyFieldY, emptyFieldX + 1, emptyFieldY);

            emptyFieldX++;
        }
        else
        {
            if (emptyFieldX == 0) return;
            
            SwapImages(emptyFieldX, emptyFieldY, emptyFieldX - 1, emptyFieldY);

            emptyFieldX--;
        }

        if (IsOrdered())
        {
            LevelCompleted();
        }
    }

    private void SwapImages(int x1, int y1, int x2, int y2)
    {
        ImagePart temp = gameImages[x1][y1];
        gameImages[x1][y1] = gameImages[x2][y2];
        gameImages[x2][y2] = temp;
        UpdatePositions();
    }

    public void pauseGame(bool pause)
    {
        gameOnPause = pause;
        
        if(pause)
            gameTime.Stop();
        else
            gameTime.Start();
    }

    public void winGame()
    {
        LevelCompleted();
    }

    public void saveGameState()
    {
        for (int i = 0; i < gameImages.Count; i++)
        {
            for (int j = 0; j < gameImages[i].Count; j++)
            {
                gameImageIds[i][j] = gameImages[i][j].id;
            }
        }

        storageManager.SaveData(gameImageIds, LEVEL_DATA_KEY);
        
        PlayerPrefs.SetInt(RESUME_KEY, (int) gameTime.ElapsedMilliseconds);
    }

    private void restoreGameState()
    {
        gameImageIds = (List<List<int>>) storageManager.LoadData(LEVEL_DATA_KEY);
        restoreFromIDs();
        UpdatePositions();
    }
    
    private void LevelCompleted()
    {
        pauseGame(true);
        
        victoryScreenAnimator.SetTrigger("GameWin");
        
        if(PlayerPrefs.GetInt(RECORD_KEY) > (int) gameTime.ElapsedMilliseconds)
            PlayerPrefs.SetInt(RECORD_KEY, (int) gameTime.ElapsedMilliseconds);
    }

    public void setTimeText(TextMeshProUGUI timerText)
    {
        this.timerText = timerText;
    }

    public void setRecordText(TextMeshProUGUI recordText)
    {
        this.recordText = recordText;
    }

    private void OnApplicationQuit()
    {
        saveGameState();
    }

    public void SetScreenAnimator(Animator animator)
    {
        victoryScreenAnimator = animator;
    }
    
    public bool onPause()
    {
        return gameOnPause;
    }
}
