using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boxer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log(other.name + "HELLO TELEPORRTED");
        BlastladCharacterController character = other.GetComponent<BlastladCharacterController>();
        if (character)
        {
        }
    }
}
