# Exercise 5: Particular Software Platform

**Important: Before attempting the exercise, please ensure you have followed [the instructions for preparing your machine](README.md#preparing-your-machine-for-the-workshop) and that you have read [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).**

One of the reasons that monolith systems are seen as something bad is because it's so easy to break a part of the system that wasn't even touched, while adding a new feature or fixing a bug. The reason is that it's hard to see the coupling between all the components.

With a distributed system based on messaging, it is less likely to break a component like that, because of the asynchronous nature and the temporal decoupling. However it can be hard to see what messages are being send or published. And if something fails, discover the reason why it failed and how to solve this.

Within Particular Software we saw the need for visualization of your system and the messages being sent through it. This helps developers tremendously. Monitoring your endpoints is another feature that can greatly add value to your system. For this we developed several tools which we'll experience in this advanced exercise.

## Overview

In the previous exercises, we completed development on several endpoints by sending commands and publishing messages. In this exercises we will have a look at how we can monitor our system and visualize the message flow throughout or system.

Note : Please ensure you followed the instructions for the [advanced exercises](https://github.com/Particular/Workshop.Microservices/blob/master/README.md#preparing-your-machine-for-advanced-exercises).

## Start-up projects

For more info, please see [the instructions for running the exercise solutions](/README.md#running-the-exercise-solutions).

* Divergent.Customers
* Divergent.Customers.API
* Divergent.Finance
* Divergent.Finance.API
* Divergent.Frontend
* Divergent.ITOps
* Divergent.Sales
* Divergent.Sales.API
* Divergent.Shipping
* PaymentProviders

## Business requirements

The *operations department* would like the ability to monitor the system.

- If an endpoint goes down, operations should be able to notice this and take action.
- If an endpoint is unable to process one or more messages, operations should be able to retry those messages.

The *development team* would like to be able to visualize the system.

Instead of having multiple sources of truth related to the above requirements, we should have central storage of the needed information

Luckily for us, Particular Software build exactly this system. It is known as the *Particular Software Platform* and consists of the tools

- ServiceControl for central processing and storage of audit and error messages
- ServiceInsight for visualizing the system
- ServicePulse for monitoring endpoints and retrying failed messages


## Exercise 5.1: Monitor endpoints

In this exercise, we'll check to see if endpoints are up and running.

<u>Note 1:</u> This an advanced exercise so we'll require you to read documentation, of which links will be provided, and work your way through the exercise yourself.

<u>Note 2:</u> You can work with the Visual Studio solution you used for exercise 4. If you haven't finished it, be sure to load up the `after` solution for exercise 4.

### Step 1

In Visual Studio, open the project `Divergent.Customers` and have a look at the endpoint configuration. There should be configuration for forwarding messages to the audit queue and sending poison messages to the error queue.

You can read much more about auditing messages and different ways to configure this [in our documentation](https://docs.particular.net/nservicebus/operations/auditing).

### Step 2

Verify if the queues are created. You can use Windows' Computer Management tool. Press `ctrl` and `x` to open the menu in Windows and select 'Computer Management'. Then find MSMQ and verify if the queues are created under 'Private Queues'. 

Note: The MSMQ MMC snap-in is very limit. Queue Explorer is a great tool which provides more value.

### Step 3

If you've properly set up ServiceControl, it should already be running and have processed messages while running your exercises. Let's have a look at ServicePulse.

Open the browser at http://localhost:9090/

Note: If ServicePulse doesn't seem to be running, or it cannot connect to ServiceControl, you can either verify if the proper ServiceControl instanace is started. Or you can check 'Services' in Windows itself to see if both services are running. By default, all services should start with the name 'Particular' in front of it.

### Step 4

As you can see, ServicePulse is informing us via the list of 'Last 10 events' that it received messages from an endpoint, but it is not monitored yet. We need to set up the monitoring plugin first.

### Step 5

Let's install the Particular Software **heartbeat plugin**. You can find [documentation here](https://docs.particular.net/servicecontrol/plugins/heartbeat). Install this plugin into every project that hosts an endpoint, via the NuGet user interface or via the 'Package Manager Console'.

### Step 6

The **heartbeat plugin** works using a different queue than audit and error messages. You can read in the documentation how to configure NServiceBus and tell it which queue it should send to. 

You can find the name of the queue by accessing the 'ServiceControl Management' tool, which you can find in the Windows Start menu. The name of the instance is also the name of the queue.

Make sure you configure every project that hosts an endpoint. You can easily copy & paste this to every project, as the queue doesn't (and shouldn't) change in every project.

### Step 7

Run the solution and check ServicePulse while it starts.

After a while the 'Last 10 events' should show that `Divergent.Customers` or any other endpoint should have started. After a while it will show that these endpoints should be running the heartbeats plugin.

### Step 8

Turn off the endpoints by stopping debugging in Visual Studio or shutting down the console windows.

Remember that ServiceControl is expecting heartbeat messages to come in. If it won't receive those it will wait a little while longer before immediately reporting an endpoint as being down. But if you wait for half a minute or so, ServicePulse should report the endpoints being down.

After starting up the solution, ServicePulse should report the endpoints are working again.

### Step 9

At the top of the page in ServicePulse, you see a menu with various options. Check 'Failed Messages' if there are any messages that you weren't able to process in the past. Check how they can be group and retried individually or per group.

This is a powerful feature that can be used to retry message. But discuss this with, for example, the operations department. Imagine a system with a high throughput, but to performance maintenance, the database with your business data was brought offline for a couple of minutes. This could mean thousands of messages will end up in the error queue and thus in ServiceControl.

Once the system is up and running again, it has to handle the high throughput again. Should we retry all those messages we could introduce a spike of messages the system might be able to deal with. Resulting in more error messages, which could could introduce the same problem.

### Step 10

We now have a dashboard that can inform us when an endpoint is going down. A few things should be mentioned.

- Operations doesn't want to monitor a dashboard the entire day. Luckily ServiceControl also uses publish/subscribe to notify any subscribers using messages. You can build a special endpoint that subscribes to ServiceControl its integration events and decide how operations should be informed. By email, sms or anything else. Read more about [integration events](https://docs.particular.net/servicecontrol/contracts).
- You might notice several endpoints with the same name. Endpoints send heartbeats by providing a unique host identifier, made up of their endpoint name and a hash of the folder the endpoint is installed in.
  Our exercises all have the same endpoint name, but different folders. Another example is when you deploy endpoints using [Octopus](https://octopus.com/). This will deploy every version in its own folder, with the result that every version will spawn a new monitored endpoint in ServicePulse. You can solve this by [overriding the host identifier](https://docs.particular.net/nservicebus/hosting/override-hostid) yourself.

## Exercise 5.2: Visualizing the system

We've seen how ServicePulse can monitor and inform us of our endpoints. Let's have a look how ServiceInsight visualizes the system.

### Step 1

Open up ServiceInsight from the Windows Start menu.

### Step 2

ServiceInsight connects to ServiceControl via its api to retrieve information about all endpoints and messages. If not connected to ServiceControl, the top-left icon with the tooltip 'Connect Endpoint Explorer to ServiceControl instance' allows you to connect to ServiceControl. The default address should be

`http://localhost:33333/api/`

In the 'ServiceControl Management' tool you can create multiple instances of ServiceControl and provide different ports for each of them.

### Step 3

Once connected, on the left side of ServiceInsight, you should be able to see every endpoint sending audit messages to ServiceControl. On the right you should be able to see various messages in a grid.

Below the grid you should see the flow diagram for the last send message. The currently selected message is visible in the diagram by a large border around the message. Scrolling through the messages in the grid should show the border move from message to message in the diagram.

You can also use this the other way around, by clicking on messages in the diagram to highlight the corresponding message in the grid.

### Step 4

On the right side of the diagram are the properties of each message. They provide various details such as

- The message type, its unique message id and the conversation id.
- Performance details on when the message was sent, how long it took to process it and how long the critical time was, the time from sending it until successfully processing it.
- If the message wasn't successfully processed, it will show the complete stacktrace here. If you continuously find the stacktrace to include way too much information, especially when dealing with async stacktraces, you can use [a sample to clean them up a little](https://docs.particular.net/samples/logging/stack-trace-cleaning/).

### Step 5

At the bottom of the diagram, you can find alternate views, like the sequence diagram.

The saga view is empty because the message might not have entered a saga. Next to that, for this view to be useable we need to install the plugin for saga auditing.

## Exercise 5.3: Enable saga auditing

In this exercise, we'll set up every endpoint to send heartbeats, but especially audit messages to ServiceControl.

### Step 1

You can read about the [Saga Audit plugin on our documentation website](https://docs.particular.net/servicecontrol/plugins/saga-audit).

We only need to install the plugin in the `Divergent.Shipping` project, since it is the only project containing a saga at the moment. If you happen to add sagas to other endpoints as well, don't forget to add the plugin there as well.

Don't forget to configure the endpoint to tell it where to send saga audit messages to.

### Step 2

Start up the solution and create another order using the web interface.

Once messages start arriving in the saga, additional messages will be send to ServiceControl which contain more detailed information on what happened with the saga.

### Step 3

In ServiceInsight both the Flow diagram as the Saga diagram should display additional data.

The Flow diagram will now show which messages initiated, updated or completed the saga.

The Saga diagram will now display all incoming and outgoing messages.

## Exercise 5.4: Errors and saga timeouts

This exercise is for those interested in more features of sagas and the Particular Software platform. They might overlap the advanced exercises of module 03.

### Step 1

Try to throw an error in a handler, which will prevent the saga from progressing any further. With immediate- and delayed retries this might take a while, you can read up on how to configure [immediate retries](https://docs.particular.net/nservicebus/recoverability/configure-immediate-retries) and [delayed retries](https://docs.particular.net/nservicebus/recoverability/configure-delayed-retries) so that they will be disabled.

The result should be that our saga won't receive expected messages and wait forever.

Once error messages arrive in ServiceControl, you should see them appear in ServicePulse and ServiceInsight. You can now retry the messages from either user interface. If you remove throwing the exception, the messages should be processed normally and the saga should complete as well.

### Step 2

Imagine the payment to never return to the saga. Should we wait forever for this message, without knowing why? Perhaps we could ask the business if we could add an additional step in our process to take action upon waiting too long for an endpoint to reply.

Perform the action from step 1 again, throwing an exception. But now send a timeout message from the saga. In this exercise we'll complete the saga. But it is completely depended on the business what we should do. Options could be to send an email to finance to take action, to cancel the order or ask the customer to manually pay the invoice via our website, to just name a few.

Summary of actions to take

- Throw the exception so the payment will never succeed
- Send a timeout message in the saga, with more information about [how to in our documentation](https://docs.particular.net/nservicebus/sagas/timeouts).
- Process the timeout message and complete the saga
- Check ServiceInsight to see how this is visualized

### Step 3

Discuss the following items

- Did you ever have a business process where you did something equal to the example used above? Like waiting forever on an action to happen. You should be able to come up with multiple examples, because almost every larger business process has this.
- If you could come up with examples, how did you deal with them? And if you did deal with them, was it easier or harder to fix then with a timeout message?
- If you have or haven't done something like just discussed, did you discuss it with the business? Did they provide solutions? What option was chosen and how difficult was it to implement?

We hope you get an idea on how powerful sagas can be to orchestrate business processes.

If you would like to discuss more, we would love to as well! Don't hesitate to contact us at support@particular.net.