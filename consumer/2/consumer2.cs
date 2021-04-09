using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using GeneratedSchemas;

namespace Confluent.Kafka.Examples.AvroSpecific
{
    class Program
    {
        static void Main(string[] args)
        {
            var topicName = "Motion";

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "comsumer-group"
            };

            var avroSerializerConfig = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                BufferBytes = 100
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var consumer =
                new ConsumerBuilder<string, MotionEvent>(consumerConfig)
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<MotionEvent>(schemaRegistry).AsSyncOverAsync())
                    .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                    .Build())
            {
            
                CancellationTokenSource cts = new CancellationTokenSource();

                consumer.Subscribe(topicName);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);
                            Console.WriteLine($"Consumed id={consumeResult.Message.Value.id}, timestamp= {consumeResult.Message.Value.timestamp}, camera={consumeResult.Message.Value.camera}, plate={consumeResult.Message.Value.plate}.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }           
        }
    }
}
