using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParkourSystem
{
    [CreateAssetMenu(menuName = "Parkour System/Custom Actions/New Slide Action")]
    public class SlideAction : ParkourAction
    {
        public override bool CheckIfPossible(EnvironmentScanner.ObstacleHitData hitData, Transform player)
        {
            // Comprueba si el obst�culo tiene la etiqueta v�lida
            if (!string.IsNullOrEmpty(ObstacleTag) && hitData.slideHit.transform.tag != ObstacleTag)
            {
                return false;
            }
            // Opcional: A�ade cualquier l�gica adicional espec�fica para deslizarse.
            return true;
        }
    }
}
