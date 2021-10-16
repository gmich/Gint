using Gint.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Gint.Tests
{
    public class PipeTests
    {
        private readonly ITestOutputHelper output;

        public PipeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private IPipe GetPipe(PipeOptions options = null) => new GintPipe();

        [Theory]
        [InlineData("hello world!")]
        [InlineData("αβγδέζηθ")]
        public void Pipe_Simple_Write_Read(string text)
        {
            var pipe = GetPipe();

            pipe.Write(text.ToUTF8EncodedByteArray());

            var bytesRead = pipe.Read();
            var readResult = bytesRead.Buffer.ToUTF8String();

            Assert.Equal(text, readResult);
        }

        [Theory]
        [InlineData("hello world", "hello again, world")]
        public void Pipe_Write_Read_AdvanceCursor(string first, string second)
        {
            var pipe = GetPipe();

            void WriteAndRead(string text)
            {
                pipe.Write(text.ToUTF8EncodedByteArray());

                var bytesRead = pipe.Read(advanceCursor: true);
                var readResult = bytesRead.Buffer.ToUTF8String();

                Assert.Equal(text, readResult);
            }

            WriteAndRead(first);
            WriteAndRead(second);
        }

        [Theory]
        [InlineData("hello world", "hello again, world")]
        public void Pipe_Write_Read_Without_AdvancingCursor(string first, string second)
        {
            var pipe = GetPipe();

            void WriteAndRead(string texttoWrite, string shouldbe)
            {
                pipe.Write(texttoWrite.ToUTF8EncodedByteArray());

                var bytesRead = pipe.Read(advanceCursor: false);
                var readResult = bytesRead.Buffer.ToUTF8String();

                Assert.Equal(shouldbe, readResult);
            }

            WriteAndRead(first, first);
            WriteAndRead(second, first + second);
        }

        [Theory]
        [InlineData("small", "really large text that will cause a buffer extension...")]
        public void Pipe_Write_With_Advance_And_Buffer_Resize(string first, string second)
        {
            var pipe = GetPipe(new PipeOptions { PreferredBufferSegmentSize = 16 });

            void WriteAndRead(string text)
            {
                pipe.Write(text.ToUTF8EncodedByteArray());

                var bytesRead = pipe.Read(advanceCursor: true);
                var readResult = bytesRead.Buffer.ToUTF8String();

                Assert.Equal(text, readResult);
            }

            WriteAndRead(first);
            WriteAndRead(second);
        }

        [Theory]
        [InlineData("small", "really large text that will cause a buffer extension...")]
        public void Pipe_Write_Without_Advance_And_Buffer_Resize(string first, string second)
        {
            var pipe = GetPipe(new PipeOptions { PreferredBufferSegmentSize = 16 });

            void WriteAndRead(string textToWrite, string shouldbe)
            {
                pipe.Write(textToWrite.ToUTF8EncodedByteArray());

                var bytesRead = pipe.Read(advanceCursor: false);
                var readResult = bytesRead.Buffer.ToUTF8String();

                Assert.Equal(shouldbe, readResult);
            }

            WriteAndRead(first, first);
            WriteAndRead(second, first + second);
        }

        [Theory]
        [InlineData("hello world")]
        public void Pipe_Write_And_Read_From_Stream(string text)
        {
            var pipe = GetPipe();

            var bytesToWrite = text.ToUTF8EncodedByteArray();
            pipe.Write(bytesToWrite);

            var stream = pipe.AsStream();
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var streamString = buffer.ToUTF8String();

            Assert.Equal(text, streamString);
        }

        [Theory]
        [InlineData("hello", " world")]
        public void Pipe_Simple_Write_And_Seek(string first, string second)
        {
            var pipe = GetPipe();

            pipe.Write(first.ToUTF8EncodedByteArray());
            pipe.Write(second.ToUTF8EncodedByteArray());

            var firstRes = pipe.Read(0, first.Length);
            var firstString = firstRes.Buffer.ToUTF8String();
            Assert.Equal(first, firstString);

            var secondRes = pipe.Read(first.Length, second.Length);
            var secondString = secondRes.Buffer.ToUTF8String();
            Assert.Equal(second, secondString);
        }

        [Theory]
        [InlineData("small", "really large text that will cause a buffer extension...")]
        public void Pipe_Write_And_Seek_With_Buffer_resize(string first, string second)
        {
            var pipe = GetPipe(new PipeOptions { PreferredBufferSegmentSize = 16 });

            pipe.Write(first.ToUTF8EncodedByteArray());
            pipe.Write(second.ToUTF8EncodedByteArray());

            var firstRes = pipe.Read(0, first.Length);
            var firstString = firstRes.Buffer.ToUTF8String();
            Assert.Equal(first, firstString);

            var secondRes = pipe.Read(first.Length, second.Length);
            var secondString = secondRes.Buffer.ToUTF8String();
            Assert.Equal(second, secondString);
        }


        [Theory]
        [InlineData("hello", " world")]
        public void Pipe_Read_To_End_And_Seek(string first, string second)
        {
            var pipe = GetPipe();

            pipe.Write(first.ToUTF8EncodedByteArray());
            pipe.Write(second.ToUTF8EncodedByteArray());

            var firstRes = pipe.Read();
            var firstString = firstRes.Buffer.ToUTF8String();
            Assert.Equal($"{first}{second}", firstString);

            var secondRes = pipe.Read(first.Length, second.Length);
            var secondString = secondRes.Buffer.ToUTF8String();
            Assert.Equal(second, secondString);
        }
    }
}
