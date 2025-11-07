using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject player;
    public float pickUpRange;
    GameObject outline;
    bool inList;
    private void Start()
    {
        outline = this.transform.GetChild(0).gameObject;
        inList = false;
    }
    void Update()
    {
        if(Vector2.Distance(player.transform.position, this.transform.position) < pickUpRange)
        {
            if (!inList)
            {
                player.GetComponent<playerMovement>().validItems.Add(this.gameObject);
                outline.SetActive(true);
                inList = true;
            }
        }
        else
        {
            if (inList)
            {
                player.GetComponent<playerMovement>().validItems.Remove(this.gameObject);
                outline.SetActive(false);
                inList = false;
            }
        }
    }
}
