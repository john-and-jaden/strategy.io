using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer) return;

        Vector3 pos = transform.position;
        pos.x += Input.GetAxis("Horizontal") * Time.deltaTime * 10;
        pos.y += Input.GetAxis("Vertical") * Time.deltaTime * 10;
        transform.position = pos;
    }

    public override void OnStartClient()
    {
        Vector2Int gridDimensions = GameManager.GridSystem.GetDimensions();
        int halfWidth = gridDimensions.x / 2;
        int halfHeight = gridDimensions.y / 2;
        transform.position = new Vector3(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight));
    }
}