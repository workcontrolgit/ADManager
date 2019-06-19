using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;




namespace ADManager
{
    class BLUser
    {

        private readonly User user;
        public string responseMessage { get; set; }
        public BLUser()
        {
            user = new User();

        }


        #region KullaniciGetir
        /// <summary>
        /// Active Directory sunucusundan tüm kullanıcıların getirir.
        /// </summary>
        /// <returns>Tüm kullanıcıları tablo olarak döner </returns>
        public DataTable GetAllAdUsers()
        {

            List<UsersProperties> usersPropertiesList = new List<UsersProperties>();
            try
            {

                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetUserPrincipial(principialCon))
                {
                    userPrincipial.Name = "*";
                    using (PrincipalSearcher principialSearcher = user.SetPrincipialSearcher())
                    {
                        principialSearcher.QueryFilter = userPrincipial;


                        foreach (var result in principialSearcher.FindAll())
                        {

                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            usersPropertiesList.Add(GetUserProperties(de));

                        }

                    }
                }
            }
            catch (PrincipalExistsException ex)
            {
                responseMessage = ex.Message;

            }

            catch (PrincipalException ex)
            {
                responseMessage = ex.Message;

            }

            return DataTableAktar(usersPropertiesList);
        }
        #endregion

        /// <summary>
        /// Active Directory veritabanında bulunan kullanıcı bilgilerini, UserProperties sınıfı  propertilerine set eden method.
        /// </summary>
        /// <param name="de">DirectoryEntry</param>
        /// <returns></returns>
        public UsersProperties GetUserProperties(DirectoryEntry de)
        {

            SearchResult searchResult = user.SetSearchResult(de);
            var usersProperties = new UsersProperties();
            usersProperties.cannonicalName = de.Properties["cn"].Value.ToString();
            usersProperties.samAccountName = de.Properties["samaccountname"][0].ToString();
            usersProperties.userAccountControlCode = de.Properties["useraccountcontrol"][0].ToString();
            usersProperties.userAccountControl = UserAccountControl(de.Properties["useraccountcontrol"][0].ToString());
            usersProperties.whenCreated = Convert.ToDateTime(de.Properties["whenCreated"].Value).ToLocalTime().ToString();
            usersProperties.pwdLastSet = DateTime.FromFileTime((long)searchResult.Properties["pwdLastSet"][0]).ToShortDateString();
            usersProperties.lastLogon = DateTime.FromFileTime((long)searchResult.Properties["lastLogon"][0]).ToLocalTime().ToString();
            return usersProperties;

        }


