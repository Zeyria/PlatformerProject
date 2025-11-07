using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    public enum AIType
    {
        None,
        Wander,
    }
    public AIType aiType;
    //Ingame Stats
    public float runSpeed;
    public float turnSpeed;
    [HideInInspector]
    public bool isAttacking = false;
    [SerializeField]
    public float sightRadius;

    bool invulnFrames = false;
    [SerializeField]
    protected GameObject damageNumPrefab;
    private new Rigidbody2D rigidbody2D;

    [SerializeField]
    private List<Detector> detectors;
    [SerializeField]
    private AIData aiData;
    [SerializeField]
    private float detectionDelay =.05f, aiDelay = .06f;
    [SerializeField]
    private List<SteeringBehavior> steeringBehaviors;

    [SerializeField]
    private bool showGizmos = false;
    Vector2 resultDirection = Vector2.zero;
    private float rayLength = 1;

    private Vector2 moveDir;
    private Vector2 targetPosCached = Vector2.zero;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Rigidbody2D playerRB;

    private void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        if (aiType == AIType.None)
        {
            return;
        }
        InvokeRepeating("PreformDetection", 0, detectionDelay);
        InvokeRepeating("UpdateMoveDirection", 0, aiDelay);
    }
    private void UpdateMoveDirection()
    {
        moveDir = GetDirectionToMove(steeringBehaviors, aiData);
    }
    private void PreformDetection()
    {
        foreach(Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach(SteeringBehavior behavior in steeringBehaviors)
        {
            (danger, interest) = behavior.GetSteering(danger, interest, aiData);
        }
    }

    private void Update()
    {
        if(aiType == AIType.None)
        {
            return;
        }
        moveDir = new Vector2(getAverageFloatX(moveDir.x), getAverageFloatY(moveDir.y));
        rigidbody2D.linearVelocity = moveDir * runSpeed * 8.5f; //8.5 makes it consistient with player runspeed stat 

        if (Vector2.Distance(transform.position, playerRB.transform.position) < sightRadius && aiData.currentTarget !=null && !aiData.wandering)
        {
            if(!Physics2D.Raycast(transform.position, (playerRB.transform.position - transform.position).normalized, Vector2.Distance(transform.position, playerRB.transform.position), layerMask))
            {
                targetPosCached = playerRB.transform.position;
            }
            /*
            Vector3 targetV3 = targetPosCached;
            targetV3.x -= transform.position.x;
            targetV3.y -= transform.position.y;

            float angle = Mathf.Atan2(targetV3.y, targetV3.x) * Mathf.Rad2Deg;
            Quaternion qTo = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, qTo, turnSpeed * Time.deltaTime * .85f);
            */
        }
    }
    private List<float> floatValsX = new List<float>();
    private List<float> floatValsY = new List<float>();

    //Call with a new float to add each frame
    //The return value is the average of all floats, with a maximum of 15 floats stored
    private float getAverageFloatX(float newFloat)
    {
        floatValsX.Add(newFloat);
        if (floatValsX.Count > 15)  //Remove the oldest when we have more than 15
        {
            floatValsX.RemoveAt(0);
        }

        float total = 0f;
        foreach (float f in floatValsX)  //Calculate the total of all floats
        {
            total += f;
        }
        float average = total / (float)floatValsX.Count;

        return average;
    }
    private float getAverageFloatY(float newFloat)
    {
        floatValsY.Add(newFloat);
        if (floatValsY.Count > 15)  //Remove the oldest when we have more than 15
        {
            floatValsY.RemoveAt(0);
        }

        float total = 0f;
        foreach (float f in floatValsY)  //Calculate the total of all floats
        {
            total += f;
        }
        float average = total / (float)floatValsY.Count;

        return average;
    }
    /* ADD LATER FOR ITEM IMPACT
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Collider2D>().tag == "Weapon")
        {
            WeaponScript weaponScript = collision.gameObject.GetComponentInParent<WeaponScript>();
            if (!invulnFrames)
            {
                invulnFrames = true;
                StartCoroutine(InvulnFrameTimer());

                int damage = Mathf.RoundToInt(Random.Range(weaponScript.DamageRange.x, weaponScript.DamageRange.y));

                //Damage Numbers
                Vector3 tempVector = new Vector3(transform.position.x + .5f, transform.position.y + .5f, -1);
                GameObject temp = Instantiate(damageNumPrefab, tempVector, Quaternion.identity);
                Vector3 tempVector2 = new Vector3(temp.transform.position.x + .5f, temp.transform.position.y + .5f, -1);
                temp.GetComponent<TextMeshPro>().text = damage.ToString();
                StartCoroutine(MoveOverTime(temp.transform.position, tempVector2, .9f, temp));
                StartCoroutine(KillAfterTime(1f, temp));

                //Knockback
                Vector3 tempDir = collision.transform.position - transform.position;
            }
        }
    }
    */
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && showGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, moveDir * rayLength);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, (playerRB.transform.position - transform.position).normalized);
        }
    }
    IEnumerator KillAfterTime(float time, GameObject theObject)
    {
        yield return new WaitForSeconds(time);
        Destroy(theObject);
    }
    IEnumerator InvulnFrameTimer()
    {
        yield return new WaitForSeconds(.2f);
        invulnFrames = !invulnFrames;
    }
    public Vector2 GetDirectionToMove(List<SteeringBehavior> behaviors, AIData aiData)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach (SteeringBehavior behavior in behaviors)
        {
            (danger, interest) = behavior.GetSteering(danger, interest, aiData);
        }

        for (int i = 0; i < 8; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }
        outputDirection.Normalize();
        resultDirection = outputDirection;
        return resultDirection;
    }
}
