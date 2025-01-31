using UnityEngine;

namespace ParkourSystem
{
    public class SkyboxRotation : MonoBehaviour
    {
        [SerializeField] float speed = 10f;

        void Update()
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
        }
    }
}
