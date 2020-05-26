using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace OAuthStartup.BaseClasses
{
    public abstract class BaseClientAuth<TDAO>
    {
        protected IConfiguration _config {get;set;}
        protected TDAO _dataAccessObject {get;set;}
        protected BaseClientAuth(IConfiguration confi, TDAO dataAccesObject)
        {

        }
        /// <summary>
        /// Should be overriden in order to have any valid logic.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public abstract Task<bool> CheckCredentials(string p1, string p2);
    }
}