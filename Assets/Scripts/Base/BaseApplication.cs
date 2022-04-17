using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseElement : MonoBehaviour
{
    public BaseApplication app
    {
        get
        {
            return GameObject.FindObjectOfType<BaseApplication>();
        }
    }
    public interface IBaseController
    {
        void Init();
        void Set();
        void AdvanceTime(float dt_sec);
        void Dispose();
        void SetActive(bool flag);
    }
}
public abstract class BaseApplication : MonoBehaviour
{
    public void SetActive(bool flag)
    {
        this.gameObject.SetActive(flag);
    }
    public abstract void Init();
    public abstract void AdvanceTime(float dt_sec);
    public abstract void Set();
    public abstract void Dispose();
}
