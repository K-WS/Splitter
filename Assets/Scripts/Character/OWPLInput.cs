using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldChar))]
public class OWPLInput : MonoBehaviour
{
    private WorldChar mov;
    private void Awake()
    {
        mov = GetComponent<WorldChar>();
    }

    // Update is called once per frame
    void Update()
    {
        float movX = Input.GetAxis("Horizontal");
        float movZ = Input.GetAxis("Vertical");

        mov.Move(new Vector3(movX, 0, movZ));
    }
}
