namespace Subnetting.Pages
{
    public partial class Index
    {
        private int cont = 1;
        /*Eliminamos input al restar contador el for recorre uno menos y se ve uno menos*/
        private void Eliminar()
        {
            if(cont > 1)
            {
                cont--;
            }
        }
        /*A�adimos input al sumar contador el for recorre uno m�s y se ve uno m�s*/
        private void Contar()
        {
            cont++;
        }
    }
}