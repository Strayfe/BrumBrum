# BrumBrum

An example of a .NET Core 3.1 console application that can be registered
as a Windows Service or Linux Daemon which can host an internally accessible
API to handle ingress of events, use the MassTransit library to publish those messages
onto the RabbitMQ message broker and simultaneously subscribe to messages on the queue
to process them on a separate thread.

TODO: K6 performance tests required.