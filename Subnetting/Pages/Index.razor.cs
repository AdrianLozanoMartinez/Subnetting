using System.Text.RegularExpressions;

namespace Subnetting.Pages
{
    public partial class Index
    {
        private string error; //Variable en la que metemo un error, si es que hay
        private string direccionIP; //Variable de la direcci�n IP dada por el usuario
        private int numSubnets; //Variable de n�mero de SubRedes pedidas por el usuario
        private List<Subnet> userSubnets = new List<Subnet>(); //Lista de Subredes dadas por el usuario (solo se guarda el nombre y el 
                                                               //n�mero de hosts de cada subred
        private List<Subnet> resultSubnets = new List<Subnet>(); //Lista de Subredes resultantes con todos los datos de la clase Subnet

        public class Subnet //Clase Subnet que refleja los datos que hay que mostrar al usuario de cada subred
        {
            public string Name { get; set; } //Nombre de la subred
            public int Size { get; set; } //N�mero de hosts de la subred pedidos por el usuario
            public int HostMax { get; set; } //N�mero de hosts m�ximo que puede tener dicha subred
            public string IPAddress { get; set; } //Direcci�n red de la subred
            public string Mask { get; set; } //M�scara de la subred
            public string AsignableRange { get; set; } //Rango de hosts asiganble de la subred
            private string Broadcast { get; set; } //Direcci�n broadcast de la subred
        }

        private void CreateSubnets() //M�todo de crea y almacena los datos de las subredes dadas por el usuario
        {
            userSubnets = new List<Subnet>();
            for (int i = 0; i < numSubnets; i++)
            {
                userSubnets.Add(new Subnet());
            }
        }

        private void CalculateSubnets() //M�todo con toda la l�gica de la calculadora
        {
            bool valida = Regex.IsMatch(direccionIP, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}/\d{1,2}\b");
            //Comprobamos que el formato de la IP es correcta 'X.X.X.X/X'

            if (valida)
            {
                direccionIP = direccionIP.Replace('/', '.'); //Cambiamos la barra del final de la IP por un '.' para
                                                             //poder hacer el .Split() sin problemas

                string[] ipNumsString = direccionIP.Split('.'); //Metemos en un array cada valor de la ip 

                int[] ipNumsInt = new int[ipNumsString.Length]; //Ahora creamos un array de tipo 'int' y metemos todos los valores de
                                                                //'ipNumsString' pero cambiados a 'int' para poder trabajar mejor con ellos
                
                for (int i = 0; i < ipNumsInt.Length; i++) //Metemos los datos de 'ipNumsString' a 'ipNumsInt' 
                    ipNumsInt[i] = int.Parse(ipNumsString[i]);


                //Este 'if' comprueba que los cuatro octetos de la IP y de la m�scara dada por el usuario sean v�lidos
                if (ipNumsInt[0] > 0 && ipNumsInt[0] < 256 && Range(ipNumsInt[1]) && Range(ipNumsInt[2]) && Range(ipNumsInt[3]) && ipNumsInt[4] < 33 && ipNumsInt[4] > 0)
                {
                    //Aqu� calculamos los bits que son para host y posteriormente el numero de 
                    //subredes m�ximo que puede tener la direcci�n ip dada por el usuario
                    int bitsParaHost = 32 - ipNumsInt[4]; 
                    double y = Math.Pow(2, bitsParaHost) / 2;

                    //Aqui comprobamos que el numero de subredes pedidas por el usuario sea menor que el numero maximo que puede tener
                    if (userSubnets.Count <= y)
                    {
                        //Metemos en un string la direcci�n IP completa en formato binario
                        string ipBinario = "";

                        for(int i = 0; i < ipNumsInt.Length - 1;i++)
                        {
                            ipBinario += DecimalABinario(ipNumsInt[i]);
                        }

                        //Del string creado anteriormente, nos quedamos con los 'x' �ltimos bits, donde 'x' es el n�mero de bits para hosts
                        string bitsRestantes = ipBinario.Substring(ipBinario.Length - bitsParaHost);

                        //Aqu� calculamos el n�mero de bits que tenemos que utilizar dependiendo del n�mero de subredes que tengamos
                        double j = Math.Sqrt(userSubnets.Count);
                        int bitsNecesarios = (int)Math.Ceiling(j);

                        //Aqu� nos quedamos con los 'x' ultimos bits de 'bitsRestantes' donde 'x' es el numero de bit necesarios para la
                        //direccion de la subred menos la longuitud de 'bitsRestantes' 
                        string numeroHosts = bitsRestantes.Substring(bitsRestantes.Length - (bitsRestantes.Length - bitsNecesarios));

                        numeroHosts = numeroHosts.Replace('0', '1');
                        int hostsMaximosSubred = BinarioADecimal(numeroHosts) - 1;

                        bool noSupera = true;

                        foreach (Subnet userSubnet in userSubnets)
                        {
                            if(userSubnet.Size > hostsMaximosSubred && noSupera)
                                noSupera = false;
                        }

                        if (noSupera)
                        {
                            foreach(Subnet userSubnet in userSubnets)
                            {
                                Subnet subnet = new Subnet();

                                subnet.Name = userSubnet.Name;
                                subnet.Size = userSubnet.Size;
                                subnet.HostMax = hostsMaximosSubred;
                                subnet.Mask = "/" + ipNumsString[4];


                                //Falta direccion IP
                                //Falta rango IPs
                                //Falta direccion Broadcast


                                resultSubnets.Add(subnet);
                            }
                        }
                        else
                            error = "El n�mero de hosts supera el m�ximo de hosts por subred";
                        //Notificamos al usuario si algunos de los hosts de alguna subred supera el m�ximo de hosts por subred
                    }
                    else
                        error = "El n�mero de subredes supera el m�ximo de subredes que puede tener est� direcci�n de IP";
                    //Notificamos al usuario si el n�mero de subredes que ha pedido supera el n�mero m�ximod e subredes m�ximo que 
                    //puede tener la direcci�n Ip que ha pedido
                }
                else
                    error = "Algunos de los valores de la IP (ya sean los de la direcci�n IP o el de la m�scara) " +
                                      "no cumplen con el rango de valores que tienen que tener";
                //Notificamos al usuario si algunos de los valores de la direcci�n IP o el valor de la m�scara no 
                //cumple el rango que tiene que cumplir
            }
            else
                error = "El formato de la IP introducida no es correcto.El formato deberia ser: XXX.XXX.XXX.XXX/XX";
            //Notificamos al usuario cuando el formato de la ip no es correcto
        }


        // -------------------- M�TODOS REUTILIZABLES --------------------

        //M�TODO QUE COMPRUEBA QUE EL NUMERO EST� ENTRE 1 Y 255
        static public bool Range(int num)
        {
            return num >= 0 && num < 256;
        }

        //METODO QUE PASA DE DECIMAL A BINARIO
        public static string DecimalABinario(int numDecimal) => Convert.ToString(numDecimal, 2);

        //METODO QUE PASA DE BINARIO A DECIMAL
        public static int BinarioADecimal(string numBinario) => numBinario.Reverse().Select((c, i) => c == '1' ? (int)Math.Pow(2, i) : 0).Sum();
    }
}