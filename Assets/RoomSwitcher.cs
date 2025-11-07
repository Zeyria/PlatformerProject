using UnityEngine;

public class RoomSwitcher : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        this.transform.position = new Vector3((int)Mathf.Round(player.transform.position.x / 10.0f) * 10, 0, -10) ;
    }
}
