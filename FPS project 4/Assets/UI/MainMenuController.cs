using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public VisualElement ui;

    public Button playButton;
    public Button quitButton;

    public playerMovementControl player;

    private void Awake()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 0f;
        var MenuObject = GameObject.Find("Menu");
        if (MenuObject != null)
            ui = MenuObject.GetComponent<UIDocument>().rootVisualElement;
        else
            Debug.LogError("GameObject 'Menu' introuvable.");
    }

    private void OnEnable()
    {
        playButton = ui.Q<Button>("PlayButton");
        quitButton = ui.Q<Button>("QuitButton");
        playButton.clicked += OnPlayButtonClicked;
        quitButton.clicked += OnQuitButtonClicked;
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnPlayButtonClicked()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        player.enabled = true;
    }
}
