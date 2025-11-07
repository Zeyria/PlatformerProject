using UnityEngine;
using System.Collections;

public class LookAtTarget : MonoBehaviour
{
    public bool onlyZRot = true;
    public GameObject player;
    public AIData aiData;
    public EnemyAI enemyAI;
    public LayerMask sightBlockers;
    bool rotating;
    Vector3 vectorToTarget;
    private void Start()
    {
        rotating = false;
    }
    void Update()
    {
        if (aiData.currentTarget != null)
        {
            vectorToTarget = aiData.currentTarget.position - transform.position;
        }
        if (Vector3.Distance(this.transform.position, player.transform.position) < enemyAI.sightRadius)
        {
            if (!Physics2D.Raycast(transform.position, (player.transform.position - transform.position).normalized, Vector2.Distance(transform.position, player.transform.position), sightBlockers))
            {
                vectorToTarget = player.transform.position - transform.position;
            }
        }

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 0) * vectorToTarget;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        if (!rotating)
        {
            StartCoroutine(Rotate(transform.rotation, targetRotation));
        }
    }
    IEnumerator Rotate(Quaternion start, Quaternion end)
    {
        rotating = true;
        float t = 0f;
        while (t < .3f)
        {
            transform.rotation = Quaternion.Slerp(start, end, t / .3f);
            yield return null;
            t += Time.deltaTime;
        }
        rotating = false;
    }
}
