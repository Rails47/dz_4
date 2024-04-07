using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApp13
{
    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Manufacturer { get; set; }

        public Product(string name, double price, string manufacturer)
        {
            Name = name;
            Price = price;
            Manufacturer = manufacturer;
        }
    }

    class Server
    {
        private TcpListener listener;

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                Product product = new Product("ExampleProduct", 10.0, "ExampleManufacturer");
                SendObject(client.GetStream(), product);

                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }

        private void SendObject(NetworkStream stream, object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        public void Stop()
        {
            listener.Stop();
        }
    }

    class Client
    {
        public void Connect(string serverIp, int serverPort)
        {
            try
            {
                TcpClient client = new TcpClient(serverIp, serverPort);
                Console.WriteLine("Connected to server.");
                
                Product product = ReceiveObject(client.GetStream());
                Console.WriteLine($"Received product: {product.Name}, Price: {product.Price}, Manufacturer: {product.Manufacturer}");

                client.Close();
                Console.WriteLine("Disconnected from server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private Product ReceiveObject(NetworkStream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (Product)formatter.Deserialize(stream);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(12345);
            server.Start();

            Client client = new Client();
            client.Connect("192.168.100.3", 12345);
        }
    }
}
