using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUiElementYIfNotch : MonoBehaviour
{
    private Rect safeArea;
    private RectTransform rectTransform;

    public float Yoffset;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        safeArea = Screen.safeArea;

        if (safeArea.yMin != 0)
        {
            rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + Yoffset);
        }

    }
}
