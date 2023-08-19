using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

public class Node<T, TItem>
{
    private T _value;

    private TItem _item;

    public TItem Item
    {
        get { return _item; }
        set 
        {
            if(value == null)
            {
                throw new ArgumentNullException("Item cannot be null");
            }
            else
            {
                _item = value;
            }  
        }
    }
    public T Value
    {
        get { return _value; }
        set
        {
            if (value == null)
            {
                throw new Exception("Value cannot be null!");
            }
            else
            {
                _value = value;
            }
        }
    }
    public Node<T, TItem>? Left { get; set; } = null;
    public Node<T, TItem>? Right { get; set; } = null;

    public Node(T value, TItem item)
    {
        _value = value;
        this._item = item;
    }
}

public class BinarySearchTree<TItem>
{
    private int length;
    private Node<long, TItem>? root;

    public BinarySearchTree()
    {
        this.length = 0;
        this.root = null;
    }

    private void Organize(Node<long, TItem> node)
    {
        var curr = this.root;
        while (curr != null)
        {
            if (node.Value <= curr!.Value)
            {
                if (curr.Left == null)
                {
                    curr.Left = node;
                    break;
                }
                curr = curr.Left;
            }
            else if (node.Value > curr.Value)
            {
                if (curr.Right == null)
                {
                    curr.Right = node;
                    break;
                }
                curr = curr.Right;
            }
        }
    }

    private List<Node<long, TItem>?> GetNode(Node<long, TItem>? node, long value, List<Node<long, TItem>?> nodes)
    {
        nodes.Add(node);

        if (node == null)
        {
            return nodes;
        }

        if (value == node.Value)
        {
            return nodes;
        }

        if (value < node.Value)
        {
            return this.GetNode(node.Left, value, nodes);
        }
        return this.GetNode(node.Right, value, nodes);
    }

    private Node<long, TItem>? GetNodeWithParent(Node<long, TItem>? node, long value, out Node<long, TItem>? parent)
    {
        parent = null;

        while (node != null)
        {
            if (value == node.Value)
            {
                return node;
            }
            else if (value < node.Value)
            {
                parent = node;
                node = node.Left;
            }
            else
            {
                parent = node;
                node = node.Right;
            }

        }
        return null;
    }

    public void Insert(long value, TItem item)
    {
        this.length++;
        Node<long, TItem> node = new Node<long, TItem>(value, item);
        if (this.root == null)
        {
            this.root = node;
            return;
        }

        this.Organize(node);
    }

    public void Remove(long value)
    {
        if (this.length == 0)
        {
            Console.WriteLine("Tree is currently empty.");
            return;
        }

        Node<long, TItem>? parent;
        var node = this.GetNodeWithParent(this.root, value, out parent);

        if (node != null)
        {
            this.length--;

            if (node.Left == null && node.Right == null)
            {
                if (parent != null)
                {
                    parent.Left = null;
                    parent.Right = null;
                }
                else
                {
                    this.root = null;
                }
                return;
            }

            if (node.Left != null && node.Right != null)
            {
                if (parent != null)
                {
                    if (parent.Left != null && !parent.Left.Equals(node) || parent.Left == null)
                    {
                        var tmpNode = node.Left;
                        parent.Right = node.Right;
                        node.Left = null;
                        node.Right = null;
                        this.Organize(tmpNode);
                    }
                    else if (parent.Left != null && parent.Left.Equals(node))
                    {
                        var tmpNode = node.Right;
                        parent.Left = node.Left;
                        node.Left = null;
                        node.Right = null;
                        this.Organize(tmpNode);
                    }
                }
                else
                {
                    var tmpNode = node.Right;
                    this.root = node.Left;
                    node.Left = null;
                    node.Right = null;
                    this.Organize(tmpNode);
                }

                return;
            }

            if (node.Left != null)
            {
                if (parent != null)
                {
                    if (parent.Left != null && !parent.Left.Equals(node) || parent.Left == null)
                    {
                        var tmpNode = node.Left;
                        parent.Right = null;
                        this.Organize(tmpNode);
                    }
                    else if (parent.Left != null && parent.Left.Equals(node))
                    {
                        parent.Left = node.Left;
                        node.Left = null;
                    }
                }
                else
                {
                    this.root = node.Left;
                    node.Left = null;
                }

                return;
            }

            if (node.Right != null)
            {
                if (parent != null)
                {
                    if (parent.Left != null && !parent.Left.Equals(node) || parent.Left == null)
                    {
                        parent.Right = node.Right;
                        node.Right = null;
                    }
                    else if (parent.Left != null && parent.Left.Equals(node))
                    {
                        var tmpNode = node.Right;
                        parent.Left = null;
                        node.Right = null;
                        this.Organize(tmpNode);
                    }
                }
                else
                {
                    this.root = node.Right;
                    node.Right = null;
                }

                return;
            }
        }
        Console.WriteLine($"{value} was not found!");
    }

    private List<Node<long, TItem>> ReturnNodes(Node<long, TItem>? node)
    {
        List<Node<long, TItem>> nodes = new List<Node<long, TItem>>();
        Stack<Node<long, TItem>> stack = new Stack<Node<long, TItem>>();
        Node<long, TItem>? current = node;

        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.Right;
            }

            current = stack.Pop();
            nodes.Add(current);

            current = current.Left;
        }

        return nodes;
    }

    public List<Node<long, TItem>> Traverse()
    {
        List<Node<long, TItem>> myList = ReturnNodes(this.root);

        return myList;
    }
}
