using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class TriggerWithCoroutine : MonoBehaviour
    {
        public AudioSource audioSource; // Asigna aquí el AudioSource desde el Inspector
        public float delay = 0f; // Tiempo de retraso antes de reproducir el sonido

        private bool hasTriggered = false; // Para evitar múltiples activaciones

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Deberia sonar algo");
            // Verifica si el objeto que entra al trigger es el Player
            if (other.CompareTag("Player") && !hasTriggered)
            {
                hasTriggered = true; // Evita que se vuelva a activar
                StartCoroutine(PlaySoundAndDestroy());
            }
        }

        private IEnumerator PlaySoundAndDestroy()
        {
            yield return new WaitForSeconds(delay);
            // Reproduce el sonido si no está reproduciéndose
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            StartCoroutine(DestroyAfterSound());
        }

        private IEnumerator DestroyAfterSound()
        {
            // Espera a que termine de reproducirse el audio
            yield return new WaitWhile(() => audioSource.isPlaying);

            // Destruye el objeto
            Destroy(gameObject);
        }
    }   
}
