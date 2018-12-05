using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Could be an interface and have a "ResetListener" instance added to every GameObject to call the "Reset" function instead of every object listening for themselves.
public abstract class ResetableObject : MonoBehaviour
{
    protected virtual void Start()
    {
        LevelDirector levelManager = LevelDirector.Instance;

        if (levelManager != null)
            levelManager.LevelResetEvent += OnReset;
    }

    protected virtual void OnDestroy()
    {
        LevelDirector levelManager = LevelDirector.Instance;

        if (levelManager != null)
            levelManager.LevelResetEvent -= OnReset;
    }

    protected abstract void OnReset();
}
