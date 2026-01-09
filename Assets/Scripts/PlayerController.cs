using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject left, right, front, back;
    public float movespeed = 5f;

    const float deadzone = 0.2f;
    const int DIRECTION_UP = 0, DIRECTION_DOWN = 1, DIRECTION_LEFT = 2, DIRECTION_RIGHT = 3;

    private Animator leftAnim, rightAnim, frontAnim, backAnim;
    private int direction = 0;
    private Rigidbody2D rb;
    private bool moving=false;

    public bool IsMoving()
    {
        return Moving;
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

    public bool Moving
    {
        get => moving; set
        {
            moving = value;
            if(left.activeInHierarchy)
                leftAnim.SetBool("Moving", moving);
            if (right.activeInHierarchy)
                rightAnim.SetBool("Moving", moving);
            if (front.activeInHierarchy)
                frontAnim.SetBool("Moving", moving);
            if (back.activeInHierarchy)
                backAnim.SetBool("Moving", moving);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        leftAnim = left.GetComponent<Animator>();
        rightAnim = right.GetComponent<Animator>();
        frontAnim = front.GetComponent<Animator>();
        backAnim = back.GetComponent<Animator>();
    }

    public void Move(float horizontal, float vertical)
    {
        Moving = true;
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
            Moving = false;
        }
        if (Moving)
        {
            Vector2 speed = new Vector2(horizontal, vertical) * movespeed;
            if (GameController.Eng <= 0)
                speed = speed * 0.6f;
            rb.linearVelocity = speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
