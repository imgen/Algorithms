using System;
using System.Collections.Generic;

namespace Algorithms;

public class BinaryTreeNode<T>
{
    public T Value { get; set; }
    public BinaryTreeNode<T> Left { get; set; }
    public BinaryTreeNode<T> Right { get; set; }
}

public record BinaryTreeNodeAndLevel<T>(BinaryTreeNode<T> Node, int Level);

public static class BinaryTreeBreadthFirstTraverser
{
    public static void TestBinaryTreeBreadthFirstTraverser()
    {
        var binaryTree = new BinaryTreeNode<int>
        {
            Value = 1,
            Left = new BinaryTreeNode<int>
            {
                Value = 2,
                Left = new BinaryTreeNode<int>
                {
                    Value = 3,
                },
                Right =  new BinaryTreeNode<int>
                {
                    Value = 4,
                }
            },
            Right =  new BinaryTreeNode<int>
            {
                Value = 5,
                Left = new BinaryTreeNode<int>
                {
                    Value = 6,
                    Left = new BinaryTreeNode<int>
                    {
                        Value = 9,
                        Right = new BinaryTreeNode<int>
                        {
                            Value = 10
                        }
                    },
                    Right =  new BinaryTreeNode<int>
                    {
                        Value = 7,
                    }
                },
                Right = new BinaryTreeNode<int>
                {
                    Value = 8,
                }
            }
        };
        
        Traverse(binaryTree);
    }
    
    public static void Traverse(BinaryTreeNode<int> root)
    {
        if (root == null)
        {
            return;
        }

        int level = 1;
        Queue<BinaryTreeNodeAndLevel<int>> queue = new ();
        queue.Enqueue(new BinaryTreeNodeAndLevel<int>(root, level));
        
        while (queue.Count > 0)
        {
            var (node, nodeLevel) = queue.Peek();
            while (nodeLevel == level)
            {
                queue.Dequeue();

                Console.WriteLine($"Visiting node with value {node.Value} at level {nodeLevel}");
                
                if (node.Left != null)
                {
                    queue.Enqueue(new BinaryTreeNodeAndLevel<int>(node.Left, level + 1));
                }

                if (node.Right != null)
                {
                    queue.Enqueue(new BinaryTreeNodeAndLevel<int>(node.Right, level + 1));
                }

                if (queue.Count == 0)
                {
                    break;
                }
                (node, nodeLevel) = queue.Peek();
            }

            level++;
        }
    }
}