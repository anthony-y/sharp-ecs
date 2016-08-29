# #ECS [![Build status](https://ci.appveyor.com/api/projects/status/icv0g4g8iok114l9)](https://ci.appveyor.com/project/anthony-y/sharp-ecs)

An easy to use Entity Component System library for C#.

# Getting Started

## Compiling

First, you'll need to clone the project:

```
git clone http://www.github.com/anthony-y/sharp-ecs
```

Then you need to build the project, you can do this with the .bat file provided:

```
compile.bat
```

**Note: This requires Visual Studio 2015 or newer!**

This will output new binaries into the Build folder.

3 projects will be built:

- SharpECS: the main library. This is a .dll that you can link to when you compile your projects
- SharpECS.Samples: a test program to show off the capabilities of the library
- SharpECS.Tests: unit tests to check how fast some basic operations take

## Using the library

The first thing you'll need to do is include the SharpECS namespace in the appropriate files:

```csharp
using SharpECS;
```

Next, you'll need to make an instance of EntityPool which is where you'll store your Entities and where your Entity Systems will find your Entities.

**Note: You can have multiple EntityPools to store different groups of Entities**

```csharp
EntityPool entityPool = EntityPool.New("MyEntityPoolIdentifier");
```

Creating your first Entity is as easy as:

```csharp
Entity myEntity = entityPool.CreateEntity("MyEntity");
```

