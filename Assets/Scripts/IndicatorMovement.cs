using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class IndicatorMovement : MonoBehaviour
    {
        public enum MovementType {xRotation, yRotation, zRotation, forwardBack, upDown}
        [SerializeField]private MovementType movement;
        void Start()
        {
            switch(movement)
            {
                case MovementType.xRotation:
                    break;
                case MovementType.yRotation:
                    break;
                case MovementType.zRotation:
                    break;
                case MovementType.forwardBack:
                    break;
                case MovementType.upDown:
                    break;
            }
        }


        void Update()
        {
            
        }

        void OnDisable()
        {
            //TODO add DoTween.Kill here
            DOTween.Kill(transform, false);
        }
    }
}
