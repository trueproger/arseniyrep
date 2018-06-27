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
using AutoMapper;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Listen();
            Console.ReadKey();
        }

        private static async Task Listen()
        {
            var listener = new HttpListener();

            listener.Prefixes.Add(@"http://10.1.0.69/");
            listener.Start();

            Console.WriteLine("Ожидание подключений...");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse responce = context.Response;
                Stream body = request.InputStream;
                using (var sr = new StreamReader(body, Encoding.UTF8))
                    try
                    {

                        responce.StatusCode = Convert.ToInt32(HttpStatusCode.OK);
                        string str = await sr.ReadToEndAsync();
                        myObject obj = JsonConvert.DeserializeObject<myObject>(str);

                         BasicHttpBinding_IEmergencyCard UPDCase = new BasicHttpBinding_IEmergencyCard();
                         UPDCase.UpdateCase(Map(obj));

                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                    finally
                    {
                        responce.Close();
                        body.Close();
                        sr.Close();

                        Console.WriteLine("Сигнал обработан");
                    }
            }

        }

        public static EmergencyCaseInfo Map(myObject pars) {

            var res = new EmergencyCaseInfo();

            res.externalCardId = pars.card.id; //Id в системе ISS
            res.caller.firstName = pars.ier.fullName.lastName; // фамилия
            res.caller.lastName = pars.ier.fullName.firstName; // имя
            res.caller.midleName = pars.ier.fullName.middleName;//отчество
            res.caller.phoneNumber = pars.ier.cgPn;// номер
            res.info = pars.card.commonData.typeStr + " " + pars.card.commonData.isDanger + " " + pars.card.commonData.description+" "+ pars.card.commonData.timeIsoStr; //информация о происшествии
            res.location.entrance = pars.card.location.address.porch;//подьезд
            res.location.highway.km = pars.card.location.address.distanceInKm; //километр трассы
            res.location.highway.name = pars.card.location.address.road;//название трассы
            res.location.house = pars.card.location.address.houseNumber; // номер дома 
            string buf = pars.card.location.coordinates;// координаты
            string buff = buf.Replace('.', ',');
            string[] split = buff.Split(' ');
            double x = double.Parse(split[0]);
            long xx = Convert.ToInt64(x * 3600000);
            res.location.latitude = xx;//широта
            x = double.Parse(split[1]);
            xx = Convert.ToInt64(x * 3600000);
            res.location.longitude = xx;//долгота
            res.location.locality = pars.card.location.address.city;//город
            res.location.municipality = pars.card.location.address.district;//район
            res.location.street = pars.card.location.address.streetShort + " " + pars.card.location.address.street;//улица 


            return res;
        }

    }

    public class myObject {
        private Card Card;
        private Ier Ier;
        private Video Video;

        public Card card { get => Card; set => Card = value; }
        public Ier ier { get => Ier; set => Ier = value; }
        public Video video { get => Video; set => Video = value; }

        public myObject() {
            this.Card = new Card();
            this.Ier = new Ier();
            this.Video = new Video();
        }
    }

    public class Card {
        private CommonData CommonData;
        private string Id;
        private Location Location;

        public CommonData commonData { get => CommonData; set => CommonData = value; }
        public string id { get => Id; set => Id = value; }
        public Location location { get => Location; set => Location = value; }

        public Card() {
            this.CommonData = new CommonData();
            this.Location = new Location();
        }
    }

    public class CommonData {
        private string Description;
        private string IsDanger;
        private string TimeIsoStr;
        private string TypeStr;

        public string description { get => Description; set => Description = value; }
        public string isDanger { get => IsDanger; set => IsDanger = value; }
        public string timeIsoStr { get => TimeIsoStr; set => TimeIsoStr = value; }
        public string typeStr { get => TypeStr; set => TypeStr = value; }

        public CommonData() { }
    }

    public class Location {
        private Address Address;
        private string Coordinates;

        public Address address { get => Address; set => Address = value; }
        public string coordinates { get => Coordinates; set => Coordinates = value; }

        public Location() {
            this.Address = new Address();
        }
    }

    public class Address {
        public string City;
        public int DistanceInKm;
        public string District;
        public string HouseNumber;
        public string Porch;
        public string Road;
        public string Street;
        public string StreetShort;

        public string city { get => City; set => City = value; }
        public int distanceInKm { get => DistanceInKm; set => DistanceInKm = value; }
        public string district { get => District; set => District = value; }
        public string houseNumber { get => HouseNumber; set => HouseNumber = value; }
        public string porch { get => Porch; set => Porch = value; }
        public string road { get => Road; set => Road = value; }
        public string street { get => Street; set => Street = value; }
        public string streetShort { get => StreetShort; set => StreetShort = value; }

        public Address() { }
    }

    public class Ier {
        private string CgPn;
        private FullName FullName;

        public string cgPn { get => CgPn; set => CgPn = value; }
        public FullName fullName { get => FullName; set => FullName = value; }

        public Ier() {
         this.FullName = new FullName();
        }
    }

    public class FullName {
        private string FirstName;
        private string LastName;
        private string MiddleName;

        public FullName() { }

        public string firstName { get => FirstName; set => FirstName = value; }
        public string lastName { get => LastName; set => LastName = value; }
        public string middleName { get => MiddleName; set => MiddleName = value; }
    }

    public class Video {
       public string RTSPArchive;
       public string RTSPLive;

        public Video(){}

        public string RTSPArchive1 { get => RTSPArchive; set => RTSPArchive = value; }
        public string RTSPLive1 { get => RTSPLive; set => RTSPLive = value; }
    }

}