using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterCarouselLevelButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Instead of changing the backgound & icon we just swap buttons entirly.
    //This looks "ineficcient" but otherwise we have to manually correct all the icon sizes etc. (they aren't all the same size)
    //This basically causes more clutter in the editor and less in code
    [SerializeField]
    private GameObject m_PlayButton;

    [SerializeField]
    private GameObject m_CompletedButton;

    [SerializeField]
    private GameObject m_LockedButton;

    private LevelDataDefinition m_LevelData;

    //Used for debug unlocking
    private bool m_IsHovering;

    public void Initialize(LevelDataDefinition level, bool isUnlocked)
    {
        m_LevelData = level;

        //Deactivate all of them (even happens when it's an invalid level)
        m_PlayButton.SetActive(false);
        m_CompletedButton.SetActive(false);
        m_LockedButton.SetActive(false);

        //Change our appearance depending on if the level is completed/locked/ready to be played
        if (level == null)
            return;

        if (m_LevelData.HasLevelBeenCompleted())
            m_CompletedButton.SetActive(true);

        else if (isUnlocked)
            m_PlayButton.SetActive(true);

        else
            m_LockedButton.SetActive(true);
    }

    public void Click()
    {
        if (LevelManager.Instance != null)
            LevelManager.Instance.LoadLevel(m_LevelData);
    }

    private void Update()
    {
        if (SaveGameManager.GetBool(SaveGameManager.SAVE_CHEATS) == true)
        {
            if (m_IsHovering && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C))
            {
                m_LevelData.CompleteLevel();
            }
        }
    }

    //IPointerEnterHandler
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_IsHovering = true;
    }

    //IPointerExitHandler
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        m_IsHovering = false;
    }
}
