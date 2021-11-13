using ClienteParaVuelos.View;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClienteParaVuelos.ViewModel
{
    public class ClienteViewModel : INotifyPropertyChanged
    {
        public string Url { get; set; } = "http://vuelos.itesrc.net/Tablero";
        public ObservableCollection<Root> ListaDeVuelos { get; set; } = new ObservableCollection<Root>();
        private EditarView editarDialog;
        private AgregarView agregarDialog;
        private Root _miVuelo = new Root();

        public Root MiVuelo
        {
            get { return _miVuelo; }
            set { _miVuelo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MiVuelo)));
            }
        }

        private string _message;

        public string Mensaje
        {
            get => _message;
            set
            {
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));
            }
        }
        private string _messageconectado;

        public string MensajeConectado
        {
            get => _messageconectado;
            set
            {
                _messageconectado = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MensajeConectado)));
            }
        }
        private string _messagedesconectado;

        public string MensajeDesonectado
        {
            get => _messagedesconectado;
            set
            {
                _messagedesconectado = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MensajeDesonectado)));
            }
        }



        Dispatcher current;

        public ICommand EliminarCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand ConectarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand VerAgregarCommand { get; set; }
        public ClienteViewModel()
        {
            Conectar();
            current = Dispatcher.CurrentDispatcher;
            AgregarCommand = new RelayCommand(Agregar);
            EliminarCommand = new RelayCommand(Eliminar);
            VerEditarCommand = new RelayCommand(VerEditar);
            EditarCommand = new RelayCommand(Editar);
            VerAgregarCommand = new RelayCommand(VerAgregar);
        }

        private void Conectar()
        {

            System.Uri direccion = new System.Uri(Url);
            System.Net.WebRequest webRequest;
            webRequest = System.Net.WebRequest.Create(direccion);
            System.Net.WebResponse objResp;
            bool RedActivada;
            try
            {
                objResp = webRequest.GetResponse();
                RedActivada = true;
            
                objResp.Close();
                webRequest = null;
            }
            catch (Exception ex)
            {
                
                RedActivada = false;

                webRequest = null;
            }

            if (RedActivada)
            {
                MensajeConectado = "Usted tiene acceso internet";
           
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MensajeConectado)));
          
                WebClient wc = new WebClient();
                var datos = wc.DownloadString(Url);

                ListaDeVuelos = JsonConvert.DeserializeObject<ObservableCollection<Root>>(datos);
              

            }
            else
            {
                MensajeDesonectado = "Imposible el acceso a internet";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mensaje)));

            }

        }
        public void VerAgregar()
        {
            Mensaje = "";
            agregarDialog = new();
            agregarDialog.DataContext = this;
            MiVuelo = new();
          
            agregarDialog.ShowDialog();
        }
        private void Agregar()
        {

            if (string.IsNullOrWhiteSpace(MiVuelo.destino) || string.IsNullOrWhiteSpace(MiVuelo.vuelo) || string.IsNullOrWhiteSpace(MiVuelo.hora) || string.IsNullOrWhiteSpace(MiVuelo.estado))
            {
                Mensaje = "Verifica los datos a enviar";

            }
          

            else
            {
                System.Uri direccion = new System.Uri(Url);
                System.Net.WebRequest webRequest;
                webRequest = System.Net.WebRequest.Create(direccion);
                System.Net.WebResponse objResp;
                bool RedActivada;
                try
                {
                    objResp = webRequest.GetResponse();
                    RedActivada = true;

                    objResp.Close();
                    webRequest = null;
                }
                catch (Exception ex)
                {

                    RedActivada = false;

                    webRequest = null;
                }
                if (RedActivada==true)
                {
                    Mensaje = "";
                   
                    HttpResponseMessage respuesta;
                    string registro = JsonConvert.SerializeObject(MiVuelo);
                    HttpClient client = new HttpClient();
                    HttpRequestMessage requpost = new HttpRequestMessage
                    {
                        Content = new StringContent(registro, Encoding.Default, "application/json"),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("http://vuelos.itesrc.net/tablero")
                    };
                    respuesta = client.SendAsync(requpost).Result;
                    Conectar();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListaDeVuelos)));

                }
                else
                {
                    Mensaje = "No tienes internet para hacer el envio";
                }
            
            }

        }
        public void VerEditar()
        {
            editarDialog = new();
            editarDialog.DataContext = this;
            if (MiVuelo != null)
            {
                Mensaje = "";

                editarDialog.ShowDialog();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MiVuelo"));
            }
        }
        private void Editar()
        {

            if (string.IsNullOrWhiteSpace(MiVuelo.destino) || string.IsNullOrWhiteSpace(MiVuelo.vuelo) || string.IsNullOrWhiteSpace(MiVuelo.hora) || string.IsNullOrWhiteSpace(MiVuelo.estado))
            {
                Mensaje = "Verifica los datos a editar";

            }


            else
            {
                System.Uri direccion = new System.Uri(Url);
                System.Net.WebRequest webRequest;
                webRequest = System.Net.WebRequest.Create(direccion);
                System.Net.WebResponse objResp;
                bool RedActivada;
                try
                {
                    objResp = webRequest.GetResponse();
                    RedActivada = true;

                    objResp.Close();
                    webRequest = null;
                }
                catch (Exception ex)
                {

                    RedActivada = false;

                    webRequest = null;
                }
                if (RedActivada==true)
                {
                    HttpResponseMessage respuesta;
                    string registro = JsonConvert.SerializeObject(MiVuelo);
                    HttpClient client = new HttpClient();
                    HttpRequestMessage requput = new HttpRequestMessage
                    {
                        Content = new StringContent(registro, Encoding.Default, "application/json"),
                        Method = HttpMethod.Put,
                        RequestUri = new Uri(Url)
                    };
                    respuesta = client.SendAsync(requput).Result;
                    Conectar();
                }
                else
                {
                    Mensaje = "No tienes internet para hacer la edición";
                }
            }
           


        }
        private void Eliminar()
        {
            if ( string.IsNullOrWhiteSpace(MiVuelo.vuelo) || string.IsNullOrWhiteSpace(MiVuelo.hora))
            {
                Mensaje = "Debe seleccionar la hora y elvuelo para poder eliminar";

            }
           else
            {
                System.Uri direccion = new System.Uri(Url);
                System.Net.WebRequest webRequest;
                webRequest = System.Net.WebRequest.Create(direccion);
                System.Net.WebResponse objResp;
                bool RedActivada;
                try
                {
                    objResp = webRequest.GetResponse();
                    RedActivada = true;

                    objResp.Close();
                    webRequest = null;
                }
                catch (Exception ex)
                {

                    RedActivada = false;

                    webRequest = null;
                }
                if (RedActivada == true)
                {
                    Mensaje = "";
                    HttpResponseMessage respuesta;


                    string registro = JsonConvert.SerializeObject(MiVuelo);

                    HttpClient client = new HttpClient();
                    HttpRequestMessage requpost = new HttpRequestMessage
                    {
                        Content = new StringContent(registro, Encoding.Default, "application/json"),
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri("http://vuelos.itesrc.net/tablero")
                    };
                    respuesta = client.SendAsync(requpost).Result;
                    Conectar();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListaDeVuelos)));
                }
                else
                {
                    Mensaje = "No tienes internet para borrar un vuelo";
                }

            }
        }
          

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
