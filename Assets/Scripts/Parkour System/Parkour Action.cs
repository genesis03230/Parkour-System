using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ParkourSystem.EnvironmentScanner;

namespace ParkourSystem
{
    [CreateAssetMenu(menuName = "Parkour System/New Parkour Action")]

    public class ParkourAction : ScriptableObject
    {
        [Header("Action Info")]
        [SerializeField] private string animName;
        [SerializeField] private string obstacleTag;
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;
        [SerializeField] private bool rotateToObstacle;
        [SerializeField] private float postActionDelay;
     

        [Header("Target Matching")]
        [SerializeField] private bool enableTargetMatching = true;
        [SerializeField] private bool isLongAction = false;
        [SerializeField] private protected AvatarTarget matchBodyPart;
        [SerializeField] private float matchStartTime;
        [SerializeField] private float matchTargetTime;
        [SerializeField] private Vector3 matchPosWeight = new Vector3(0, 1, 0);
        [SerializeField] private Vector3 extraOffset;

        public Quaternion TargetRotation { get; set; }
        public Vector3 MatchPosition { get; set; }
        public bool Mirror {  get; set; }
        public string ObstacleTag => obstacleTag; //Se agrego para prueba de Slide nueva propiedad

        public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
        {
            //Check Tag
            if (!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
                return false;

            if (hitData.forwardHit.transform.tag == "NotForActions")
                return false;

            //Height Tag
            float height = hitData.heightHit.point.y - player.position.y;

            if (height < minHeight || height > maxHeight)
                return false;

            if (rotateToObstacle)
                TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
                

            if (enableTargetMatching)
            {
                if (!isLongAction)
                     MatchPosition = hitData.heightHit.point + extraOffset;
                else
                     MatchPosition = hitData.secondHeightHit.point + extraOffset;
            }

            return true;
        }

        public string AnimName => animName;
        public bool RotateToObstacle => rotateToObstacle;
        public float PostActionDelay => postActionDelay;
        public Vector3 MatchPosWeight => matchPosWeight;
        public bool EnableTargetMatching => enableTargetMatching;
        public AvatarTarget MatchBodyPart => matchBodyPart;
        public float MatchStartTime => matchStartTime;
        public float MatchTargetTime => matchTargetTime;
    }
}
