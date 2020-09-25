using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Data
{
    public class DapperFactoryOptions
    {
        public IList<Action<ConnectionProvider>> DapperActions { get; } = new List<Action<ConnectionProvider>>();
    }
    public class DefaultDapperFactory : IDapperFactory
    {
        private readonly IServiceProvider _services;
        private readonly IOptionsMonitor<DapperFactoryOptions> _optionsMonitor;
        public string DBKey { get; set; }
        public DefaultDapperFactory(IServiceProvider services, IOptionsMonitor<DapperFactoryOptions> optionsMonitor)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public DapperClient CreateClient(string dbkey)
        {
            if (dbkey == null)
                throw new ArgumentNullException(nameof(dbkey));

            var client = new DapperClient(new ConnectionProvider { });

            var option = _optionsMonitor.Get(dbkey).DapperActions.FirstOrDefault();
            if (option != null)
                option(client.CurrentConnectionConfig);
            else
                throw new ArgumentNullException(nameof(option));

            return client;
        }

    }
}
