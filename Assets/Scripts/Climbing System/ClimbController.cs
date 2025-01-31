using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ParkourSystem
{
    public class ClimbController : MonoBehaviour
    {

        [SerializeField] private GatherInput gatherInput;
        private EnvironmentScanner envScanner;
        private PlayerController playerController;

        private ClimbPoint currentPoint;

        public static bool jumpFromLedge = false; //Se agrega booleano a modo de prueba

        [Header("Climb Offsets")]
        [SerializeField] private Vector3 handOffsetIdleToHang = new Vector3(0.23f, 0.1f, 0.07f);
        [SerializeField] private Vector3 handOffsetUp = new Vector3(0.23f, 0.1f, 0.18f);
        [SerializeField] private Vector3 handOffsetDown = new Vector3(0.2f, 0.12f, 0.12f);
        [SerializeField] private Vector3 handOffsetLeft = new Vector3(0.37f, 0.11f, 0.08f);
        [SerializeField] private Vector3 handOffsetRight = new Vector3(0.2f, 0.1f, 0.1f);
        [SerializeField] private Vector3 handOffsetShimmyRight = new Vector3(0.2f, 0.04f, 0.12f);
        [SerializeField] private Vector3 handOffsetShimmyLeft = new Vector3(0.2f, 0.04f, 0.13f);
        [SerializeField] private Vector3 handOffsetDropToHang = new Vector3(0.2f, 0.32f, -0.1f);
        [SerializeField] private Vector3 handOffsetRightShimmyLedge = new Vector3(0.2f, 0.04f, 0.12f); // Se agregan nuevos offset para pruebas de animaciones
        [SerializeField] private Vector3 handOffsetLeftShimmyLedge = new Vector3(0.2f, 0.04f, 0.13f);
        [SerializeField] private Vector3 hangHopRightAlternate = new Vector3(0.2f, 0.04f, 0.13f);
        [SerializeField] private Vector3 HangHopDownAlternate = new Vector3(0.2f, 0.04f, 0.13f);

        private void Awake()
        {
            envScanner = GetComponent<EnvironmentScanner>();
            playerController = GetComponent<PlayerController>();
        }

        void Update()
        {
            if (!playerController.IsHanging)
            {
                if (gatherInput.tryToJump && !playerController.InAction && playerController.IsGrounded())
                {
                    if (envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                    {
                        currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                        SoundManager.Instance.Play("Strain_M2", 0.3f);
                        SoundManager.Instance.Play("Hand", 0.6f);
                        playerController.SetControl(false);
                        StartCoroutine(JumpToLedge("IdleToHang", currentPoint.transform, 0.39f, 0.55f, handOffset : handOffsetIdleToHang));
                        gatherInput.tryToJump = false;
                    }
                }

                if (gatherInput.tryToDrop && !playerController.InAction && playerController.IsGrounded())
                {
                    if (envScanner.DropLedgeCheck(out RaycastHit ledgeHit))
                    {
                        SoundManager.Instance.Play("Strain_M6");
                        SoundManager.Instance.Play("Hand", 1f);
                        currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);
                        playerController.SetControl(false);
                        StartCoroutine(JumpToLedge("DropToHang", currentPoint.transform, 0.30f, 0.45f, handOffset : handOffsetDropToHang));
                        gatherInput.tryToDrop = false;
                    }
                }
            }
            else
            {
                if (gatherInput.tryToDrop && !playerController.InAction && !jumpFromLedge)
                {
                    StartCoroutine(JumpFromHang());
                    gatherInput.tryToDrop = false;
                    return;
                }

                if (gatherInput.tryToDrop && !playerController.InAction && jumpFromLedge) //Se agrega caida desde ledge al suelo
                {
                    StartCoroutine(JumpFromLedge());
                    gatherInput.tryToDrop = false;
                    return;
                }

                float h = Mathf.Round(gatherInput.Direction.x);
                float v = Mathf.Round(gatherInput.Direction.y);
                var inputDir = new Vector2(h, v);

                if (playerController.InAction || inputDir == Vector2.zero) return;

                if (currentPoint.MountPoint && inputDir.y == 1)
                {
                    StartCoroutine(MountFromHang());
                    return;
                }

                var neighbour = currentPoint.GetNeighbour(inputDir);

                if (neighbour == null) return;

                if (neighbour.connectionType == ConnectionType.Jump && gatherInput.tryToJump)
                {
                    currentPoint = neighbour.point;

                    if (neighbour.direction.y == 1)
                    {
                        SoundManager.Instance.Play("Strain_M1");
                        SoundManager.Instance.Play("Hand", 0.6f);
                        StartCoroutine(JumpToLedge("HangHopUp", currentPoint.transform, 0.34f, 0.65f, handOffset: handOffsetUp));
                    }
                    else if (neighbour.direction.y == -1)
                    {
                        SoundManager.Instance.Play("Strain_M6");
                        SoundManager.Instance.Play("Hand", 0.6f);
                        SoundManager.Instance.Play("Hand2", 0.65f);
                        StartCoroutine(JumpToLedge("HangHopDown", currentPoint.transform, 0.31f, 0.65f, handOffset: handOffsetDown));
                    }
                    else if (neighbour.direction.x == 1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("Hand", 0.6f);
                        StartCoroutine(JumpToLedge("HangHopRight", currentPoint.transform, 0.23f, 0.44f, handOffset: handOffsetRight));
                    }
                    else if (neighbour.direction.x == -1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("Hand", 0.6f);
                        StartCoroutine(JumpToLedge("HangHopLeft", currentPoint.transform, 0.23f, 0.44f, handOffset: handOffsetLeft));
                    }

                    //gatherInput.tryToJump = false;  Si colocamos esta linea, el Player debera ir presionando el boton de salto para moverse de un saliente a otro y no mantenerlo apretado
                }
                else if (neighbour.connectionType == ConnectionType.Move)
                {
                    currentPoint = neighbour.point;
                    if (neighbour.direction.x == 1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("Hand", 0.3f);
                        SoundManager.Instance.Play("Hand2", 0.8f);
                        StartCoroutine(JumpToLedge("ShimmyRight", currentPoint.transform, 0f, 0.38f, handOffset: handOffsetShimmyRight));
                    }
                    else if (neighbour.direction.x == -1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("Hand", 0.3f);
                        SoundManager.Instance.Play("Hand2", 0.8f);
                        StartCoroutine(JumpToLedge("ShimmyLeft", currentPoint.transform, 0f, 0.38f, AvatarTarget.LeftHand, handOffset: handOffsetShimmyLeft));
                    }
                }

                //Se agrega linea de prueba para Animacion alternativa de colgarse en cornisas
                else if (neighbour.connectionType == ConnectionType.Move_Ledge)
                {
                    currentPoint = neighbour.point;
                    if (neighbour.direction.x == 1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("HandM", 0.5f);
                        SoundManager.Instance.Play("HandM2", 1.2f);
                        StartCoroutine(JumpToLedge("ShimmyLedgeRight", currentPoint.transform, 0f, 0.38f, handOffset: handOffsetRightShimmyLedge));
                    }
                       
                    else if (neighbour.direction.x == -1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("HandM", 0.5f);
                        SoundManager.Instance.Play("HandM2", 1.2f);
                        StartCoroutine(JumpToLedge("ShimmyLedgeLeft", currentPoint.transform, 0f, 0.38f, AvatarTarget.LeftHand, handOffset: handOffsetLeftShimmyLedge));
                    }
                        
                }
                //Se agrega linea de prueba para Animacion alternativa de saltar colgado
                else if (neighbour.connectionType == ConnectionType.Jump_Ledge)
                {
                    currentPoint = neighbour.point;
                    if (neighbour.direction.x == 1)
                    {
                        SoundManager.Instance.Play("Strain_M4");
                        SoundManager.Instance.Play("HandM", 0.6f);
                        StartCoroutine(JumpToLedge("HangHopRightAlternate", currentPoint.transform, 0f, 0.38f, handOffset: hangHopRightAlternate));
                        jumpFromLedge = true; //Se agrega a booleano para comprobar que puedo caerme al suelo
                    }
                }
                //Se agrega linea de prueba para Animacion alternativa de caer y colgarse
                else if (neighbour.connectionType == ConnectionType.Falling_Ledge && gatherInput.tryToJump)
                {
                    currentPoint = neighbour.point;
                    if (neighbour.direction.y == -1)
                    {
                        SoundManager.Instance.Play("HandHard", 0.3f);
                        SoundManager.Instance.Play("Strain_MF5", 0.4f);
                        StartCoroutine(JumpToLedge("HangHopDownAlternate", currentPoint.transform, 0.15f, 0.45f, handOffset: HangHopDownAlternate));
                    }
                     
                }
            }
        }

        private IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime, 
                                        AvatarTarget hand = AvatarTarget.RightHand, Vector3? handOffset = null)
        {
            var matchParams = new MatchTargetParams()
            {
                pos = GetHandPosition(ledge, hand, handOffset),
                bodyPart = hand,
                startTime = matchStartTime,
                targetTime = matchTargetTime,
                posWeight = Vector3.one
            };

            var targetRotation = Quaternion.LookRotation(-ledge.forward);

            yield return playerController.DoAction(anim, matchParams, targetRotation, true, true);

            playerController.IsHanging = true;
        }

        private IEnumerator JumpFromHang()
        {
            SoundManager.Instance.Play("Strain_S8", 0.2f);
            playerController.IsHanging = false;
            yield return playerController.DoAction("JumpFromHang");
            playerController.ResetTargetRotation();
            playerController.SetControl(true);
        }

        private IEnumerator JumpFromLedge() //Se agrega metodo para caer al suelo mientras cuelga de un ledge
        {
            playerController.IsHanging = false;
            yield return playerController.DoAction("JumpFromLedge");
            playerController.ResetTargetRotation();
            playerController.SetControl(true);
            jumpFromLedge = false;
        }

        private IEnumerator MountFromHang()
        {
            SoundManager.Instance.Play("Strain_S5");
            playerController.IsHanging = false;
            yield return playerController.DoAction("MountFromHang");

            playerController.EnableCharacterController(true);

            yield return new WaitForSeconds(0.5f);

            playerController.ResetTargetRotation();
            playerController.SetControl(true);
        }

        private Vector3 GetHandPosition(Transform ledge, AvatarTarget hand, Vector3? handOffset)
        {
            var offsetValue = (handOffset != null) ? handOffset.Value : new Vector3(0.2f, 0.07f, 0.15f);
            var horizontalDir = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;
            return ledge.position + ledge.forward * offsetValue.z + Vector3.up * offsetValue.y - horizontalDir * offsetValue.x;
        }

        private ClimbPoint GetNearestClimbPoint(Transform ledge, Vector3 hitPoint)
        {
            var points = ledge.GetComponentsInChildren<ClimbPoint>();

            ClimbPoint nearestPoint = null;
            float nearestPointDistance = Mathf.Infinity;

            foreach (var point in points)
            {
                float distance = Vector3.Distance(point.transform.position, hitPoint);

                if (distance < nearestPointDistance)
                {
                    nearestPoint = point;
                    nearestPointDistance = distance;
                }
            }

            return nearestPoint;
        }

    }
}
