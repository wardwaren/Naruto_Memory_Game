using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppControl : MonoBehaviour
{
   [SerializeField] private GameObject settingsPanel;
   
   
   public void onSettingsClick(){
         settingsPanel.SetActive(true);
   }

   public void onResumeClick()
   {
       settingsPanel.SetActive(false);
   }

   public void onMainMenuClick()
   {
       SceneManager.LoadScene(0);
   }
   
   public void onQuitClick()
   {
       Application.Quit();
   }
   
}
