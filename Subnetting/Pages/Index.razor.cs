using System.Text.RegularExpressions;

namespace Subnetting.Pages
{
    public partial class Index
    {
        private string error;
        private string direccionIP;
        private int numSubnets;
        private List<Subnet> userSubnets = new List<Subnet>();

        public class Subnet
        {
            public string Name { get; set; }
            public int Size { get; set; }
        }

        private void CreateSubnets()
        {
            userSubnets = new List<Subnet>();
            for (int i = 0; i < numSubnets; i++)
            {
                userSubnets.Add(new Subnet());
            }
        }

        private void CalculateSubnets()
        {
            bool valida = Regex.IsMatch(direccionIP, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}/\d{1,2}\b");
            /*bool valida = Regex.IsMatch(ip, @"\b\d+\.\d+\.\d+\.\d+/\d+\b");*/ //Comprobamos que el formato de la IP es correcta 'X.X.X.X/X'

            if (valida)
            {
                direccionIP = direccionIP.Replace('/', '.'); //Cambiamos la barra del final de la IP por un '.' para
                                           //poder hacer el .Split() sin problemas

                string[] ipNumsString = direccionIP.Split('.'); //Metemos en un array cada valor de la ip 

                int[] ipNumsInt = new int[ipNumsString.Length]; //Ahora creamos un array de tipo 'int' y metemos todos los valores de
                                                                //'ipNumsString' pero cambiados a 'int' para poder trabajar mejor con ellos
                for (int i = 0; i < ipNumsInt.Length; i++)
                    ipNumsInt[i] = int.Parse(ipNumsString[i]);

                if (ipNumsInt[0] > 0 && ipNumsInt[0] < 256 && Range(ipNumsInt[1]) && Range(ipNumsInt[2]) && Range(ipNumsInt[3]) && ipNumsInt[4] < 33 && ipNumsInt[4] > 0)
                {
                    int x = 32 - ipNumsInt[4];
                    double y = Math.Pow(2, x) / 2;
                    if (userSubnets.Count <= y)
                    {

                    }
                    else
                    {
                        error = "El número de subredes supera el máximo de subredes que puede tener está dirección de IP";
                    }



                }
                else
                    error = "Algunos de los valores de la IP (ya sean los de la dirección IP o el de la máscara) " +
                                      "no cumplen con el rango de valores que tienen que tener";
                //Notificamos al usuario si algunos de los valores de la dirección IP o el valor de la máscara no 
                //cumple el rango que tiene que cumplir
            }
            else
                error = "El formato de la IP introducida no es correcto.El formato deberia ser: XXX.XXX.XXX.XXX/XX";
            //Notificamos al usuario cuando el formato de la ip no es correcto
        }
        
















        //MÉTODO QUE COMPRUEBA QUE EL NUMERO ESTÁ ENTRE 1 Y 255
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