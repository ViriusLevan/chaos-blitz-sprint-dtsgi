using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class StartAnimation : MonoBehaviour
    {
        public void FadeCompleted()
        {
            GameManager.Instance?.AnimationFinished();
        }
    }
}
