using System;

namespace Gint.Pipes
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipe = new GintPipe(new PipeOptions { PreferredBufferSegmentSize = 8 });

            var bytesToWrite = "hello world".ToUTF8EncodedByteArray();
            pipe.WriteUnsafe(bytesToWrite);

            var bytesRead = pipe.Read(advanceCursor: false);

            Console.WriteLine(bytesRead.Buffer.ToUTF8String());


            var bytesToWrite3 = "xello world from gint".ToUTF8EncodedByteArray();
            pipe.Write(bytesToWrite3);

            var bytesRead4 = pipe.Read(advanceCursor: false);

            Console.WriteLine(bytesRead4.Buffer.ToUTF8String());


            var bytesToWrite2 = "hello again".ToUTF8EncodedByteArray();
            pipe.Write(bytesToWrite2);

            var bytesRead2 = pipe.Read();

            Console.WriteLine(bytesRead2.Buffer.ToUTF8String());

            var bytesRead3 = pipe.Read(offset: 11, count: 5);

            Console.WriteLine(bytesRead3.Buffer.ToUTF8String());

            Console.ReadLine();
        }
    }

}
