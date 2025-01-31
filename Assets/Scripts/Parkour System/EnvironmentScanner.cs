using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ParkourSystem
{
    public class EnvironmentScanner : MonoBehaviour
    {
        [SerializeField] private Vector3 forwardRayOffset = new Vector3(0, 0.5f, 0);
        [SerializeField] private float forwardRayLength = 0.8f;
        [SerializeField] private float secondForwardRayOffsetY = 1.0f;
        [SerializeField] private float secondForwardRayLength = 1.5f;
        [SerializeField] private float heightRayLength = 5f;
        [SerializeField] private float ledgeRayLength = 10f;
        [SerializeField] private float climbLedgeRayLength = 1.5f;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask climbLedgeLayer;
        [SerializeField] private float ledgeHeightThreshold = 0.75f;
        public bool InFrontOfObstacle { get; private set; }

        // Nuevo raycast superior para detectar obstáculos y ver si son aptos para slide Action
        [SerializeField] private float slideHeightRayOffsetY = 1.0f; // Altura del raycast superior
        [SerializeField] private float slideHeightRayLength = 1.0f;  // Longitud del raycast superior
        // [SerializeField] private LayerMask slideObstacleLayer;    // Capa opcional para obstáculos de deslizamiento

        public ObstacleHitData ObstacleCheck()
        {
            var hitData = new ObstacleHitData();

            var forwardOrigin = transform.position + forwardRayOffset;
            hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward, out hitData.forwardHit, forwardRayLength, obstacleLayer);

            Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength, (hitData.forwardHitFound) ? Color.green : Color.red);

            // Lógica para detección de Slide
            var upperOrigin = transform.position + forwardRayOffset + Vector3.up * slideHeightRayOffsetY; //origen del nuevo rayo más elevado
            bool lowerHit = hitData.forwardHitFound; // Reutilizamos el raycast inferior
            bool upperHit = Physics.Raycast(upperOrigin, transform.forward, out hitData.slideHit, slideHeightRayLength, obstacleLayer); //Lanzamos rayo y lo amacenamos en slideHit

            // Determina si es posible deslizarse
            hitData.CanSlide = !lowerHit && upperHit; //Si no hay nada debajo y arriba si (obviamente, deberá coincidir con el tag en el parkourACtion) le decimos que es posible como primer paso de verificación

            if (upperHit)
            {
                InFrontOfObstacle = true;
            }

            // Dibujado de rayos para depuración
            Debug.DrawRay(upperOrigin, transform.forward * slideHeightRayLength, upperHit ? Color.green : Color.red);


            if (hitData.forwardHitFound)
            {
                InFrontOfObstacle = true;
                Vector3 forwardOffset = transform.forward * 0.001f;
                var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength + forwardOffset;
                Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightHit, heightRayLength, obstacleLayer);

                Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (hitData.heightHitFound) ? Color.green : Color.red);

                var secondForwardOrigin = transform.position + forwardRayOffset + Vector3.up * secondForwardRayOffsetY;
                hitData.secondForwardHitFound = Physics.Raycast(secondForwardOrigin, transform.forward, out hitData.secondForwardHit, secondForwardRayLength, obstacleLayer);

                Debug.DrawRay(secondForwardOrigin, transform.forward * secondForwardRayLength, (hitData.secondForwardHitFound) ? Color.yellow : Color.blue);

                if (!hitData.secondForwardHitFound)
                {
                    var secondaryHeightOrigin = secondForwardOrigin + (transform.forward * secondForwardRayLength);
                    hitData.secondHeightHitFound = Physics.Raycast(secondaryHeightOrigin, Vector3.down, out hitData.secondHeightHit, heightRayLength, obstacleLayer);
                    Debug.DrawRay(secondaryHeightOrigin, Vector3.down * heightRayLength, (hitData.secondHeightHitFound) ? Color.cyan : Color.magenta);
                }
            }
            else
            {
                InFrontOfObstacle = false;
            }

            return hitData;
        }

        public bool ClimbLedgeCheck(Vector3 direction, out RaycastHit ledgeHit)
        {
            ledgeHit = new RaycastHit();

            if (direction == Vector3.zero)
                return false;

            var origin = transform.position + Vector3.up * 1.5f;
            var offset = new Vector3(0, 0.18f, 0);

            for (int i = 0; i < 10; i++)
            {
                Debug.DrawRay(origin + offset * i, direction);
                if(Physics.Raycast(origin + offset * i, direction, out RaycastHit hit, climbLedgeRayLength, climbLedgeLayer))
                {
                    ledgeHit = hit;
                    return true;
                }
            }

            return false;
        }
        
        public bool DropLedgeCheck(out RaycastHit ledgeHit)
        {
            ledgeHit = new RaycastHit();
            var origin = transform.position + Vector3.down * 0.1f + transform.forward * 0.2f;

            Debug.DrawRay(origin, transform.forward, Color.red);

            if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, 0.8f, climbLedgeLayer))
            {
                ledgeHit = hit;
                return true;
            }
            return false;
        }

        public bool ObstacleLedgeCheck(Vector3 moveDirection, out LedgeData ledgeData)
        {
            ledgeData = new LedgeData();

            if (moveDirection == Vector3.zero)
                return false;

            var originOffset = 0.5f; //Distancia entre los pies y el borde de un saliente
            var origin = transform.position + moveDirection * originOffset + Vector3.up;

            if(PhysicsUtil.ThreeRaycasts(origin, Vector3.down, 0.25f, transform, out List <RaycastHit> hits, ledgeRayLength, obstacleLayer, true))
            {
                var validHits = hits.Where(h => transform.position.y - h.point.y > ledgeHeightThreshold).ToList();

                if (validHits.Count > 0)
                {
                    var surfaceOrigin = validHits[0].point;
                    surfaceOrigin.y = transform.position.y - 0.1f;

                    if (Physics.Raycast(surfaceOrigin, transform.position - surfaceOrigin, out RaycastHit surfaceHit, 2, obstacleLayer))
                    {
                        Debug.DrawLine(surfaceOrigin, transform.position, Color.cyan);

                        float height = transform.position.y - validHits[0].point.y;
                        
                        ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
                        ledgeData.height = height;
                        ledgeData.surfaceHit = surfaceHit;

                        return true;
                    }
                }
            }

            return false;
        }

        public struct ObstacleHitData
        {
            public bool forwardHitFound;
            public bool heightHitFound;

            public RaycastHit forwardHit;
            public RaycastHit heightHit;

            // Campos para detección de deslizamiento
            public bool CanSlide; // Indica si es posible deslizarse
            public RaycastHit slideHit; // Información del raycast para deslizamiento

            public bool secondForwardHitFound;
            public bool secondHeightHitFound;
            public RaycastHit secondForwardHit;
            public RaycastHit secondHeightHit;
        }

        public struct LedgeData
        {
            public float height;
            public float angle;
            public RaycastHit surfaceHit;
        }
    }
}
