using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerMovement : MonoBehaviour
{
    public float speed;
    public float airSpeed;
    [HideInInspector]
    public Rigidbody2D rb;
    Vector2 movement;
    public float jumpHeight;
    public ContactFilter2D ContactFilter;
    public float cTimer;
    Collider2D mainCol;
    bool IsGrounded => mainCol.IsTouching(ContactFilter);
    bool CanJump;
    bool shouldJump;
    int currentDir;
    public List<GameObject> validItems;
    public float throwSpeed;
    bool holdingItem;
    GameObject heldItem;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCol = GetComponent<CircleCollider2D>();
        holdingItem = false;
    }

    private void Update()
    {
        movement = new Vector2(currentDir, 0) * speed;
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z))
        {
            shouldJump = true;
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = 3;
        }
        if (rb.linearVelocityY < 0)
            rb.gravityScale = 3;
        if (Input.GetAxis("Horizontal") < 0)
            currentDir = -1;
        else if (Input.GetAxis("Horizontal") > 0)
            currentDir = 1;
        else
            currentDir = 0;
        if (!IsGrounded)
        {
            movement = new Vector2(currentDir, 0) * airSpeed;
        }
        if (IsGrounded)
        {
            CanJump = true;
        }
        else
        {
            StartCoroutine(CTimer());
        }

        //Item Logic
        if (holdingItem && Input.GetKeyDown(KeyCode.X))
        {
            heldItem.transform.SetParent(null);
            heldItem.GetComponent<Rigidbody2D>().simulated = true;
            heldItem.GetComponent<BoxCollider2D>().enabled = true;
            StartCoroutine(PlayerColAfterTime(heldItem.GetComponent<BoxCollider2D>()));
            heldItem.transform.GetChild(0).gameObject.SetActive(true);
            heldItem.GetComponent<Rigidbody2D>().AddForce(new Vector2(throwSpeed * currentDir, 5f), ForceMode2D.Impulse);
            heldItem = null;
            holdingItem = false;
        }
        if (validItems.Count != 0)
        {
            if (Input.GetKeyDown(KeyCode.V) && !holdingItem)
            {
                validItems[0].gameObject.transform.SetParent(this.transform.GetChild(3).GetChild(0));
                validItems[0].transform.GetChild(0).gameObject.SetActive(false);
                validItems[0].gameObject.GetComponent<Rigidbody2D>().simulated = false;
                validItems[0].gameObject.GetComponent<BoxCollider2D>().enabled = false;
                validItems[0].gameObject.transform.localPosition = new Vector3(0, -.15f, 0);
                heldItem = validItems[0].gameObject;
                validItems.RemoveAt(0);
                holdingItem = true;
            }
        }
    }
    IEnumerator PlayerColAfterTime(Collider2D col)
    {
        LayerMask tempMask = col.forceReceiveLayers;
        LayerMask tempMask2 = col.forceSendLayers;
        col.forceReceiveLayers = LayerMask.GetMask("Default", "Ground", "Water");
        col.forceSendLayers = LayerMask.GetMask("Default", "Ground", "Water", "Foilage");
        yield return new WaitForSeconds(.5f);
        col.forceReceiveLayers = tempMask;
        col.forceSendLayers = tempMask2;
    }
    IEnumerator CTimer()
    {
        yield return new WaitForSeconds(cTimer);
        if (!IsGrounded)
        {
            CanJump = false;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity += movement;
        if (shouldJump && CanJump)
        {
            rb.linearVelocityY = 0;
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            CanJump = false;
        }
        shouldJump = false;
    }
}
