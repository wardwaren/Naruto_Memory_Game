using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Camera camera;
    public Vector3 rotation;

    public GameObject cardBack;

    private bool cardBackIsActive;

    private int timer;

    private void Start()
    {
        cardBackIsActive = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit) && timer == 0
            && hit.transform.gameObject == gameObject){
                 StartFlip();
            }
            
        }
    }

    public void StartFlip()
    {
        StartCoroutine(CalculateFlip());
    }

    public void Flip()
    {
        if (cardBackIsActive == true)
        {
            cardBack.SetActive(false);
            cardBackIsActive = false;
        }
        else
        {
            cardBack.SetActive(true);
            cardBackIsActive = true;
        }
    }

    IEnumerator CalculateFlip()
    {
        for (int i = 0; i < 180; i++)
        {
            yield return new WaitForSeconds(0.005f);
            transform.Rotate(rotation);
            timer++;
            if (timer == 90 || timer == -90)
            {
                Flip();
            }
        }

        timer = 0;
    }
}
