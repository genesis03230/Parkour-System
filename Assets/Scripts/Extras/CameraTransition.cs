using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ParkourSystem
{
    public class CameraTransition : MonoBehaviour
    {
        public Image blackScreen; // Imagen negra para el fade.
        public float fadeDuration = 1.0f; // Duración del fade in/out.
        public Camera mainCamera; // Cámara principal.
        public Camera secondaryCamera; // Cámara para mostrar al personaje.
        //public AudioSource transitionSound; // Efecto de sonido para la transición.
        public AudioSource dialogueAudio; // Audio del diálogo del personaje.
        public float dialogueDuration = 3.0f; // Duración del diálogo antes de volver a la cámara principal.
        public Animator celesteAnimator;
        public float resetDelay = 1.0f;

        private void Start()
        {
            // Pantalla negra al inicio.
            //blackScreen.color = new Color(0, 0, 0, 1);
            //StartCoroutine(FadeIn(mainCamera));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (celesteAnimator != null)
                {
                    celesteAnimator.SetBool("greeting", true);
                    StartCoroutine(ResetGreeting(celesteAnimator));
                }

                else
                {
                    Debug.LogError("No Animator found on the Player");
                }

                StartCoroutine(TransitionCameras());
            }
        }

        private IEnumerator ResetGreeting(Animator animator)
        {
            // Esperar hasta que el estado actual sea "Standing Greeting"
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Standing Greeting"))
            {
                yield return null; // Esperar un frame
            }

            // Esperar a que termine la animación
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            // Volver a establecer Greeting en false
            animator.SetBool("greeting", false);
        }

        private IEnumerator TransitionCameras()
        {
            // Fade out de la cámara principal con efecto de sonido.
            //transitionSound.Play();
            yield return StartCoroutine(FadeOut(mainCamera));

            // Cambiar a la cámara secundaria con fade in.
            mainCamera.gameObject.SetActive(false);
            secondaryCamera.gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn(secondaryCamera));

            // Reproducir el diálogo del personaje.
            dialogueAudio.Play();
            yield return new WaitForSeconds(dialogueDuration);

            // Fade out de la cámara secundaria con efecto de sonido.
            //transitionSound.Play();
            yield return StartCoroutine(FadeOut(secondaryCamera));

            // Volver a la cámara principal con fade in.
            secondaryCamera.gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn(mainCamera));
        }

        public IEnumerator FadeIn(Camera camera)
        {
            camera.gameObject.SetActive(true);
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 0);
        }

        public IEnumerator FadeOut(Camera camera)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 1);
            camera.gameObject.SetActive(false);
        }
    }
}
