using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class PlaySoundOnceOnTouch : MonoBehaviour
    {
        public AudioSource audioSource; // Asigna el AudioSource desde el Inspector

        private bool hasPlayed = false; // Para asegurarnos de que el sonido se reproduzca una sola vez

        private void OnTriggerEnter(Collider other)
        {
            // Verifica si el Player entra al trigger y si el sonido no se ha reproducido aún
            if (other.CompareTag("Player") && !hasPlayed)
            {
                hasPlayed = true; // Marca como reproducido
                PlaySound();
            }
        }

        private void PlaySound()
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Opcional: Desactivar o destruir el objeto después de reproducir el sonido
            // gameObject.SetActive(false); // Desactiva el objeto
            // Destroy(gameObject); // Destruye el objeto si no es necesario mantenerlo
        }
    }
}
