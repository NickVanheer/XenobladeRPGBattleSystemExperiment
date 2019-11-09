using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float Speed = 50;
    public float duration = 5;
    public bool IsMoving = false;
    public bool IsImmortal = false;
    public Vector3 MoveDirection;

	void Update () {

        if (IsMoving)
            transform.Translate(MoveDirection * Speed * Time.deltaTime);

        if (IsImmortal)
            return;

        //Normally destroyed on collision, but just to be safe.
        duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(this.gameObject);

    }

    public void Move(Vector3 direction)
    {
        MoveDirection = direction;
        IsMoving = true;
    }

    public void Move(Vector3 direction, float speed)
    {
        MoveDirection = direction;
        Speed = speed;
        IsMoving = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }


}
