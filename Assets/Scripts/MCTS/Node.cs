using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EActionType
{
    DeleteObstacle,
    PlaceBox,
    FreezeLevel,
    MoveAgent,
    EvaluateLevel
}

public class Node : MonoBehaviour
{
    State nodeState;

    public float evaluationScoreSum = 0.0f;
    public float visitCount = 0.0f;

    public List<Node> children;
    bool isVisited = false;

    public Node(State _state)
    {
        nodeState = _state;
    }

    public void SearchTree()    // Called for root node, initial state of board
    {
        LinkedList<Node> visitedNodes = new LinkedList<Node>();
        Node currentNode = this;
        visitedNodes.AddLast(currentNode);

        // Selection
        while (currentNode.isVisited && currentNode.nodeState.saved != true)
        {
            currentNode = currentNode.Select();
            visitedNodes.AddLast(currentNode);
        }

        // Expansion
        currentNode.Expand();

        // Evaluation
        float evaluationValue = currentNode.Evaluate();

        // Backpropogation
        foreach (Node node in visitedNodes)
        {
            node.UpdateNode(evaluationValue);
        }
    }

    Node Select()
    {
        float highestUCB = 0.0f;
        Node selectedChild = null;

        foreach(Node child in children)
        {
            float currentUCB = child.GetUCB(visitCount);

            if (currentUCB > highestUCB)
            {
                highestUCB = currentUCB;
                selectedChild = child;

                if (!selectedChild.isVisited)
                    break;
                // Will always choose first child that hasn't been visited
            }
        }

        return selectedChild;
    }

    void Expand()   // Creates children nodes
    {
        if (nodeState.frozen == false)
        {
            // Available actions: Delete obstacle, Place box, Freeze level
            AddChildNode(EActionType.DeleteObstacle);
            AddChildNode(EActionType.PlaceBox);
            AddChildNode(EActionType.FreezeLevel);

        }
        else if (nodeState.saved == false)
        {
            // Available actions: Move agent, Evaluate level
            AddChildNode(EActionType.MoveAgent);
            AddChildNode(EActionType.EvaluateLevel);
        }
    }

    void AddChildNode(EActionType _action)
    {
        // Create child node
        // Create action links for child

        Node newChildNode = this;

        switch (_action)
        {
            case EActionType.DeleteObstacle:
                newChildNode.nodeState.DeleteRandomObstacle();
                break;

            case EActionType.PlaceBox:
                newChildNode.nodeState.PlaceRandomBox();
                break;

            case EActionType.FreezeLevel:
                newChildNode.nodeState.frozen = true;
                break;

            case EActionType.MoveAgent:
                newChildNode.nodeState.MoveAgentRandomly();
                break;

            case EActionType.EvaluateLevel:
                newChildNode.nodeState.saved = true;
                break;

            default:
                break;
        }

        children.Add(newChildNode);
    }

    float Evaluate()
    {
        isVisited = true;

        // scaling weights
        float b = 5.0f;
        float c = 10.0f;
        float n = 1.0f;
        // normalises score
        float k = 50.0f;

        float evaluationScore =  ((b * nodeState.Get3x3BlockCount()) + (c * nodeState.GetCongestion()) + (n * nodeState.GetBoxCount())) / k;

        if (nodeState.saved == true)
        {
            // Save node as start configuration and apply post-processing
            // TODO ///////
        }

        return evaluationScoreSum;
    }

    void UpdateNode(float _evalValue)
    {
        // Update visitCount and evaluationScore

        visitCount++;
        evaluationScoreSum += _evalValue;
    }

    float GetUCB(float _parentVisitCount)
    {
        // Check this /////

        // UCB(s) = w(PIs) + C * sqrt(lnpv / sv)

        if (visitCount == 0.0f)
        {
            // If child has never been visited, will choose this child
            return 10.0f;
        }

        float UCB = (evaluationScoreSum / visitCount) + (Util.C * Mathf.Sqrt(Mathf.Log(_parentVisitCount) / visitCount));
        // Figure out better way to get parent visit count
        // Check for division by 0

        return UCB;
    }
}
