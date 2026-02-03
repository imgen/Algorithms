using System;
using System.Collections.Generic;

namespace Algorithms;

public record BinaryTreeNodeAndLevel<T>(BinaryTreeNode<T> Node, int Level);

public static class BinaryTreeBreadthFirstTraverser
{
    public static void TestBinaryTreeBreadthFirstTraverser()
    {
        Traverse(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    private static void Traverse(BinaryTreeNode<int> root)
    {
        if (root == null) return;

        Queue<BinaryTreeNodeAndLevel<int>> queue = new ([new BinaryTreeNodeAndLevel<int>(root, 1)]);
        while (queue.Count > 0)
        {
            var (node, nodeLevel) = queue.Dequeue();
            Console.WriteLine($"Visiting node with value {node.Value} at level {nodeLevel}");
            if (node.Left != null)
                queue.Enqueue(new BinaryTreeNodeAndLevel<int>(node.Left, nodeLevel + 1));
            if (node.Right != null)
                queue.Enqueue(new BinaryTreeNodeAndLevel<int>(node.Right, nodeLevel + 1));
        }
    }
}