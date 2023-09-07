using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class yRotator : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(new Vector3(0,50*Time.deltaTime,0));
        }
    }
}
