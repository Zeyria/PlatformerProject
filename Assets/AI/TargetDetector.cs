using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField]
    private float targetDetectionRange = 5f;
    [SerializeField]
    private LayerMask obstaclesLayerMask, playerLayerMask;
    [SerializeField]
    private bool showGizmos = false;
    [SerializeField]
    private Transform tempTarget;

    private List<Transform> colliders;
    [SerializeField]
    private float wanderPointTime;
    [SerializeField]
    private Vector2 wanderRange;
    private Vector2 spawnPoint;
    private Vector2 wanderPoint;
    private float chillTimer;
    private bool chillin = false;
    private bool noticed = false;
    private bool whaed = false;
    [SerializeField]
    private GameObject noticePrefab;
    [SerializeField]
    private GameObject whaPrefab;
    [SerializeField]
    private LayerMask obstacleLayerMask;
    private void Start()
    {
        spawnPoint = transform.position;
        wanderPoint = spawnPoint;
        wanderPointTime = Random.Range(wanderPointTime * .9f, wanderPointTime * 1.1f);
        chillTimer = wanderPointTime;

        InvokeRepeating("PickWanderPoint", 0, wanderPointTime);
    }
    public override void Detect(AIData aiData)
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, targetDetectionRange, playerLayerMask);
        if(playerCollider != null)
        {
            Vector2 direction = (playerCollider.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDetectionRange, obstaclesLayerMask);
            if(hit.collider != null && (playerLayerMask & (1<< hit.collider.gameObject.layer)) != 0)
            {
                colliders = new List<Transform>() { playerCollider.transform };
                if (!noticed)
                {
                    noticed = true;
                    Vector3 tempVector = new Vector3(transform.position.x + .5f, transform.position.y + .5f, -1);
                    GameObject temp = Instantiate(noticePrefab, tempVector, Quaternion.identity);
                    Vector3 tempVector2 = new Vector3(temp.transform.position.x + .5f, temp.transform.position.y + .5f, -1);
                    StartCoroutine(MoveOverTime(temp.transform.position, tempVector2, .9f, temp));
                    StartCoroutine(KillAfterTime(1f, temp));
                }
            }
            else
            {
                noticed = false;
                colliders = null;
            }
        }
        else
        {
            colliders = null;
        }
        aiData.targets = colliders;
        if(aiData.targets == null)
        {
            if (!chillin)
            {
                StartCoroutine(ChillTimer());
                tempTarget.gameObject.SetActive(true);
                tempTarget.position = wanderPoint;
                aiData.targets = new List<Transform>();
                aiData.targets.Add(tempTarget);
                aiData.wandering = true;
            }
            if (!whaed)
            {
                whaed = true;
                Vector3 tempVector = new Vector3(transform.position.x + .5f, transform.position.y + .5f, -1);
                GameObject temp = Instantiate(whaPrefab, tempVector, Quaternion.identity);
                Vector3 tempVector2 = new Vector3(temp.transform.position.x + .5f, temp.transform.position.y + .5f, -1);
                StartCoroutine(MoveOverTime(temp.transform.position, tempVector2, .9f, temp));
                StartCoroutine(KillAfterTime(1f, temp));
            }
        }
        else
        {
            whaed = false;
            tempTarget.gameObject.SetActive(false);
            aiData.wandering = false;
        }
    }
    IEnumerator ChillTimer()
    {
        chillin = true;
        yield return new WaitForSeconds(chillTimer);
        chillin = false;
    }
    void PickWanderPoint()
    {
        wanderPoint = new Vector2(spawnPoint.x + Random.Range(wanderRange.x, wanderRange.y), spawnPoint.y + Random.Range(wanderRange.x, wanderRange.y));
        while(Physics2D.Raycast(wanderPoint, Vector2.up, .1f, obstacleLayerMask))
        {
            wanderPoint = new Vector2(spawnPoint.x + Random.Range(wanderRange.x, wanderRange.y), spawnPoint.y + Random.Range(wanderRange.x, wanderRange.y));
        }
    }
    private void OnDrawGizmos()
    {
        if (!showGizmos)
        {
            return;
        }
        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);
        if(colliders == null)
        {
            return;
        }
        Gizmos.color = Color.magenta;
        foreach(var item in colliders)
        {
            Gizmos.DrawSphere(item.position, .3f);
        }
    }
    IEnumerator MoveOverTime(Vector3 start, Vector3 end, float dur, GameObject gameObject)
    {
        float t = 0f;
        while (t < dur)
        {
            gameObject.transform.localPosition = Vector3.Lerp(start, end, t / dur);
            yield return null;
            t += Time.deltaTime;
        }
        gameObject.transform.localPosition = end;
    }
    IEnumerator KillAfterTime(float time, GameObject theObject)
    {
        yield return new WaitForSeconds(time);
        Destroy(theObject);
    }
}
