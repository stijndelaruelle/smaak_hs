using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizQuestionPipUI : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void SetColor(Color color)
    {
        m_Image.color = color;
    }

    public void SetSprite(Sprite sprite)
    {
        m_Image.sprite = sprite;
    }
}