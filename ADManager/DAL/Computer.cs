using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Management;

namespace ADManager
{
   public  class Computer
    {



        private readonly string domainServer;

      
        private PrincipalContext _principialContext;
        private ComputerPrincipal _computerPrincipial;

        public Computer()
        {
            domainServer =   System.Configuration.ConfigurationManager.AppSettings["domainServer"];
           
        }

     

        #region refactor

        public PrincipalContext SetPrincipialContext()
        {
            PrincipalContext principialContext = new PrincipalContext(ContextType.Domain, domainServer);
            return principialContext;

        }

        public ComputerPrincipal SetComputerPrincipial(PrincipalContext principialContext)
        {
            _principialContext = principialContext;
            _computerPrincipial = new ComputerPrincipal(_principialContext);
            return _computerPrincipial;
        }

        public PrincipalSearcher SetPrincipialSearcher()
        {
            PrincipalSearcher principialSearcher = new PrincipalSearcher(_computerPrincipial);
            return principialSearcher;

        }

        #endregion
 
    }


    public class ComputersProperties
    {

        public string computerName { get; set; }
        public string computerSID { get; set; }
        public string ipAdress { get; set; }
        public string lastPassSet { get; set; }
        public string lastBadAttempt { get; set; }

        public ComputersProperties(string computerName, string computerSID, string ipAdress, string lastPassSet, string lastBadAttempt)
        {
            this.computerName = computerName;
            this.computerSID = computerSID;
            this.ipAdress = ipAdress;
            this.lastPassSet = lastPassSet;
            this.lastBadAttempt = lastBadAttempt;

        }

     

    }



}
