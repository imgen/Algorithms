using System;
using System.Collections.Generic;
namespace Algorithms;

using static BinaryTreeUtils;
using static TraversalType;

public static class BinaryTreeTraverser
{
    public static void TestPreOrderTraversal()
    {
        PreOrderTraverse(IntegerBinaryTree);
    }
    
    public static void TestPreOrderTraversal2()
    {
        PreOrderTraverse2(IntegerBinaryTree);
    }
    
    public static void TestInOrderTraversal()
    {
        InOrderTraverse(IntegerBinaryTree);
    }
    
    public static void TestInOrderTraversal2()
    {
        InOrderTraverse2(IntegerBinaryTree);
    }
    
    public static void TestPostOrderTraversalWithFlag()
    {
        PostOrderTraverseWithFlag(IntegerBinaryTree);
    }
    
    public static void TestPostOrderTraversalDoubleStack()
    {
        PostOrderTraverseDoubleStack(IntegerBinaryTree);
    }
    
    public static void TestPostOrderTraversalWithoutFlag()
    {
        PostOrderTraverseWithoutFlag(IntegerBinaryTree);
    }

    public static void TestUniversalTraversal()
    {
        Console.WriteLine("PreOrder traversal using universal method");
        UniversalTraverse(IntegerBinaryTree, PreOrder);
        
        Console.WriteLine();
        
        Console.WriteLine("InOrder traversal using universal method");
        UniversalTraverse(IntegerBinaryTree, InOrder);
        
        Console.WriteLine();
        
        Console.WriteLine("PostOrder traversal using universal method");
        UniversalTraverse(IntegerBinaryTree, PostOrder);
    }

    public static void TestElegantUniversalTraversal()
    {
        Console.WriteLine("PreOrder traversal using elegant universal method");
        ElegantUniversalTraverse(IntegerBinaryTree, PreOrder);

        Console.WriteLine();

        Console.WriteLine("InOrder traversal using elegant universal method");
        ElegantUniversalTraverse(IntegerBinaryTree, InOrder);

        Console.WriteLine();

        Console.WriteLine("PostOrder traversal using elegant universal method");
        ElegantUniversalTraverse(IntegerBinaryTree, PostOrder);
    }