        #region KullaniciArama
        /// <summary>
        /// Actif Dizin veritabanında kullanıcı arama yapmak için kullanılır.
        /// </summary>
        /// <param name="arananKisi">UI'da girilen , aranan kullanıcının adı </param>
        /// <returns>Kullanıcı listesini döner.</returns>
        public List<UsersProperties> SearchUser(string arananKisi)
        {
            List<UsersProperties> userList = new List<UsersProperties>();

            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetUserPrincipial(principialCon))
                {
                    userPrincipial.Name = $"{arananKisi}*";
                    using (PrincipalSearcher principialSearcher = new PrincipalSearcher(userPrincipial))
                    {

                        foreach (var ps in principialSearcher.FindAll().Where(x => x.SamAccountName.Contains(arananKisi)))
                        {
                            DirectoryEntry de = (DirectoryEntry)ps.GetUnderlyingObject();
                            userList.Add(GetUserProperties(de));


                        }
                    }
                }

            }

            catch (PrincipalExistsException ex)
            {
                responseMessage = ex.Message;

            }

            catch (PrincipalException ex)
            {
                responseMessage = ex.Message;

            }
            return userList;
        }

        public DataTable SetSearchedPersonDT(IEnumerable<UsersProperties> userList)
        {
            IEnumerable<UsersProperties> _userList = userList;
            return DataTableAktar(_userList);

        }
        #endregion


        #region KullanıcıKayit

        /// <summary>
        /// Aktif Dizin Veritabanına yeni kayıt ekleme methodu.
        /// Kullanıcı Kayıt UI formunda girilen parametreleri UserFormInputs sınıfı vasıtasıyla alır ve veritabanıına kaydeder.
        /// </summary>
        /// <param name="userFormInputs"></param>
        /// <returns></returns>
        public bool CreateUserAccount(UserFormInputs userFormInputs)
        {
            UserFormInputs _userFormInputs = userFormInputs;
            bool kayitDurum = false;


            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetUserPrincipial(principialCon, _userFormInputs.userName, _userFormInputs.userPass))
                {
                    userPrincipial.SamAccountName = _userFormInputs.userName;
                    userPrincipial.Name = string.Format("{0} {1}", _userFormInputs.name, _userFormInputs.surname);
                    userPrincipial.Surname = _userFormInputs.surname;
                    userPrincipial.DisplayName = string.Format("{0} {1}", _userFormInputs.name, _userFormInputs.surname);
                    userPrincipial.Enabled = true;
                    userPrincipial.Save();

                    //Kullanıcı ilk oturum açılısında parolasını değiştirsin .
                    userPrincipial.ExpirePasswordNow();
                    kayitDurum = true;

                }


            }

            catch (PrincipalExistsException ex)
            {
                responseMessage = ex.Message;

            }

            catch (PrincipalException ex)
            {
                responseMessage = ex.Message;

            }
            return kayitDurum;


        }

        #endregion

        #region KullaniciSilme
        //Aktif Dizin Veritabanından kullanıcı silmeye yarayan method.
        public string DeleteUser(string samAccountName)
        {
            string _samAccountName = samAccountName;


            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetPrincipialToFind(principialCon, _samAccountName))
                {
                    if (_samAccountName != null)
                    {

                        userPrincipial.Delete();
                        responseMessage = "Kullanıcı başarıyla silindi";
                    }
                }
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
            }

            return responseMessage;
        }

        #endregion


        #region KullaniciPasif

        /// <summary>
        /// Kullanıcı hesabını Aktif Dizin veritabanında Pasife almaya yarayan method.
        /// </summary>
        /// <param name="samAccountName">Kullanıcı Adı </param>
        /// <returns>İşlem gerçekleşme durumunu döner</returns>
        public string DisableUser(string samAccountName)
        {
            string _samAccountName = samAccountName;

            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetPrincipialToFind(principialCon, IdentityType.SamAccountName, _samAccountName))
                {
                    if (userPrincipial.Enabled == true)
                    {
                        userPrincipial.Enabled = false;
                        userPrincipial.Save();
                        responseMessage = "Kullanıcı başarıyla Pasif edildi";
                    }

                    else
                    {
                        responseMessage = "Kullanıcı zaten pasif";
                    }
                }
            }

            catch (Exception ex)
            {
                responseMessage = ex.Message;
            }

            return responseMessage;
        }

        #endregion

        #region AdminGrubaEkleme

        public string AddUserToAdminGroup(string samAccountName, string adminGroupName)
        {
            string _samAccountName = samAccountName;
            string _adminGroupName = adminGroupName;

            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())


                using (GroupPrincipal adminGroup = user.FindAdminGroup(principialCon, IdentityType.Name, _adminGroupName))
                {
                    adminGroup.Members.Add(principialCon, IdentityType.SamAccountName, _samAccountName);
                    adminGroup.Save();
                    responseMessage = "İşlem Başarılı";


                }

            }
            catch (DirectoryServicesCOMException ex)
            {
                responseMessage = ex.Message;
            }

            catch (Exception exc)
            {
                responseMessage = exc.Message;
            }

            return responseMessage;
        }



        #endregion

        #region KullaniciUyelikleriGoster
        public IEnumerable<string> GetUserGroups(string samAccountName)
        {
            string _samAccountName = samAccountName;
            List<string> groupList = new List<string>();
            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetPrincipialToFind(principialCon, IdentityType.SamAccountName,_samAccountName))
                {
                    var groups = userPrincipial.GetAuthorizationGroups();

                    foreach (Principal gp in groups)
                    {
                        groupList.Add(gp.ToString());
                    }


                }

            }
            catch (Exception ex)
            {

                responseMessage = ex.Message; 
            }
            return groupList;
        }


        #endregion

        #region ParolaSifirla

        /// <summary>
        /// Kullanıcı Parola SIfırlama işlemini yapan method.
        /// </summary>
        /// <param name="samAccountName">Kullanıcı Adı</param>
        /// <param name="password"> Yeni Parolası </param>
        /// <returns></returns>
        public string ResetUserPassword(string samAccountName, string password)
        {
            try
            {
                using (PrincipalContext principialCon = user.BaglantiKur())
                using (UserPrincipal userPrincipial = user.SetPrincipialToFind(principialCon, samAccountName))
                {
                    if (samAccountName != null)
                    {

                        userPrincipial.SetPassword(password);
                        responseMessage = "Kullanıcı Parolası Değiştirildi";
                    }
                }
            }
            catch (Exception ex)
            {
                responseMessage = ex.Message;
            }

            return responseMessage;
        }

        #endregion

        public DataTable DataTableAktar(IEnumerable<UsersProperties> userList)
        {
            IEnumerable<UsersProperties> _userList = userList;
            var fillDataTable = new DataTableFill();
            return fillDataTable.FillDataTable(_userList);
        }



     
        // AD veritabanından bulunan Kullanıcı Durum kodlarının kontrolünü sağlayan method.
        private string UserAccountControl(string durumKodu)
        {
            string _durumKodu = durumKodu;
            string durumResponse = "Bilinmiyor";
            switch (_durumKodu)

            {
                case "512":

                    durumResponse = "Normal hesap";
                    break;
                

                case "544":

                    durumResponse = "Aktif - Parola Gerektirmiyor";
                    break;

                case "546":

                    durumResponse = "Pasif-Parola Gerektirmiyor";
                    break;
                case "514":

                    durumResponse =  "Pasif";
                    break;
            }
            return durumResponse;
        }

        


    }
}
