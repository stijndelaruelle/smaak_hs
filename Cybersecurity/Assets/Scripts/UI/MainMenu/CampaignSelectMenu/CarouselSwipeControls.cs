using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarouselSwipeControls : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private CarouselUI m_Carousel;

    //Touch controls
    private Vector2 m_StartTouchPosition;
    private float m_MinDragDistance;
    private float m_MinOffsetDistance;

    private void Start()
    {
        m_MinDragDistance = (Screen.height * 5) / 100; //Only register moves if the player has swiped at least 5% of the screen height
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        m_StartTouchPosition = pointerEventData.pressPosition;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Vector2 lastTouchPosition = pointerEventData.position;

        Vector2 diff = lastTouchPosition - m_StartTouchPosition;

        if (Mathf.Abs(diff.x) >= m_MinDragDistance)
        {
            //Debug.Log("Start drag: " + m_StartTouchPosition + " end drag: " + lastTouchPosition + " min drag distance on both axis: " + m_MinDragDistance);

            //Left
            if (diff.x <= 0)
            {
                m_Carousel.NextPage();
            }

            //Right
            else if (diff.x > 0)
            {
                m_Carousel.PreviousPage();
            }
        }
        else
        {
            //Debug.Log("Lower than min drag distance, swipe ignored");
        }
    }
}
