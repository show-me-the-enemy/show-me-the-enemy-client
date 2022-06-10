using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBarView : MonoBehaviour
{
    public RectTransform fillRect;
    public Text timeText;
    public float value = 1f;
    
    public void setValue(float v, float t=-1)
    {
        value = v;
        fillRect.anchorMax = new Vector2(v,1);
        if (t < 0) return;
        timeText.text = string.Format("{0:F1}", t);
    }
}
