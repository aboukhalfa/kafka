using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using GeneratedSchemas;

namespace Genetec.Events
{
    class ProducerV1
    {
        static async Task Main(string[] args)
        {
            var topicName = "Motion";

            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };

            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            var avroSerializerConfig = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                BufferBytes = 100
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig))
            using (var producer =
                new ProducerBuilder<string, MotionEvent>(config)
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry, avroSerializerConfig))
                    .SetValueSerializer(new AvroSerializer<MotionEvent>(schemaRegistry, avroSerializerConfig))
                    .Build())
            {
                Console.WriteLine($"Starting producer {producer.Name} producing on {topicName} version {nameof(MotionEvent)}. Ctl-C to abort.");

                var i = 0;
                while (true)
                {
                    var now = DateTime.UtcNow;
                    var key = $"1-{now.ToString("ddHHmmss")}";
                    var motionEvent = new MotionEvent { 
                        id = key,
                        timestamp = now,
                        camera = "GUSTAV 1M",
                    };

                    try
                    {
                        var deliveryReport = await producer.ProduceAsync(
                            topicName, new Message<string, MotionEvent> 
                            { 
                                Key = key, 
                                Value = motionEvent 
                            });

                        Console.WriteLine($"Prod#1 id={motionEvent.id} cam={motionEvent.camera}");
                        
                        i++;
                    }
                    catch (ProduceException<string, MotionEvent> e)
                    {
                        Console.WriteLine($"Failed to produce event {i} {e.Message} [{e.Error.Code}]");
                    }

                    await Task.Delay(1000);                 
                }
            }

        }
    }
}
