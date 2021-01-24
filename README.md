# Spigot
### Your tap into custom event streams from any source
 
[![Build status](https://ci.appveyor.com/api/projects/status/xm19400akpjdb0uy/branch/master?svg=true)](https://ci.appveyor.com/project/ArchetypicalSoftware/spigot/branch/master)


Spigot allows you to simply connect into any event stream to pull in and publish strongly typed objects rather than simple strings.

In todays distributed, cloud-enabled world, more and more things have to communicate with existing systems they may not even know about. They also need to be able to let other applications know when they change the state of something so they can take action etc. The only scalable way to do this is by implementing an Event-driven architecture.

> <b>Event-driven architecture (EDA)</b>, is a software architecture pattern promoting the production, detection, consumption of, and reaction to events. 
An event can be defined as <i>"a significant change in state".</i> For example, when a consumer purchases a car, the car's state changes from "for sale" to "sold". A car dealer's system architecture may treat this state change as an event whose occurrence can be made known to other applications within the architecture. From a formal perspective, what is produced, published, propagated, detected or consumed is a (typically asynchronous) message called the event notification, and not the event itself, which is the state change that triggered the message emission. Events do not travel, they just occur. However, the term event is often used metonymically to denote the notification message itself, which may lead to some confusion. This is due to Event-Driven architectures often being designed atop message-driven architectures, where such communication pattern requires one of the inputs to be text-only, the message, to differentiate how each communication should be handled. 

[<i> Wikipedia definition of Event Driven Architecture</i>](https://en.wikipedia.org/wiki/Event-driven_architecture)

This helps you design your system in a way that decouples how the data gets to your system and what you do with it. 

Spigot abstracts this away from you so you can concentrate on when to send these events and what to do with the ones you receive. This is done using the Pub-Sub model:

> In software architecture, <b>publish–subscribe</b> is a messaging pattern where senders of messages, called publishers, do not program the messages to be sent directly to specific receivers, called subscribers, but instead categorize published messages into classes without knowledge of which subscribers, if any, there may be. Similarly, subscribers express interest in one or more classes and only receive messages that are of interest, without knowledge of which publishers, if any, there are. 
Publish–subscribe is a sibling of the message queue paradigm, and is typically one part of a larger message-oriented middleware system. Most messaging systems support both the pub/sub and message queue models in their API, e.g. Java Message Service (JMS). 
This pattern provides greater network scalability and a more dynamic network topology, with a resulting decreased flexibility to modify the publisher and the structure of the published data. 

[<i> Wikipedia definition of Publish–subscribe pattern</i>](https://en.wikipedia.org/wiki/Publish–subscribe_pattern)

Subscribing to an event is as easy as creating a Knob:

```csharp
 public class ComplexClassHandler : Knob<ComplexClass>
        {
            public ComplexClassHandler(EventNumber number, Archetypical.Software.Spigot.Spigot spigot, ILogger<Knob<ComplexClass>> logger) : base(spigot, logger)
            {
                //stuff
            }

            protected override void HandleMessage(EventArrived<ComplexClass> message)
            {
                //handle
            }
        }
```
Then registering that Knob
```csharp
                services
                .AddLogging()
                .AddSpigot(config)
                .AddKnob<ComplexClassHandler, ComplexClass>()
```

Sending is just as simple:

```csharp

            var complexSender = provider.GetService<MessageSender<ComplexClass>>();
                
            complexSender.Send(new ComplexClass(testNumber));

```

For more details, check out the [full documentation](docs)




## Did you know: 

### Spigot is [CloudEvents](https://cloudevents.io) Compatible
<img src='https://cloudevents.io/img/logos/cloudevents-icon-color.png' width='250px'>

### Spigot can run on all of these providers:


|Stream|Description|||
|---|---|---|---|
| Archetypical.Software.Spigot.Streams.AWS  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.AWS.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.AWS/)   | [![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.AWS.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.AWS/)
| Archetypical.Software.Spigot.Streams.Azure  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.Azure.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.Azure/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.Azure.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.Azure/)
| Archetypical.Software.Spigot.Streams.GoogleCloud  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.GoogleCloud.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.GoogleCloud/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.GoogleCloud.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.GoogleCloud/)
| Archetypical.Software.Spigot.Streams.RabbitMQ  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.RabbitMQ.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.RabbitMQ/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.RabbitMQ.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.RabbitMQ/)
| Archetypical.Software.Spigot.Streams.Redis  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.Redis.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.Redis/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.Redis.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.Redis/)
| Archetypical.Software.Spigot.Streams.ZeroMQ  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.ZeroMQ.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.ZeroMQ/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.ZeroMQ.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.ZeroMQ/)
| Archetypical.Software.Spigot.Streams.Kafka  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.Kafka.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.Kafka/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.Kafka.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.Kafka/)
| Archetypical.Software.Spigot.Streams.KubeMQ  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Streams.KubeMQ.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.KubeMQ/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Streams.KubeMQ.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Streams.KubeMQ/)


### Spigot supports these serialization formats:

|Stream|Description|||
|---|---|---|---|
| Archetypical.Software.Spigot.Serializers.Json  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Serializers.Json.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Serializers.Json/)   | [![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Serializers.Json.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Serializers.Json/)
| Archetypical.Software.Spigot.Serializers.MsgPack  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Serializers.MsgPack.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Serializers.MsgPack/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Serializers.MsgPack.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Serializers.MsgPack/)
| Archetypical.Software.Spigot.Serializers.Protobuf  |   |[![Nuget](https://img.shields.io/nuget/v/Archetypical.Software.Spigot.Serializers.Protobuf.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Serializers.Protobuf/)   |[![Nuget](https://img.shields.io/nuget/dt/Archetypical.Software.Spigot.Serializers.Protobuf.svg?style=for-the-badge)](https://www.nuget.org/packages/Archetypical.Software.Spigot.Serializers.Protobuf/)


### Check out some hands-on [Demos](https://github.com/ArchetypicalSoftware/Spigot.Demo/tree/main) 