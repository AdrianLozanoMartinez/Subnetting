using System;
using System.Text.RegularExpressions;

namespace Subnetting.Pages
{
    public partial class Index
    {
        private string error; //Variable en la que metemo un error, si es que hay
        private string direccionIP; //Variable de la direcci�n IP dada por el usuario
        private int numSubnets; //Variable de n�mero de SubRedes pedidas por el usuario

        private List<Subnet> userSubnets = new List<Subnet>(); 
        private List<Subnet> resultSubnets = new List<Subnet>(); 

        public class Subnet 
        {
            public string Name { get; set; } //Nombre de la subred
            public int Size { get; set; } //N�mero de hosts de la subred pedidos por el usuario
            public int HostMax { get; set; } //N�mero de hosts m�ximo que puede tener dicha subred
            public string IPAddress { get; set; } //Direcci�n red de la subred
            public string Mask { get; set; } //M�scara de la subred
            public string AsignableRange { get; set; } //Rango de hosts asiganble de la subred
            public string Broadcast { get; set; } //Direcci�n broadcast de la subred
        }

        private void CreateSubnets() 
        {
            /*Si el n�mero que mete el usuario es mayor que el que hay se amplia los input*/
            if(userSubnets.Count < numSubnets)
            { 
                for (int i = userSubnets.Count; i < numSubnets; i++) /*Desde el total hasta el n�mero elegido*/
                    userSubnets.Add(new Subnet{ Name = ((char)('A' + i)).ToString() }); /*A�ade input con las letras del abecedario A, A+1=B, B+1=C...*/
            }
            /*Si el n�mero que mete el usuario es menor que se disminuya los input*/
            else
            {
                for (int i = numSubnets; i < userSubnets.Count; i++) /*Lo recorremos al rev�s para borrar desde el �ltimo input*/
                    while (userSubnets.Count > numSubnets) /*Mientras la cantidad sea mayor que el introducido nos vaya borrando*/
                    {
                        userSubnets.Remove(userSubnets[i]);
                    }
            }
        }

