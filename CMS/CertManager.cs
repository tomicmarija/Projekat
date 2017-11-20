using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CMS
{
    public class CertManager
    {
      
        private static string directoryName = null;
        private static string RVdirectoryName = null;
        private static string issuer = "TestCA";
        //  private static Dictionary<string,X509Certificate2> certicates=new Dictionary<string, X509Certificate2>();
        //   private static Dictionary<string, X509Certificate2> RevocationList = new Dictionary<string, X509Certificate2>();
        private static List<Certificate> certificates = new List<Certificate>();
        private static List<Certificate> RevocationList = new List<Certificate>();
        private static List<string> Users = new List<string>();
        private static List<string> RVUsers = new List<string>();

        public static string Issuer
        {
            get { return issuer; }
        }

        public static List<Certificate> Certificates
        {
            get { return certificates; }
        }

        public static List<Certificate> RevList
        {
            get { return RevocationList; }
        }



        public CertManager()
        {
            DirectorySecurity directorySecurity = new DirectorySecurity();
            IdentityReference identity = WindowsIdentity.GetCurrent().User;
            SecurityIdentifier sid = (SecurityIdentifier)identity.Translate(typeof(SecurityIdentifier));
            var name = sid.Translate(typeof(NTAccount));
            string nameStr = name.ToString();
            nameStr = nameStr.Substring(nameStr.IndexOf('\\') + 1);

            FileSystemAccessRule serverAccess = new FileSystemAccessRule(identity, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
            directorySecurity.AddAccessRule(serverAccess);
            directorySecurity.SetOwner(identity);

            try
            {
                directoryName = "Sertifikati\\";
                RVdirectoryName = "RV\\";
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName, directorySecurity);

                if (!Directory.Exists(RVdirectoryName))
                    Directory.CreateDirectory(RVdirectoryName, directorySecurity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (!File.Exists(directoryName + Issuer + ".pvk"))
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = String.Format("/c makecert -n \"CN = {0}\" -r -sv {0}.pvk {0}.cer", Issuer); ;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                Console.WriteLine(p.StandardOutput.ReadToEnd());


                File.Copy(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\" + Issuer + ".cer", directoryName + Issuer + ".cer");
                File.Copy(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\" + Issuer + ".pvk", directoryName + Issuer + ".pvk");
            }

        }



        private static int counter = 1;
        public static void CreateCertificate(string user)
        {
            if (File.Exists(directoryName + user + ".pvk"))
            {
                Audit.CreationCertificationFailed(user, "User certificates already exists");
                Console.WriteLine("ERROR!\nYou already have certificates!");
                return;
            }

            string argument = String.Format("/c makecert -sv {0}.pvk -iv TestCA.pvk -n \"CN = {0}\" -pe -ic TestCA.cer {0}.cer -sr localmachine -ss My -sky exchange & pvk2pfx.exe /pvk {0}.pvk /pi {1} /spc {0}.cer /pfx {0}.pfx", user, counter.ToString());

            Console.WriteLine("Your password is:\t" + counter.ToString());



            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = argument;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            string s = p.StandardOutput.ReadToEnd();

            if (s.Contains("Succeeded"))
            {
                Audit.CreationCertificateSuccess(user);
            }
            else
                Audit.CreationCertificationFailed(user, "Makecert failed");


            //  Console.WriteLine("Press ENTER to continue");
            //  Console.ReadLine();


            File.Move(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\" + user + ".cer", directoryName + user + ".cer");
            File.Move(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\" + user + ".pvk", directoryName + user + ".pvk");
            File.Move(@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.1A\Bin\" + user + ".pfx", directoryName + user + ".pfx");

            X509Certificate2 certificate = null;

            try
            {
                certificate = new X509Certificate2(directoryName + user + ".pfx", counter.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Erroro while trying to GetCertificateFromFile {0}. ERROR = {1}", directoryName + user + ".pfx", e.Message);
            }

            Certificate c = new Certificate();
            c.cert = certificate;
            c.name = user;
            certificates.Add(c);
            Users.Add(user);
            /*Validation val = new Validation();
            try
            {
                val.Validate(certificate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }*/

            counter++;
        }

        public static void CompromisedCert(string user)
        {
          
            if (!(certificates.Find(item => item.name == user).Equals(default(Certificate))))
            {

                foreach (Certificate c in certificates)
                {

                    if (c.name == user)
                    {


                        RevocationList.Add(c);
                        certificates.Remove(c);
                        Audit.RevocationSuccess(user);

                        RVUsers.Add(user);
                        Users.Remove(user);

                        break;
                    }
                }



                File.Move(directoryName + user + ".cer", RVdirectoryName + user + ".cer");
                File.Move(directoryName + user + ".pvk", RVdirectoryName + user + ".pvk");
                File.Move(directoryName + user + ".pfx", RVdirectoryName + user + ".pfx");

                CreateCertificate(user);



            }
            else
            {
                Audit.RevocationFailed("Certificate does not exist");
            }




        }

        public void Deserialize(string str)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(List<string>));
            using (TextReader reader = new StreamReader(str + ".xml"))
            {
                object obj = deserializer.Deserialize(reader);
                if(str=="Users")
                    Users = (List<string>)obj;
                else
                    RVUsers = (List<string>)obj;
            }
        }

        public void Serialize(string str)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            using (TextWriter textWriter = new StreamWriter(str + ".xml"))
            {
                if(str=="Users")
                    serializer.Serialize(textWriter, Users);
                else
                    serializer.Serialize(textWriter, RVUsers);
            }
        }


        public static void GetCertificateFromFile(string str)
        {
            X509Certificate2 certificate = null;

            if (str == "Users")
            {
                foreach (string u in Users)
                {
                    try
                    {
                        certificate = new X509Certificate2(string.Format(@"Sertifikati\{0}.cer", u));
                        certificates.Add(new Certificate(certificate, u));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while trying to GetCertificateFromFile {0}. ERROR = {1}", u, e.Message);
                    }
                }
            }
            else
            {
                foreach (string u in RVUsers)
                {
                    try
                    {
                        certificate = new X509Certificate2(string.Format(@"RV\{0}.cer", u));
                        RevocationList.Add(new Certificate(certificate, u));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while trying to GetCertificateFromFile {0}. ERROR = {1}", u, e.Message);
                    }
                }
            }
           
        }
    }
}
