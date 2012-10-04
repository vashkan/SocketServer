using System;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace SocketServerSample
{
#if OLD
    class Drive
    {
        private Boolean cf_alloc = true;
        private OutputPort cf_MotorLF = new OutputPort(Pins.GPIO_PIN_D13, false);
        private OutputPort cf_MotorRF = new OutputPort(Pins.GPIO_PIN_D12, false);
        private OutputPort cf_MotorLB = new OutputPort(Pins.GPIO_PIN_D11, false);
        private OutputPort cf_MotorRB = new OutputPort(Pins.GPIO_PIN_D8, false);
        private InterruptPort cf_button = new InterruptPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
        public byte PowerLF;
        public byte PowerRF;
        public byte PowerLB;
        public byte PowerRB;
        private int selDrive;
        OutputPort[] DriveS;
        //object blockObj;
        AutoResetEvent InitEnd;
        AutoResetEvent EndInit;
        //PWM servo = new PWM(Pins.GPIO_PIN_D5);
        public Drive(byte startPower)
        {
            cf_button.OnInterrupt += new NativeEventHandler(button_OnInterrupt);
            PowerLF = startPower;
            InitEnd = new AutoResetEvent(false);
            EndInit = new AutoResetEvent(false);
            DriveS = new OutputPort[4];
            DriveS[0] = cf_MotorLF;
            DriveS[1] = cf_MotorRF;
            DriveS[2] = cf_MotorLB;
            DriveS[3] = cf_MotorRB;

            //servo.SetPulse(1000,500);

            for (int i = 0; i < 4; i++)
            {
               
                    selDrive = i;

                    new Thread(new ThreadStart(this.m_DrivePowerSet)).Start();
                    Debug.Print("Start waiting init drive: " + i.ToString());
                    InitEnd.WaitOne();
                    Debug.Print("End waiting init drive: " + i.ToString());
                
                //Thread newTr = new Thread(this.m_DrivePowerSet);
                //newTr.Start(k);
            }   
                EndInit.Set();
           


        }

        void button_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            //cf_alloc=!(data2==0);
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

        void m_DrivePowerSet()
        {
            int Power = 0;
            int selectedDrive;
            OutputPort selPower;
           
                Debug.Print("Start initilize drive: " + selDrive.ToString());
                selectedDrive = selDrive;
                selPower = DriveS[selDrive];
                InitEnd.Set();
                Debug.Print("End initilize drive: " + selDrive.ToString());
            
            Debug.Print(" Start wait other motors of  drive: " + selectedDrive.ToString());
            EndInit.WaitOne();
            EndInit.Set();
            Debug.Print(" Stop wait other motors of  drive: " + selectedDrive.ToString());
            while (cf_alloc)
            {
                //servo.SetPulse(10000, (uint)(10000.0 * (Double)PowerLF / 100.0));
                switch (selectedDrive)
                {
                    case 0:
                        Power = PowerLF;
                        break;
                    case 1:
                        Power = PowerRF;
                        break;
                    case 2:
                        Power = PowerLB;
                        break;
                    case 3:
                        Power = PowerRB;
                        break;
                }
                if (Power != 0)
                {
                    selPower.Write(true);
                    Thread.Sleep((Int32)(10.0 * (Double)Power / 100.0));
                    selPower.Write(false);
                    Thread.Sleep((Int32)(10.0 * (100.0 - (Double)Power) / 100.0));                   
                }
                else { Thread.Sleep(0); }
            }

        }
    }
#endif
}
