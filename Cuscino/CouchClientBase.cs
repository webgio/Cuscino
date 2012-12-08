using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuscino
{
    public abstract class CouchClientBase
    {
        protected string host;
        protected string username;
        protected string password;
        protected string db;

        protected void ValidateClient()
        {
            if (string.IsNullOrEmpty(db))
                throw new ArgumentException("Database should not be empty");
        }
    }
}
