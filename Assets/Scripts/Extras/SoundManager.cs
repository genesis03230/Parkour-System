using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance; // Singleton

        [System.Serializable]
        public class Sound
        {
            public string name; // Nombre del sonido
            public AudioClip clip; // Archivo de audio
            public float volume = 1f; // Volumen del sonido
            [Range(0.1f, 3f)] public float pitch = 1f; // Tono del sonido
            public bool loop = false; // Si el sonido debe repetirse
            public float delay = 0f; // Retraso en segundos antes de reproducir
        }

        public Sound[] sounds; // Array de sonidos
        private Dictionary<string, Sound> soundDictionary; // Diccionario para acceso rápido
        private List<AudioSource> audioSourcePool; // Pool dinamico de AudioSources

        public int initialPoolSize = 10; // Tamaño inicial del pool

        private void Awake()
        {
            // Implementación del Singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persistencia entre escenas
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            //Inicializar el diccionario de sonidos
            soundDictionary = new Dictionary<string, Sound>();
            foreach (var sound in sounds)
            {
                soundDictionary[sound.name] = sound;
            }

            //Crear el pool inicial de AudioSources
            audioSourcePool = new List<AudioSource>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewAudioSource();
            }

        }

        //Crear un nuevo AudioSource y agregar al pool
        private AudioSource CreateNewAudioSource()
        {
            AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            audioSourcePool.Add(newAudioSource);
            return newAudioSource;
        }

        // Obtener un AudioSource disponible del pool
        private AudioSource GetAvailableAudioSource()
        {
            foreach (var audioSource in audioSourcePool)
            {
                if (!audioSource.isPlaying) // Verificar si no está reproduciendo
                {
                    return audioSource;
                }
            }

            // Si no hay ninguno disponible, crear uno nuevo
            return CreateNewAudioSource();
        }

        // Método para reproducir un sonido por su nombre, con opción de especificar un delay y el volumen
        public void Play(string soundName, float customDelay = -1f, float customVolume = -1f)
        {
            if (soundDictionary.TryGetValue(soundName, out var sound))
            {
                AudioSource audioSource = GetAvailableAudioSource();

                float delayToUse = (customDelay >= 0f) ? customDelay : sound.delay;
                float volumeToUse = (customVolume >= 0f) ? customVolume : sound.volume;

                audioSource.clip = sound.clip;
                audioSource.volume = volumeToUse;
                audioSource.pitch = sound.pitch;
                audioSource.loop = sound.loop;

                if (delayToUse > 0f)
                {
                    audioSource.PlayDelayed(delayToUse);
                }
                else
                {
                    audioSource.Play();
                }
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
            }
        }

        // Método para detener un sonido específico por nombre
        public void Stop(string soundName)
        {
            foreach (var audioSource in audioSourcePool)
            {
                if (audioSource.isPlaying && audioSource.clip != null && audioSource.clip.name == soundName)
                {
                    audioSource.Stop();
                    break;
                }
            }
        }

        // Método para cambiar el volumen de un sonido específico en el diccionario
        public void SetVolume(string soundName, float volume)
        {
            if (soundDictionary.TryGetValue(soundName, out var sound))
            {
                sound.volume = volume;
            }
            else
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
            }
        }

        //Instrucciones del SoundManager:

        // Reproducir un sonido
        // SoundManager.Instance.Play("Footstep");

        // Detener un sonido
        // SoundManager.Instance.Stop("Footstep");

        // Cambiar el volumen de un sonido
        // SoundManager.Instance.SetVolume("Footstep", 0.5f); Volumen a un 50%

        // Cambiar el delay y el volumen
        // SoundManager.Instance.SetVolume("Footstep", 2f, 0.5f); Delay de 2 segundos de retraso + volumen 50%
    }
}

