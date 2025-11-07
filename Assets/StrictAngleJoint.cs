using UnityEngine;

public class StrictAngleJoint : MonoBehaviour
{
    public Transform target;
    public bool onlyZRot = true;
    void FixedUpdate()
    {
        transform.up = target.position - transform.position;
        if (onlyZRot)
        {
            Quaternion rot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
            transform.rotation = rot;
        }
    }
}
