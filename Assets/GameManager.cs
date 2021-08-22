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
    
    private List<List<ImagePart>> gameImages;
    private int SCREEN_WIDTH;
    
    private List<ImagePart> initialOrder;
    private List<Vector3> initialPositions;

    private TextMeshProUGUI timerText;
    private TextMeshProUGUI recordText;
    private Stopwatch gameTime;
    private bool levelCompleted = false;

    private int emptyFieldX = 4;
    private int emptyFieldY = 4;
    
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    
    private String RECORD_KEY = GameSettings.difficulty + "record";
    public float SWIPE_THRESHOLD = 15f;
    
    private enum swipeDirections{left, right, up, down}
    
    void Start()
    {
        initializeGameImages();
        
        gameTime = new Stopwatch();
        initialOrder = new List<ImagePart>();
        initialPositions = new List<Vector3>();

        if (PlayerPrefs.HasKey(RECORD_KEY))
        {
            TimeSpan gameRecord = TimeSpan.FromMilliseconds(PlayerPrefs.GetInt(RECORD_KEY));
            recordText.text = gameRecord.ToString(@"mm\:ss") + " /";
        }
        else
        {
            recordText.gameObject.SetActive(false);
        }
            
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

                if (gameImages[i][j].Sprite.sprite == null)
                {
                    emptyFieldX = i;
                    emptyFieldY = j;
                }
            }
        }
        UpdatePositions();
        
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
        TimeSpan timeTaken = gameTime.Elapsed;
        timerText.text = timeTaken.ToString(@"mm\:ss");

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
    
    void checkSwipe()
    {
        Debug.Log(verticalMove() + " " + horizontalValMove());
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

    public void setTimer(bool mode)
    {
        if(mode)
            gameTime.Start();
        else
            gameTime.Stop();
    }

    public void winGame()
    {
        LevelCompleted();
    }
    
    private void LevelCompleted()
    {
        gameTime.Stop();

        
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
}
