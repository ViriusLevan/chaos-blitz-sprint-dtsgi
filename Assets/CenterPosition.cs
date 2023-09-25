using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelUpStudio.ChaosBlitzSprint
{
    public class CenterPosition : MonoBehaviour
    {
        private RectTransform cursorRectTransform;

        private void Start()
        {
            // Ambil RectTransform dari GameObject yang mewakili cursor
            cursorRectTransform = GetComponent<RectTransform>();

            // Setel posisi cursor ke (0, 0, 0)
            cursorRectTransform.anchoredPosition = Vector3.zero;
        }
    }
}
