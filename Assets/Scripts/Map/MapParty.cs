using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapParty : MonoBehaviour
{
    public float moveSpeed;
    private bool isMovingToEncounter;

    public void MoveToEncounter(Encounter encounter, UnityAction<Encounter> onArriveCallBack)
    {
        if (isMovingToEncounter)
        {
            return;
        }

        isMovingToEncounter = true;
        StartCoroutine(Move());

        IEnumerator Move()
        {
            transform.LookAt(encounter.transform.position);

            while (transform.position != encounter.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, encounter.transform.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            isMovingToEncounter = false;
            onArriveCallBack?.Invoke(encounter);
        }
    }
}