    private static void PreOrderTraverse(BinaryTreeNode<int> root)
    {
        var stack = new Stack<BinaryTreeNode<int>>([root]);
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            node.Visit();
            if (node.Right is not null)
            {
                stack.Push(node.Right);
            }

            if (node.Left is not null)
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
            while (node is not null)
            {
                node.Visit();
                if (node.Right is not null) //如果该结点有右孩子，右孩子进栈
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
            while (node is not null)
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
        while (node is not null || stack.Count > 0)
        {
            //  如果node不为NULL，将其压栈并遍历其左子树
            if (node is not null)
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
    private static void PostOrderTraverseWithFlag(BinaryTreeNode<int> root)
    {
        var stack = new Stack<(BinaryTreeNode<int> node, int flag)>();
        
        var node = root;
        while (node is not null || stack.Count > 0)
        {
            while (node is not null)
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
    private static void PostOrderTraverseDoubleStack(BinaryTreeNode<int> root)
    {
        var rootRightLeftStack =  new Stack<BinaryTreeNode<int>>([root]);
        var postOrderStack = new Stack<BinaryTreeNode<int>>();
        while (rootRightLeftStack.Count > 0)
        {
            var node = rootRightLeftStack.Pop();
            postOrderStack.Push(node);
            if (node.Left is not null)
            {
                rootRightLeftStack.Push(node.Left);
            }

            if (node.Right is not null)
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
     * see https://zhuanlan.zhihu.com/p/80578741
     * 
     * 二叉树后序遍历：单栈写法
     * 
     * 实际上，我们可以只使用一个栈去模拟后序遍历，但是会比较麻烦。
     * 为了避免问题变得复杂，我们可以先考虑一下能不能借鉴一下前序遍历的思路。
     * 首先，我们很确定的是，后序遍历的开头和前序遍历是可以一样的，都是先经过二叉树的最左分支，
     * 直到经过的节点是个叶子节点（没有左右孩子）为止。
     * 代码如下：
     *
     * while(node is not null) { // 经过所有左节点
     *      stack.push(node);
     *      node = node.left;
     * }
     *
     * 接下来很关键，我们得考虑什么时候才能访问节点。首先我们可以很确定一种情况：
     * 发现是叶子节点，必然会访问。这是第一种情况。
     * 我们回想前序遍历和中序遍历的时候，它们经过的路径都是左根右，对于前序和中序来说，
     * 访问路径基本上跟经过路径是一致的。但是在后序遍历中，我们先经过根节点，
     * 但是我们不会去访问它，而是会选择先访问它的右子节点。所以在这种情况下，
     * 我们会将根节点留在栈中不弹出，等到需要访问它的时候再出。
     * 
     * 那么，什么时候才需要访问根节点呢？答案当然就是访问完右子节点之后了。我们如何
     * 获取这个信息？这并不难，我们可以用一个指针pre来记录一下上一次访问的节点，如果
     * 当前经过的节点，它的右子节点是上一次访问过的，那么当前节点为我们需要访问的节点
     * （因为左子节点和右子节点都被访问过了）。这是第二种情况。
     * 总结起来，我们什么时候才能访问节点。有如下两种情况：
     * 当前经过节点是叶子节点。
     * 当前经过节点的右子节点是上一次访问的节点。
     */
    private static void PostOrderTraverseWithoutFlag(BinaryTreeNode<int> root)
    {
        var stack =  new Stack<BinaryTreeNode<int>>([root]);
        var node = root;
        // 用于记录上一次访问的节点
        BinaryTreeNode<int> previousVisitedNode = null;
        while (node is not null || stack.Count > 0)
        {
            while (node is not null)
            {
                stack.Push(node);
                node = node.Left;
            }

            node = stack.Pop();

            if (node.Right is null || node.Right == previousVisitedNode)
                // 右子节点为null或上一次访问的节点为当前节点的右子节点。
            {
                node.Visit();
                previousVisitedNode = node;
                // 此处为了跳过下一次循环的访问左子节点的过程，直接进入栈的弹出阶段，
                // 因为但凡在栈中的节点，它们的左子节点都肯定被经过且已放入栈中。
                node = null;
            }
            else // 有右子节点且尚未访问
            {
                // 还没到访问当前子树的根节点的时候，因此将已弹出的根节点重新放回栈中。
                stack.Push(node);
                // 指向右子节点，意味着下一个循环开始遍历右子树。
                node = node.Right;
            }
        }
    }

    /*
     * see https://zhuanlan.zhihu.com/p/80578741
     * 
     * 前中后序遍历的统一写法
     */
    private static void UniversalTraverse(BinaryTreeNode<int> root, TraversalType traversalType)
    {
        var stack =  new Stack<BinaryTreeNode<int>>();
        var node = root;
        // 用于记录上一次访问的节点
        BinaryTreeNode<int> previousVisitedNode = null;
        while (node is not null || stack.Count > 0)
        {
            while (node is not null)
            {
                stack.Push(node);
                if (traversalType is PreOrder)
                {
                    node.Visit();
                }
                node = node.Left;
            }

            node = stack.Pop();
            if (traversalType is InOrder)
            {
                // 中序遍历访问节点
                node.Visit();
            }
            if (node.Right is null || 
                (
                    node.Right == previousVisitedNode && 
                    traversalType is PostOrder
                )
            )
                // 右子节点为null或上一次访问的节点为当前节点的右子节点。
            {
                if (traversalType is PostOrder)
                {
                    node.Visit();
                    previousVisitedNode = node;
                }
                
                // 此处为了跳过下一次循环的访问左子节点的过程，直接进入栈的弹出阶段，
                // 因为但凡在栈中的节点，它们的左子节点都肯定被经过且已放入栈中。
                node = null;
            }
            else // 有右子节点且尚未访问
            {
                // 还没到访问当前子树的根节点的时候，因此将已弹出的根节点重新放回栈中。
                if (traversalType is PostOrder)
                {
                    stack.Push(node);
                }

                // 指向右子节点，意味着下一个循环开始遍历右子树。
                node = node.Right;
            }
        }
    }
    
    /*
     * see https://zhuanlan.zhihu.com/p/260497281
     *
     * 前中后序遍历的统一写法
     * 此时我们在二叉树：一入递归深似海，从此offer是路人中用递归的方式，实现了二叉树前中后序的遍历。
     * 在二叉树：听说递归能做的，栈也能做！中用栈实现了二叉树前后中序的迭代遍历（非递归）。
     * 之后我们发现「迭代法实现的先中后序，其实风格也不是那么统一，除了先序和后序，有关联，
     * 中序完全就是另一个风格了，一会用栈遍历，一会又用指针来遍历。」
     *
     * 实践过的同学，也会发现使用迭代法实现先中后序遍历，很难写出统一的代码，不像是递归法，
     * 实现了其中的一种遍历方式，其他两种只要稍稍改一下节点顺序就可以了。
     *
     * 其实「针对三种遍历方式，使用迭代法是可以写出统一风格的代码！」
     *
     * 「重头戏来了，接下来介绍一下统一写法。」
     *
     * 我们以中序遍历为例，在二叉树：听说递归能做的，栈也能做！中提到说使用栈的话，
     * 「无法同时解决访问节点（遍历节点）和处理节点（将元素放进结果集）不一致的情况」。
     *
     * 「那我们就将访问的节点放入栈中，把要处理的节点也放入栈中但是要做标记。」
     *
     * 如何标记呢，「就是要处理的节点放入栈之后，紧接着放入一个空指针作为标记。」
     * 这种方法也可以叫做标记法。
     */
    private static void ElegantUniversalTraverse(BinaryTreeNode<int> root, TraversalType traversalType)
    {
        var stack =  new Stack<BinaryTreeNode<int>>([root]);

        BinaryTreeNode<int> PushNode(BinaryTreeNode<int> node)
        {
            // 节点入栈（空节点不入栈）
            if (node is not null)
            {
                stack.Push(node); 
            }
            return node;
        }

        BinaryTreeNode<int> PushNodeAndNull(BinaryTreeNode<int> node)
        {
            PushNode(node);
            stack.Push(null);
            return node;
        }

        while (stack.Count > 0)
        {
            var node = stack.Peek();
            if (node is not null)
            {
                // 将该节点弹出，避免重复操作，下面再将右中左节点添加到栈中
                stack.Pop();

                // 第一步
                // 前序和中序遍历, 则压右节点入栈, 遍历顺序分别为中左右, 左中右
                // 因中节点在最后, 故最先压栈
                // 后序遍历, 则压中节点入栈, 遍历顺序是左右中
                // 因中节点在最后, 故最先压栈
                _ = traversalType switch
                {
                    PreOrder or InOrder => PushNode(node.Right), // 添加右节点（空节点不入栈)
                    PostOrder => PushNodeAndNull(node), // 添加中节点, 中节点访问过，但是还没有处理，加入空节点做为标记。
                    _ => null
                };

                // 第二步
                // 前序遍历, 则压左节点入栈, 因其访问顺序是中左右, 左节点在中间
                // 中序遍历, 则压中节点入栈, 因其访问顺序是左中右, 中节点在中间
                // 后序遍历, 则压右节点入栈, 因其访问顺序是左右中, 右节点在中间
                _ = traversalType switch
                {
                    PreOrder => PushNode(node.Left), // 添加左节点（空节点不入栈)
                    InOrder => PushNodeAndNull(node), // 添加中节点, 中节点访问过，但是还没有处理，加入空节点做为标记。
                    PostOrder => PushNode(node.Right), // 添加右节点（空节点不入栈)
                    _ => null
                };

                // 第三步
                // 若是前序遍历, 则压中节点入栈, 其访问顺序是中左右
                // 因中节点在最前, 故最后入栈
                // 中序遍历和后续遍历, 则压左节点入栈, 其访问顺序分别是左中右, 左右中
                // 因左节点在最前, 故最后入栈
                _ = traversalType switch
                {
                    PreOrder => PushNodeAndNull(node), // 添加中节点, 中节点访问过，但是还没有处理，加入空节点做为标记。
                    InOrder or PostOrder => PushNode(node.Left), // 添加左节点（空节点不入栈),
                    _ => null
                };
            }
            else // 只有遇到空节点的时候，才访问下一个栈内节点
            {
                stack.Pop(); // 将空节点弹出
                node = stack.Pop(); // 重新取出栈中节点
                node.Visit(); // 访问该节点
            }
        }
    }
}