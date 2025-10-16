using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject left, right, front, back;
    public float movespeed = 5f;

    const float deadzone = 0.2f;
    const int DIRECTION_UP = 0, DIRECTION_DOWN = 1, DIRECTION_LEFT = 2, DIRECTION_RIGHT = 3;

    private int direction = 0;
    private Rigidbody2D rb;
    private bool moving=false;

    public bool IsMoving()
    {
        return moving;
    }

    public int Direction
    {
        get => direction; set
        {
            direction = value;
            left.SetActive(direction == DIRECTION_LEFT);
            right.SetActive(direction == DIRECTION_RIGHT);
            front.SetActive(direction == DIRECTION_DOWN);
            back.SetActive(direction == DIRECTION_UP);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float horizontal, float vertical)
    {
        moving = true;
        if (vertical > deadzone)
        {
            Direction = DIRECTION_UP;
        }
        else if (vertical < -deadzone)
        {
            Direction = DIRECTION_DOWN;
        }
        else if (horizontal > deadzone)
        {
            Direction = DIRECTION_RIGHT;
        }
        else if(horizontal < -deadzone)
        {
            Direction = DIRECTION_LEFT;
        }
        else
        {
            moving = false;
        }
        if (moving)
        {
            Vector2 speed = new Vector2(horizontal, vertical) * movespeed;
            if (GameController.Eng <= 0)
                speed = speed * 0.3f;
            rb.linearVelocity = speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
