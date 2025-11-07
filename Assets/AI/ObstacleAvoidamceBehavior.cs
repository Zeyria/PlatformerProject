using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidamceBehavior : SteeringBehavior
{
    [SerializeField]
    private float radius = 2f, agentColliderSize = .6f;
    [SerializeField]
    private bool showGizmos = false;

    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        foreach(Collider2D obstacleCollider in aiData.obstacles)
        {
            Vector2 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
            float distanceToObstacle = directionToObstacle.magnitude;

            float weight
                = distanceToObstacle <= agentColliderSize
                ? 1
                : (radius - distanceToObstacle) / radius;
            Vector2 directionToObstacleNormalized = directionToObstacle.normalized;
            for(int i = 0; i < Directions.eightDirections.Count; i++)
            {
                float result = Vector2.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);

                float valueToPutIn = result * weight;
                if(weight == 1)
                {
                    float temp = result -.5f;
                    interest[i] = temp * weight * .2f;
                }
                if(valueToPutIn > danger[i])
                {
                    danger[i] = valueToPutIn;
                }
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }
    private void OnDrawGizmos()
    {
        if(showGizmos == false)
        {
            return;
        }
        if(Application.isPlaying && dangersResultTemp != null)
        {
            if(dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for(int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * dangersResultTemp[i]);
                }
            }
        }
        else
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
public static class Directions
{
    public static List<Vector2> eightDirections = new List<Vector2>
        {
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
}
