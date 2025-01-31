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
            // Comprueba si el obstáculo tiene la etiqueta válida
            if (!string.IsNullOrEmpty(ObstacleTag) && hitData.slideHit.transform.tag != ObstacleTag)
            {
                return false;
            }
            // Opcional: Añade cualquier lógica adicional específica para deslizarse.
            return true;
        }
    }
}
