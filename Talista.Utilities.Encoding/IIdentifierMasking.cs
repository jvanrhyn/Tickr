namespace Talista.Utilities.Encoding
{
    public interface IIdentifierMasking
    {
        string RevealIdentifier(string hidden);
        string HideIdentifier(string id);
    }
}