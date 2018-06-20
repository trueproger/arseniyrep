using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Listen();
            Console.ReadKey();
        }

        private static async Task Listen()
        {
            HttpListener listener = new HttpListener();

            string uri = @"http://10.1.0.69/";

            listener.Prefixes.Add(uri);
            listener.Start();

            Console.WriteLine("Ожидание подключений...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse responce = context.Response;
                Stream body = request.InputStream;
                StreamReader sr = new StreamReader(body, Encoding.UTF8);
                try
                {
                    responce.StatusCode = 200;
                    
                    string str = sr.ReadToEnd();
                    File.WriteAllText(@"E:\text2.txt", str, Encoding.UTF8);//для просмотра приходящиго Json

                    // BasicHttpBinding_IEmergencyCard UPDCase = new BasicHttpBinding_IEmergencyCard();
                    //  UPDCase.UpdateCase(myPars(str));
                    Console.WriteLine("ok");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                finally
                {
                    responce.Close();
                    body.Close();
                    sr.Close();
                }
            }

            // останавливаем прослушивание подключений
            
            listener.Stop();
            Console.WriteLine("Обработка подключений завершена");
            Console.ReadKey();
        }

        public static EmergencyCaseInfo myPars(string str) {

            EmergencyCaseInfo res =new EmergencyCaseInfo();

            dynamic pars = JsonConvert.DeserializeObject(str);

           

            res.externalCardId = pars.Card.Id; //Id в системе ISS
            res.caller.firstName = pars.Ier.FullName.LastName; // фамилия
            res.caller.lastName = pars.Ier.FullName.FirstName; // имя
            res.caller.midleName = pars.Ier.FullName.MiddleName;//отчество
            res.caller.phoneNumber = pars.Ier.CgPn;// номер
            res.info = pars.Card.CommonData.TypeStr + " " + pars.Card.CommonData.IsDanger + " " + pars.Card.CommonData.Description; //информация о происшествии
            res.location.entrance = pars.Card.Location.Address.Porch;//подьезд
            res.location.highway.km = pars.Card.Location.Address.DistanceInKm; //километр трассы
            res.location.highway.name = pars.Card.Location.Address.Road;//название трассы
            res.location.house = pars.Card.Location.Address.HouseNumber; // номер дома 
            string buf = pars.Card.Location.Coordinates;// координаты
            string buff= buf.Replace('.',',');
            string[] split = buff.Split(' ');
            double x = double.Parse(split[0]);
            long xx = Convert.ToInt64(x * 3600000);
            res.location.latitude = xx;//широта
            x = double.Parse(split[1]);
            xx = Convert.ToInt64(x * 3600000);
            res.location.longitude = xx;//долгота
            res.location.locality = pars.Card.Location.Address.City;//город
            res.location.municipality = pars.Card.Location.Address.District;//район
            res.location.street = pars.Card.Location.Address.StreetShort + " " + pars.Card.Location.Address.Street;//улица 
            return res;

        }

    }
    /*
    public class myClass {
       

    }

    public class Card { }

    public class Ier { }

    public class Video { }

    public class Location { }

    public class CommonData { }

    public class FullName { }

    public class Address { }
    */
    

}

