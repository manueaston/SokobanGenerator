using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 moveDir;
    bool levelWon = false;

    public LayerMask goalLayer;

    GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (levelWon)
            return;
        // Player shouldn't be able to move if the level has been won

        GetMoveDir();

        CheckTargetPos();

        Move();

        CheckLevelWon();
    }

    void GetMoveDir()
    {
        moveDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDir.x = Input.GetAxisRaw("Horizontal");
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDir.y = Input.GetAxisRaw("Vertical");
        }
    }

    void CheckTargetPos()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir, 1.0f, ~goalLayer);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.transform.gameObject;

            if (hitObject.layer == LayerMask.NameToLayer("StopMovement"))
            {
                moveDir = Vector3.zero;
                // Reset move direction so no movement
            }
            else if (hitObject.layer == LayerMask.NameToLayer("Box"))
            {
                Box box = hitObject.GetComponent<Box>();

                if (!box.Move(moveDir))
                {
                    moveDir = Vector3.zero;
                    // Wall beside box so no movement
                }
            }
        }
    }

    void Move()
    {
        transform.position += moveDir;
    }

    void CheckLevelWon()
    {
        if (gameManager.GetLevelWon())
        {
            levelWon = true;
        }
    }
}
