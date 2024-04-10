using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    State nodeState;

    public float evaluationScoreSum = 0.0f;
    public float visitCount = 0.0f;

    public List<Node> children;
    int childrenVisited = 0;
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
        while (currentNode.isVisited)
        {
            currentNode = Select(currentNode);
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

    Node Select(Node _node)
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
            }
        }

        return selectedChild;
    }

    void Expand()   // Creates children nodes
    {
        if (nodeState.frozen)
        {
            // Available actions: Move agent, Evaluate level


        }
        else
        {
            // Available actions: Delete obstacle, Place box, Freeze level


        }
    }

    Node AddNode()
    {
        // Create child node
        // Create action links for child

        return null;
    }

    float Evaluate()
    {
        // scaling weights
        float b = 5.0f;
        float c = 10.0f;
        float n = 1.0f;
        // normalises score
        float k = 50.0f;

        float evaluationScore =  ((b * nodeState.Get3x3BlockCount()) + (c * nodeState.GetCongestion()) + (n * nodeState.GetBoxCount())) / k;

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

        float UCB = (evaluationScoreSum / visitCount) + (Util.C * Mathf.Sqrt(Mathf.Log(_parentVisitCount) / visitCount));
        // Figure out better way to get parent visit count
        // Check for division by 0

        return UCB;
    }
}
