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
    public List<float> evaluationScores = new List<float>();

    public List<Node> children = new List<Node>();
    bool isVisited = false;

    public LevelGenerator levelGenerator;

    public Node(State _state, LevelGenerator _lg)
    {
        nodeState = _state;
        levelGenerator = _lg;
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
        if (!currentNode.nodeState.saved)
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
            AddChildrenDeleteObstacle();
            AddChildrenPlaceBox();
            AddChildFreezeLevel();
        }
        else if (nodeState.saved == false)
        {
            // Available actions: Move agent, Evaluate level
            AddChildrenMovePlayer();
            AddChildSaveLevel();           
        }
    }

    void AddChild(State _state)
    {
        Node newChildNode = new Node(_state, levelGenerator);
        children.Add(newChildNode);
    }

    void AddChildrenDeleteObstacle()
    {
        for (int y = 0; y < Util.height; y++)
        {
            for (int x = 0; x < Util.width; x++)
            {
                State newState = nodeState.DeleteObstacle(y, x);
                if (newState != null)
                {
                    AddChild(newState);
                }
            }
        }
    }

    void AddChildrenPlaceBox()
    {
        for (int y = 0; y < Util.height; y++)
        {
            for (int x = 0; x < Util.width; x++)
            {
                State newState = nodeState.PlaceBox(y, x);
                if (newState != null)
                {
                    AddChild(newState);
                }
            }
        }
    }

    void AddChildFreezeLevel()
    {
        if (nodeState.GetBoxCount() > 0 && nodeState.GetEmptyCount() > 1)
        {
            State newState = new State(nodeState);
            newState.frozen = true;
            AddChild(newState);
        }
    }
    
    void AddChildrenMovePlayer()
    {
        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            State newState = nodeState.MoveAgent(dir);
            if (newState != null)
            {
                AddChild(newState);
            }
        }
    }

    void AddChildSaveLevel()
    {
        State newState = nodeState.SaveLevel();
        if (newState != null)
        {
            AddChild(newState);
        }
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

        evaluationScores.Add(_evalValue);
    }

    float GetUCB(Node child)
    {
        // UCB(s) = w(PIs) + C * sqrt(lnpv / sv)

        if (!child.isVisited)
        {
            // If child has never been visited, will choose this child
            return 10.0f;
        }

        float w = (evaluationScoreSum + child.GetAverageEvaluationScore()) / (visitCount + 1);    // Average evaluation score including child node 
        float v = (Util.C * Mathf.Sqrt(Mathf.Log(visitCount) / child.visitCount));

        float UCB = w + v;

       // Debug.Log("w " + w + ", v = " + v);

        return UCB;
    }

    public float GetAverageEvaluationScore()
    {
        if (visitCount == 0)
            return 0.0f;
        else
            return (evaluationScoreSum / visitCount);
    }
}
