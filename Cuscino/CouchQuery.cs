using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Cuscino
{
    public class CouchQuery
    {
        internal string viewName;
        private string designDocument;
        private string startkey;
        private string endkey;

        internal string GetCriteria()
        {
            var criteria = string.Empty;
            if (String.IsNullOrEmpty(this.startkey) == false)
                criteria += @"startkey=""" + this.startkey + @"""";
            if (String.IsNullOrEmpty(this.startkey) == false && String.IsNullOrEmpty(this.endkey) == false)
                criteria += @"&";
            if (String.IsNullOrEmpty(this.endkey) == false)
                criteria += @"endkey=""" + this.endkey + @"""";
            return criteria;
        }

        internal string GetUri()
        {
            if (string.IsNullOrEmpty(designDocument))
                throw new InvalidOperationException("Design document not defined");
            if (string.IsNullOrEmpty(viewName))
                throw new InvalidOperationException("View not defined");
            return designDocument + "/_view/" + viewName + "/";
        }

        private CouchQuery()
        {
            this.designDocument = "application";
        }

        public static CouchQuery OnView(string viewname)
        {
            var query = new CouchQuery {viewName = viewname};
            return query;
        }

        //public CouchQuery OnView(string viewName)
        //{
        //    this.viewName = viewName;
        //    return this;
        //}

        public CouchQuery OfDesignDocument(string designDocument)
        {
            this.designDocument = designDocument;
            return this;
        }

        public CouchQuery StartingFrom(string startkey)
        {
            this.startkey = startkey;
            return this;
        }

        public CouchQuery EndingIn(string endkey)
        {
            this.endkey = endkey;
            return this;
        }
    }
}
