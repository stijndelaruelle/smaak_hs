using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Character
{
    [Header("Guard")]
    [Space(5)]
    [SerializeField]
    private PatrolBehaviour m_PatrolBehaviour;

    protected override void Start()
    {
        base.Start();

        //Assign all behaviors
        if (m_PatrolBehaviour != null)
            m_PatrolBehaviour.Initialize(this);
        else
            Debug.LogWarning("Guard doesn't have a patrol behaviour!");

        LevelDirector.Instance.LevelUpdateEvent += OnLevelUpdate;
        LevelDirector.Instance.LevelPlayerUpdateEvent += OnLevelPlayerUpdate;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        LevelDirector levelManager = LevelDirector.Instance;

        if (levelManager != null)
        {
            levelManager.LevelUpdateEvent -= OnLevelUpdate;
            levelManager.LevelPlayerUpdateEvent -= OnLevelPlayerUpdate;
        }
    }

    private void Update()
    {
        m_PatrolBehaviour.FrameUpdate();
    }

    private void OnLevelUpdate()
    {
        //Check if someone is now in range
        KillCharactersInRange();
    }

    private void OnLevelPlayerUpdate()
    {
        OnLevelUpdate();

        //The game could have ended by now
        if (LevelDirector.Instance.HasGameEnded() == true)
            return;

        m_PatrolBehaviour.LevelUpdate();
        KillCharactersInRange();
    }

    private bool KillCharactersInRange()
    {
        if (LevelDirector.Instance.HasGameEnded() == true)
            return false;

        List<Character> characterInRange = new List<Character>();

        //Check if there is someone on our tile, if so add them to the list
        characterInRange.AddRange(m_CurrentNode.Characters);

        //Check if there is someone right in front of us, if so add them to the list
        Node nextNode = m_PatrolBehaviour.GetNextNode();

        if (nextNode != null)
            characterInRange.AddRange(nextNode.Characters);

        //If so, "Kill 'Em All"! 
        bool success = false;
        foreach (Character character in characterInRange)
        {
            //Kill everyone but our allies
            if (IsEnemy(character))
            {
                //Analytics
                if (LevelDirector.Instance != null)
                    LevelDirector.Instance.CallLevelFailAnalyticsEvent(true, false, false, false);

                character.Die();
                success = true;
            }
        }

        return success;
    }
}
