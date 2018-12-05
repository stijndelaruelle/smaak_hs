using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviour : ResetableObject
{
    public abstract void Initialize(Character character);

    public abstract void OnEnter();
    public abstract void OnLeave();

    public abstract void FrameUpdate();
    public abstract void LevelUpdate();

    public abstract bool HasPath();
    public abstract Node GetNextNode();

    public abstract void OnMoveEnd();
}
