using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public void Move(Vector2 position)
    {
        // Vector2‚ðVector3‚É•ÏŠ·
        transform.position = position;
    }
}
