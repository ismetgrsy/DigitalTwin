using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BilletApp

{
    public class Stand
    {
        // Property tanımlamaları
        public double Speed { get; set; }
        public double FBLSpeed;
        public double looph14, looph15, loopFBL;
        public double SB, DESC;
        public double loadfbl;
        public ProgressBar ProgressBar { get; set; }
    
        public double Yuk { get; set; }
        public DateTime Starttime { get; set; } // StartTime özelliği için getter ve setter ekledim
        public DateTime Endtime { get; set; }   // EndTime özelliği için getter ve setter ekledim
        public double billetSpeed { get; set; }

        //public double P = 1000000 * 10; // stand motor gucu
        // Constructor
        public string Fan;
        public string kapak;
        public double WZ1, WZ2, WZ3, WZ4;

     

        public Stand(double speed, double torque, double yuk, DateTime startTime, DateTime endTime)
        {
            Speed = speed;
           
            Yuk = yuk;
            Starttime = startTime;
            Endtime = endTime;
        }
        public Stand()
        {
                
        }




    }

}

    


