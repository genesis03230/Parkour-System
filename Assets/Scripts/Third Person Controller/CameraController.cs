using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GatherInput gatherInput;
        [SerializeField] private Transform target;

        [SerializeField] private float rotationSpeedMouse = 5f;
        [SerializeField] private float rotationSpeedGamepad = 100f;
        private float rotationSpeed;

        [SerializeField] private float distance = 5f;
        [SerializeField] private float minYAngle = -45f;
        [SerializeField] private float maxYAngle = 45f;

        [SerializeField] private Vector2 offset;
        [SerializeField] private bool invertX, invertY;

        private float rotationY, rotationX;

        [SerializeField] private float collisionRadius = 0.5f;
        [SerializeField] private LayerMask collisionLayers;
        private Vector3 currentCameraPosition;

        private void Start()
        {
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            float invertValueX = invertX ? -1 : 1;
            float invertValueY = invertX ? -1 : 1;

            float rotationSpeed = gatherInput.usingGamepad ? rotationSpeedGamepad : rotationSpeedMouse;

            rotationX += gatherInput.lookInput.y * invertValueY * rotationSpeed * Time.deltaTime;
            rotationY += gatherInput.lookInput.x * invertValueX * rotationSpeed * Time.deltaTime;
          
          

            rotationX = Mathf.Clamp(rotationX, minYAngle, maxYAngle);

            var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            var focusPosition = target.position + new Vector3 (offset.x, offset.y, 0);

            Vector3 desiredCameraPosition = focusPosition - targetRotation * new Vector3(0, 0, distance);

            if (Physics.SphereCast(focusPosition, collisionRadius, (desiredCameraPosition - focusPosition).normalized, out var hit, distance, collisionLayers))
            {
                float adjustedDistance = hit.distance - collisionRadius;
                currentCameraPosition = focusPosition - targetRotation * new Vector3(0, 0, adjustedDistance);
            }
            else
            {
                currentCameraPosition = desiredCameraPosition;
            }

            transform.SetPositionAndRotation(currentCameraPosition, targetRotation);

            //transform.SetPositionAndRotation(focusPosition - targetRotation * new Vector3(0,0,distance), targetRotation);
        }

        public Quaternion GetYRotation => Quaternion.Euler(0, rotationY, 0);

    }

}
