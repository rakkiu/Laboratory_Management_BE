using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Application.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
        string EncryptDeterministic(string plainText);
        string DecryptDeterministic(string cipherText);
    }
}
