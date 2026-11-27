using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject RestartButton;
    public InputAction OpenMenuAction;
    public bool MenuPaused;
    public Material LightMaterial;
    public Color LightColor1;
    public Color LightColor2;
    private float LightIntensity =  8f;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
        }
        else
        {
            StartAlarm();
            Cursor.visible = false;
        }
    }

    private void Update()
    {
      //  PauseMenu();
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

    [ContextMenu("Switch light color")]

    public void StartAlarm()
    {
        StartCoroutine(LightAlarm());
    }

    private IEnumerator LightAlarm()
    {
        float factor = Mathf.Pow(2, LightIntensity);

        LightColor2 *= factor;
        LightColor1 *= factor;
        AudioManager.Instance.PlaySFX("AlarmVoice");
        int alarmCounter = 0;
        while (true)
        {
            alarmCounter++;
            if (alarmCounter % 10 == 0)
            {
                AudioManager.Instance.PlaySFX("AlarmVoice");
            }
            AudioManager.Instance.PlaySFX("Alarm");
            LightMaterial.SetColor("_EmissionColor", LightColor2);
            yield return new WaitForSecondsRealtime(0.5f);
            LightMaterial.SetColor("_EmissionColor", LightColor1);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("DigitalTwin");
    }

    public void Quit()
    {
        Application.Quit();
    }
    
}
