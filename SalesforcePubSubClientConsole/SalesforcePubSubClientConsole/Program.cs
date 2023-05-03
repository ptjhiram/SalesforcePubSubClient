using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Google.Api.Gax.Grpc.GrpcCore;
using Google.Protobuf.Reflection;
using Eventbus.V1;
using Google.Protobuf;
using SolTechnology.Avro;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using ConsoleApp4;

namespace SalesforcePubSubClientConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Creete a JsonFormatter with default settings.
            var jsonFormatter = JsonFormatter.Default;
            // Create a new instance of the class.

            var url = "https://api.pubsub.salesforce.com:443";
            var topicName = "/event/FOStatusChangedEvent";
            //var serviceDescriptor = await GetServiceDescriptorAsync(url);

            // metadata to request
            var metadata = new Metadata
            {
                { "accesstoken", "<your access token>" },
                { "instanceurl", "<your instance url>" },
                { "tenantid", "<your organization id>" }
            };

            var topicResp = GetTopic(metadata, topicName);
            var schemaResp = GetSchema(metadata, topicResp.SchemaId);

            var fetchRequest = new FetchRequest
            {
                TopicName = topicName,
                NumRequested = 100,
                ReplayPreset = ReplayPreset.Latest
            };

            // Assuming you have generated the client stubs and imported them
            using var channel = GrpcChannel.ForAddress(url);
            //create client
            var client = new PubSub.PubSubClient(channel);
            var sub = client.Subscribe(metadata);

            await sub.RequestStream.WriteAsync(fetchRequest);

            Console.WriteLine("Starting to receive messages");
            var readTask = Task.Run(async () =>
            {
                await foreach (var resp in sub.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(resp);
                    if (resp.Events.Any())
                    {
                        foreach (var evt in resp.Events)
                        {
                            var stream = new MemoryStream();
                            evt?.Event?.Payload.WriteTo(stream);
                            Console.WriteLine("Decoded string....");
                            try
                            {
                                var schema = Avro.RecordSchema.Parse(schemaResp.SchemaJson) as Avro.RecordSchema;

                                var avJson = AvroConvert.Avro2Json(evt?.Event?.Payload.ToByteArray(), schemaResp.SchemaJson);
                                Console.WriteLine(avJson);
                                var accountChangeEventHeader = JsonConvert.DeserializeObject<AccountChangeEventHeader>(avJson);
                                var accountChangeEventHeaderDynamic = JsonConvert.DeserializeObject<JToken>(avJson);

                                var bitMapProcessor = new AvroSchemaBitmapProcessor();
                                List<string> values = bitMapProcessor.ProcessBitmap(schema, accountChangeEventHeader?.ChangeEventHeader?.ChangedFields);
                                foreach (var value in values)
                                {
                                    if (value.Contains('.'))
                                    {
                                        Console.WriteLine($"{value}:{accountChangeEventHeaderDynamic.SelectToken(value).Value<string>()}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{value}:{accountChangeEventHeaderDynamic.Value<string>(value)}");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }
                    }

                }
            });

            Console.WriteLine("Starting to send messages");
            Console.WriteLine("Type a message to echo then press enter.");
            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }

                await sub.RequestStream.WriteAsync(fetchRequest);
            }

            Console.WriteLine("Disconnecting");
            await sub.RequestStream.CompleteAsync();
            await readTask;
        }

        private static SchemaInfo GetSchema(Metadata metadata, string schemaId)
        {
            try
            {
                var schemaReq = new SchemaRequest
                {
                    SchemaId = schemaId
                };

                using var channel = GrpcChannel.ForAddress("https://api.pubsub.salesforce.com:443");
                //create client
                var client = new PubSub.PubSubClient(channel);
                var schemaResp = client.GetSchema(schemaReq, metadata);
                return schemaResp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static TopicInfo GetTopic(Metadata metadata, string topicName)
        {
            try
            {
                var topicReq = new TopicRequest
                {
                    TopicName = topicName
                };

                using var channel = GrpcChannel.ForAddress("https://api.pubsub.salesforce.com:443");
                //create client
                var client = new PubSub.PubSubClient(channel);
                var topicResp = client.GetTopic(topicReq, metadata);
                return topicResp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return null;
            }
        }
    }
}