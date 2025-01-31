using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    public class ParkourController : MonoBehaviour
    {
        [SerializeField] private GatherInput gatherInput;

        [SerializeField] private List<ParkourAction> parkourActions;
        [SerializeField] private List<ParkourAction> longParkourActions;
        [SerializeField] private ParkourAction[] vaultFenceActions;
        [SerializeField] private ParkourAction jumpDownAction, autoJumpDownAction;
        [SerializeField] private ParkourAction finalAction;
        [SerializeField] private float autoJumpHeightLimit = 1f; //Limite de altura donde el player salta automaticamente
        [SerializeField] private ParkourAction slideAction; // Nueva acción de deslizamiento

        [SerializeField] private float angleThreshold = 50f;

        private EnvironmentScanner scanner;
        private PlayerController playerController;

        public bool AbleToAutoJump { get; set; }

        private void Awake()
        {
            scanner = GetComponent<EnvironmentScanner>();
            playerController = GetComponent<PlayerController>();
            AbleToAutoJump = true;
        }

        private void Update()
        {
            var hitData = scanner.ObstacleCheck();

            //Se agrega codigo para comprobar y poder deslizarse
            if (hitData.CanSlide && gatherInput.tryToDrop && !playerController.InAction)
            {
                if (slideAction.CheckIfPossible(hitData, transform))
                {
                    StartCoroutine(DoParkourAction(slideAction));
                    gatherInput.tryToDrop = false;
                    SoundManager.Instance.Play("Strain_M4");
                    SoundManager.Instance.Play("Slide");
                }
            }

            if (gatherInput.tryToLong && !playerController.InAction && !playerController.IsHanging && playerController.IsGrounded())
            {
                if (!hitData.secondForwardHitFound)
                {
                    foreach (var action in longParkourActions)
                    {
                        if (action.CheckIfPossible(hitData, transform))
                        {
                            switch (action.name)
                            {
                                // Seleccionar el sonido según el nombre de la acción
                                case "BigJump":
                                    SoundManager.Instance.Play("Strain_MF7");
                                    SoundManager.Instance.Play("Roll_Long", 0.8f);
                                    SoundManager.Instance.Play("Strain_MF5", 0.8f);
                                    break;

                                case "FrontFlip":
                                    SoundManager.Instance.Play("Strain_MF7");
                                    SoundManager.Instance.Play("Foot_2", 0.7f);
                                    SoundManager.Instance.Play("Foot_1", 0.8f);
                                    break;
                            }

                            StartCoroutine(DoParkourAction(action));
                            gatherInput.tryToLong = false;
                            break;
                        }
                    }
                }
            }


            if (gatherInput.tryToJump && !playerController.InAction && !playerController.IsHanging && playerController.IsGrounded())
            {
                if (hitData.forwardHitFound)
                {
                    if (hitData.forwardHit.transform.CompareTag("Fence3"))
                    {
                        ChooseVaultFenceAction(hitData);
                    }

                    else if (hitData.forwardHit.transform.CompareTag("FinalAction"))
                    {
                        if (finalAction.CheckIfPossible(hitData, transform))
                        {
                            StartCoroutine(DoParkourAction(finalAction));
                            gatherInput.tryToJump = false;
                            SoundManager.Instance.Play("Foot_2");
                            SoundManager.Instance.Play("Strain_M3");
                            SoundManager.Instance.Play("Foot_1", 1f);
                        }
                    }

                    else if (hitData.forwardHit.transform.CompareTag("Fence") || hitData.forwardHit.transform.CompareTag("Fence2"))
                    {

                        if (hitData.forwardHit.transform.CompareTag("Fence"))
                        {
                            SoundManager.Instance.Play("Strain_M2");
                            SoundManager.Instance.Play("Hand", 0.2f);
                            SoundManager.Instance.Play("Foot_2", 0.8f);
                            SoundManager.Instance.Play("Foot_1", 0.9f);
                        }

                        if (hitData.forwardHit.transform.CompareTag("Fence2"))
                        {
                            SoundManager.Instance.Play("Strain_M5");
                            SoundManager.Instance.Play("Hand", 0.2f);
                            SoundManager.Instance.Play("Foot_2", 1f);
                            SoundManager.Instance.Play("Foot_1", 1.2f);
                        }

                        foreach (var action in parkourActions)
                        {
                            if (action.CheckIfPossible(hitData, transform))
                            {
                                StartCoroutine(DoParkourAction(action));
                                gatherInput.tryToJump = false;
                                break;
                            }
                        }
                    }

                    else
                    {
                        foreach (var action in parkourActions)
                        {
                            if (action.CheckIfPossible(hitData, transform))

                            {
                                switch (action.name)
                                {
                                    // Seleccionar el sonido según el nombre de la acción
                                    case "StepUp":
                                          SoundManager.Instance.Play("Strain_M1");
                                          SoundManager.Instance.Play("Foot_1", 0.8f);
                                          SoundManager.Instance.Play("Foot_2", 1.2f);
                                    break;

                                    case "JumpUp":
                                          SoundManager.Instance.Play("Strain_S8");
                                          SoundManager.Instance.Play("Foot_1", 0.6f);
                                          SoundManager.Instance.Play("Foot_2", 0.65f);
                                    break;

                                    case "ClimbUp":
                                          SoundManager.Instance.Play("Strain_M6");
                                          SoundManager.Instance.Play("ClimbUp", 0.15f);
                                          SoundManager.Instance.Play("Strain_S5", 0.5f);
                                    break;
                                }

                                StartCoroutine(DoParkourAction(action));
                                gatherInput.tryToJump = false;
                                break;
                            }
                        }
                    }
                }
            }

            if(playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound && playerController.IsGrounded())
            {

                if(playerController.LedgeData.height <= autoJumpHeightLimit && playerController.LedgeData.angle <= angleThreshold && AbleToAutoJump)
                {
                    AbleToAutoJump = false;
                    playerController.IsOnLedge = false;
                    StartCoroutine(DoParkourAction(autoJumpDownAction));
                    SoundManager.Instance.Play("Foot_2", 1.3f);
                    SoundManager.Instance.Play("Foot_1", 1.35f);
                }
                    
                else if(playerController.LedgeData.height > autoJumpHeightLimit && playerController.LedgeData.angle <= angleThreshold && gatherInput.tryToJump)
                {
                    playerController.IsOnLedge = false;
                    StartCoroutine(DoParkourAction(jumpDownAction));
                    SoundManager.Instance.Play("Strain_M8");

                    if (gatherInput.tryToJump)
                       gatherInput.tryToJump = false;
                }
            }
        }

        private void ChooseVaultFenceAction(EnvironmentScanner.ObstacleHitData hitData)
        {
            var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

            ParkourAction selectedAction = null;

            if (hitPoint.x > -0.2f && hitPoint.x < 0.2f)
            {
                selectedAction = vaultFenceActions[2];
                SoundManager.Instance.Play("Strain_M4");
                SoundManager.Instance.Play("Hand", 0.4f);
                SoundManager.Instance.Play("Foot_2", 0.8f);
                SoundManager.Instance.Play("Foot_1", 0.85f);
            }
            else
            {
                bool useAlternative1 = Random.value > 0.5f;
                selectedAction = useAlternative1 ? vaultFenceActions[0] : vaultFenceActions[1];
            }

            if (selectedAction != null && selectedAction.CheckIfPossible(hitData, transform))
            {
                if (selectedAction == vaultFenceActions[0])
                {
                    SoundManager.Instance.Play("Strain_M2");
                    SoundManager.Instance.Play("Hand", 0.2f);
                    SoundManager.Instance.Play("Foot_2", 0.8f);
                    SoundManager.Instance.Play("Foot_1", 0.9f);
                }

                if (selectedAction == vaultFenceActions[1])
                {
                    SoundManager.Instance.Play("Strain_M5");
                    SoundManager.Instance.Play("Hand", 0.2f);
                    SoundManager.Instance.Play("Foot_2", 1f);
                    SoundManager.Instance.Play("Foot_1", 1.2f);
                }

                StartCoroutine(DoParkourAction(selectedAction));
                gatherInput.tryToJump = false;
            }
        }

        private IEnumerator DoParkourAction(ParkourAction action)
        {
            playerController.SetControl(false);

            MatchTargetParams matchParams = null;
            if (action.EnableTargetMatching)
            {
                matchParams = new MatchTargetParams()
                {
                    pos = action.MatchPosition,
                    bodyPart = action.MatchBodyPart,
                    posWeight = action.MatchPosWeight,
                    startTime = action.MatchStartTime,
                    targetTime = action.MatchTargetTime,
                };
            }

            yield return playerController.DoAction(action.AnimName, matchParams, action.TargetRotation,
                action.RotateToObstacle, false, action.PostActionDelay, action.Mirror);
          
            playerController.SetControl(true);
        }

    }
}
