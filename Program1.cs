using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("               FILE SERVER                ");
        Console.WriteLine("==========================================");

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 27001));
        socket.Listen(10);
        Console.WriteLine("Server isleyir...");

        while (true)
        {
            var client = await socket.AcceptAsync();
            _ = Task.Run(async () =>
            {
                var buffer = new byte[1024];

                int len = await client.ReceiveAsync(buffer, SocketFlags.None);
                string fileName = Encoding.UTF8.GetString(buffer, 0, len);

                byte[] sizeBuf = new byte[8];
                await client.ReceiveAsync(sizeBuf, SocketFlags.None);
                long size = BitConverter.ToInt64(sizeBuf, 0);

                byte[] data = new byte[size];
                int received = 0;
                while (received < size)
                {
                    received += await client.ReceiveAsync(new ArraySegment<byte>(data, received, (int)size - received), SocketFlags.None);
                }

                File.WriteAllBytes(fileName, data);
                Console.WriteLine($"Qebul edildi: {fileName}");
                client.Dispose();
            });
        }
    }
}