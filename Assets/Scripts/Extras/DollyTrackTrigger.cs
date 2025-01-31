using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace ParkourSystem
{
    public class DollyTrackTrigger : MonoBehaviour
    {
        public GameObject virtualCamera; // Cámara virtual asociada al Dolly Track
        public GameObject dollyTrack; // Dolly Track
        public Camera mainCamera; // Cámara principal
        public Camera finalCamera; // Cámara final
        public Image blackScreen; // Imagen negra para el fade
        public float fadeDuration = 1.0f; // Duración del fade
        public AudioSource dialogueAudio; // Audio del diálogo breve
        public AudioSource finalMusicAudio; // Audio de la música final
        public Canvas messageCanvas; // Canvas para mostrar el texto al final
        public float dollyDuration = 18.0f; // Duración del recorrido del Dolly Cart

        private void Start()
        {
            // Asegurarse de que los objetos iniciales estén desactivados
            finalCamera.gameObject.SetActive(false);
            virtualCamera.gameObject.SetActive(false);
            dollyTrack.gameObject.SetActive(false);
            messageCanvas.gameObject.SetActive(false);
            finalMusicAudio.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(HandleDollySequence());
            }
        }

        private IEnumerator HandleDollySequence()
        {
            // Reproducir el diálogo breve
            dialogueAudio.Play();
            yield return new WaitForSeconds(dialogueAudio.clip.length);

            // Transición a la cámara del Dolly Track
            yield return StartCoroutine(FadeOut(mainCamera));
            mainCamera.gameObject.SetActive(false);

            finalCamera.gameObject.SetActive(true);
            virtualCamera.gameObject.SetActive(true);
            dollyTrack.gameObject.SetActive(true);
            finalMusicAudio.gameObject.SetActive(true);

            // Iniciar el Dolly Track y la música final
            finalMusicAudio.Play();
            yield return StartCoroutine(FadeIn(finalCamera));

            // Esperar a que termine el recorrido del Dolly Track
            yield return new WaitForSeconds(dollyDuration);

            // Mostrar el texto final (sin fade)
            messageCanvas.gameObject.SetActive(true);
        }

        private IEnumerator FadeIn(Camera camera)
        {
            blackScreen.gameObject.SetActive(true);
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 0);
            blackScreen.gameObject.SetActive(false);
        }

        private IEnumerator FadeOut(Camera camera)
        {
            blackScreen.gameObject.SetActive(true);
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 1);
        }
    }
}
