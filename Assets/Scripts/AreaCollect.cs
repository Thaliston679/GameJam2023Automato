using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollect : MonoBehaviour
{
    public GameManagerT gm;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Part"))
        {
            gm.PartInArea(collision.gameObject.GetComponent<SpriteRenderer>().sprite, collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Part"))
        {
            gm.PartOutArea(collision.gameObject);
        }
    }
}
