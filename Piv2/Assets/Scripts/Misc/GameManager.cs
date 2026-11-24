using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject RestartButton;
    public InputAction OpenMenuAction;
    public bool MenuPaused;


    private void Update()
    {
        PauseMenu();
    }

    public void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.P) && !MenuPaused)
        {
            RestartButton.SetActive(true);
            MenuPaused = true;
            Time.timeScale = 0f;
            Cursor.visible = true;
        } else if (Input.GetKeyDown(KeyCode.P) && MenuPaused)
        {
            RestartButton.SetActive(false);
            Cursor.visible = false;
            MenuPaused = false;
            Time.timeScale = 1f;
        }
        
    }

    public void Restart()
    {
        RestartButton.SetActive(false);
        Cursor.visible = false;
        MenuPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
}
