using UnityEngine;

public class Move : MonoBehaviour
{
    private float horizontal;
    private float vertical;

    void Update()
    {
        MoveMovement();
    }
    private void MoveMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
        {
            transform.Translate(horizontal * Time.deltaTime * Vector3.right);
            transform.Translate(vertical * Time.deltaTime * Vector3.up);
        }
    }
}