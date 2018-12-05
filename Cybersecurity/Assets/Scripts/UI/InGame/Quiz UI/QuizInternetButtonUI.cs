using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizInternetButtonUI : MonoBehaviour
{
    [SerializeField]
    private string m_URL;

    public void Click()
    {
        Application.OpenURL(m_URL);
    }
}
