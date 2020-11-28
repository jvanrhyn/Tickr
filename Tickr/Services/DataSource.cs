using System;
using System.Security.Cryptography.X509Certificates;
using Raven.Client.Documents;

namespace Tickr
{
    public  class DataSource
    {
        private readonly RavenSettings _ravenSettings;

        public DataSource(RavenSettings ravenSettings)
        {
            _ravenSettings = ravenSettings;
            Store = CreateStore();
        }


        public IDocumentStore Store { get; }

        private  IDocumentStore CreateStore()
        {
            var documentStore = new DocumentStore
            {
                Urls = new[]
                {
                    _ravenSettings.ServerUrl
                },
                Conventions =
                {
                    MaxNumberOfRequestsPerSession = 10,
                    UseOptimisticConcurrency = true
                },
                Database = "Shrt",
                Certificate =
                    new X509Certificate2(_ravenSettings.CertificateLocation)
            }.Initialize();

            return documentStore;
        }
    }
}