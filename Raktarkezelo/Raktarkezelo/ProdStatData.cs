using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktarkezelo
{
    public class ProdStatData
    {
        public string cikkszam { get; set; }
        public int darabszam { get; set; }
        public string honnan { get; set; }
        public string hova { get; set; }
        public string user { get; set; }
        public DateTime indul { get; set; }
        public DateTime erkezik { get; set; }
        public string statusz
        {
            get
            {
                return DateTime.Now < erkezik ? "Szállítás alatt" : "Kiszállítva";
            }
        }

        public ProdStatData()
        {
            indul = DateTime.Now;
            erkezik = indul.AddDays(3);
        }
    }
}