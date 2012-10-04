using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace SocketServerSample
{
    class Drive2
    {
        //передний левый мотор
        private PWM cf_motorLF = new PWM(Pins.GPIO_PIN_D5);
        private byte cf_PowerLF;
        public byte PowerLF
        {
            get
            {
                return cf_PowerLF;
            }
            set
            {
                cf_PowerLF=value;
                cf_motorLF.SetPulse(10000,(uint)(10000.0 * (Double)value / 100.0));
            }

        }
        //передний правый мотор
        private PWM cf_motorRF = new PWM(Pins.GPIO_PIN_D6);
        private byte cf_PowerRF;
        public byte PowerRF{
            get
            {
                return cf_PowerRF;
            }
            set
            {
                cf_PowerRF=value;
                cf_motorRF.SetPulse(10000,(uint)(10000.0 * (Double)value / 100.0));
            }

        }
        //задний левый мотор
        private PWM cf_motorLB = new PWM(Pins.GPIO_PIN_D9);
        private byte cf_PowerLB;
        public byte PowerLB{
            get
            {
                return cf_PowerLB;
            }
            set
            {
                cf_PowerLB=value;
                cf_motorLB.SetPulse(10000,(uint)(10000.0 * (Double)value / 100.0));
            }

        }
        //задний правый мотор
        private PWM cf_motorRB = new PWM(Pins.GPIO_PIN_D10);
        private byte cf_PowerRB;
        public byte PowerRB{
            get
            {
                return cf_PowerRB;
            }
            set
            {
                cf_PowerRB=value;
                cf_motorRB.SetPulse(10000,(uint)(10000.0 * (Double)value / 100.0));
            }

        }
        //кнопка SW1 срабатывает как прерывание
        private InterruptPort cf_button = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);

        public Drive2(byte startPower)
        {
            cf_button.OnInterrupt += new NativeEventHandler(button_OnInterrupt);
            PowerLF=startPower;
            PowerRF=startPower;
            PowerLB=startPower;
            PowerRB=startPower;
            //new Thread(new ThreadStart(DriveStart)).Start();
        }

        //private void DriveStart()
        //{
        //    Thread.Sleep(-1);
        //}

        void button_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (data2 != 0)
            {
                PowerLF = (byte)((PowerLF + 1 < 100) ? PowerLF + 1 : (PowerLF + 1) % 100);
                Debug.Print("Change power drive0 = "+PowerLF.ToString());

                PowerRF = (byte)((PowerRF + 2 < 100) ? PowerRF + 2 : (PowerRF + 2) % 100);
                Debug.Print("Change power drive1 = " + PowerRF.ToString());

                PowerLB = (byte)((PowerLB + 3 < 100) ? PowerLB + 3 : (PowerLB + 3) % 100);
                Debug.Print("Change power drive2 = " + PowerLB.ToString());

                PowerRB = (byte)((PowerRB + 4 < 100) ? PowerRB + 4 : (PowerRB + 4) % 100);
                Debug.Print("Change power drive3 = " + PowerRB.ToString()+"\n");
            }
        }


    }
}
