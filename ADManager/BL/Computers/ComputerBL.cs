using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Net;


namespace ADManager
{
    class ComputerBL
    {


        private readonly Computer _computer;
        private readonly string computerDomain;
        private string _arananPc { get; set; }
        public string errorMessage { get; set; }
        private List<ComputersProperties> computerList { get; set; }

      
        public ComputerBL()
        {

            _computer = new Computer();
            computerDomain = System.Configuration.ConfigurationManager.AppSettings["computerDomain"];

        }

        #region Methodlar 
        /// <summary>
        /// ComputerForm UI'da Sorgula butonu ya da Tük kayıtları getir butonu tıklandığından çağrılan method.
        /// </summary>
        /// <param name="btnName"> Tıklanılan Butonun adını parametre olarak alır. Tıklanılan butona göre farklı liste oluşturur.</param>
        /// <param name="arananPc">Arama alanına girilen değeri parametre olarak alır.Bilgisayar arama yapılırken kullanılır
        ///Girilen text , AD Veritabanında Bilgisayar adı ya da bilgisayar IP'si ile eşleşme kontrolü için. 
        /// </param>
        /// <returns>From üzerindeki Datagrid'in veri kaynağı olan Datatable döner.</returns>
        public DataTable FillGridView(string btnName, string arananPc)
        {
            _arananPc = arananPc;
            string _btnName = btnName;
            List<ComputersProperties> secilenPcList = new List<ComputersProperties>();
            var dataTableFill = new DataTableFill();

            if (computerList == null)
            {
                computerList = GetAllComputers().ToList();
            }

            switch (_btnName)
                {
                case "AraBtn":

                    secilenPcList = SearchSingleComputer();
                    break;

                case "ListAll":
                    secilenPcList = computerList;
                    break;

                }

            DataTable dataTableComputers = dataTableFill.FillDataTableWithComputers(secilenPcList);
            return dataTableComputers;

        }

        #region BilgisayarListele
        /// <summary>
        /// AD Sunucusundan kayıtlı tüm bilgisayar listesini getirir.
        /// </summary>
        /// <returns> Bilgisayar Listesini döner </returns>
        public IEnumerable<ComputersProperties> GetAllComputers()
        {
            List<ComputersProperties> computerPropertiesList = new List<ComputersProperties>();
            try
            {
                using (var principialContext = _computer.SetPrincipialContext())
                using (var computerPrincipial = _computer.SetComputerPrincipial(principialContext))
                using (var principialSearcher = _computer.SetPrincipialSearcher())
                {
                    computerPrincipial.Name = "*";
                    principialSearcher.QueryFilter = computerPrincipial;
                    PrincipalSearchResult<Principal> _computerSearchResult = principialSearcher.FindAll();
                    foreach (var p in _computerSearchResult)
                    {
                        ComputerPrincipal pc = (ComputerPrincipal)p;
                        string ipAdress = GetComputerIpAddress(p.Name);
                        var computerPro = new ComputersProperties(pc.Name, pc.Sid.ToString(), ipAdress, pc.LastPasswordSet.ToString(), pc.LastBadPasswordAttempt.ToString());
                        computerPropertiesList.Add(computerPro);
                    }
                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return computerPropertiesList; 
        }
        #endregion

        #region BilgisayarArama
        /// <summary>
        /// Aktif Dizin sunucusu veritabanında bilgisayar araması için kullanılan method.Ip ya da cihaz adına göre arama yapar.
        /// </summary>
        /// <returns>Bulunan Cihazların listesini döner.</returns>
       private List<ComputersProperties> SearchSingleComputer()
        {
            List<ComputersProperties> foundComputers = new List<ComputersProperties>();
           
            try
            {
                foundComputers = SearchWithName();
            }

            catch (Exception ex)
            {
                errorMessage = CatchError(ex.Message);

            }

            return foundComputers;


        }

        private List<ComputersProperties> SearchWithName()
        {
            List<ComputersProperties> bulunanCihazlar = new List<ComputersProperties>();
            bool isContain = false; 

            for (int a = 0; a < computerList.Count; a++)
            {
                var computerPro = computerList[a];
                isContain = computerPro.computerName.Contains(_arananPc) || computerPro.ipAdress.Contains(_arananPc);

                if (isContain)
                {
                    bulunanCihazlar.Add(computerPro);
                }
            }

            return bulunanCihazlar;

        }


        #endregion


        #region IpAdresCozumle
        private string GetComputerIpAddress(string computerName)
        {
            string ipAdress = string.Empty;
            string computerDomainName = $"{computerName}.{computerDomain}";

            try
            {
                if (computerName != null)
                {
                    ipAdress = Dns.Resolve(computerDomainName).AddressList[0].ToString();
                }
                   
            }

            catch (Exception ex)
            {
                ipAdress = CatchError("İp Adresi Çözümlenemedi");

            }
            return ipAdress;

        }

        #endregion
        public string CatchError(string erorrString)
        {

            return erorrString;

        }
     

     

        #endregion
        public void ShutDownComputer(string computerName)
        {
            string _computerName = computerName;
            System.Diagnostics.ProcessStartInfo shutDownPs = new System.Diagnostics.ProcessStartInfo("shutdown");
            shutDownPs.Arguments = $"/m \\\\{_computerName} /s /t 0 ";
            System.Diagnostics.Process.Start(shutDownPs);


        }

        public void RebootComputer(string computerName)
        {
            string _computerName = computerName;
            System.Diagnostics.ProcessStartInfo rebootPs = new System.Diagnostics.ProcessStartInfo("shutdown");
            rebootPs.Arguments = $"/m \\\\{_computerName} /r /t 0";
            System.Diagnostics.Process.Start(rebootPs);
        }



    }
}
