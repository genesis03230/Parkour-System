using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class AudioStop : MonoBehaviour
    {
        public AudioSource audioSource; // Asigna el AudioSource desde el Inspector
        public float fadeDuration = 3f; // Duración del fade out en segundos

        private void OnTriggerEnter(Collider other)
        {
            // Verifica si el objeto que entra es el Player
            if (other.CompareTag("Player"))
            {
                StopAudio();
            }
        }

        public void StopAudio()
        {
            if (audioSource != null)
            {
                StartCoroutine(FadeOutAndStop());
            }
            else
            {
                Debug.LogWarning("No se asignó AudioSource en el Inspector.");
            }
        }

        private IEnumerator FadeOutAndStop()
        {
            float startVolume = audioSource.volume; // Guarda el volumen inicial

            // Reduce gradualmente el volumen
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }

            // Asegúrate de que el volumen sea 0 al final
            audioSource.volume = 0;
            audioSource.Stop(); // Detén el audio
            audioSource.volume = startVolume; // Restaura el volumen inicial por si se vuelve a usar el AudioSource
        }
    }
}

