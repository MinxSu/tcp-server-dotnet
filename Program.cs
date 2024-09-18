using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpService
{
    static async Task Main(string[] args)
    {
        int port = 8888;
        TcpListener listener = new TcpListener(IPAddress.Any, port);

        try
        {
            listener.Start();
            Console.WriteLine($"TCP Server Start:{port}");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            listener.Stop();
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        Console.WriteLine($"client connected: {client.Client.RemoteEndPoint}");

        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received Message: {message}");

                byte[] response = Encoding.UTF8.GetBytes($"Server Received Message: {message}");
                await stream.WriteAsync(response);
            }
        }

        Console.WriteLine($"disconnected: {client.Client.RemoteEndPoint}");
        client.Close();
    }
}