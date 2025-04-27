// ConveyorMovement.cs
using UnityEngine;

public class ConveyorMovement : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}
