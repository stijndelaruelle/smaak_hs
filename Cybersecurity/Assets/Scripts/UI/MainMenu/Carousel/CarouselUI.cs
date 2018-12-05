using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class CarouselUI : MonoBehaviour
{
    public delegate void CarouselDelegate();

    [SerializeField]
    protected List<CarouselPageUI> m_CarouselPages; //Should be a linked list

    [Header("Used to limit progression")]
    [SerializeField]
    protected GameObject m_PreviousButton;

    [SerializeField]
    protected GameObject m_NextButton;

    private float m_PageWidth;

    protected int m_CurrentPageID = 0;
    protected int m_MaxPageID = 0;

    private RectTransform m_RectTransform;
    private bool m_IsMoving = false;

    public event CarouselDelegate StartMovingPageEvent;
    public event CarouselDelegate StopMovingPageEvent;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        if (m_CarouselPages == null)
            return;

        if (m_CarouselPages.Count <= 2)
        {
            Debug.LogWarning("A carousel needs at least 2 pages to function!");
            return;
        }

        m_PageWidth = m_CarouselPages[0].GetWidth();
    }


    public virtual void NextPage()
    {
        if (m_IsMoving)
            return;

        if (m_NextButton.activeSelf == false)
            return;

        m_CurrentPageID += 1;
        m_CurrentPageID %= m_MaxPageID; //Loop

        //Move everything to the left
        m_RectTransform.DOAnchorPosX(-m_PageWidth, 0.5f).OnComplete(NextPageMoveComplete);

        //Hide the buttons so we can't spam them
        m_IsMoving = true;
        m_PreviousButton.SetActive(false);
        m_NextButton.SetActive(false);

        //Let the world know
        if (StartMovingPageEvent != null)
            StartMovingPageEvent();
    }

    protected virtual void NextPageMoveComplete()
    {
        //The left page fell out of view, put it on the right side
        CarouselPageUI movedCarouselPage = m_CarouselPages[0];
        m_CarouselPages.RemoveAt(0);
        m_CarouselPages.Add(movedCarouselPage);

        //Put it at the bottom of the hierarcy
        movedCarouselPage.gameObject.transform.SetAsLastSibling();

        //Resnap all positions to the center
        for (int i = 0; i < m_CarouselPages.Count; ++i)
        {
            m_CarouselPages[i].SetPositionX(m_PageWidth * i);
        }

        m_RectTransform.anchoredPosition = new Vector2(0, m_RectTransform.anchoredPosition.y);

        //Show the buttons so we can move again
        m_IsMoving = false;
        HandleProgressionButtons();

        //Let the world know
        if (StopMovingPageEvent != null)
            StopMovingPageEvent();
    }

    public virtual void PreviousPage()
    {
        if (m_IsMoving)
            return;

        if (m_PreviousButton.activeSelf == false)
            return;

        m_CurrentPageID -= 1;

        if (m_CurrentPageID < 0)
            m_CurrentPageID = m_MaxPageID - 1; //Loop

        //We have no page on the left side, so let's move the rightmost there
        CarouselPageUI movedCarouselPage = m_CarouselPages[m_CarouselPages.Count - 1];
        m_CarouselPages.RemoveAt(m_CarouselPages.Count - 1);
        m_CarouselPages.Insert(0, movedCarouselPage);

        movedCarouselPage.SetPositionX(-m_PageWidth);

        //Actually move everything to the right
        m_RectTransform.DOAnchorPosX(m_PageWidth, 0.5f).OnComplete(PreviousPageMoveComplete);

        //Put it at the top of the hierarcy
        movedCarouselPage.gameObject.transform.SetAsFirstSibling();

        //Hide the buttons so we can't spam them
        m_IsMoving = true;
        m_PreviousButton.SetActive(false);
        m_NextButton.SetActive(false);

        if (StartMovingPageEvent != null)
            StartMovingPageEvent();
    }

    protected virtual void PreviousPageMoveComplete()
    {
        //Resnap all positions to the center
        for (int i = 0; i < m_CarouselPages.Count; ++i)
        {
            m_CarouselPages[i].SetPositionX(m_PageWidth * i);
        }

        m_RectTransform.anchoredPosition = new Vector2(0, m_RectTransform.anchoredPosition.y);

        //Show the buttons so we can move again
        m_IsMoving = false;
        HandleProgressionButtons();

        //Let the world know
        if (StopMovingPageEvent != null)
            StopMovingPageEvent();
    }

    protected virtual void HandleProgressionButtons()
    {
        m_PreviousButton.SetActive(!m_IsMoving);
        m_NextButton.SetActive(!m_IsMoving);
    }
}
