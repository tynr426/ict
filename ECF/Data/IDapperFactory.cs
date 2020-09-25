using System;
using System.Collections.Generic;
using System.Text;

namespace ECF.Data
{
    public interface IDapperFactory
    {
        string DBKey { get; set; }
        DapperClient CreateClient(string name);
    }
}
