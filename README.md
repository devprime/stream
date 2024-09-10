# Stream
[Devprime](https://devprime.io) is a platform that accelerates software developer productivity, helping to save around 70% on backend development costs. This is achieved using AI, modern software architecture design, intelligent components, distributed systems practices, and code implementation accelerators, enabling the development of the first microservice in 30 minutes.

This repository contains examples of using resources related to the Devprime Stream Adapter (RabbitMQ, Kafka, Azure, AWS, Google, AMQP, MQTT, FIFO, Queues, Topics, Pub/Sub). For additional information, please refer to the [documentation](https://docs.devprime.io).

# Available examples using the Devprime Stream Adapter
- [Native implementations of RabbitMQ](https://docs.devprime.io/settings/stream/rabbitmq/using-native-rabbitmq-methods/)


### Checklist and preperation of the initial environment:
- Open an account on the [Devprime platform](https://devprime.io) and purchase a [Developer/Enterprise license](https://devprime.io/pricing).
- Install an updated version of .NET (Linux, macOS, and Windows)
- Install and/or update Visual Studio Code and/or Visual Studio and/or Rider .
- Install and/or update docker (For Windows, use WSL2).
- Initialize the [MongoDB and RabbitMQ containers on docker](https://docs.devprime.io/quick-start/docker/introduction/) and [add the 'orderevents' queue in RabbitMQ](https://docs.devprime.io/quick-start/docker/using-rabbitmq/).
- Install and activate the latest version of the [Devprime CLI](https://docs.devprime.io/cli/).
- Create a folder for your projects and set read and write permissions.
- See the article "[Creating the First Microservice](https://docs.devprime.io/quick-start/creating-the-first-microservice/)" to explore getting started with Devprime.


### Example ready with all the steps of the article
This article includes a complete project that demonstrates the feature discussed. You can choose to download it by following the steps below or simply proceed to the next items.

1) Make a clone in the repo.
`git clone https://github.com/devprime/stream.git`
2) Enter the folder .
`cd stream`
3) Upgrade your Devprime license and Stack.
`dp stack`
4) Navigate to the folder related to the example of your interest.
5) Update the MongoDB/RabbitMQ settings in the src/App/appsettings.json file.


### LEGAL INFORMATION
This source code is the property of Devprime LLC. All rights reserved.