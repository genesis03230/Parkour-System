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
            // Aseg�rate de que el texto est� inicialmente desactivado.
            if (textObject != null)
            {
                textObject.SetActive(false);
            }

            // Inicia el proceso de activaci�n y desactivaci�n.
            StartCoroutine(ActivateAndDeactivateText());
        }

        private IEnumerator ActivateAndDeactivateText()
        {
            // Espera el tiempo de activaci�n.
            yield return new WaitForSeconds(activationDelay);

            // Activa el objeto de texto.
            if (textObject != null)
            {
                textObject.SetActive(true);
            }

            // Espera el tiempo de desactivaci�n.
            yield return new WaitForSeconds(deactivationDelay);

            // Desactiva el objeto de texto.
            if (textObject != null)
            {
                textObject.SetActive(false);
            }
        }
    }
}
