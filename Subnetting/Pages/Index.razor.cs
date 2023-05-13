namespace Subnetting.Pages
{
    public partial class Index
    {
        private string direccionIP;
        private int numSubnets;
        private List<Subnet> userSubnets = new List<Subnet>();

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
            //AQUI VAN LOS CALCULOS DE LAS SUBREDES
        }
        public class Subnet
        {
            public string Name { get; set; }
            public int Size { get; set; }
        }
    }
}