using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.PlaceableBehaviour
{
    public class ObstacleShooterEvent : MonoBehaviour
    {
        [SerializeField]private ShootingObstacle shooterObstacle;

        public void ArrowFired(){
            shooterObstacle.Shoot();
        }
    }
}
