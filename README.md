# RabbitMQ-Example-Service
Creates nonsense "messages" and send them in a silly loop, for science.

## Project consists of three workers:
### FileHandler
Worker task.Uses the <b>FileService.cs</b> ScrapeIncommingSimpleMessages funciton to scrape the incoming folder for SimpleMessage files. 
Publishes findings back onto the Queue. Method stores a physical copy in the processed-folder, alters the body of the message slightly and gives it a new number and sender.

### RabbitPublishWorker
Runs MQPublisher.Publish every 100ms.
MQPublisher.Publish will create a new SimpleMessage and add it to the queue if the function is ran without arguments and the max amount of messages aren't reached.
Will retry the publish a configurable amount of times (default is 3) onto the configurable Queue name on the configurable Host

### RabbitReadWorker
Runs MQReceiver.Receive every 100ms.
MQReceiver.Receive declares the queue it wants to receive from, it's durability and starts consuming messages. 
Uses FileService to write incomming messages onto to the Incoming folder.

## Configurability
* Number of files
* Number of retries
* RabbitMQ Host
* Queue Name
* Workspace base path
</br>
</i>See appsettings.json for configurability.</i>

## Disclaimer:
A RabbitMQ Server must be set up as well to test this service.

Please see https://www.rabbitmq.com/download.html for more information on service install. </br>
<i>Default install will give you a host on "localhost" address</i>
