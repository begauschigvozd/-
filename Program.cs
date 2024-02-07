using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

IPAddress localAddress = IPAddress.Parse("127.0.0.1");
Console.Write("Введите свое имя: ");
string? username = Console.ReadLine();
Console.Write("Введите порт для приема сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var localPort)) return;
Console.Write("Введите порт для отправки сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
Console.WriteLine();

Task.Run(ReceiveMessageAsync);
await SendMessageAsync();

async Task SendMessageAsync()
{
    using Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
    while (true)
    {
        var message = Console.ReadLine(); 
        if (string.IsNullOrWhiteSpace(message)) break;
        message = $"{username}: {message}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        await sender.SendToAsync(data, new IPEndPoint(localAddress, remotePort));
    }
}
async Task ReceiveMessageAsync()
{
    using UdpClient receiver = new UdpClient(localPort);
    while (true)
    {
        var result = await receiver.ReceiveAsync();
        var message = Encoding.UTF8.GetString(result.Buffer);
        Print(message);
    }
}
void Print(string message)
{
    if (OperatingSystem.IsWindows())   
    {
        var position = Console.GetCursorPosition(); 
        int left = position.Left;   
        int top = position.Top;   
        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
        Console.SetCursorPosition(0, top);
        Console.WriteLine(message);
        Console.SetCursorPosition(left, top + 1);
    }
    else Console.WriteLine(message);
}