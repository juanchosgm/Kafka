# Kafka with .Net
## Configuration to start
* Download [Kafka](https://kafka.apache.org/downloads)
* Create a folder where the extracted files will be located
* Once the files are extracted run this command on separate Command Prompt
  * `.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties`
  * `.\bin\windows\kafka-server-start.bat .\config\server.properties`
* Create topics
  * `.\bin\windows\kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 3 --topic SubmittedOrders`
  * `.\bin\windows\kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 3 --topic ValidatedOrders`
  * `.\bin\windows\kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 3 --topic ProcessedOrders`
  * `.\bin\windows\kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 3 --topic ReportedOrders`
* Verified if every topic was created
  * `.\bin\windows\kafka-topics.bat --list --zookeeper localhost:2181`

### Create a producer and send messages
If you wanna test the things easly without any framework help, you can run this command in order to create a producer and consumer just to see how the communication is going on
* Producer
  * `.\bin\windows\kafka-console-producer.bat --broker-list localhost:9092 --topic {TopicCreated}`
* Consumer
  * `.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic {TopicCreated} --from-beginning`
