using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AlgoPrac;

public class TrieNode<T>
{
    private T _value;
    public T Value
    {
        get { return _value; }
        set
        {
            if (value == null)
            {
                throw new Exception("Value cannot be null.");
            }
            else if(value is char c && char.IsWhiteSpace(c))
            {
                _value = value;
            }
            else
            {
                throw new Exception("Invalid character");
            }
        }
    }
    public List<TrieNode<T>> Children { get; } = new List<TrieNode<T>>();
    public Dictionary<T, TrieNode<T>> ChildrenMap { get; } = new Dictionary<T, TrieNode<T>>();
    public bool IsWord { get; set; } = false;

    public TrieNode(T value)
    {
        _value = value;
    }
}

public class TrieTree
{
    private TrieNode<char> root;
    public TrieTree()
    {
        this.root = new TrieNode<char>('*');
    }

    private Queue<TrieNode<char>> NodesInOrder(TrieNode<char> node)
    {
        Queue<TrieNode<char>> queue = new Queue<TrieNode<char>>();
        Stack<TrieNode<char>> stack = new Stack<TrieNode<char>>();

        stack.Push(node);

        while(stack.Count > 0)
        {
            node = stack.Pop();
            queue.Enqueue(node);

            for(int i = node.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(node.Children[i]);

            }
        }

        return queue;
    }

    public void Insert(string word)
    {
        var node = this.root;

        foreach(char c in word)
        {
            char charToAdd = char.IsWhiteSpace(c) ? ' ' : c;

            if (!node.ChildrenMap.TryGetValue(c, out TrieNode<char>? childNode))
            {
                childNode = new TrieNode<char>(charToAdd);
                node.Children.Add(childNode);
                node.ChildrenMap[c] = childNode;
            }

            node = childNode;
        }

        node.IsWord = true;

    }

    private TrieNode<char>? FindLastNode(string word, out DoublyLinkedList<char, TrieNode<char>> doubly)
    {
        var node = this.root;
        doubly = new DoublyLinkedList<char, TrieNode<char>>();

        foreach (char c in word)
        {
            char charToAdd = char.IsWhiteSpace(c) ? ' ' : c;

            if(!node.ChildrenMap.TryGetValue(c, out _))
            {
                charToAdd = Char.ToUpper(charToAdd);
            }

            if (node.ChildrenMap.TryGetValue(charToAdd, out TrieNode<char>? child))
            {
                node = child;
                doubly.Insert(charToAdd, node);
            }
            else
            {
                this.Insert(word);
                return null;
            }
        }

        doubly.Remove();
        return node;
    }

    public List<string> AutoComplete(string prefix)
    {
        prefix = prefix.ToLower();

        DoublyLinkedList<char, TrieNode<char>> doublyLinkedList;
        var node = this.FindLastNode(prefix, out doublyLinkedList);

        if(node == null)
        {
            return new List<string>();
        }

        Queue<TrieNode<char>> queue = this.NodesInOrder(node);
        List<string> words = new List<string>();
        StringBuilder sb = new StringBuilder();

        while (queue.Count > 0)
        {
            var tmpNode = queue.Dequeue();
            doublyLinkedList.Insert(tmpNode.Value, tmpNode);

            // This whole block of code it there to check if the last item added to the 
            // list is at the correct spot (could probably be simplified, but my brain is fried right now).
            var dbllNode = doublyLinkedList.Tail();

            for (int i = 0; i < doublyLinkedList.Length && doublyLinkedList.Length > 1; i++)
            {
                if (!dbllNode.Prev.Item.Children.Contains(dbllNode.Item))
                {
                    doublyLinkedList.RemoveItem(dbllNode.Prev.Item);
                }
            }

            var dbHead = doublyLinkedList.Head();
            while(dbHead != null)
            {
                if (dbHead.Item.Children.Contains(dbllNode.Item) && dbHead.Item != dbllNode.Item)
                {
                    while (!dbHead.Item.Equals(dbllNode.Prev.Item))
                    {
                        doublyLinkedList.RemoveItem(dbllNode.Prev.Item);
                    }
                }

                dbHead = dbHead.Next;
            }
            // End.
            if (tmpNode.IsWord)
            {
                var list = doublyLinkedList.Traverse();

                foreach (var child in list)
                {
                    sb.Append(child.Value);
                }

                if (!words.Contains(sb.ToString()))
                {
                    words.Add(sb.ToString());
                }
                sb.Clear();
            }
        }

        return words;
    }

    public void Remove(string word)
    {
        word = word.ToLower();
        DoublyLinkedList<char, TrieNode<char>> dbList = new DoublyLinkedList<char, TrieNode<char>>();
        var node = this.root;
        foreach(char c in word)
        {
            char charToRemove = char.IsWhiteSpace(c) ? ' ' : c;
            if (node.ChildrenMap.TryGetValue(c, out TrieNode<char>? child))
            {
                dbList.Insert(charToRemove, child);
                node = child;
            }
            else
            {
                dbList.Clear();
                Console.WriteLine($"{word} could not be found.");
                return;
            }
        }

        if(dbList.Tail().Item.Children.Count != 0)
        {
            dbList.Tail().Item.IsWord = false;
            Console.WriteLine($"{word} is no longer a word.");
            return;
        }
        
        if(dbList.Tail().Item.Children.Count == 0 && dbList.Length > 1)
        {
            DBLLNode<char, TrieNode<char>>? dblNode = dbList.Tail().Prev;
            while(dblNode.Item.Children.Count == 1)
            {
                dblNode.Item.Children.Clear();
                dblNode.Item.ChildrenMap.Clear();
                dbList.Remove();
                
                if(dblNode.Prev == null)
                {
                    break;
                }
                dblNode = dblNode.Prev;
            }
        }

        if(dbList.Head().Item.Children.Count == 0)
        {
            this.root.Children.Remove(dbList.Head().Item);
            this.root.ChildrenMap.Remove(dbList.Head().Value);
        }

        Console.WriteLine($"{word} has been removed.");
    }
}
