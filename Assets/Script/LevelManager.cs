using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool isOver = false;
    [SerializeField] private bool isVictory = false;

    [SerializeField] private int levelSelect = 0;

    // Update is called once per frame
    private void Update(){
        if (Input.GetKeyDown(KeyCode.Return) && isOver == true && isPaused == true && isVictory == false){
            SceneManager.LoadScene("MainMenu");
            Time.timeScale = 1f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) && isOver == false && isPaused == true && isVictory == true){
            if (levelSelect == 0){
                SceneManager.LoadScene("MainMenu");
                Time.timeScale = 1f;
                return;
            } else if (levelSelect == 1){
                SceneManager.LoadScene("Level_2");
                Time.timeScale = 1f;
                return;
            } else {
                SceneManager.LoadScene("Level_3");
                Time.timeScale = 1f;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && isPaused == false && isOver == false){
            Time.timeScale = 0f;
            pauseScreen.SetActive(true);
            isPaused = true;
        } else if (Input.GetKeyDown(KeyCode.Return) && isPaused == true && isOver == false) {
            Time.timeScale = 1f;
            isPaused = false;
            pauseScreen.SetActive(false);
        }

    }

    public void OpenGameOverScreen(){
        Time.timeScale = 0.5f;
        gameOverScreen.SetActive(true);
        isOver = true;
        isPaused = true;
        playerObject.SetActive(false);
    }


    public void OpenVictoryScreen(){
        Time.timeScale = 0f;
        isVictory = true;
        isPaused = true;
        victoryScreen.SetActive(true);
    }
}
