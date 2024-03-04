// Consistent Hashing

// Create nodes
var node1 = new Node("Server 1");
var node2 = new Node("Server 2");
var node3 = new Node("Server 3");
var node4 = new Node("Server 4");


// Create consistent hashing
var hashing = new ConsistentHashing();
hashing.Add(node1);
hashing.Add(node2);
hashing.Add(node3);
hashing.Add(node4);


// Add data to the nodes
hashing.GetNode("key1")?.Add("key1", "my_value11");
hashing.GetNode("key2")?.Add("key2", "3_value22");
hashing.GetNode("key3")?.Add("key3", "value33");
hashing.GetNode("key4")?.Add("key4", "value44");


// Print the data of the nodes
Console.WriteLine("Node data:");
node1.PrintNodeData();
node2.PrintNodeData();
node3.PrintNodeData();
node4.PrintNodeData();


// Remove node
hashing.Remove(node2);


// Print the data of the nodes after the removal
Console.WriteLine("\n\nNode data after removal:");
node1.PrintNodeData();
node3.PrintNodeData();
node4.PrintNodeData();


class Node
{
    public readonly string Id;
    public Dictionary<string, string> Data = [];

    public Node(string id) => Id = id;

    public void Add(string key, string value) => Data[key] = value;

    public string? Get(string key) => Data.ContainsKey(key) ? Data[key] : null;

    public void Remove(string key) => Data.Remove(key);
}

class ConsistentHashing
{
    // private readonly SortedDictionary<int, T> _circle = new SortedDictionary<int, T>(); // TODO: working here
    private readonly SortedList<int, Node> _ring = [];

    public void Add(Node node)
    {
        var hash = _getHash(node.Id);
        _ring[hash] = node;

        // Logic to move data to the new node, if necessary
        var processorNode = _getProcessor(node);
        var keysToMove = processorNode.Data.Keys
            .Where(key => _getHash(key) > hash)
            .ToList();

        foreach (var key in keysToMove)
        {
            node.Add(key, processorNode.Data[key]);
            processorNode.Remove(key);
        }
    }

    public void Remove(Node node)
    {
        var hash = _getHash(node.Id);
        if (!_ring.ContainsKey(hash)) return;

        var successor = _getSuccessor(node);
        foreach ((var key, var value) in node.Data)
        {
            successor.Add(key, value);
        }

        node.Data.Clear();

        _ring.Remove(hash);
    }

    public Node? GetNode(string key)
    {
        if (_ring.Count == 0) return null;

        var hash = _getHash(key);
        if (!_ring.ContainsKey(hash))
        {
            var index = _ring.Keys.ToList().BinarySearch(hash);
            index = index < 0 ? (~index) % _ring.Count : index;
            hash = _ring.Keys[index];
        }

        return _ring[hash];
    }

    private int _getHash(string key)
    {
        // A basic hashing function for demonstration purposes.
        // In a real-world scenario, you should use a more robust hashing function.
        var hash = key.Sum(c => c);
        return hash;
    }

    private Node _getSuccessor(Node node)
    {
        var hash = _getHash(node.Id);
        var keys = _ring.Keys.ToList();
        var index = keys.BinarySearch(hash);
        index = index < 0 ? (~index) % _ring.Count : (index + 1) % _ring.Count;
        return _ring[_ring.Keys[index]];
    }

    private Node _getProcessor(Node node)
    {
        var hash = _getHash(node.Id);
        var keys = _ring.Keys.ToList();
        var index = keys.BinarySearch(hash);
        index = index < 0 ? ((~index) - 1 + _ring.Count) % _ring.Count : index;
        return _ring[_ring.Keys[index]];
    }
}

static class Helper
{
    public static void PrintNodeData(this Node node)
    {
        Console.WriteLine($"\tNode data {node.Id}:");
        foreach ((var key, var value) in node.Data)
        {
            Console.WriteLine($"\t\t{key}: {value}");
        }
    }
}
