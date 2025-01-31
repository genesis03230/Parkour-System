using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ParkourSystem
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pauseMenuUI; // Panel del menú de pausa
        private bool isPaused = false;

        [Header("Configuración de la escena")]
        public string sceneNameToRestart = "ParkourSystemFinal"; // Nombre de la escena a reiniciar

        private void Update()
        {
            // Verificar si se presiona la tecla Esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    PauseGame();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        // Pausar el juego
        public void PauseGame()
        {
            pauseMenuUI.SetActive(true); // Mostrar el menú de pausa
            Time.timeScale = 0f; // Detener el tiempo del juego
            isPaused = true; // Cambiar estado de pausa
        }

        // Reanudar el juego
        public void ResumeGame()
        {
            pauseMenuUI.SetActive(false); // Ocultar el menú de pausa
            Time.timeScale = 1f; // Reanudar el tiempo del juego
            isPaused = false; // Cambiar estado de pausa
        }

        // Reiniciar la escena especifica
        public void RestartGame()
        {
            Time.timeScale = 1f; // Asegurarse de que el tiempo se reanude
            SceneManager.LoadScene(sceneNameToRestart); // Cargar la escena especificada
        }

        // Salir de la aplicación
        public void QuitGame()
        {
            Debug.Log("Saliendo del juego..."); // Solo visible en el editor
            Application.Quit(); // Cerrar la aplicación
        }
    }
}
