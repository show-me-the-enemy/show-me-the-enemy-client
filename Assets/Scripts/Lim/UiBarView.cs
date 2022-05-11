using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBarView : MonoBehaviour
{
    public RectTransform fillRect;
    public float value = 1f;
    
    public void setValue(float v)
    {
        value = v;
        fillRect.anchorMax = new Vector2(v,1);
    }
}
