using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace ParkourSystem
{
    public class SlowMotionTrigger : MonoBehaviour
    {
        [Header("Slow Motion Settings")]
        public float slowMotionFactor = 0.2f; // Factor de ralentización
        public float slowMotionDuration = 2f; // Duración del slow motion
        public float restorationDuration = 1f; // Duración de la restauración gradual

        [Header("Audio Settings")]
        public AudioClip slowMotionStartSound; // Sonido inicial
        public AudioClip slowMotionEndSound; // Sonido final
        public float slowMotionSoundPitch = 0.5f; // Control de la velocidad de reproducción del sonido inicial
        public float slowMotionEndSoundPitch = 1.0f; // Control de la velocidad de reproducción del sonido final

        [Header("Vignette Settings")]
        public float vignetteIntensityDuringSlowMotion = 0.5f; // Intensidad máxima de viñeta
        public float vignetteSmoothnessDuringSlowMotion = 0.8f; // Suavidad máxima de viñeta
        public float vignetteFadeSpeed = 1f; // Velocidad de transición de la viñeta

        private AudioSource audioSource;
        private Vignette vignetteEffect; // Referencia al efecto de viñeta
        private Volume postProcessingVolume; // Volumen de post-procesado

        [System.Obsolete]
        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            // Busca un Post-Processing Volume en la escena y obtén el efecto de viñeta
            postProcessingVolume = FindObjectOfType<Volume>();
            if (postProcessingVolume != null && postProcessingVolume.profile.TryGet(out Vignette vignette))
            {
                vignetteEffect = vignette;
            }
            else
            {
                Debug.LogWarning("No se encontró un Volume de Post-Processing con un efecto de Vignette.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Verifica si el objeto que entra es el Player
            if (other.CompareTag("Player"))
            {
                StartCoroutine(ActivateSlowMotion());
            }
        }

        private IEnumerator ActivateSlowMotion()
        {
            // Reproducir el sonido inicial inmediatamente
            if (audioSource != null && slowMotionStartSound != null)
            {
                audioSource.pitch = slowMotionSoundPitch; // Ajusta el pitch para el sonido incial
                audioSource.PlayOneShot(slowMotionStartSound);
            }

            // Aplicar el slow motion inmediatamente
            Time.timeScale = slowMotionFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Aumentar gradualmente el efecto de viñeta al inicio
            float elapsedTime = 0f;
            while (elapsedTime < slowMotionDuration / 2f)
            {
                elapsedTime += Time.unscaledDeltaTime * vignetteFadeSpeed;

                if (vignetteEffect != null)
                {
                    vignetteEffect.intensity.value = Mathf.Lerp(0f, vignetteIntensityDuringSlowMotion, elapsedTime / (slowMotionDuration / 2f));
                    vignetteEffect.smoothness.value = Mathf.Lerp(0f, vignetteSmoothnessDuringSlowMotion, elapsedTime / (slowMotionDuration / 2f));
                }

                yield return null;
            }

            // Mantener el slow motion durante su duración
            yield return new WaitForSecondsRealtime(slowMotionDuration);

            // Reproducir el sonido final antes de restaurar gradualmente
            if (audioSource != null && slowMotionEndSound != null)
            {

                audioSource.pitch = slowMotionEndSoundPitch; // Ajusta el pitch para el sonido final
                audioSource.PlayOneShot(slowMotionEndSound);
            }

            // Restaurar gradualmente el tiempo y desvanecer la viñeta
            elapsedTime = 0f;
            while (elapsedTime < restorationDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;

                if (vignetteEffect != null)
                {
                    vignetteEffect.intensity.value = Mathf.Lerp(vignetteIntensityDuringSlowMotion, 0f, elapsedTime / restorationDuration);
                    vignetteEffect.smoothness.value = Mathf.Lerp(vignetteSmoothnessDuringSlowMotion, 0f, elapsedTime / restorationDuration);
                }

                // Restaurar gradualmente el tiempo a la normalidad
                Time.timeScale = Mathf.Lerp(slowMotionFactor, 1f, elapsedTime / restorationDuration);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                yield return null;
            }

            // Asegurarse de restaurar completamente al final
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = 0f;
                vignetteEffect.smoothness.value = 0f;
            }
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}

