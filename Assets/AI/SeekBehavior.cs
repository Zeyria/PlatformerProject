using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehavior : SteeringBehavior
{
    [SerializeField]
    private float targetReachedThreshold = 2f;
    [SerializeField]
    private float toClose = 1f;
    private float toCloseCached;
    [SerializeField]
    private bool showGizmos = false;
    bool reachedLastTarget = true;
    [SerializeField]
    private float sightRadius;
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Rigidbody2D playerRB;

    [HideInInspector]
    public Vector2 targetPositionCached;
    private float[] interestTemp;
    private void Start()
    {
        toCloseCached = toClose;
    }
    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        if (aiData.wandering)
        {
            toClose = 0f;
        }
        else
        {
            toClose = toCloseCached;
        }
        if(aiData.currentTarget != null)
        {
            if (Vector2.Distance(transform.position, aiData.currentTarget.position) > Vector2.Distance(transform.position, playerRB.transform.position) && !Physics2D.Raycast(transform.position, (playerRB.transform.position - transform.position).normalized, sightRadius, layerMask)
                && Vector2.Distance(transform.position, playerRB.transform.position) < sightRadius)
            {
                reachedLastTarget = true;
            }
        }
        if (reachedLastTarget)
        {
            if(aiData.targets == null || aiData.targets.Count <= 0)
            {
                aiData.currentTarget = null;
                return (danger, interest);
            }
            else
            {
                reachedLastTarget = false;
                aiData.currentTarget = aiData.targets.OrderBy(target => Vector2.Distance(target.position, transform.position)).FirstOrDefault();
            }
            if (reachedLastTarget)
            {
                if (aiData.wandering)
                {
                    aiData.targets = null;
                }
            }
        }

        if (aiData.currentTarget != null && aiData.targets != null && aiData.targets.Contains(aiData.currentTarget))
            targetPositionCached = aiData.currentTarget.position;
        if(Vector2.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            aiData.currentTarget = null;
        }

        Vector2 directionToTarget = (targetPositionCached - (Vector2)transform.position);
        if (Vector2.Distance(transform.position, targetPositionCached) < toClose)
        {
            for (int i = 0; i < danger.Length; i++)
            {
                float result = Vector2.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

                if (result > 0)
                {
                    float valueToPutIn = result;
                    if (valueToPutIn > danger[i])
                    {
                        danger[i] = valueToPutIn;
                    }
                }
            }
            for (int i = 0; i < interest.Length; i++)
            {
                float result = Vector2.Dot(directionToTarget.normalized, Directions.eightDirections[i]);
                result = -result;

                if (result > 0)
                {
                    float valueToPutIn = result;
                    if (valueToPutIn > interest[i])
                    {
                        interest[i] = valueToPutIn;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < interest.Length; i++)
            {
                float result = Vector2.Dot(directionToTarget.normalized, Directions.eightDirections[i]);

                if (result > 0)
                {
                    float valueToPutIn = result;
                    if (valueToPutIn > interest[i])
                    {
                        interest[i] = valueToPutIn;
                    }
                }
            }
        }
        interestTemp = interest;
        return (danger, interest);
    }
    private void OnDrawGizmos()
    {
        if(showGizmos == false)
        {
            return;
        }
        Gizmos.DrawSphere(targetPositionCached, .2f);

        if(Application.isPlaying && interestTemp != null)
        {
            if(interestTemp != null)
            {
                Gizmos.color = Color.green;
                for(int i = 0; i < interestTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestTemp[i]);
                }
                if(reachedLastTarget == false)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(targetPositionCached, .1f);
                }
            }
        }
    }
}
