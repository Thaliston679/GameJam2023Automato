using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPart : MonoBehaviour
{
    public GameManagerT gm;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Part"))
        {
            gm.InCollectPart(collision.gameObject.GetComponent<SpriteRenderer>().sprite, collision.gameObject);
        }
    }
}
