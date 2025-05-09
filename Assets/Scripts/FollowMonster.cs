using UnityEngine;

public class FollowMonster : MonoBehaviour
{
    public Transform monster;

    void Update()
    {
        if (monster != null && monster.gameObject.activeInHierarchy)
        {
            Vector3 targetPos = monster.position;
            targetPos.y += 1f; // raise by 1 unit
            transform.position = targetPos;
        }
    }

}
