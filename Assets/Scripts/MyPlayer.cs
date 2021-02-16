using UnityEngine;
using Mirror;

public class MyPlayer : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            return;
        }

        Vector3 pos = transform.position;
        pos.x += Input.GetAxis("Horizontal") * Time.deltaTime * 10;
        pos.y += Input.GetAxis("Vertical") * Time.deltaTime * 10;
        transform.position = pos;
    }
}