using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float evaluationScore = 0.0f;
    public float visitCount = 0.0f;

    public List<Node> children;
    bool isFullyExpanded; // Need to determine if this is true

    public void SearchTree()    // Called for root node, initial state of board
    {
        LinkedList<Node> visitedNodes = new LinkedList<Node>();
        Node currentNode = this;
        visitedNodes.AddLast(currentNode);

        while (currentNode.isFullyExpanded)
        {
            currentNode = Select(currentNode);
            visitedNodes.AddLast(currentNode);
        }

        // Expand selected node

        // Evaluate child node

        // Backpropogation
        foreach (Node node in visitedNodes)
        {
            node.Update();
        }
    }

    Node Select(Node _node)
    {
        Node selectedChild = children[0];
        //select child with highest UCB

        return selectedChild;
    }

    void Expand()
    {
        // Create child node
        // Create action links for child


    }

    void Evaluate()
    {
        // Use evaluation function
    }

    void Update()
    {
        // Update timesVisited and evaluationScore
    }

    float GetUCB(float _parentVisitCount)
    {
        // Check this /////

        // UCB(s) = w(PIs) + C * sqrt(lnpv / sv)

        float UCB = (evaluationScore / visitCount) + (Util.C * Mathf.Sqrt(Mathf.Log(_parentVisitCount) / visitCount));
        // Figure out better way to get parent visit count
        // Check for division by 0

        return UCB;
    }
}
