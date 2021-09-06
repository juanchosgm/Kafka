# Kafka with .Net
## Configuration to start
The below configuration only apply when you're using Command Prompt:
1. First we need to start a zookeeper tool. Therefore, you need to run this command `~/zookeeper-server-start.bat ~/zookeeper.properties`
2. Right now, we can start a kafka server. Run this command: `~/kafka-server-start.bat ~/server.properties`

Note: You can replace "~" for the specific folder where you have your bat files. Just to exemplify we're going to use ~

### Create topics
You need to run this command `kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic testtopic1`. You can set the name that you want
### Read topics
You need to run this command `kafka-topics.bat --list --zookeeper localhost:2181`
### Create a producer and send messages
You need to run this command `kafka-console-producer.bat --broker-list localhost:9092 --topic testtopic2`
### Create a consumer and receive messages
You need to run this command `kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic testtopic2 --from-beginning`
