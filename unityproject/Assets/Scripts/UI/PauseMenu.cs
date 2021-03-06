﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenu;
        public GameObject optionsMenu;
        public GameObject pauseMenuButtons;
        public GameObject resumeButton;
        [HideInInspector] public bool canOpenPauseMenu = true;

        public InputAction pauseAction;

        private void OnEnable()
        {
            pauseAction.Enable();
        }

        private void OnDisable()
        {
            pauseAction.Disable();
        }

        void Update()
        {
            if (pauseAction.triggered && canOpenPauseMenu)
            {
                Time.timeScale = pauseMenu.activeSelf ? 1 : 0;
                optionsMenu.SetActive(false);
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                pauseMenuButtons.SetActive(true);
                AudioListener.pause = pauseMenu.activeSelf;
                if (pauseMenu.activeSelf)
                    EventSystem.current.SetSelectedGameObject(resumeButton);
            }

        }

        public void PauseMenuResume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            AudioListener.pause = false;
        }

        public void OpenOptionsMenu()
        {
            pauseMenuButtons.SetActive(false);
            optionsMenu.SetActive(true);
        }
    
        public void GoToMainMenu()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            AudioListener.pause = false;
            SceneManager.LoadScene(0);
        }
    }
}
