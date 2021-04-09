using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using GeneratedSchemas;

namespace Genetec.Events
{
    class ProducerV1
    {
        static async Task Main(string[] args)
        {
            Avro.Generic.GenericWriter<string> h = new Avro.Generic.GenericWriter<string>();


            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };


            //AvroContainer cont = new AvroContainer();
            {
                //Console.WriteLine($"Starting producer {producer.Name} producing on {topicName} version {nameof(MotionEvent)}. Ctl-C to abort.");

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
