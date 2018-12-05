using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintSystem : ResetableObject
{
    [Serializable]
    public class Hint
    {
        public enum Side
        {
            Top,
            Bottom,
            Left,
            Right
        }

        [SerializeField]
        private GameObject m_HintableObject;
        private IHintable m_ActualHintableObject;
        public IHintable HintableObject
        {
            get { return m_ActualHintableObject; }
        }

        [SerializeField]
        private Side m_SideOfObject;
        public Side SideOfObject
        {
            get { return m_SideOfObject; }
        }

        [SerializeField]
        private Vector3 m_Offset;
        public Vector3 Offset
        {
            get { return m_Offset; }
        }

        public void Initialize()
        {
            m_ActualHintableObject = m_HintableObject.GetComponent<IHintable>();
            if (m_ActualHintableObject == null)
            {
                Debug.Log("Hint error: " + m_HintableObject.name + " isn't an actually HintableObject!");
            }
        }

        public void SetPosition(Transform pointer)
        {
            pointer.position = m_HintableObject.transform.position + m_Offset;
        }
    }

    [SerializeField]
    private Transform[] m_HintPointers = new Transform[4]; //Left, Right, Top, Bottom

    [SerializeField]
    private FactionTypeDefinition m_AllowedFaction;

    [SerializeField]
    private List<Hint> m_Hints;
    private bool m_IsActive = false;

    private int m_CurrentStepID = 0;


    protected override void Start()
    {
        base.Start();

        //Initialize
        foreach(Hint hint in m_Hints)
            hint.Initialize();

        //Hide all the pointers!
        for (int i = 0; i < m_HintPointers.Length; ++i)
        {
            if (m_HintPointers[i] != null)
                m_HintPointers[i].gameObject.SetActive(false);
        }
            
        OnReset();
    }

    public void ActivateHints(bool state)
    {
        //Only activate when we're inactive
        if (m_IsActive == false && state == true)
        {
            m_IsActive = true;
            ShowHint();
        }

        //Hide everything
        if (state == false)
        {
            m_IsActive = false;

            for (int i = 0; i < m_HintPointers.Length; ++i)
            {
                if (m_HintPointers[i] != null)
                    m_HintPointers[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShowHint()
    {
        if (m_CurrentStepID < 0 || m_CurrentStepID >= m_Hints.Count)
        {
            ActivateHints(false);
            return;
        }

        for (int i = 0; i < m_HintPointers.Length; ++i)
        {
            if (m_HintPointers[i] != null)
                m_HintPointers[i].gameObject.SetActive(i == (int)m_Hints[m_CurrentStepID].SideOfObject);
        }

        m_Hints[m_CurrentStepID].SetPosition(m_HintPointers[(int)m_Hints[m_CurrentStepID].SideOfObject].transform); //Shortcut
        m_Hints[m_CurrentStepID].HintableObject.HintUsedEvent += OnHintUsed;
    }

    private void OnHintUsed(IHintable hintableObject, Character character)
    {
        if (m_CurrentStepID < 0 || m_CurrentStepID >= m_Hints.Count)
            return;

        if (character.Faction != m_AllowedFaction)
            return;

        //Shouldn't happen
        if (hintableObject != m_Hints[m_CurrentStepID].HintableObject)
        {
            hintableObject.HintUsedEvent -= OnHintUsed;
            return;
        }

        m_Hints[m_CurrentStepID].HintableObject.HintUsedEvent -= OnHintUsed;

        m_CurrentStepID += 1;
        ShowHint();
    }

    //Used for editor purposes
    public void ToggleHintPointer(Hint.Side side)
    {
        GameObject obj = m_HintPointers[(int)side].gameObject;
        obj.SetActive(!obj.activeSelf);
    }

    public void MoveHintPointerEditor(Hint.Side side, Transform parent, Vector3 offset)
    {
        m_HintPointers[(int)side].position = parent.position + offset;
    }

    protected override void OnReset()
    {
        if (m_CurrentStepID >= 0 && m_CurrentStepID < m_Hints.Count)
            m_Hints[m_CurrentStepID].HintableObject.HintUsedEvent -= OnHintUsed;

        m_CurrentStepID = 0;

        for (int i = 0; i < m_HintPointers.Length; ++i)
        {
            if (m_HintPointers[i] != null)
            {
                m_HintPointers[i].parent = transform;
                m_HintPointers[i].localPosition = Vector3.zero;

            }
        }

        ActivateHints(false);
    }
}
