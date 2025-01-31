using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class TextActivator : MonoBehaviour
    {
        public GameObject textObject; // Objeto de texto a activar/desactivar.
        public float activationDelay = 2.0f; // Tiempo antes de activar el texto.
        public float deactivationDelay = 5.0f; // Tiempo antes de desactivar el texto.

        private void Start()
        {
            // Asegúrate de que el texto esté inicialmente desactivado.
            if (textObject != null)
            {
                textObject.SetActive(false);
            }

            // Inicia el proceso de activación y desactivación.
            StartCoroutine(ActivateAndDeactivateText());
        }

        private IEnumerator ActivateAndDeactivateText()
        {
            // Espera el tiempo de activación.
            yield return new WaitForSeconds(activationDelay);

            // Activa el objeto de texto.
            if (textObject != null)
            {
                textObject.SetActive(true);
            }

            // Espera el tiempo de desactivación.
            yield return new WaitForSeconds(deactivationDelay);

            // Desactiva el objeto de texto.
            if (textObject != null)
            {
                textObject.SetActive(false);
            }
        }
    }
}
