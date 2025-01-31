using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class AudioTransition : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource currentAmbientAudio; // Sonido que se está reproduciendo
        public AudioSource newAmbientAudio;     // Sonido que debe empezar a reproducirse

        [Header("Fade Settings")]
        public float fadeDuration = 2.0f;       // Duración del fade (en segundos)

        private bool hasTriggered = false;      // Para evitar múltiples activaciones

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !hasTriggered)
            {
                hasTriggered = true; // Asegura que esto solo se ejecute una vez
                StartCoroutine(FadeOutAndIn());
            }
        }

        private IEnumerator FadeOutAndIn()
        {
            // Opcional: Asegúrate de que ambos audios no estén en bucle
            if (currentAmbientAudio != null) currentAmbientAudio.loop = false;
            if (newAmbientAudio != null) newAmbientAudio.loop = true;

            // Fade Out del audio actual
            if (currentAmbientAudio != null)
            {
                float startVolume = currentAmbientAudio.volume;
                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    currentAmbientAudio.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                    yield return null;
                }
                currentAmbientAudio.volume = 0;
                currentAmbientAudio.Stop();
            }

            // Fade In del nuevo audio
            if (newAmbientAudio != null)
            {
                newAmbientAudio.volume = 0;
                newAmbientAudio.Play();

                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    newAmbientAudio.volume = Mathf.Lerp(0, 1, t / fadeDuration);
                    yield return null;
                }
                newAmbientAudio.volume = 1;
            }
        }
    }
}
