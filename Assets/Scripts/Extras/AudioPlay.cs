using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class AudioPlay : MonoBehaviour
    {
        public AudioSource audioSource; // Asigna el AudioSource desde el Inspector
        public float fadeDuration = 3f; // Duración del fade in en segundos

        private void OnTriggerEnter(Collider other)
        {
            // Verifica si el objeto que entra es el Player
            if (other.CompareTag("Player"))
            {
                PlayAudio();
            }
        }

        public void PlayAudio()
        {
            if (audioSource != null)
            {
                StartCoroutine(FadeInAndPlay());
            }
            else
            {
                Debug.LogWarning("No se asignó AudioSource en el Inspector.");
            }
        }

        private IEnumerator FadeInAndPlay()
        {
            float targetVolume = audioSource.volume; // Guarda el volumen objetivo
            audioSource.volume = 0; // Asegúrate de que el volumen inicial sea 0
            audioSource.Play(); // Inicia el audio

            // Aumenta gradualmente el volumen
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
                yield return null;
            }

            // Asegúrate de que el volumen sea el objetivo al final
            audioSource.volume = targetVolume;
        }
    }
}
