using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ondato.Model
{

    public class CarsByMaker
    {
        public string Maker { get; set; }
        public List<Car> Cars { get; set; }

    }
    public class CarsByMakerWithExp : CarsByMaker
    {
        public string ExpirationDate { get; set; }
        public int DaysToExtend { get; set; }
    }
}
