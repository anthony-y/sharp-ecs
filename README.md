# #ECS [![Build status](https://ci.appveyor.com/api/projects/status/icv0g4g8iok114l9?svg=true)](https://ci.appveyor.com/project/anthony-y/sharp-ecs)

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

**Note: This requires Visual Studio 2015 or newer)**

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

Create your first Entity is as easy as:

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

And that's the basics! Keep checking back over the coming weeks and months and I will most likely be adding stuff to the library and this readme.