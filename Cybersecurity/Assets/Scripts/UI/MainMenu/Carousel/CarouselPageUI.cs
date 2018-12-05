using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CarouselPageUI : MonoBehaviour
{
    private RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public virtual void Initialize(ScriptableObject data)
    {
    }

    public float GetWidth()
    {
        return m_RectTransform.rect.width;
    }

    public void SetPositionX(float posX)
    {
        m_RectTransform.anchoredPosition = new Vector2(posX, m_RectTransform.anchoredPosition.y);
    }
}
