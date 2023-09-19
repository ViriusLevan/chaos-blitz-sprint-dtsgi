using System.Collections;
using System.Collections.Generic;
using LevelUpStudio.ChaosBlitzSprint.Player;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class DeathAnimation : MonoBehaviour
    {
        [SerializeField]private PlayerController playerController;

        public void Died()
        {
            playerController.DisableMesh();
        }
    }
}
