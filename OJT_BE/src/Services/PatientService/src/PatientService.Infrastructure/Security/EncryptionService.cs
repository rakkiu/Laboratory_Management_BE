using PatientService.Application.Interfaces;
using PatientService.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientService.Infrastructure.Services
{
    public class EncryptionService : IEncryptionService
    {
        public string Encrypt(string plainText)
        {
            return EncryptionHelper.Encrypt(plainText);
        }

        public string Decrypt(string cipherText)
        {
            return EncryptionHelper.Decrypt(cipherText);
        }

        public string EncryptDeterministic(string plainText)
        {
            return EncryptionHelper.EncryptDeterministic(plainText);
        }

        public string DecryptDeterministic(string cipherText)
        {
            return EncryptionHelper.DecryptDeterministic(cipherText);
        }
    }
}
