using System.Security.Cryptography;
using System.Text;

namespace ProyectoERP.Helpers
{
    public class HelperAuth
    {
        public static string GenerarSalt()
        {
            Random rdm= new Random();
            string salt = "";
            for (int i=0; i<=32; i++)
            {
                int aleat=rdm.Next(0,255);
                char letra=Convert.ToChar(aleat);
                salt += letra;
            }
            return salt;
        }
        public static bool CompararClaves(byte[] clave, byte[] claveintroducida)
        {
            bool iguales = true;
            if(clave.Length!=claveintroducida.Length)
            {
                iguales = false;
            } else
            {
                for (int i = 0; i < clave.Length; i++)
                {
                    if (clave[i].Equals(claveintroducida[i]) == false)
                    {
                        iguales = false;
                        break;
                    }
                }
            }
            return iguales;
        }

        public static byte[] EncriptarClave(string clave, string salt)
        {
            string contenido = clave + salt;
            SHA512 sHA = SHA512.Create();
            byte[] salida = Encoding.UTF8.GetBytes(contenido);
            for (int i = 0; i <=82; i++)
            {
                salida = sHA.ComputeHash(salida);
            }
            sHA.Clear();
            return salida;
        }
    }
}
