using System.Collections.Generic;
using System.Linq;

namespace Algorithms;

public static class BinaryTreeTraverser
{
    public static void TestPreOrderTraversal()
    {
        PreOrderTraverse(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    public static void TestPreOrderTraversal2()
    {
        PreOrderTraverse2(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    public static void TestInOrderTraversal()
    {
        InOrderTraverse(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    public static void TestInOrderTraversal2()
    {
        InOrderTraverse2(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    public static void TestPostOrderTraversal()
    {
        PostOrderTraverse(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    public static void TestPostOrderTraversal2()
    {
        PostOrderTraverse2(BinaryTreeUtils.IntegerBinaryTree);
    }
    
    public static void TestPostOrderTraversal3()
    {
        PostOrderTraverse3(BinaryTreeUtils.IntegerBinaryTree);
    }

    private static void PreOrderTraverse(BinaryTreeNode<int> root)
    {
        var stack = new Stack<BinaryTreeNode<int>>([root]);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            node.Visit();
            if (node.Right != null)
            {
                stack.Push(node.Right);
            }

            if (node.Left != null)
            {
                stack.Push(node.Left);
            }
        }
    }
    /*
     * see https://zhuanlan.zhihu.com/p/687719104
     */
    private static void PreOrderTraverse2(BinaryTreeNode<int> root)
    {
        var stack = new Stack<BinaryTreeNode<int>>([root]);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            while (node != null)
            {
                node.Visit();
                if (node.Right != null) //如果该结点有右孩子，右孩子进栈
                {
                    stack.Push(node.Right);
                }
                
                //一直指向根结点最后一个左孩子
                node = node.Left;
            }
        }
    }
    
    /*
     * see https://zhuanlan.zhihu.com/p/687719104
     * 
     * 非递归中序遍历一棵树二叉树，具体有两种实现方案：
     * 方案一：从根结点开始，不断地遍历当前结点的左子树并将该结点压入栈中，直至不再有左子树。
     * 然后从栈顶取出一个结点并访问它，然后将它的右孩子压入栈中，继续以同样的方式遍历它的右子树；
     * 
     * 方案二：和方案一唯一的不同之处在于，压入栈中的只会是每个结点的左孩子，右孩子不需要入栈。
     */
    private static void InOrderTraverse(BinaryTreeNode<int> root)
    {
        var stack = new Stack<BinaryTreeNode<int>>([root]);
        while (stack.Count > 0)
        {
            var node = stack.Peek();
            while (node != null)
            {
                //将该结点的左孩子进栈，如果没有左孩子，NULL进栈
                stack.Push(node.Left);
                node = node.Left;
            }
            //  跳出循环，栈顶元素肯定为NULL，将NULL弹栈
            stack.Pop();
            
            if (stack.Count == 0) continue;
            node  = stack.Pop();
            node.Visit();
            // If right node is null, signals that this tree is finished visiting,
            // so will move up one level
            // 右孩子进栈
            stack.Push(node.Right);
        }
    }
    
    private static void InOrderTraverse2(BinaryTreeNode<int> root)
    {
        var stack = new Stack<BinaryTreeNode<int>>();
        var node = root;
        //当node为NULL或者栈为空时，表明树遍历完成
        while (node != null || stack.Count > 0)
        {
            //  如果node不为NULL，将其压栈并遍历其左子树
            if (node != null)
            {
                stack.Push(node);
                node = node.Left;
            }
            //如果node为NULL，表明左子树遍历完成，需要遍历上一层结点的右子树
            else
            {
                node = stack.Pop();
                node.Visit();
                node = node.Right;
            }
        }
    }
    
    /*
     * see https://zhuanlan.zhihu.com/p/687719104
     * 
     * 后序遍历是在遍历完当前结点的左右孩子之后才访问该结点，所以需要在当前结点进栈时为其配备一个标志位。
     * 当遍历该结点的左孩子时，设置当前结点的标志位为 0；当要遍历该结点的右孩子时，设置当前结点的标志位
     * 为 1，进栈。
     * 
     * 这样当遍历完该结点的左右子树并将其弹栈时，查看该结点标志位的值：
     * 如果是 0，表示该结点的右孩子还没有遍历；
     * 如果是 1，说明该结点的左右孩子都遍历完成，可以访问此结点。
     */
    private static void PostOrderTraverse(BinaryTreeNode<int> root)
    {
        var stack = new Stack<(BinaryTreeNode<int> node, int flag)>();
        
        var node = root;
        while (node != null || stack.Count > 0)
        {
            while (node != null)
            {
                //为该结点入栈做准备
                //由于是遍历左孩子，设置标志位flag为0
                stack.Push((node, flag: 0));
                //以该结点为根结点，遍历左孩子
                node = node.Left;
            }
            
            var nodeWithFlag = stack.Pop();
            node = nodeWithFlag.node;
            //如果flag==0，说明该结点还没有遍历它的右孩子
            if (nodeWithFlag.flag == 0)
            {
                //更改该结点的标志位，重新压栈
                stack.Push((node, flag: 1));
                // 以该结点的右孩子为根结点，重复循环
                node = node.Right;
            }
            else //如果取出来的栈顶元素的flag==1，说明此结点左右子树都遍历完了，可以访问该结点了
            {
                node.Visit();
                node = null;
            }
        }
    }

    /*
     * see https://zhuanlan.zhihu.com/p/80578741
     * 
     * 二叉树后序遍历：双栈写法
     * 
     * 后序遍历的遍历顺序是左右根。我们是否可以从我们熟悉且简单的前序遍历转化过去后序遍历呢？
     * 答案是可以的。我们可以观察到，可以先求出遍历顺序是根右左的节点序列，再倒序，便刚好是后序遍历的顺序：左右根。
     * 而遍历顺序是根右左的话，很好办，从前序遍历的代码中修改两行就是了。
     */
    private static void PostOrderTraverse2(BinaryTreeNode<int> root)
    {
        var rootRightLeftStack =  new Stack<BinaryTreeNode<int>>([root]);
        var postOrderStack = new Stack<BinaryTreeNode<int>>();
        while (rootRightLeftStack.Count > 0)
        {
            var node = rootRightLeftStack.Pop();
            postOrderStack.Push(node);
            if (node.Left != null)
            {
                rootRightLeftStack.Push(node.Left);
            }

            if (node.Right != null)
            {
                rootRightLeftStack.Push(node.Right);
            }
        }

        foreach (var node in postOrderStack)
        {
            node.Visit();
        }
    }
    
    /*
     * 二叉树后序遍历：单栈写法
     * 我们回想前序遍历和中序遍历的时候，它们经过的路径都是左根右，对于前序和中序来说，
     * 访问路径基本上跟经过路径是一致的。
     * 
     * 但是在后序遍历中，我们先经过根节点，但是我们不会去访问它，而是会选择先访问它的左右子节点。
     * 所以在这种情况下，我们会将根节点留在栈中不弹出，等到需要访问它的时候再出。
     */
    private static void PostOrderTraverse3(BinaryTreeNode<int> root)
    {
        var stack =  new Stack<BinaryTreeNode<int>>([root]);
        List<BinaryTreeNode<int>> nodes = [];
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            nodes.Add(node);
            if (node.Left != null)
            {
                stack.Push(node.Left);
            }

            if (node.Right != null)
            {
                stack.Push(node.Right);
            }
        }

        foreach (var node in ((IEnumerable<BinaryTreeNode<int>>)nodes).Reverse())
        {
            node.Visit();
        }
    }
}