In order for your Entities to be detected and manipulated by Entity Systems (which we'll cover shortly), you need to add Components to your Entities.
Components also store data for that specific Entity which allows Systems to modify Entities independently.

Here is an example of a simple transform component class which holds an X and Y position for any Entities which have it "attached" to them.

```csharp
using SharpECS;

public class TransformComponent
    : IComponent
{
    public float X { get; set; }
    public float Y { get; set; }
}
```

And now, to add this component to our previously created "MyEntity":

```csharp
myEntity += new TransformComponent() { X = 123, Y = 123 }; // or whatever values you want
```

I will now write a "transform system" which will simply display the X and Y position of each Entity that has a TransformComponent every frame.
You usually wouldn't actually write a component like this unless you were debugging or something but I just want to show the process of making components and systems and how the two interact as simply as possible.

```csharp
using SharpECS;

public class TransformSystem
    : EntitySystem
{
    public TransformSystem(EntityPool entityPool)
        : base(entityPool, typeof(TransformComponent))
        /*
            The above line:
            1. Tells SharpECS to look for compatible Entities only in the EntityPool the user entered
            2. Tells SharpECS that we only want to interact with Entities which have a TransformComponent on them.
            
            Note: you could obviously have the user pass the component types or hard-code the EntityPool, etc. 
                  it's totally up to you.
        */
    { 

    }

    /* 
        This is not a built in method
        SharpECS aims to be platform independent so it doesn't matter what engine or framework(s) you use
    */
    public void Update()
    {
        // Loop through every Entity which was found with a TransformComponent on it in the specified EntityPool
        foreach (var entity in Compatible)
        {
            float X = entity.GetComponent<TransformComponent>().X;
            float Y = entity.GetComponent<TransformComponent>().Y;

            System.Console.WriteLine($"Entity {entity.Id} found at {X}, {Y}!");
        }

        /* 
            Note: I recommend that you cache components to prevent slowdown grabbing the same components every frame.
                  Currently SharpECS has no built in mechanism for this.
        */
    }
}
```

You can now create an instance of your system and call it as you wish!

```csharp
TransformSystem transformSystem = new TransformSystem(entityPool);

void Update()
{
    transformSystem.Update();
}
```

Systems are very powerful because they can do anything, from rendering all your Entities to updating physics in the world. 
See the [samples](https://github.com/anthony-y/sharp-ecs/tree/master/SharpECS.Samples) for a full demo of System usage.

Entities don't have to be made before Systems, nor do Components have to be added to Entities before Systems have been made!
This is because Systems are notified internally when anything changes, and they will rescan for Entities, so I could have easily made the system just after creating the entity pool.

### What is the Entity Cache and how Does it Work?

The EntityPool class is essentially what manages your Entities and gives both Systems, and you, a contained place to access them all.

But there is a mechanism behind the scenes which greatly improves memory usage and, in some cases, performance by "caching" Entities instead of destroying them.

When you destroy/delete/remove an Entity like so:

```csharp
entityPool.DestroyEntity(ref myEntity);
```

It is stripped of all it's components, given an empty string as an Id and has all it's children removed. In this state it is called a blank or cached Entity.
It is then placed into a Stack of Entities where it sits until a new Entity is requested (with ```CreateEntity```). When that happens, instead of contructing a new Entity with "new", the pool
pops an Entity off the stack, fills it's information back in and returns it.

It's important to know that if we do destroy "myEntity", the variable will be made null (which is why you must pass it as ```ref```). However, you can of course reassign it to a new Entity with ```CreateEntity```.

### Child Entities

We can give "myEntity" a kid with just one method call!

```csharp
myEntity.CreateChild("MyEntitysChild");
```

I'm so proud! "myEntity" has now reproduced (asexually O_O) to make a beautiful new baby Entity.

Now lets say "MyEntitysChild" is all grown up and of legal age and it wants to have kids too! Easy!

```csharp
myEntity.GetChild("MyEntitysChild").CreateChild("MyEntitysGrandChild");
```

There is an optional parameter for ```CreateChild``` called ```inheritComponents``` (which defaults to false).
As you can imagine, if you pass this as true:

```csharp
myEntity.CreateChild("TheLeastFavourite", true);
```

Then the new child will recieve all the components that it's parent has.

Not sure why the least favourite got the inheritance, I'm sure "MyEntitysChild" will be having words. And maybe knock a few teeth out. Unless it doesn't have a component for that.

### Family tree

You can walk the "family tree" of an Entity and do stuff with the family ( ͡° ͜ʖ ͡°)

```csharp
foreach (var entity in myEntity.FamilyTree())
{
    System.Console.WriteLine($"Entity related to {myEntity.Id}: {entity.Id} (Parent: {entity.Parent.Id}");
}
```

### Entity State

An Entity can be in 1 of 3 states at a time:

- Active
- Inactive
- Cached

The state of an Entity is controlled internally by SharpECS but is accessible from the outside with:

```csharp
EntityState myEntityState = myEntity.State;
```

SharpECS has a method for toggling an Entity between active and inactive:

```csharp
myEntity.Switch();
```

It's up to you to decide how your Systems act when Entities are in different states.

Let's modify our previously created TransformSystem's update method to only print out the position of Entities which have an Active state.

```csharp
public void Update()
{
    // Loop through every Entity which was found with a TransformComponent on it in the specified EntityPool
    foreach (var entity in Compatible)
    {
        // Make the sure the Entity is active!
        if (entity.IsActive())
        {
            float X = entity.GetComponent<TransformComponent>().X;
            float Y = entity.GetComponent<TransformComponent>().Y;

            System.Console.WriteLine($"Active Entity {entity.Id} found at {X}, {Y}!");
        }
    }

    /* 
        Note: I recommend that you cache components to prevent slowdown grabbing the same components every frame.
              Currently SharpECS has no built in mechanism for this.
    */
}
```

### Removing Components

Sometimes you might want to remove a Component from an Entity at runtime. You can do this with one method call:

```csharp
myEntity.RemoveComponent<TransformComponent>();
```

Easy. You can do it without a generic too using Runtime type checking, although I don't recommend you do this as it's much slower as the above method and, as I mentioned, uses Runtime type checking instead of compiletime type checking.

```csharp
myEntity.RemoveComponent(typeof(TransformComponent));
``` 

### More Entity methods

SharpECS gives you a lot of choice in how to do things, for example:

Components can be retrieved in two ways:

```csharp
var transformComponent = myEntity.GetComponent(typeof(TransformComponent)) as TransformComponent;
```

or

```csharp
var transformComponent = myEntity.GetComponent<TransformComponent>();
```

I've been using the latter in these demos and I recommend you do too as it is faster and more typesafe because the type is checked at compiletime whereas the former is checked at runtime.

Components can also be added in a few ways:

```csharp
myEntity += new MyComponent();
myEntity += new MyOtherComponent();
```

or

```csharp
myEntity.AddComponents
(
    new MyComponent(),
    new MyOtherComponent()
);
```

or

```csharp
IComponent[] componentCollection = new IComponent[] 
{
    new MyComponent(),
    new MyOtherComponent()
}; 

myEntity.AddComponents(componentCollection);
```

I encourage you to read the [code](https://github.com/anthony-y/sharp-ecs/tree/master/SharpECS/Source) for SharpECS in order to get an understanding of how it works and the methods you can use for each class.

#

And that's the basics! Check back here every once in a while or "watch" the project on GitHub to recieve updates on the library itself and this documentation. And don't forget to star :smiley: