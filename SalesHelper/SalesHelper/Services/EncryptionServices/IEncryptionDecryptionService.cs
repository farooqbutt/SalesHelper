namespace SalesHelper.Services.EncryptionServices
{
    public interface IEncryptionDecryptionService
    {
        public string Encrypt(string simplePassword);
        public string Decrypt(string encodedPassword);
    }
}
