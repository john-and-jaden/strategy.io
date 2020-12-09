using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Resource(int posX, int posY)
    {
        transform.position = new Vector2(posX, posY);
    }
}
