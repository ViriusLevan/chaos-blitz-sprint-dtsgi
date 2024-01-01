using UnityEngine;
using DG.Tweening;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class SimpleRotator : MonoBehaviour
    {
        public enum RotationAxis{x,y,z}
        public RotationAxis axis;
        public float timeToFullRotation=10f;
        public bool isLocal=false;

        void Start()
        {
            Vector3 rotation = new Vector3(0,0,0);
            if(axis==RotationAxis.x)
                rotation.x = 360;
            else if(axis==RotationAxis.y)
                rotation.y = 360;
            else
                rotation.z = 360;

            if (isLocal)
                transform.DOLocalRotate(rotation,timeToFullRotation,RotateMode.FastBeyond360)
                            .SetLoops(-1, LoopType.Incremental)
                            .SetEase(Ease.Linear);
            else
                transform.DORotate(rotation,timeToFullRotation,RotateMode.FastBeyond360)
                            .SetLoops(-1, LoopType.Incremental)
                            .SetEase(Ease.Linear);
        }

        void Update()
        {
            
        }

        private void OnDestroy() {
            DOTween.Kill(transform);
        }
    }
}