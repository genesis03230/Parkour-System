using UnityEngine;

namespace ParkourSystem
{
    public class FootIKController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask obstacleLayer;

        // Transforms de los pies izquierdo y derecho
        [SerializeField] private Transform leftFootTarget;
        [SerializeField] private Transform rightFootTarget;

        // Desplazamiento para evitar que los pies atraviesen la pared
        [SerializeField] private float footOffset = 0.2f;

        // Velocidad de la interpolación del IK
        [SerializeField] private float ikLerpSpeed = 0.2f;

        // Peso del IK, 0 = sin IK, 1 = completo
        [SerializeField] private float ikWeight = 0f;

        //enable IK lleva el peso del IK a 0 cuando es falso, haciendo que no tenga efecto (para no interferir cuando no es necesario)
        private bool enableIK = false;

        private void Update()
        {
            if (animator)
            {
                if (ShouldEnableIK())
                {
                    enableIK = true;
                }
                else
                {
                    enableIK = false;
                }
            }
        }


        private void OnAnimatorIK (int layerIndex)
        {
            // Si el animator está activo...
            if (animator)
            {
                // ...según el enableIK, Mandaremos suavizar los pesos del IK hacia 1 (activado) o 0 (desactivado) interpolando valores
                if (enableIK)
                    ikWeight = Mathf.MoveTowards(ikWeight, 1, ikLerpSpeed * Time.deltaTime);
               else
                    ikWeight = Mathf.MoveTowards(ikWeight, 0, ikLerpSpeed * Time.deltaTime);

                // Calcular la posición de los pies usando un Raycast
                AdjustFootToWall(AvatarIKGoal.LeftFoot, leftFootTarget);
                AdjustFootToWall(AvatarIKGoal.RightFoot, rightFootTarget);
            }
        }

        private void AdjustFootToWall(AvatarIKGoal foot, Transform footTarget)
        {
            //posición origen de los pies
            Vector3 footPosition = animator.GetIKPosition(foot);

            //Raycast para chocar contra la pared
            if (Physics.Raycast(footPosition, -footTarget.forward, out RaycastHit hit, 1f, obstacleLayer))
            {
                // Ajustar la posición y rotación del pie en función de la normal de la pared
                Vector3 newFootPosition = hit.point + hit.normal * footOffset;
                animator.SetIKPositionWeight(foot, ikWeight);
                animator.SetIKPosition(foot, newFootPosition);

                // Ajustar la rotación del pie según la normal de la superficie impactada
                Quaternion footRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
                animator.SetIKRotationWeight(foot, ikWeight);
                animator.SetIKRotation(foot, footRotation);
            }
            else
            {
                // Si no se detecta pared, aplicar solo el peso del IK sin modificar la posición ni rotación
                animator.SetIKPositionWeight(foot, ikWeight);
                animator.SetIKRotationWeight(foot, ikWeight);
            }
        }

        //Control automático para activar/desactivar el IK
        private bool ShouldEnableIK()
        {
            //Tomamos la información de la animación actual
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            //Si la animación es HangingIdle y no estamos en transición...
            if (stateInfo.IsName("HangingIdle") && !animator.IsInTransition(0)) //&& stateInfo.normalizedTime > 0.05f  || antes también le añadía un pequeño delay, pero no es obligatorio
            {
                return true; //..."activaremos" el IK
            }
            else
            {
                return false; // de lo contrario, lo "desactivaremos".
            }


        }
    }

}

