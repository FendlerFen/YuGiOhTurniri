using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Servisi
{
    public class OgranicenjaServis
    {
        private dynamic Ucitaj()
        {
            string putanja = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "ogranicenja.json"
            );

            string json = File.ReadAllText(putanja);
            return JsonConvert.DeserializeObject(json);
        }

        public int DajMaxBrojTakmicara()
        {
            var obj = Ucitaj();
            return (int)obj.maxBrojTakmicara;
        }

        public int DajMinBrojKarataMain()
        {
            var obj = Ucitaj();
            return (int)obj.minBrojKarataMain;
        }

        public int DajMaxBrojKarataMain()
        {
            var obj = Ucitaj();
            return (int)obj.maxBrojKarataMain;
        }

        public int DajMaxBrojKarataExtra()
        {
            var obj = Ucitaj();
            return (int)obj.maxBrojKarataExtra;
        }

        public int DajMaxBrojKarataSide()
        {
            var obj = Ucitaj();
            return (int)obj.maxBrojKarataSide;
        }

        public int DajMaxBrojIsteKarte()
        {
            var obj = Ucitaj();
            return (int)obj.maxBrojIsteKarte;
        }
    }
}