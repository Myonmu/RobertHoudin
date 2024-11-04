# This Project is a WIP. It is still far from complete

**Current Focus: Changing the variable system to reduce allocation for each variable, and add binding (referencing another variable). (currently functional, now adding editor juice)**

A generic BTS for Unity, using graph view and SO. There is a search window that should automatically add custom nodes in the correct hierachy. Though, it is said that Unity has a non released major update of the graph view and changes can have significant impact on this project... hopefully the basic logic stays the same. 

![image](https://user-images.githubusercontent.com/62897460/166552187-deaf7304-e2b7-496d-ba8a-ab03dd15ea4a.png)

The motive of this project is to replace an Animator based FSM in my game. Just for reusability I decide to make the tool generic.

This tool supports the famous Scriptable Object eventing system natively. Though you might need to implement some methods on your own if you are already using one.

There are also tutorials that instruct you to implement one from scratch, notabaly the tutorial from The Kiwi Coder from which I took my first steps. I basically started by combining the best part of the different tutorials and change the architecture to fit my own needs... 

## Create a new BT

To create a new BT, right click in the project view and find *Create > BTS > BehaviorTree*. You can now click the newly created asset and double click. This will bring out the editor.

![image](https://user-images.githubusercontent.com/62897460/167006715-c675d166-24dd-4bc1-adab-4491fded6015.png)

You can now play with some basic nodes. To create a node, press space bar to bring out the search window.

![image](https://user-images.githubusercontent.com/62897460/167007044-31c3d66d-4575-477a-ac12-22c61b421263.png)

To be able to communicate with other nodes or other parts of your game, you will need to create Blackboard or VariableBoard. In MochiBTS, Blackboard can be derived from the blackboard class (which is a SO) and add whatever you want. It comes with a default list of event that you can use keys to access via event nodes.You can create a blackboard instance with the same method mentioned above and assign it to a BT (You have to go though the process of implenting a SO). 

![image](https://user-images.githubusercontent.com/62897460/167007270-2b2300c6-5f01-4e30-ae5f-9fcffe8b0c60.png)

Some nodes can use reflection to get a specific field from the blackboard: 

![image](https://user-images.githubusercontent.com/62897460/167008358-27a28a03-2b19-465f-a408-2c5804b1a391.png)

However, since Blackboard is based on SO, you can't assign scene reference to it. This is why you will need an Agent. In MochiBTS, an Agent is a Monobehaviour component that represents an entity that carries the behavior tree. It can, like the blackboard, be rewritten to add new fields. Each Agent can also have a Variable Board, which is another kind of monobehaviour component. The variable board is a list of variables of different types. You can assign a variable board to an agent component to see it in the editor directly:

![image](https://user-images.githubusercontent.com/62897460/167009661-a4fd64ff-47a6-4397-9527-18bac26be2ea.png)

It is possible to share Variable board and Blackboard between trees, by simply assigning the same instance to different trees or agents. But you have to know what you are doing. By default, Uppon initialization of a runtime instance of a tree, the blackboard is cloned (this does not mean that custom fields that are normally passed by reference is also cloned!). Events will be cloned if the event entry's *cloned* field is set to true. Variable board by default is not cloned. 

To be able to run a tree, you will need a Tree Runner component. Assign a Tree and an Agent to it, and you can press play. The editor will take a while to get refreshed. When the editor enters play mode, nodes will show there corresponding state (green = success, blue = running, red = failure).

## Word about performance

Variable boards and Blackboards are handy, but the ease of use is "balanced" by its performance. If you use any of these data containers, during the execution of a BT there will be lots of reflection, boxing and unboxing, which can hurt performance if you use it on performance critical systems. (For example, do not use a BT to animate a bullet). Generally speaking, Variable board is the slowest, and then it is accessing fields using reflection (blackboard or agent), after that you have interface implementations, and finally direct access (class.field). If you want to use the BT in performance heavy part, you will have to implement custom nodes that uses direct access.

## DataProcessors (DP)

DP are used by data processor nodes. They are scriptable objects that only contain a method: ``object Process(object o)``. They can be used to do specific calculations without the need to create a dedicated node. There are two different base classes that your custom DP can derive from: BaseDataProcessor and DataProcessor<T>. The difference is that the latter enforces typing. 
  
 For instance, if you want to have a DP that add 1 to an integer when invoked:
  ```csharp
  [CreateAssetMenu]
  public class IntegerIncrement : DataProcessor<int>
  {
       protected override int OnProcess(int o){
             return o+1;
       }
  }
 ```
  There are also BaseMultiDataProcessor and MultiDataProcessor<T> that takes 2 objects and returns an object. Note that if you derive from BaseMultiDataProcessor, the input values can be different since they are boxed in an objet, whereas MultiDataProcessor<T> requires both objects to be of type T and returns T.
