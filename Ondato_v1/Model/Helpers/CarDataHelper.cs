using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ondato.Model.Helpers
{
    public class CarDataHelper
    {
        public FileInfo _file = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Data.json");
        private readonly IConfiguration Configuration;
        public List<CarsByMakerWithExp> data = new List<CarsByMakerWithExp>();
        public CarDataHelper(IConfiguration iConfig)
        {
            Configuration = iConfig;
        }

        public string AddNew(CarsByMaker makerCars, int? daysValid)
        {
            var Item = new CarsByMakerWithExp
            {
                Maker = makerCars.Maker.ToUpper(),
                Cars = makerCars.Cars,
                ExpirationDate = DateTime.Now.AddDays(daysValid ?? Convert.ToInt32(Configuration["CarsApiConst:Expires"])).ToString(),
                DaysToExtend = daysValid ?? Convert.ToInt32(Configuration["CarsApiConst:Expires"])
            };
            InsertToStorage(Item);
            return $"{Item.Maker} added. Valid until {Item.ExpirationDate}.";
        }

        public string Update(CarsByMaker makerCars, int? daysValid)
        {
            var ManufCars = data.Where(m => m.Maker.ToUpper() == makerCars.Maker.ToUpper()).First();
            var NewCarsList = makerCars.Cars;
            var a = ManufCars.Cars.Where(x => !NewCarsList.Any(o => CompareCar(o, x)));
            NewCarsList.AddRange(a);

            data.Remove(ManufCars);
            ManufCars.Cars = NewCarsList;
            ManufCars.ExpirationDate = DateTime.Now.AddDays(daysValid ?? ManufCars.DaysToExtend).ToString();
            ManufCars.DaysToExtend = daysValid ?? ManufCars.DaysToExtend;
            data.Add(ManufCars);

            File.WriteAllText(_file.FullName, JsonConvert.SerializeObject(data));
            return $"{ManufCars.Maker} updated. Valid until {ManufCars.ExpirationDate}.";

        }
        public string DeleteByKey(string maker)
        {
            if (CheckExistence(maker))
            {
                data.Remove(data.Where(m => m.Maker.ToUpper() == maker.ToUpper()).First());
                File.WriteAllText(_file.FullName, JsonConvert.SerializeObject(data));
                return $"{maker} deleted.";
            }
            else
            {
                return $"No {maker.ToUpper()} data found";
            }
        }
        public void DeleteExpired()
        {
            if (_file.Exists)
            {
                data = JsonConvert.DeserializeObject<List<CarsByMakerWithExp>>(File.ReadAllText(_file.FullName));
            }
            var ManufToDelete = data.Where(m => DateTime.Parse(m.ExpirationDate) < DateTime.Now).ToList();
            foreach (var item in ManufToDelete)
            {
                data.Remove(item);
            }
            File.WriteAllText(_file.FullName, JsonConvert.SerializeObject(data));
        }
        public string GetByKey(string maker)
        {
            if (CheckExistence(maker))
            {
                var ManufData = data.Where(m => m.Maker.ToUpper() == maker.ToUpper()).First();
                ManufData.ExpirationDate = DateTime.Now.AddDays(ManufData.DaysToExtend).ToString();
                data.Remove(ManufData);
                InsertToStorage(ManufData);
                return JsonConvert.SerializeObject(data.Where(m => m.Maker.ToUpper() == maker.ToUpper()).ToList());
            }
            else
            {
                return $"No {maker.ToUpper()} data found";
            }
        }

        public void InsertToStorage(CarsByMakerWithExp makerCars)
        {
            data.Add(makerCars);
            File.WriteAllText(_file.FullName, JsonConvert.SerializeObject(data));
        }

        public bool CheckExistence(string maker)
        {
            bool Resp = false;
            if (_file.Exists)
            {
                data = JsonConvert.DeserializeObject<List<CarsByMakerWithExp>>(File.ReadAllText(_file.FullName));
                Resp = data.Where(mk => mk.Maker.ToLower() == maker.ToLower()).Any();
            }
            return Resp;
        }

        public bool CompareCar(Car oCar, Car nCar)
        {
            if (oCar.Engine == nCar.Engine && oCar.Model == nCar.Model && oCar.FuelType == nCar.FuelType && oCar.Year == nCar.Year)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
