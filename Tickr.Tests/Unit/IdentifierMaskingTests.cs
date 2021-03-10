namespace Tickr.Tests.Unit
{
    using Masking;
    using Xunit;

    public class IdentifierMaskingTests
    {
        [Fact]
        public void CanMaskIdentifier()
        {
            IdentifierMasking identifierMasking = new();

            var hidden = identifierMasking.HideIdentifier("HiddenWord");
            Assert.NotEmpty(hidden);

            var exposed = identifierMasking.RevealIdentifier(hidden);
            Assert.Equal("HiddenWord", exposed);
        }
    }
}