using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

//Estas referencias son necesarias para usar GLIDE
using GHI.Glide;
using GHI.Glide.Display;
using GHI.Glide.UI;

namespace Practica3DSCC
{
    public partial class Program
    {
        //Objetos de interface gráfica GLIDE
        private GHI.Glide.Display.Window controlWindow;
        private GHI.Glide.Display.Window camaraWindow;
        private Button btn_start;
        private Button btn_stop;
        SensorProximidad sensor_prox;
        enum Estado { ESTADO_1, ESTADO_2, ESTADO_3 };
        Estado actual;
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            //Carga las ventanas
            controlWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.controlWindow));
            camaraWindow = GlideLoader.LoadWindow(Resources.GetString(Resources.StringResources.camaraWindow));
            GlideTouch.Initialize();

             sensor_prox = new SensorProximidad(extender);

            //Inicializa los botones en la interface
            btn_start = (Button)controlWindow.GetChildByName("start");
            btn_stop = (Button)controlWindow.GetChildByName("stop");
            btn_start.TapEvent += btn_start_TapEvent;
            btn_stop.TapEvent += btn_stop_TapEvent;

            actual = Estado.ESTADO_1;

            camera.CameraConnected += camera_CameraConnected;
            camera.BitmapStreamed += camera_BitmapStreamed;
            //Selecciona mainWindow como la ventana de inicio
            Glide.MainWindow = controlWindow;

            sensor_prox.ObjectOn += sensor_prox_ObjectOn;
            sensor_prox.ObjectOff += sensor_prox_ObjectOff;
            
        }

        private void camera_BitmapStreamed(Camera sender, Bitmap e)
        {
            displayT35.SimpleGraphics.DisplayImage(e, 0, 0);
        
            Debug.Print("Funcioooon callback");
        }

        private void camera_CameraConnected(Camera sender, EventArgs e)
        {
           // camera.StartStreaming();
            Debug.Print("connected");
        }

        void sensor_prox_ObjectOff()
        {
          //  camera.StopStreaming();
            cambiarEstado(Estado.ESTADO_2);
        }

        void sensor_prox_ObjectOn()
        {
          //  camera.StartStreaming();
            cambiarEstado(Estado.ESTADO_3);
        }

        void btn_stop_TapEvent(object sender)
        {
            Debug.Print("Stop");
           // sensor_prox.StopSampling();
            cambiarEstado(Estado.ESTADO_1);

        }

        void btn_start_TapEvent(object sender)
        {
            Debug.Print("Start");
           // sensor_prox.StartSampling();
            cambiarEstado(Estado.ESTADO_2);
        }

        private void cambiarEstado(Estado es) {
            TextBlock text = (TextBlock)controlWindow.GetChildByName("status");
                   
            switch(es){
                case Estado.ESTADO_1:
                    
                    Glide.MainWindow = controlWindow;
                    sensor_prox.StopSampling();
                    text.Text = "Monitoreo OFF";
                    actual = Estado.ESTADO_1;
                    break;
                case Estado.ESTADO_2:
                    
                    Glide.MainWindow = controlWindow;
                    sensor_prox.StartSampling();
                    camera.StopStreaming();
                    text.Text = "Monitoreo ON";
                    actual = Estado.ESTADO_2;
                    break;
                case Estado.ESTADO_3:
                    if (actual != Estado.ESTADO_3)
                    { Glide.MainWindow = camaraWindow; }
                    camera.StartStreaming();
                    actual = Estado.ESTADO_3;
                    break;
                default:
                    Debug.Print("Aiiiuudaaaa");
                    break;


            }
        }

    }
}
