using System;
using Microsoft.SPOT;
using GTM = Gadgeteer.Modules;
using GT = Gadgeteer;

namespace Practica3DSCC
{
    // Referencia tipo "delegate" para función callback ObjectOn
    public delegate void ObjectOnEventHandler();

    // Referencia tipo "delegate" para función callback ObjectOff
    public delegate void ObjectOffEventHandler();

    /*
     * Clase SensorProximidad, encapsula el funcionanmiento del sensor de proximidad infrarrojo.
     * Esta clase gestiona los dos componentes del sensor: el LED infrarrojo y el foto-transistor.
     * Además, dispara dos eventos: ObjectOn y ObjectOff cuando el sensor detecta la presencia o
     * ausencia de un objeto.
     */
    class SensorProximidad
    {
        //EVENTO ObjectOff: Disparar este evento cuando el sensor detecte la ausencia del objeto
        public event ObjectOffEventHandler ObjectOff;

        //EVENTO ObjectOn: Disparar este evento cuando el sensor detecte la presencia de un objeto
        public event ObjectOnEventHandler ObjectOn;
        GTM.GHIElectronics.Extender extender;

        GT.SocketInterfaces.DigitalOutput dig_out;
        GT.SocketInterfaces.AnalogInput anag_in;
        GT.Socket.Pin pin_socket_3 = GT.Socket.Pin.Three;
        GT.Socket.Pin pin_socket_5 = GT.Socket.Pin.Five;

        GT.Timer timer;

        public SensorProximidad(GTM.GHIElectronics.Extender extender)
        {
            //TODO: Inicializar el sensor
            this.extender = extender;
            this.dig_out = this.extender.CreateDigitalOutput(pin_socket_5, false);
            this.anag_in = this.extender.CreateAnalogInput(pin_socket_3);
            timer = new GT.Timer(500, GT.Timer.BehaviorType.RunContinuously); // Create a timer
            timer.Tick += timer_Tick; // Run the method timer_tick when the timer ticks
           
        }

        public void StartSampling()
        {
            //TODO: Activar el LED infrarrojo y empezar a muestrear el foto-transistor

            this.dig_out.Write(true);
            
            this.anag_in.IsActive = true;
           // this.anag_in.ReadVoltage();
            timer.Start();

        }

        private void timer_Tick(GT.Timer timer)
        {
           double volt= this.anag_in.ReadVoltage();

           Debug.Print(volt.ToString());
           if (volt < 3.10)
           {
               Debug.Print("Hay objeto");
               ObjectOn();
               
           }
           else {
               Debug.Print("No hay objeto");
               ObjectOff();
           }
        }

        public void StopSampling()
        {
            //TODO: Desactivar el LED infrarrojo y detener el muestreo del foto-transistor
            this.dig_out.Write(false);
            this.anag_in.IsActive = false;
            timer.Stop();
        }
    }
}
