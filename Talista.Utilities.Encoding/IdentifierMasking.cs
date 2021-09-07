namespace Talista.Utilities.Encoding
{
    using System.Linq;
    using System.Text;

    //
    // https://ayende.com/blog/191297-C/avoiding-exposing-identifier-details-to-your-users
    //

    public class IdentifierMasking : IIdentifierMasking
    {
        private static byte[] _key;

        public IdentifierMasking(byte[] key = null)
        {
            _key = key ?? Sodium.SecretBox.GenerateKey();
        }

        public string RevealIdentifier(string hidden)
        {
            var data = SimpleBase.Base58.Bitcoin.Decode(hidden);
            var nonce = data.Slice(0, 12).ToArray();
            var encrypted = data.Slice(12).ToArray();
            var plain = Sodium.SecretAeadAes.Decrypt(encrypted, nonce, _key);
            return Encoding.UTF8.GetString(plain);

        }
        public string HideIdentifier(string id)
        {
            var nonce = Sodium.SecretAeadAes.GenerateNonce();
            var encrypted = Sodium.SecretAeadAes.Encrypt(Encoding.UTF8.GetBytes(id), nonce, _key);

            return SimpleBase.Base58.Bitcoin.Encode(nonce.Concat(encrypted).ToArray());
        }
    }
}
