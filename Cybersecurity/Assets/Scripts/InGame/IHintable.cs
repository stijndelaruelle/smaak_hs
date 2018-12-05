using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HintDelegate(IHintable hintableObject, Character character);

public interface IHintable
{
    //Event that fires once the hint ha been used (so the next hint can continue)
    event HintDelegate HintUsedEvent;
}
