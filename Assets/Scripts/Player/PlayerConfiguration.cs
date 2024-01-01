using UnityEngine;
using UnityEngine.InputSystem;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class PlayerConfiguration
    {
        public PlayerConfiguration(PlayerInput pi)
        {
            playerIndex = pi.playerIndex;
            input = pi;
            scoreTotal=0;
        }

        public PlayerInput input { get; private set; }
        public int playerIndex { get; private set; }
        public bool isReady { get; set; }
        public Material playerMaterial {get; set;}
        //TODO rename to materialIndex after splitting 
        //the corresponding property in GameManager to another class
        //AND maybe use enum instead of int
        public int cursorIndex;
        public int scoreTotal;
    }
}
