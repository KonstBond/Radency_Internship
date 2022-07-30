using System;
using System.Collections.Generic;
using Temp.Validate;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temp
{
    public class Program
    {
        public static async Task Main()
        {
            Core core = new Core();

            char separator = ';';
            string[] clients = await File.ReadAllLinesAsync(@"C:\Users\admin\Desktop\23.csv");
            var context = new Context(new Name(), separator);

            for (int i = 1; i < clients.Length; i++)
            {
                Client client = new Client();
                for (int j = 0; j < 7; j++) //7 различных атрибутов
                { 
                    clients[i] = context.RewriteInput(clients[i], client);
                    if (clients[i].StartsWith("ERROR"))
                    {
                        Console.WriteLine(clients[i]);
                        context = new Context(new Name(), separator);
                        break;  
                    }      
                }
            }


        }
    }
}


public static class Validator
{
    public static List<Client> Check(string[] input)
    {
        List<Client> clients = new List<Client>();
        long errors = 0;
        for (int i = 0; i < input.Count(); i++)
        {
            Client client = new Client();
            bool isValid = true;

            //Вытаскием дату и удаляем её из начальной строки
            int firstIndexAddress = input[i].IndexOf("\"");
            string address = input[i].Substring(firstIndexAddress, input[i].LastIndexOf("\"") - firstIndexAddress+1);
            input[i] = input[i].Remove(firstIndexAddress, address.Length+3);
            string[] attributes = input[i].Split(',');

            //ЛОГИ!!!
            //Проверка Имени
            if (!(attributes[0].Length > 0))
            {
                errors++;
                isValid = false;
            }
            else
                client.Name = attributes[0];

            //Проверка Фамилии
            if (!(attributes[1].Length > 0))
            {
                errors++;
                isValid = false;
            }
            else
                client.LastName = attributes[1];

            //Проверка адреса
            if (!(address.Length > 0))
            {
                errors++;
                isValid = false;
            }
            else
                client.Address = address;

            //Проверка выплаты
            if (!(attributes[2].Length > 0) && !decimal.TryParse(attributes[2], out _))
            {
                errors++;
                isValid = false;
            }
            else
            {
                decimal payment = Convert.ToDecimal(attributes[2], new CultureInfo("en-US"));
                client.Payment = payment;
            }

            //Проверка даты
            if (!(attributes[3].Length > 0) && !DateTime.TryParse(attributes[3], out _))
            {
                errors++;
                isValid = false;
            }
            else
            {
                attributes[3] = attributes[3].Replace('-', '/');
                DateTime date = DateTime.ParseExact(attributes[3], "", CultureInfo.InvariantCulture, DateTimeStyles.None);
                client.Date = date;
            }

            //Проверка номера
            if (attributes[4].Length > 0 && !Int64.TryParse(attributes[4], out _))
            {
                errors++;
                isValid = false;
            }
            else
            {
                long accNumb = Convert.ToInt32(attributes[4]);
                client.AccountNumber = accNumb;
            }

            //Проверка услуг
            if (attributes[5].Length > 0)
            {
                errors++;
                isValid = false;
            }
            else
                client.Service = attributes[5];

            if(isValid)
                clients.Add(client);
            else
                //LOG
                Console.WriteLine("ERROR");

        }

        return clients;

        
    }
}


public class Client
{
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public decimal Payment { get; set; }
    public DateTime Date { get; set; }
    public long AccountNumber { get; set; }
    public string Service { get; set; }
}
