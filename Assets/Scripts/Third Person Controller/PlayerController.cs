using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ParkourSystem.EnvironmentScanner;

namespace ParkourSystem
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GatherInput gatherInput;

        [SerializeField] private float moveSpeed = 5f, rotationSpeed = 500;

        [SerializeField] private float extraSpeed = 1.5f, actualSpeed, jumpPower = 8;
        private bool isJumping;
        private bool rollSoundPlayed = false; //Agregado boleano para comprobar si el sonido ya ha sido reproducido
        private bool hardLandingSoundPlayed = false; //Agregado boleano para comprobar si el sonido ya ha sido reproducido

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPositionLeft;
        [SerializeField] private Transform groundCheckPositionRight;

        [SerializeField] private float groundCheckRadious = 0.2f;
        [SerializeField] private Vector3 groundCheckOffset;
        [SerializeField] private LayerMask groundLayer;

        private CameraController cameraController;
        private CharacterController characterController;
        private ParkourController parkourController;
        private EnvironmentScanner environmentScanner;
        private Animator animator;

        private Vector3 moveInput;
        private Quaternion targetRotation;
        private float ySpeed; 

        private bool hasControl = true;
        public bool InAction { get; private set; }
        public bool IsHanging { get; set; }

        private Vector3 desiredMoveDir;
        private Vector3 moveDir;
        private Vector3 velocity;
        private Vector3 currentVelocity; //TEST
        private bool isFalling; //TEST
        public bool IsOnLedge { get; set; }
        public LedgeData LedgeData { get; set; }

        [SerializeField] private float fallThresholdOne = 0.5f, fallThresholdTwo = 1.0f;
        [SerializeField] private float fallTime;


        private void Awake()
        {
            cameraController = Camera.main.GetComponent<CameraController>();
            characterController = GetComponent<CharacterController>();
            parkourController = GetComponent<ParkourController>();
            environmentScanner = GetComponent<EnvironmentScanner>();
            animator = GetComponent<Animator>();
            Application.targetFrameRate = 60;
        }

        void Update()
        {
            Vector2 direction = gatherInput.smoothedDirection;
            moveInput = new Vector3(direction.x, 0, direction.y);

            float moveAmount = Mathf.Clamp01(Mathf.Abs(direction.x) + Mathf.Abs(direction.y));

            desiredMoveDir = cameraController.GetYRotation * moveInput;
            moveDir = desiredMoveDir;

            if (!hasControl || IsHanging)
                return;

            //velocity = Vector3.zero;
            animator.SetBool("isGrounded", IsGrounded());
            ApplyGravity();

            // Actualiza la velocidad segun si el jugador esta corriendo o no (Nueva accion de Sprint)
            if (gatherInput.tryToRun)
                actualSpeed = moveSpeed + extraSpeed;
            else
                actualSpeed = moveSpeed;


            if (gatherInput.tryToJump && IsGrounded() && !InAction && !isJumping && !IsOnLedge && !IsHanging && !environmentScanner.InFrontOfObstacle)
            {
                SoundManager.Instance.Play("Strain_M2");
                ySpeed += jumpPower;
                StartCoroutine(DoAction("NormalJump"));
                isJumping = true;
                gatherInput.tryToJump = false;
            }
           

            characterController.Move(velocity * Time.deltaTime);

            if (moveAmount > 0f && moveDir.sqrMagnitude > 0.05f)
            {
                targetRotation = Quaternion.LookRotation(moveDir);
            }
            else
            {
                velocity = Vector3.zero;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

        private void LedgeMovement()
        {
            float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDir, Vector3.up);
            float angle = Mathf.Abs(signedAngle);

            if(Vector3.Angle(desiredMoveDir, transform.forward) >= 80) 
            {
                //No permitimos el movimiento, pero si rotar hacia la cornisa
                velocity = Vector3.zero;
                return;
            }

            if (angle < 50)
            {
                velocity = Vector3.zero;
                //Comenta la siguiente linea para permitir que el player rote al llegar a una cornisa =>
                moveDir = Vector3.zero;           //Rotar previene que si el player hace "el tonto" en una esquina, nos caigamos eventualmente
                                                    //Como contra, el "feeling" de rotar sin el movimiento, puede ser peor que sin la rotacion
            }
            else if(angle < 90)
            {
                //Si el angulo esta entre 60 y 90, limitamos velocity a solo la direccion horizontal del saliente en el que estemos
                var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
                var dir = left * Mathf.Sign(signedAngle);

                velocity = velocity.magnitude * dir;
                moveDir = dir;
            }
        }

        public IEnumerator DoAction(string animName, MatchTargetParams matchParams = null, Quaternion targetRotation = new Quaternion(), 
                                    bool rotate = false, bool rotateWithStartTime = false, float postDelay = 0f, bool mirror = false)

        {
            InAction = true;
           
            animator.SetBool("mirrorAction", mirror);
            animator.CrossFadeInFixedTime(animName, 0.2f);

            yield return null;

            var animState = animator.GetNextAnimatorStateInfo(0);

            if (!animState.IsName(animName))
            {
                Debug.LogError("The Parkour animation is Wrong");
            }

            float rotateStartTime = 0;
            if (rotateWithStartTime)
            {
                rotateStartTime = (matchParams != null) ? matchParams.startTime : 0f;
            }
        

            float timer = 0f;
            while (timer < animState.length)
            {
                timer += Time.deltaTime;
                float nomalizedTime = timer / animState.length;
                if (rotate && nomalizedTime > rotateStartTime)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (matchParams != null)
                    MatchTarget(matchParams);

                if (animator.IsInTransition(0) && timer > 0.5f)
                    break;

                yield return null;
            }

            yield return new WaitForSeconds(postDelay);

            InAction = false;

            if (!parkourController.AbleToAutoJump)
                parkourController.AbleToAutoJump = true;
        }

        private void MatchTarget(MatchTargetParams mtp)
        {
            if (animator.isMatchingTarget || animator.IsInTransition(0)) return;

            animator.MatchTarget(mtp.pos, transform.rotation, mtp.bodyPart, new MatchTargetWeightMask(mtp.posWeight, 0), mtp.startTime, mtp.targetTime);
        }


        private void ApplyGravity()
        {
            if (IsGrounded() && ySpeed < 0f)
            {
                ySpeed = -1f;
                isFalling = false;

                if (isJumping) { isJumping = false; }
                velocity = desiredMoveDir * actualSpeed;

                currentVelocity = velocity; //TEST

                IsOnLedge = environmentScanner.ObstacleLedgeCheck(desiredMoveDir.normalized, out LedgeData ledgeData);

                if (IsOnLedge)
                {
                    LedgeData = ledgeData;
                    LedgeMovement();
                }


                //Determina el aterrizaje al tocar el suelo
                if (fallTime > 0)
                {
                    ChooseLandingAnim(); //Evalua la animacion y el sonido al aterrizar
                    fallTime = 0f; //Reinicia el tiempo de caida
                }

                float maxSpeed = moveSpeed + extraSpeed;
                animator.SetFloat("moveAmount", velocity.magnitude / maxSpeed, 0.1f, Time.deltaTime);
            }

            else
            {
                // Aplica una pequeña componente horizontal mientras está cayendo
                if (isFalling)
                {
                    velocity = transform.forward * moveSpeed / 4;
                }
             

                if (isJumping)
                {
                    //Mantener la velocidad horizontal mientras esta saltando
                    velocity.x = currentVelocity.x;
                    velocity.z = currentVelocity.z;
                }

                //Verificar si esta cayendo (no en el suelo y velocidad negativa)
                if (!isJumping && ySpeed < 0f)
                {
                    if (!isFalling)
                    {
                        isFalling = true;
                        fallTime = 0f;

                        // Reinicia los estados de sonido para una nueva caída
                        rollSoundPlayed = false;
                        hardLandingSoundPlayed = false;
                    }

                    fallTime += Time.deltaTime; //Acumula el tiempo de caida
                }

                //Aplicar gravedad de forma normal
                ySpeed += Physics.gravity.y * Time.deltaTime;
            }
         
            velocity.y = ySpeed;
        }

        private void ChooseLandingAnim()
        {
            if (fallTime < fallThresholdOne)
            {
                // Aterrizaje suave: No requiere animación o sonido especial
                animator.SetBool("HardLanding", false);
                animator.SetBool("RollLanding", false);

                // Reinicia sonidos
                rollSoundPlayed = false;
                hardLandingSoundPlayed = false;
            }
            else if (fallTime >= fallThresholdOne && fallTime < fallThresholdTwo)
            {
                // Aterrizaje con rodillo
                animator.SetBool("RollLanding", true);
                animator.SetBool("HardLanding", false);

                if (!rollSoundPlayed)
                {
                    SoundManager.Instance.Play("Roll", 0.1f);
                    SoundManager.Instance.Play("Strain_MF3");
                    rollSoundPlayed = true;
                }
            }
            else if (fallTime >= fallThresholdTwo)
            {
                // Aterrizaje fuerte
                animator.SetBool("HardLanding", true);
                animator.SetBool("RollLanding", false);

                if (!hardLandingSoundPlayed)
                {
                    SoundManager.Instance.Play("HardLanding");
                    SoundManager.Instance.Play("Strain_Hard");
                    hardLandingSoundPlayed = true;
                }
            }
        }

        public void SetControl(bool hasControl)
        {
            this.hasControl = hasControl;
            characterController.enabled = hasControl;

            if (!hasControl)
            {
                animator.SetFloat("moveAmount", 0);
                targetRotation = transform.rotation;
            }
        }

        public void EnableCharacterController(bool enabled)
        {
            characterController.enabled = enabled;
        }

        public void ResetTargetRotation()
        {
            targetRotation = transform.rotation;
        }

        public bool HasControl
        {
            get => hasControl;
            set => hasControl = value;
        }

        public bool IsGrounded()
        {
            bool isGrounded;
            
            bool leftShoe = Physics.CheckSphere(groundCheckPositionLeft.transform.TransformPoint(groundCheckOffset), groundCheckRadious, groundLayer);
            bool rightShoe = Physics.CheckSphere(groundCheckPositionRight.transform.TransformPoint(groundCheckOffset), groundCheckRadious, groundLayer);

            if (leftShoe || rightShoe)
                isGrounded = true;
            else
                isGrounded = false;

            return isGrounded;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(groundCheckPositionLeft.transform.TransformPoint(groundCheckOffset), groundCheckRadious);
            Gizmos.DrawSphere(groundCheckPositionRight.transform.TransformPoint(groundCheckOffset), groundCheckRadious);
        }

        public float RotationSpeed => rotationSpeed;

    }

    public class MatchTargetParams
    {
        public Vector3 pos;
        public AvatarTarget bodyPart;
        public Vector3 posWeight;
        public float startTime;
        public float targetTime;
    }

  
}
