using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BilletApp

{
    public class Billet
    {
        public string ID { get; set; }
        public string Kalite { get; set; }
        public double Temperature { get; set; }
        public double Speed { get; set; }
        public double RMspeed { get; set; }
        public double FMspeed { get; set; }
        public double SermeKafaSıcaklığı { get; set; }
        public string Acıklama { get; set; }
        public double FBLB { get; set; }
        public double WZ { get; set; }
        public double H8Temp { get; set; }
        public double CV2 { get; set; }
        public double CV4 { get; set; }

        // Yapıcı metod: Parametrelerle Billet nesnesini oluşturur
        public Billet(string id, string quality, double temperature, double speed, double rMspeed, double fMspeed, double sermeKafaSıcaklığı, string description, double fblb, double wz, double h8Temp, double cv2, double cv4)
        {
            ID = id;
            Kalite = quality;
            Temperature = temperature;
            Speed = speed;
          
           
            SermeKafaSıcaklığı = sermeKafaSıcaklığı;
            Acıklama = description;
            FBLB = fblb;
            WZ = wz;
            H8Temp = h8Temp;
            CV2 = cv2;
            CV4 = cv4;
        }

        // Varsayılan Yapıcı metod
        public Billet()
        {
        }
    }
}
