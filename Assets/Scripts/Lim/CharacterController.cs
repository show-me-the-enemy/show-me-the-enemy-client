using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [System.Serializable]
    public class Animations
    {
        [SerializeField]
        public Sprite[] animations;

    }
    public SpriteRenderer spriteRenderer;
    private int direction;
    private Rigidbody2D rb;
    public Animations[] animations;
    private int idx = 0;
    public float interval;
    public float speed;
    void Start()
    {
        StartCoroutine(AnimationRoutine());
        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator AnimationRoutine()
    {
        while (true)
        {
            if (direction != 0)
            {
                spriteRenderer.sprite = animations[direction - 1].animations[idx];
                idx++;
                if (idx >= animations[direction - 1].animations.Length)
                    idx = 0;
            }
            yield return new WaitForSeconds(interval);
        }
    }

    private void Update()
    {
        rb.velocity = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = 1;
            rb.velocity += new Vector2(-speed, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = 2;
            rb.velocity += new Vector2(speed, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = 3;
            rb.velocity += new Vector2(0, speed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            direction = 4;
            rb.velocity += new Vector2(0, -speed);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            idx = 0;
            direction = 0;
            rb.velocity = new Vector2(0, 0);
        }
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }
}
