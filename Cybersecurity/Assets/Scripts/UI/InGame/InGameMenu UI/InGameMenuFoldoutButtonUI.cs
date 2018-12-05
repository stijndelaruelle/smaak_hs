using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuFoldoutButtonUI : MonoBehaviour
{
    [SerializeField]
    private Sprite m_OpenIcon;

    [SerializeField]
    private Sprite m_ClosedIcon;

    [Header("Required References")]
    [SerializeField]
    private Image m_IconImage;

    [SerializeField]
    private RectTransform m_FoldoutPanel;

    private bool m_IsOpen = false;

    private void Start()
    {
        m_FoldoutPanel.anchoredPosition = new Vector2(-m_FoldoutPanel.rect.width, 0);
    }

    public void Toggle()
    {
        m_IsOpen = !m_IsOpen;

        //Switch our icon & tween the panel
        if (m_IsOpen)
        {
            m_IconImage.sprite = m_OpenIcon;
            m_FoldoutPanel.DOAnchorPosX(0, 0.5f, false).SetEase(Ease.OutElastic, 1.0f, 0.5f);
        }
        else
        {
            m_IconImage.sprite = m_ClosedIcon;
            m_FoldoutPanel.DOAnchorPosX(-m_FoldoutPanel.rect.width, 0.5f, false).SetEase(Ease.OutQuad);
        }
    }
}
