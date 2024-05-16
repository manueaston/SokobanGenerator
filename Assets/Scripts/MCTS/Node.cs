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

public class Node
{
    public State nodeState;

    public float evaluationScoreSum = 0.0f;
    public float visitCount = 0.0f;

    public List<Node> children = new List<Node>();
    bool isVisited = false;

    public LevelGenerator levelGenerator;

    // Debug Variable
    EActionType action;

    public Node(State _state, LevelGenerator _lg, EActionType _action)
    {
        nodeState = new State(_state);
        levelGenerator = _lg;
        action = _action;
    }

    public void SearchTree()    // Called for root node, initial state of board
    {
        LinkedList<Node> visitedNodes = new LinkedList<Node>();
        Node currentNode = this;
        visitedNodes.AddLast(currentNode);

        int counter = 0;

        // Selection
        while (currentNode.isVisited)
        {
            Node selectedChild = currentNode.Select();
            if (selectedChild == null)
                break;
            // No possible actions from current Node

            currentNode = selectedChild;
            visitedNodes.AddLast(currentNode);

            counter++;

            if (currentNode.nodeState.saved)
                break;
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
            float currentUCB = GetUCB(child);

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
            if ((nodeState.GetEmptyCount() + nodeState.GetBoxCount()) < Util.numSpaces)
            {
                // If there are any obstacles to delete
                AddChildNode(EActionType.DeleteObstacle);
            }

            if (nodeState.GetEmptyCount() > 5)
            {
                // If there are enough spaces to place a box on
                AddChildNode(EActionType.PlaceBox);
            }
            
            if (nodeState.GetBoxCount() > 1 && nodeState.GetEmptyCount() > 6)
            {
                // If there are at least two boxes and 6 spaces to move around in
                AddChildNode(EActionType.FreezeLevel);
            }
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

        Node newChildNode = new Node(nodeState, levelGenerator, _action);

        bool childIsValid = false;

        switch (_action)
        {
            case EActionType.DeleteObstacle:
                childIsValid = newChildNode.nodeState.DeleteRandomObstacle();
                break;

            case EActionType.PlaceBox:
                childIsValid = newChildNode.nodeState.PlaceRandomBox();
                break;

            case EActionType.FreezeLevel:
                childIsValid = newChildNode.nodeState.frozen = true;
                break;

            case EActionType.MoveAgent:
                childIsValid = newChildNode.nodeState.MoveAgentRandomly();
                break;

            case EActionType.EvaluateLevel:
                childIsValid = newChildNode.nodeState.Save();
                break;

            default:
                break;
        }

        if (childIsValid)
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

        float blockCount = nodeState.Get3x3BlockCount();
        float congestion = nodeState.GetCongestion();
        float boxCount = nodeState.GetBoxCount();

        float evaluationScore =  ((b * blockCount) + (c * congestion) + (n * boxCount)) / k;

        //Debug.Log("Block count = " + blockCount + ", Congestion = " + congestion + ", Box Count = " + boxCount);

        if (nodeState.saved == true)
        {
            levelGenerator.SaveLevel(this);
        }

        //Debug.Log("Action selected: " + action + " with score: " + evaluationScore);

        return evaluationScore;
    }

    void UpdateNode(float _evalValue)
    {
        // Update visitCount and evaluationScore

        visitCount++;
        evaluationScoreSum += _evalValue;
    }

    float GetUCB(Node child)
    {
        // UCB(s) = w(PIs) + C * sqrt(lnpv / sv)

        if (child.visitCount == 0.0f)
        {
            // If child has never been visited, will choose this child
            return 10.0f;
        }

        float w = (evaluationScoreSum + (child.evaluationScoreSum / visitCount)) / (visitCount + 1);    // Average evaluation score including child node 

        float UCB = w + (Util.C * Mathf.Sqrt(Mathf.Log(visitCount) / child.visitCount));

        return UCB;
    }
}
