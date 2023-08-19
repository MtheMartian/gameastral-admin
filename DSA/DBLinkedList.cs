using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AlgoPrac
{
    public class DBLLNode<T, TItem>
    {
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (value != null)
                {
                    _value = value;
                }
                else
                {
                    throw new Exception("Cannot be null!");
                }
                
            }
        }

        public TItem? Item { get; set; }
        public DBLLNode<T, TItem>? Next { get; set; } = null;
        public DBLLNode<T, TItem>? Prev { get; set; } = null;

        public DBLLNode(T value, TItem item)
        {
            _value = value;
            this.Item = item;
        }
    }

    public class DoublyLinkedList<T, TItem>
    {
        private DBLLNode<T, TItem>? head;
        private DBLLNode<T, TItem>? tail;
        private int _length;
        public int Length
        {
            get { return _length; }
        }


        public DoublyLinkedList()
        {
            this.head = this.tail = null;
            this._length = 0;
        }

        /// <summary>
        /// Insert a new item into the list.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        public void Insert(T value, TItem item)
        {
            this._length++;
            DBLLNode<T, TItem> newNode = new DBLLNode<T, TItem>(value, item);

            if (this.tail == null)
            {
                this.head = newNode;
                this.tail = newNode;
                return;
            }

            newNode.Prev = this.tail;
            this.tail.Next = newNode;
            this.tail = newNode;
        }

        /// <summary>
        /// Find and return an item based on the entered value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DBLLNode<T, TItem>? Find(T value)
        {
            DBLLNode<T, TItem>? node = this.head;
            while (node != null)
            {
                if (value.Equals(node.Value))
                {
                    return node;
                }

                node = node.Next;
            }

            return null;
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            this.head = null;
            this.tail = null;
            this._length = 0;
        }

        /// <summary>
        /// Removes an item with the corresponding value.
        /// </summary>
        /// <param name="value"></param>
        public void Remove(T value)
        {
            DBLLNode<T, TItem>? node = this.head;

            while (node != null)
            {
                if (value.Equals(node.Value))
                {
                    this._length--;

                    if (node.Prev == null)
                    {
                        this.head = node.Next;
                        node.Next = null;
                        return;
                    }

                    if (node.Next == null)
                    {
                        this.tail = node.Prev;
                        this.tail.Next = null;
                        node.Prev = null;
                        return;
                    }

                    node.Prev.Next = node.Next;
                    node.Next.Prev = node.Prev;
                    node.Next = null;
                    node.Prev = null;
                    return;
                }

                node = node.Next;
            }
        }

        /// <summary>
        /// Removes the specified item from the list.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(TItem item)
        {
            DBLLNode<T, TItem>? node = this.head;

            while (node != null)
            {
                if (item.Equals(node.Item))
                {
                    this._length--;

                    if (node.Prev == null)
                    {
                        this.head = node.Next;
                        node.Next = null;
                        return;
                    }

                    if (node.Next == null)
                    {
                        this.tail = node.Prev;
                        this.tail.Next = null;
                        node.Prev = null;
                        return;
                    }

                    node.Prev.Next = node.Next;
                    node.Next.Prev = node.Prev;
                    node.Next = null;
                    node.Prev = null;
                    return;
                }

                node = node.Next;
            }
        }

        /// <summary>
        /// Remove the last item of the list.
        /// </summary>
        public void Remove()
        {
            if(this.Length == 0)
            {
                return;
            }

            if(this.Length == 1)
            {
                this.Clear();
                return;
            }

            this._length--;
            var node = this.tail;
            
            this.tail = node.Prev;
            this.tail.Next = null;
            node.Prev = null;
        }

        /// <summary>
        /// Returns a list of all items in the list.
        /// </summary>
        /// <returns></returns>
        public List<DBLLNode<T, TItem>> Traverse()
        {
            if (this.head == null)
            {
                Console.WriteLine("It's Empty!");
                return new List<DBLLNode<T, TItem>>();
            }

            List<DBLLNode<T, TItem>> nodes = new List<DBLLNode<T, TItem>> ();
            DBLLNode<T, TItem>? node = this.head;
            while (node != null)
            {
                nodes.Add(node);
                node = node.Next;
            }
            return nodes;
        }

        /// <summary>
        /// Returns the first item of the list.
        /// </summary>
        /// <returns></returns>
        public DBLLNode<T, TItem>? Head()
        {
            return this.head;
        }

        /// <summary>
        /// Returns the last item of the list.
        /// </summary>
        /// <returns></returns>
        public DBLLNode<T, TItem>? Tail()
        {
            return this.tail;
        }
    }
}
