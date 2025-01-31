using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ParkourSystem
{
    public class ScreenFade : MonoBehaviour
    {
        public Image blackScreen; // La imagen negra que cubre la pantalla.
        public float fadeDuration = 1.0f; // Duración del fade in/out.
        public AudioSource audioSource; // El sonido que se reproducirá antes del fade in.

        private void Start()
        {
            // Inicializa la pantalla completamente negra.
            blackScreen.color = new Color(0, 0, 0, 1);

            // Comienza la rutina para reproducir el sonido y luego hacer el fade in.
            StartCoroutine(PlaySoundAndFadeIn());
        }

        private IEnumerator PlaySoundAndFadeIn()
        {
            // Reproduce el sonido.
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();

                // Espera a que termine el sonido.
                yield return new WaitForSeconds(audioSource.clip.length);
            }

            // Inicia el fade in después del sonido.
            yield return StartCoroutine(FadeIn());
        }

        public IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 0); // Asegura que sea completamente transparente.
        }

        public IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 1); // Asegura que sea completamente opaco.
        }
    }
}
