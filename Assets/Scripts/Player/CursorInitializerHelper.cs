using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint.Player
{
    public class CursorInitializerHelper : MonoBehaviour
    {
        [SerializeField]
        private RectTransform[] cursorTransforms;
        [SerializeField]
        private Canvas canvasReference;
        [SerializeField]
        private RectTransform canvasRectTransform;

        public RectTransform[] GetCursorTransforms() {return cursorTransforms;}
        public Canvas GetCanvas() {return canvasReference;}
        public RectTransform GetCanvasRectTransform(){return canvasRectTransform;}

        private void Start()
        {
            cursorTransforms[0].anchoredPosition = new Vector2(-200, -200);
        }
    }
}
