using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Very stupid way to make drop shadows
//- Projectors are doing weird things right now.
//- Raycasts won't work as we have no colliders
public class ForceYPosition : MonoBehaviour
{
    [SerializeField]
    private float m_Y;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, m_Y, transform.position.z);
    }
}
