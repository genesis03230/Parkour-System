using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    [CreateAssetMenu(menuName = "Parkour System/Custom Actions/New Vault action")]

    public class VaultAction : ParkourAction
    {
        public override bool CheckIfPossible(EnvironmentScanner.ObstacleHitData hitData, Transform player)
        {
            if (!base.CheckIfPossible(hitData, player))
                return false;

            var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

            if(hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
            {
                //Mirror animation
                Mirror = true;
                matchBodyPart = AvatarTarget.RightHand;
            }
            else
            {
                //Don't Mirror
                Mirror = false;
                matchBodyPart = AvatarTarget.LeftHand;
            }
                return true;
        }
    }
}
