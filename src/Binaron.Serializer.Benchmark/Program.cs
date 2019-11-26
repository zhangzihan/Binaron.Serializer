﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Binaron.Serializer;
using BinSerializerTest.DtoSamples;
using Newtonsoft.Json;

namespace BinSerializerTest
{
    public static class Program
    {
        public class BinaronVsJson
        {
            private readonly Book book = Book.Create();

            [GlobalSetup]
            public async ValueTask Setup()
            {
                // warm-up
                for (var i = 0; i < 250; i++)
                {
                    NewtonsoftJsonTest_Validate(book);
                    await BinaronTest_Validate(book);
                }
            }

            [Benchmark]
            public void Json_Serialize()
            {
                NewtonsoftJsonTest_Serialize(book);
            }

            [Benchmark]
            public async ValueTask Binaron_Serialize()
            {
                await BinaronTest_Serialize(book);
            }

            [Benchmark]
            public void Json_Deserialize()
            {
                NewtonsoftJsonTest_Deserialize(book);
            }

            [Benchmark]
            public async ValueTask Binaron_Deserialize()
            {
                await BinaronTest_Deserialize(book);
            }
        }

        public static void Main()
        {
            BenchmarkRunner.Run<BinaronVsJson>();
        }

        private static async ValueTask BinaronTest_Serialize(Book input)
        {
            await using var stream = new MemoryStream();
            for (var i = 0; i < 200; i++)
            {
                await BinaronConvert.Serialize(input, stream, new SerializerOptions {SkipNullValues = true});
                stream.Position = 0;
            }
        }

        private static async ValueTask BinaronTest_Deserialize(Book input)
        {
            await using var stream = new MemoryStream();
            await BinaronConvert.Serialize(input, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;

            for (var i = 0; i < 200; i++)
            {
                var _ = BinaronConvert.Deserialize<Book>(stream);
                stream.Position = 0;
            }
        }

        private static async ValueTask BinaronTest_Validate(Book input)
        {
            await using var stream = new MemoryStream();
            await BinaronConvert.Serialize(input, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;

            var book = BinaronConvert.Deserialize<Book>(stream);

            Trace.Assert(book.Changes.Count == 3);

            Trace.Assert((string) book.Metadata["Matrix"] == "Neo");

            foreach (var page in book.Pages)
            foreach (var note in page.Notes)
            {
                Trace.Assert(note.Headnote.Note != null);
            }

            Trace.Assert(book.Genres[0] == Genre.Action);
            Trace.Assert(book.Genres[1] == Genre.Comedy);
        }

        private static void NewtonsoftJsonTest_Serialize(Book input)
        {
            using var stream = new MemoryStream();
            for (var i = 0; i < 200; i++)
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);
                {
                    var ser = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
                    ser.Serialize(jsonWriter, input);
                    jsonWriter.Flush();
                }
                stream.Position = 0;
            }
        }

        private static void NewtonsoftJsonTest_Deserialize(Book input)
        {
            using var stream = new MemoryStream();
            {
                using var writer = new StreamWriter(stream, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);
                {
                    var ser = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
                    ser.Serialize(jsonWriter, input);
                    jsonWriter.Flush();
                }
                stream.Position = 0;
            }
            for (var i = 0; i < 200; i++)
            {
                using var reader = new StreamReader(stream, leaveOpen: true);
                using var jsonReader = new JsonTextReader(reader);
                var ser = new JsonSerializer();
                var _ = ser.Deserialize<Book>(jsonReader);
                stream.Position = 0;
            }
        }

        private static void NewtonsoftJsonTest_Validate(Book input)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(writer);
            {
                var ser = new JsonSerializer {NullValueHandling = NullValueHandling.Ignore};
                ser.Serialize(jsonWriter, input);
                jsonWriter.Flush();
            }
            stream.Position = 0;
            {
                using var reader = new StreamReader(stream, leaveOpen: true);
                using var jsonReader = new JsonTextReader(reader);
                var ser = new JsonSerializer();
                var book = ser.Deserialize<Book>(jsonReader);

                Trace.Assert(book.Changes.Count == 3);

                Trace.Assert((string) book.Metadata["Matrix"] == "Neo");

                foreach (var page in book.Pages)
                foreach (var note in page.Notes)
                {
                    Trace.Assert(note.Headnote.Note != null);
                }

                Trace.Assert(book.Genres[0] == Genre.Action);
                Trace.Assert(book.Genres[1] == Genre.Comedy);
            }
        }
    }
}
