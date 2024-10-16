using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDelete : MonoBehaviour
{

    private void OnMouseDown()
    {
        Destroy (gameObject);
    }
}
