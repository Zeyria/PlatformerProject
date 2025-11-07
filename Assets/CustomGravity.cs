using UnityEngine;

public class CustomGravity : MonoBehaviour
{
    public float gravScale;
    public bool useAngle;
    Rigidbody2D rb;
    public float gravAngle;
    public Vector2 gravVector;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        gravAngle = this.transform.parent.transform.rotation.eulerAngles.z;
        gravVector = new Vector2(0, -9.81f);
        if (useAngle)
        {
            gravVector = rotate(gravVector, Mathf.Deg2Rad * gravAngle);
        }
    }
    void FixedUpdate()
    {
        rb.AddForce(gravVector, ForceMode2D.Force);
    }
    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}
