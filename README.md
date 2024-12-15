# RobertHoudin
## THIS IS A WIP, AND IS NOT READY TO USE
A Houdini-like node evaluation framework with (hopefully) common tech art functions.
At the moment, the code base contains a significant amount of behaviour tree concepts, because it is derived from an earlier behaviour tree plugin MochiBTS

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

To implement the `OnEvaluate` method, typically you call `GetValueNoBoxing()` on an input port, and `SetValueNoBoxing()` on an output port. For Multi Ports, however, you will need to call `ForEachConnected()` instead as a Multi Port doesn't store a value itself.

Once the implementation is finished, the node should show up in the node search window (RobertHoudin Editor, press space). 

### DataSource

When using `DataSource`, there are 3 different types where the data can come from:
- `None`: the value is embedded into the data source. In the editor, an extra field will appear after the type selection.

![image](https://github.com/user-attachments/assets/8488a636-8636-4b93-a5d1-225303abff07)
- `Port`: the value comes from evaluating the connected port.

![image](https://github.com/user-attachments/assets/fde2e67a-5b94-43cd-a9b3-03390ad6eae7)
- `PropertyBlock`: the value is retrieved from the property block in `RhExecutionContext`, using cached reflection. The text box before the type selection becomes editable and you should input the field name. (a drop down version might be implemented in the future).

![image](https://github.com/user-attachments/assets/5a1ef8b4-e07d-49ca-ab56-ecc63599c7af)