        private void CalculateSubnets() //M�todo con toda la l�gica de la calculadora
        {
            error = "";
            resultSubnets.Clear();
            bool valida = Regex.IsMatch(direccionIP, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}/\d{1,2}\b");
            //Comprobamos que el formato de la IP es correcta 'X.X.X.X/X'

            if (valida)
            {
                string direccionIPsinBarra = direccionIP.Replace('/', '.'); //Cambiamos la barra del final de la IP por un '.' para
                                                             //poder hacer el .Split() sin problemas

                string[] ipNumsString = direccionIPsinBarra.Split('.'); //Metemos en un array cada valor de la ip 

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

                        for (int i = 0; i < ipNumsInt.Length - 1; i++)
                        {
                            ipBinario += DecimalABinario(ipNumsInt[i]).PadLeft(8, '0');
                        }

                        //Del string creado anteriormente, nos quedamos con los 'x' �ltimos bits, donde 'x' es el n�mero de bits para hosts
                        string bitsRestantes = ipBinario.Substring(ipBinario.Length - bitsParaHost, bitsParaHost);

                        //Aqu� calculamos el n�mero de bits que tenemos que utilizar dependiendo del n�mero de subredes que tengamos
                        double j = Math.Sqrt(userSubnets.Count);
                        int bitsNecesarios = (int)Math.Ceiling(j);

                        //Aqu� nos quedamos con los 'x' ultimos bits de 'bitsRestantes' donde 'x' es el numero de bit necesarios para la
                        //direccion de la subred menos la longuitud de 'bitsRestantes' 
                        string numeroHosts = bitsRestantes.Substring(bitsRestantes.Length - (bitsRestantes.Length - bitsNecesarios)).Replace('1', '0');

                        string numeroHostsConUnos = numeroHosts.Replace('0', '1');
                        int hostsMaximosSubred = BinarioADecimal(numeroHostsConUnos) - 1;

                        bool noSupera = true;
                        foreach (Subnet userSubnet in userSubnets)
                        {
                            if (userSubnet.Size > hostsMaximosSubred && noSupera)
                                noSupera = false;
                        }

                        string[] bitsRedSubred = new string[userSubnets.Count];
                        for (int i = 0; i < bitsRedSubred.Length; i++)
                        {
                            bitsRedSubred[i] = DecimalABinario(i).PadLeft(bitsNecesarios, '0');
                        }

                        int count = 0;

                        if (noSupera)
                        {
                            foreach (Subnet userSubnet in userSubnets)
                            {
                                Subnet subnet = new Subnet();

                                subnet.Name = userSubnet.Name;
                                subnet.Size = userSubnet.Size;
                                subnet.HostMax = hostsMaximosSubred;
                                subnet.Mask = "/" + (ipNumsInt[4] + bitsNecesarios);

                                string unos = new string('1', ipNumsInt[4]);
                                string unosConCeros = unos.PadRight(32, '0');
                                string ipRemplazar1 = AndLogico(ipBinario, unosConCeros);

                                string ipRemplazar2 = ipRemplazar1.Substring(0, ipBinario.Length - bitsRestantes.Length) + bitsRedSubred[count] + numeroHosts;

                                string direccionIpBinario = ipRemplazar2.Substring(0, ipBinario.Length - (bitsRestantes.Length - bitsNecesarios)) + numeroHosts;
                                string direccionBroadCastBinario = ipRemplazar2.Substring(0, ipBinario.Length - (bitsRestantes.Length - bitsNecesarios)) + numeroHostsConUnos;
                                string principioRangoBinario = ipRemplazar2.Substring(0, ipBinario.Length - (bitsRestantes.Length - bitsNecesarios)) + numeroHosts.Substring(0, numeroHosts.Length - 1) + "1";
                                string finalRangoBinario = ipRemplazar2.Substring(0, ipBinario.Length - (bitsRestantes.Length - bitsNecesarios)) + numeroHostsConUnos.Substring(0, numeroHostsConUnos.Length - 1) + "0";

                                string direccionIp = IpBinarioADecimal(direccionIpBinario);
                                string direccionBroadCast = IpBinarioADecimal(direccionBroadCastBinario);
                                string principioRango = IpBinarioADecimal(principioRangoBinario);
                                string finalRango = IpBinarioADecimal(finalRangoBinario);


                                subnet.IPAddress = direccionIp;
                                subnet.Broadcast = direccionBroadCast;
                                subnet.AsignableRange = principioRango + " - " + finalRango;


                                resultSubnets.Add(subnet);
                                count++;
                            }
                        }
                        else
                            error = "El n�mero de hosts supera el m�ximo de hosts por subred";
                    }
                    else
                        error = "El n�mero de subredes supera el m�ximo de subredes que puede tener est� direcci�n de IP";
                }
                else
                    error = "Algunos de los valores de la IP (ya sean los de la direcci�n IP o el de la m�scara) " +
                                      "no cumplen con el rango de valores que tienen que tener";
            }
            else
                error = "El formato de la IP introducida no es correcto.El formato deberia ser: XXX.XXX.XXX.XXX/XX";
        }


        // ----------------------- M�TODOS REUTILIZABLES -----------------------

        //M�TODO QUE COMPRUEBA QUE EL NUMERO EST� ENTRE 1 Y 255
        public static bool Range(int num) => num >= 0 && num< 256;


        //M�TODO QUE PASA DE DECIMAL A BINARIO
        public static string DecimalABinario(int numDecimal) => Convert.ToString(numDecimal, 2);


        //M�TODO QUE PASA DE BINARIO A DECIMAL
        public static int BinarioADecimal(string numBinario) => numBinario.Reverse().Select((c, i) => c == '1' ? (int)Math.Pow(2, i) : 0).Sum();

        //M�TODO QUE PASA DE IP EN BINARIO A SU FORMATO DECIMAL
        public static string IpBinarioADecimal(string ipBinario)
        {
            string result = "";

            result += BinarioADecimal(ipBinario.Substring(0, 8)) + ".";
            result += BinarioADecimal(ipBinario.Substring(8, 8)) + ".";
            result += BinarioADecimal(ipBinario.Substring(16, 8)) + ".";
            result += BinarioADecimal(ipBinario.Substring(24, 8));

            return result;
        }

        //M�TODO QUE HACE EL AND L�GICO ENTRE DOS N�MEROS BINARIOS
        public static string AndLogico(string cadena1, string cadena2)
        {
            string resultado = "";

            for (int i = 0; i < cadena1.Length; i++)
            {
                if (cadena1[i] == '1' && cadena2[i] == '1')
                    resultado += "1";
                else
                    resultado += "0";
            }
            return resultado;
        }
    }
}