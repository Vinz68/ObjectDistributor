# VibeSoft.ObjectDistributor

## Description
This folder contains the ObjectDistributor which distributes <B>message objects</B> to subscribers using the fully qualified assembly name of the message class.

It contains all source code (C#) including unit tests and a simple example program.

The message objects are distributed to all subscribers that have registered a handler for that message type.
It decouples consumers (subscribers) from producers which makes the architecture more modular and easier to extend and maintain.
Distribution of the message objects are done immediately (synchronously) to all subscribers.

The speed and efficiency of the distribution depends on the number of subscribers and the total subscriber-handler processing time.
So keep subscriber handling time short as possible. The distribution order is the same as the order of the subscribers.

<B>Note:</B> the handlers runs in the same thread as where the Distribute command was given. You might need thread synchronization in your handler.


| Interfaces | Description |
----------|--------------
| Subscribe | Subscribes on a message type and registers your handler when that message type is distributed |
| UnSubscribe | Unsubscribe one specific previously subscribed message type. The message handler will be removed.  |
| UnSubScribeAll | Unsubscribes all previously subscribed message types of one specific subscriber Commonly done when object gets deactivated/disposed |
| Distribute | Distribute one specific message type to previously subscribed (0 or more) handlers |


## Usage
The usage is simple, see the ObjectDistributor_Example project.






