using System;

namespace Algorithms;

public class BinaryTreeNode<T>
{
    public T Value { get; set; }
    public BinaryTreeNode<T> Left { get; set; }
    public BinaryTreeNode<T> Right { get; set; }
}

public static class BinaryTreeUtils
{
    public static BinaryTreeNode<int> IntegerBinaryTree => new ()
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
    
    public static void Visit<T>(this BinaryTreeNode<T> node)
    {
        Console.WriteLine($"Visiting node with value {node.Value}");
    }
}