using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ADManager
{
    public class User
    {


        private readonly string userName;

        private readonly string userPassword;

        // Config dosyasından domain sunucu bilgisini alır.
        private readonly string domain = System.Configuration.ConfigurationManager.AppSettings["domainServer"];
        public User()
        {
            this.userName = Giris._userName;
            this.userPassword = Giris._userPassword;

        }

        #region Methodlar

        public PrincipalContext BaglantiKur()
        {
            PrincipalContext _principialContext = new PrincipalContext(ContextType.Domain, domain, userName, userPassword);
            return _principialContext;



        }

        public UserPrincipal SetUserPrincipial(PrincipalContext con)
        {
            UserPrincipal userPrincipial = new UserPrincipal(con);
            return userPrincipial;

        }

        public UserPrincipal SetUserPrincipial(PrincipalContext con, string userName, string Password)
        {
            string _userName = userName;
            string _Password = Password;

            UserPrincipal userPrincipial = new UserPrincipal(con, _userName, _Password, true);
            return userPrincipial;

        }

        public UserPrincipal SetPrincipialToFind(PrincipalContext principialContext, string samAccountName)
        {
            PrincipalContext _principialContext = principialContext;
            string _samAccountName = samAccountName;
            return UserPrincipal.FindByIdentity(_principialContext, _samAccountName);

        }

        public UserPrincipal SetPrincipialToFind(PrincipalContext principialContext, IdentityType Itype, string samAccountName)
        {
            PrincipalContext  _principialContext = principialContext;
            string _samAccountName = samAccountName;
            return UserPrincipal.FindByIdentity(_principialContext, _samAccountName);

        }

        public PrincipalSearcher SetPrincipialSearcher()
        {

            PrincipalSearcher pS = new PrincipalSearcher();
            return pS;

        }

        public SearchResult SetSearchResult(DirectoryEntry de)
        {
            DirectorySearcher sc = new DirectorySearcher(de);
            SearchResult results = sc.FindOne();
            return results;
        }

        public GroupPrincipal FindAdminGroup(PrincipalContext principialContext, IdentityType Itype, string adminGroupName)
        {
            PrincipalContext _principialContext = principialContext;
            IdentityType _Itype = Itype;
            string _adminGroupName = adminGroupName;

            GroupPrincipal groupPrincipial = GroupPrincipal.FindByIdentity(_principialContext, _Itype, _adminGroupName);
            return groupPrincipial;

        }
        #endregion

    } 

    public class UserProperties
    {
        public string cannonicalName { get; set; }

        public string samAccountName { get; set; }

        public string userAccountControlCode { get; set; }

        public string userAccountControl { get; set; }

        public string lastLogon { get; set; }

        public string whenCreated { get; set; }

        public string pwdLastSet { get; set; }


    }
  
}

