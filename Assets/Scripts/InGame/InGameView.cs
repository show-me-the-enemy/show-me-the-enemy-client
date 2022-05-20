using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameView : MonoBehaviour
{
    [SerializeField]
    private GameObject _loadingPopup;
    public GameObject LoadingPopup
    {
        get
        {
            return _loadingPopup;
        }
    }
}
