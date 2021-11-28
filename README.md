# RabbitMQ-Example-Service
Creates nonsense "messages" and send them in a silly loop, for science.

## Project consists of three workers:
### FileHandler
Worker task. Uses the <b>FileService.cs</b> ScrapeIncommingSimpleMessages function to scrape the incoming folder for SimpleMessage files. </br>
Publishes findings back onto the Queue. Method stores a physical copy in the processed-folder, alters the body of the message slightly and gives it a new number and sender.</br>
</br>
### RabbitPublishWorker
Runs MQPublisher.Publish every 100ms.</br>
MQPublisher.Publish will create a new SimpleMessage and add it to the queue if the function is ran without arguments and the max amount of messages aren't reached.</br>
Will retry the publish a configurable amount of times (defaults to 3) onto the configurable Queue name on the configurable Host</br>
</br>
### RabbitReadWorker
Runs MQReceiver.Receive every 100ms.</br>
MQReceiver.Receive declares the queue it wants to receive from, it's durability and starts consuming messages. </br>
Uses FileService to write incomming messages onto to the Incoming folder.</br>

## Configurability
* RabbitMQ Host 
  * <i>Default: "localhost"</i>
* Number of files 
  * <i>Default: "1000"</i>
* Number of retries 
  * <i>Default: "3"</i>
* Queue Name 
  * <i>Default: "RabbitMQTestService_SimpleMessage"</i>
* Workspace base path 
  * <i>Default: "F:\\\\RabbitMQ\\\\Workspace\\\\"</i>
</br>
</i>See appsettings.json for configurability.</i>

## Disclaimer:
A RabbitMQ Server must be set up as well to test this service.

Please see https://www.rabbitmq.com/download.html for more information on service install. </br>
<i>Default install will give you a host on "localhost" address</i>
