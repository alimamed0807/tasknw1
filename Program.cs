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
        while (true)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("               FILE CLIENT                ");
            Console.WriteLine("==========================================");
            Console.WriteLine("1 - Fayl gonder (sekil/ses)");
            Console.WriteLine("0 - Cixis");
            Console.WriteLine("==========================================");
            Console.Write("Secim edin: ");
            string choice = Console.ReadLine();

            if (choice == "0") break;

            if (choice == "1")
            {
                Console.Write("Fayl yolu: ");
                string path = Console.ReadLine().Trim('"');

                if (!File.Exists(path))
                {
                    Console.WriteLine("Fayl tapilmadi!");
                    continue;
                }

                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await socket.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001));

                string name = Path.GetFileName(path);
                await socket.SendAsync(Encoding.UTF8.GetBytes(name), SocketFlags.None);
                await Task.Delay(50);

                byte[] data = File.ReadAllBytes(path);
                await socket.SendAsync(BitConverter.GetBytes((long)data.Length), SocketFlags.None);
                await Task.Delay(50);

                await socket.SendAsync(data, SocketFlags.None);
                Console.WriteLine("Fayl ugurla gonderildi!");

                socket.Dispose();
            }
        }
    }
}