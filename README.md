# RobertHoudin

![GIF 2024-12-15 22-56-45](https://github.com/user-attachments/assets/7f1170de-8f5b-4781-b99d-8d83c865109d)

A Houdini-like node evaluation framework with (hopefully) common tech art functions.

The project is *useable* but still needs quality of life improvements to be suitable to integrate in large scale production.

At the moment, the code base contains a minor amount of behaviour tree concepts, because it is derived from an earlier behaviour tree plugin MochiBTS

### Why "Robert Houdin"?

If your name is actually Robert Houdin then it is a coincidence. The project is named after the famous French magician, and the reason being, well, Harry *Houdini* is also a famous and remarkable figure in the history of prestidigitation. Plus, *Houdin* is so similar to *Houdini* so I couldn't help it.

## Current API
![image](https://github.com/user-attachments/assets/3843c96b-4818-4b0e-886e-7053546df9f9)

### Create an RhTree asset
```Create -> RobertHoudin -> RH Tree```

An **RhTree** represents a collection of inter-connected nodes, equivalent to a network in Houdini. Double click on an RhTree asset to open the RobertHoudin Editor.

In the Editor, press space to bring up the node search window, click on an entry to add the node to the RhTree.

Each node will have an "OUTPUT" flag, which determines when the RhTree stops evaluating nodes. This is equivalent to the render flag in Houdini. Click on the output flag will turn the flag to green, and any previously activated output flag will become grey. There can only be one active output flag per tree.

For tree that returns a result as output, or requires *Property Blocks*, a driver script is usually needed.

### Extending the node library

To create a custom node, simply inherit `RhNode`.

```csharp
    public class CustomNode : RhNode
    {
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
```

The `OnEvaluate` method must be implemented to tell what the node does. But for now, we should add **ports** first.

A port is any class inherit from `RhPort`, and there are plenty of ports to choose from. In general, ports are categorized as follows:

- **Simple Ports** and **Data Source Ports**: A simple port receives its value from connected port, while a data source port *can* receive a value from connected port, directly configure a fixed value in the inspector, or from a *Property Block* via reflection. Builtin data source ports can be identified by their suffix `Ds` in type name.
- **Single Ports** and **Multi Ports**: A single port can only accept one inbound connection, while a multi port can accept multiple. Note that this only limits input, an output single port can still connect to multiple ports.

Declaring a port is simply adding a member field, plus adding a port attribute in front of that field.

For example, the *Sum* sample node:

```csharp
     public class SumNode : RhNode
    {
        [RhInputPort] public MultiIntPort inputs;
        [RhOutputPort] public IntPort output;
        

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var sum = 0;
            inputs.ForEachConnected(i => sum += i);
            output.SetValueNoBoxing(sum);
            return true;
        }
    }
```

Here, `[RhInputPort]` signifies `inputs` is an input port, and `[RhOutputPort]` means `output` is an output port. 

There are other attributes that are specifically used in Loop nodes, but the API is still very unstable.

To implement the `OnEvaluate` method, typically you call `GetValueNoBoxing()` on an input port, and `SetValueNoBoxing()` on an output port. **For Multi Ports, however, you will need to call `ForEachConnected()` instead as a Multi Port doesn't store a value itself**.

Once the implementation is finished, the node should show up in the node search window (RobertHoudin Editor, press space).

### Number Types

The framework provides a proxy type `Number` that can be used as both `int` and `float`. This is added to eliminate the need to implement a float and int version of the same node, as well as using a conversion node if you need int or float but only have the other.

You can use `NumberPort`, `NumberPortDs` or `MultiNumberPort` to replace int or float ports in most cases.

The caveat, is that `Number` secretly uses `float` and can result in overflow if you are manipulating a large enough integer. If you find yourself needing to do so, use int or float ports explicitly. 

### DataSource

When using `DataSource`, there are 3 different types where the data can come from:
- `None`: the value is embedded into the data source. In the editor, an extra field will appear after the type selection.

![image](https://github.com/user-attachments/assets/8488a636-8636-4b93-a5d1-225303abff07)
- `Port`: the value comes from evaluating the connected port.

![image](https://github.com/user-attachments/assets/fde2e67a-5b94-43cd-a9b3-03390ad6eae7)
- `PropertyBlock`: the value is retrieved from the property block in `RhExecutionContext`, using cached reflection. The text box before the type selection becomes editable and you should input the field name. (a drop down version might be implemented in the future).

![image](https://github.com/user-attachments/assets/5a1ef8b4-e07d-49ca-ab56-ecc63599c7af)

### Property Block

`IRhPropertyBlock` is the primary way to pass external data or volatile data to the `RhTree` for evaluation. Typically, you would use a datasource port set to *PropertyBlock* to access values in the property block.

For example, this class defines a simple context for scattering:

```csharp
    [Serializable]
    public class SimpleScatterPropertyBlock: IRhPropertyBlock
    {
        public Transform rootTransform;
        public int maxActivePoints;
        public int k;
        public Bounds bounds;
        public Vector2 distance;
        public SimpleObjectProvider objectProvider;
    }
```

You could either expose them in inspector or manually set them with other code.

You could directly access the property block inside `OnEvaluate()` method of a node, since the property block is passed as part of the `RhExecutionContext`, but doing so would create a strong dependency between the node and the property block class, and is best to avoid.

As describe in the previous section, to access a field via datasource port, you simply need to fill the text box with the name of the field, and reflection will take care of the rest:


### ForEach Nodes

`ForEachNode` is a very special generic class that not only has input and output ports, but also *item port* and *item result port*. If you are familiar with LINQ, `ForEachNode` is very similar to `Select`:

```csharp
var inputCollection = new List<Item>();
var outputCollection = new List<ItemResult>();
//selector takes an Item, and outputs an ItemResult
Func<Item, ItemResult> selector;
outputCollection = inputCollection.Select(selector).ToList();
```

The generic class takes care of most of the hard working but you need to specify 6 type arguments:

- Input Item type `T`
- Output Item type `U`
- Input collection port type (port that takes `List<T>` or other indexable collection)
- Output collection port type (port that takes `List<U>` or other indexable collection)
- Item port type (port that takes `T`)
- Item result port type (port that takes `U`)

You will also need to provide a way to extract an item from the input collection, as well as a way to put a result into the output collection:

```csharp
//for example, in ForEachNumber
protected override Number Extract(NumberCollectionPort input, int i)
{
     return input.value[i];
}

protected override void Put(NumberCollectionPort outputPort, int i, Number value)
{
    outputPort.value.Add(value);
}
```

And that's it. You should be able to see 2 ports on the right of the node that represent item port and item result port. 

It is legal to connect a node that isn't part of the loop with a node that is in the loop, but note that this can produce confusing results at times. The loop itself resets every node that is part of the loop with each iteration, and this can propagate the reset behaviour unexpectedly if you try to mix nodes that are outside the loop. Though I am thinking about implementing scopes and capturing so that it somewhat aligns with common programming schemes.

## Under the Hood

Here are some implementation details if you wish to modify the framework.

### Node/Port Referencing

The framework uses string GUIDs to reference ports and nodes, using Unity's GUID generator. The GUID is used in several ways:

- Each `RhNode` and `RhPort` keeps a serialized string as GUID (hidden from inspector). If the GUID is empty, we generate a new one.
- Unity's Graph View system finds an `RhPortView` by using `GetPortByGuid`, and finding `RhNodeView` by `GetNodeByGuid`. These two methods expects setting the `.viewDataKey` field of the node or port. 
- In `RhTree`, we keep separate dictionaries that map string GUIDs to respective `RhNode` and `RhPort`. 
- Each `RhPort` serializes the GUIDs of the connected ports.

### Evaluation Process

We always start evaluating an `RhTree` from the result node, and check each of its inputs. If the input has not been evaluated, we do the same for the input node. This is similar to a depth-first search.

`OnBeginEvaluate` will be called before checking the inputs. This is a virtual method but usually no override is needed. Currently, the only thing it does is to setup bindings in DataSource ports.

During a node's evaluation, which is calling the node's `OnEvaluate` method, the node reads from the input ports and sets values of the output ports. After that, the consumer of the output value will perform a *Port Forwarding* process that copies the output value to the connected input port (see `ForwardValue`). Though this is only applicable for single ports, as multi ports cannot have a single value. 

Ports also have `IsActive` state. This doesn't mean whether they contain values or not, but rather if we should propagate the evaluation process through that port. 



