using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class DirectionalLightRotation : MonoBehaviour
    {
        [SerializeField] float rotationSpeed = 10f;
        
        void Update()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
