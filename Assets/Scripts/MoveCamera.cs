using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        transform.position = player.transform.position;
    }
}
