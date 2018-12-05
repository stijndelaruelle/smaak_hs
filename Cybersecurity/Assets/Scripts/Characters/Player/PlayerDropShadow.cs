using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Doesn't actually work for multiplayer yet. But then again, what does?
public class PlayerDropShadow : MonoBehaviour
{
    [SerializeField]
    private Player m_Player;

    [SerializeField]
    [Tooltip("Don't actually parent this! We will manage this ourself.")]
    private Transform m_ParentTransform;
    private Transform m_OriginalParent;

    //Original transform
    private Vector3 m_OriginalPosition;
    private Quaternion m_OriginalRotation;
    private Vector3 m_OriginalScale;

    private Coroutine m_ResetRoutine;

    private void Start()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent     += OnPlayerReset;
            LevelDirector.Instance.LevelResetEvent     += OnPlayerReset;
            LevelDirector.Instance.LevelEndDefeatEvent += OnPlayerDied;
        }

        if (m_Player != null)
        {
            m_Player.IntoAnimationEndEvent += OnIntroAnimationEnd;
        }

        m_OriginalParent = transform.parent;

        m_OriginalPosition = transform.position.Copy();
        m_OriginalRotation = transform.localRotation.Copy();
        m_OriginalScale = transform.localScale.Copy();
    }

    private void OnDestroy()
    {
        if (LevelDirector.Instance != null)
        {
            LevelDirector.Instance.LevelStartEvent     -= OnPlayerReset;
            LevelDirector.Instance.LevelResetEvent     -= OnPlayerReset;
            LevelDirector.Instance.LevelEndDefeatEvent -= OnPlayerDied;
        }

        if (m_Player != null)
        {
            m_Player.IntoAnimationEndEvent -= OnIntroAnimationEnd;
        }
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, m_OriginalPosition.y, transform.position.z);
    }

    private void OnPlayerReset()
    {

    }

    private void OnIntroAnimationEnd()
    {
        if (m_ResetRoutine != null)
            StopCoroutine(m_ResetRoutine);

        m_ResetRoutine = StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        transform.SetParent(m_OriginalParent);

        yield return new WaitForEndOfFrame();

        transform.position = m_OriginalPosition;
        transform.localRotation = m_OriginalRotation;
        transform.localScale = m_OriginalScale;

        transform.SetParent(m_ParentTransform);

        m_ResetRoutine = null;
        yield return null;
    }

    private void OnPlayerDied()
    {
        transform.SetParent(m_OriginalParent);
    }
}
