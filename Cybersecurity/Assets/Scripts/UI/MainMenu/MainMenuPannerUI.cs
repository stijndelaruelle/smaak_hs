using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MainMenuPannerUI : MonoBehaviour
{
    [SerializeField]
    private CanvasScaler m_CanvasScaler;
    private RectTransform m_RectTransform;

    private bool m_IsUp = true;
    public bool IsUp
    {
        get { return m_IsUp; }
    }

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (LevelManager.Instance == null)
            return;

        //Skip the main menu, go straight to the chapter panel
        if (LevelManager.Instance.CurrentLevel != null)
        {
            m_RectTransform.anchoredPosition = new Vector2(m_RectTransform.anchoredPosition.x, m_CanvasScaler.referenceResolution.y);
            m_IsUp = false;
        }
    }

    public void PanDown()
    {
        m_RectTransform.DOAnchorPosY(m_CanvasScaler.referenceResolution.y, 0.5f);
        m_IsUp = false;
    }

    public void PanUp()
    {
        m_RectTransform.DOAnchorPosY(0, 0.5f);
        m_IsUp = true;
    }
}
