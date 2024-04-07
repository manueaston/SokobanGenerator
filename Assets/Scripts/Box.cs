using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject atGoalOverlay;
    public LayerMask goalLayer;
    BoxCollider2D coll;

    bool atGoal = false;

    GameManager gameManager;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public bool Move(Vector3 _direction)
    {
        coll.enabled = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, 1.0f, ~goalLayer);

        coll.enabled = true;

        if (hit.collider != null)
        {
            return false;
        }

        transform.position += _direction;

        // Check whether box has moved onto or off goal
        CheckAtGoal();

        return true;
    }

    void SetAtGoal(bool _atGoal)
    {
        atGoal = _atGoal;
        atGoalOverlay.SetActive(_atGoal);
        gameManager.UpdateBoxes(_atGoal);
    }

    public bool GetAtGoal()
    {
        return atGoal;
    }

    void CheckAtGoal()
    {
        if (Physics2D.OverlapBox(transform.position, Vector2.one, 0.0f, goalLayer))
        {
            if (!atGoal)
            {
                SetAtGoal(true);
            }
        }
        else if (atGoal)
        {
            SetAtGoal(false);
        }

    }
}
