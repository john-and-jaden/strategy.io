using UnityEngine;
using Mirror;

public class MyPlayer : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 pos = transform.position;
        pos.x += Input.GetAxis("Horizontal") * Time.deltaTime * 10;
        pos.y += Input.GetAxis("Vertical") * Time.deltaTime * 10;
        transform.position = pos;
    }
}