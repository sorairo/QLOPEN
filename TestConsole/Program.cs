using System;
using System.IO;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Web;
using System.Net.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QLKFCommon;

using P11Interface;
using Net.Sf.Pkcs11;
using Net.Sf.Pkcs11.Objects;
using Net.Sf.Pkcs11.Wrapper;
using Net.Sf.Pkcs11.EtokenExtensions;

namespace TestConsole
{
    class Program
    {
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        } 

        static void Main(string[] args)
        {
            #region httpwebrequest
            //ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            //string result = string.Empty;
            //string url = string.Empty;
            //HttpWebRequest request = WebRequest.Create("https://ratest.chinaclear.cn/RaGateway/WebServiceInterface.ws?wsdl") as HttpWebRequest;
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //StreamReader reader = new StreamReader(response.GetResponseStream());
            //while (reader.EndOfStream)
            //{
            //    result = result + reader.ReadLine() + "<br />";
            //}
            //reader.Close();
            #endregion
            
            #region 新意接口查询是否已开股东号及沪市指定交易
            shine.OlCheckCtrlService.OlCheckCtrlService service = new shine.OlCheckCtrlService.OlCheckCtrlService();
            //CreateAccountLibrary.Shine.OlCheckCtrlService.olReturnMsgDTO dto = CreateAccountLibrary.Shine.Check.AddZdQueryReq("230206198805220210", "00", "李欣欣", "CHN", "QLJNMH0210");
            CreateAccountLibrary.Shine.OlCheckCtrlService.olReturnMsgDTO dto = CreateAccountLibrary.Shine.Check.AddZdQueryReq("230206198805220210", "00", "李欣欣", "CHN", "QLJNMH0210");

            if (dto != null && dto.errCode == "0")
            {
                string serialNo = dto.value;
                //CreateAccountLibrary.Shine.OlCheckCtrlService.guDongListDTO dto_serial = CreateAccountLibrary.Shine.Check.QueryZdQueryRep("230206198805220210", "00", "李欣欣", serialNo);
                CreateAccountLibrary.Shine.OlCheckCtrlService.guDongListDTO dto_serial = CreateAccountLibrary.Shine.Check.QueryZdQueryRep("230206198805220210", "00", "李欣欣", serialNo);

                CreateAccountLibrary.Shine.OlCheckCtrlService.guDongInfoDTO[] list = dto_serial.list;

                if (list != null && list.Length != 0)
                {
                    foreach (CreateAccountLibrary.Shine.OlCheckCtrlService.guDongInfoDTO info in list)
                    {
                        if (info.mktCode == "01" && info.invAcc.StartsWith("A") && info.invStatus == "0")
                        {
                            string SHSecuAcc = info.invAcc;
                            CreateAccountLibrary.Shine.OlCheckCtrlService.olReturnMsgDTO dto_zdjy = CreateAccountLibrary.Shine.Check.AddShzdjycxWt(SHSecuAcc);

                            if (dto_zdjy != null && dto_zdjy.errCode == "0")
                            {
                                string zdjy_serialNo = dto_zdjy.value;
                                CreateAccountLibrary.Shine.OlCheckCtrlService.shzdjyListDTO dto_zdjy_serial = CreateAccountLibrary.Shine.Check.QueryShzdjycxHb(zdjy_serialNo);
                            }
                        }
                    }
                }
            }
            #endregion

            #region 中登接口
            //ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            //cn.chinaclear.ratest.WebServiceInterface service = new cn.chinaclear.ratest.WebServiceInterface();
            //service.Url = "https://ratest.chinaclear.cn/RaGateway/WebServiceInterface.ws";

            //string arg0 = "<?xml version='1.0' encoding='UTF-8'?><request><caid>1</caid><transactioncode>72924634000000000003</transactioncode><parameters><accountholdername>郑超</accountholdername><accountholderabbre>zc</accountholderabbre><nationality>156</nationality><cardtype1>01</cardtype1><cardnum1>370102198308140017</cardnum1><cardissuingauthority1>济南市公安局历下分局</cardissuingauthority1><cardexpirationtime1>20151027</cardexpirationtime1><headpic></headpic><fingerprint></fingerprint><mobilephone>13701319872</mobilephone><mail>x-hiro@hotmail.com</mail><certtype>1</certtype><timestamp>20130503173000</timestamp><usbkeyid></usbkeyid><requesttype>02</requesttype><pkcs10>MIICYzCCAUsCADAfMQswCQYDVQQGEwJjbjEQMA4GA1UEAxMHRWFzeXNlYzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANKM/8eI7TCvi2zhPFefM+9hAp4WfEtcTULkywUq0xxSCynNgJr+Xr+/nVCaoTqbStjhQgNW7gXlQjiG34BWnCVrXbuHpc6jLgnPmbzMh+YeDPvFPFDVKyZlVNgLC5N4irybAKOfIjn7dPB6XHfXtgJs/cxj0KgpooYLoh7jEby6tWl5g+GKkR5dCF7bR1x61CKJrirqX8yCBQzKpghrohcHCoBcpqlmQAt1Bv30J+nfG8EvnHyPn0k2ZHEnbp+bFuZxgvrnjNK6BxZR/NydarqcD5ncmRdmgVDmYrSBeC+FTbDr6g3B6npOrvQKWsiJdpfVZ/s2PGvQeTBDIiaZZhcCAwEAAaAAMA0GCSqGSIb3DQEBBQUAA4IBAQAHZljIOR3dZlAAgOXkrTmiBUYnrE5XEm/pk8qz32tuaxDW+N9AAwl2eY4+ixUgM6Wa6fQXQmmYo3hiBa/z+BkUFfEgZsy5/Uzke4bcOTJm9fU9KDUptGUpaIiAc+qMjpjrXVOj5cCHl90pW3BOvfhTqoMIhYSwOSeYCtMlcelK211wbe2k/PJDq1gPYz1vhuGpaeIwd23VOuKSgtaaU4QqkTcSpVYDMhRyuB+Gvn1KdhPZeAwvjLQudpiM3qBLYWrY0DHg25f3LFy87aEwextmgKfmSotRWFQuLi31IxJ1xe0AfbsDO5VoKLtAvoVk9qCWjZ6qMJfdaapLwwvtKgzH</pkcs10></parameters><sign><signAlg>SHA1withRSA</signAlg><signValue>GDZg+f72FQPJz1fQVrAjAPtXCEAkzYSsPshnkCnvo0G9p0KnO8dB1RRLE6WYG2GysTQpUUWYGPut5KchrHeXzMQCZU9Jc0jOTnDTEnRApvBlIyHjOh0vznIZEvOe5wgUG0SswY4g+3kiBdAdoAP26TIlD8OGWXETp519rljPr417udrqBn5wKC2YtqwgCq9c4oRxsIhP0MbvB4uqOL0uH8R71Nn6kAWCLyhFDRox5y7o8GvCUNzQlOw8YMz+iX4KfVWNjAFQeg9cPV4nUxmvC+TwoxQL6wgqdtaYQkNWtdULtcD0fQti1rF0iOYyp3CiUfbNkY7LWw8sm3GnsSt5Eg==</signValue><signCertDN>CN=72924634@73924633,OU=Access,O=CSDC Test,C=CN</signCertDN></sign></request>";

            //string result = service.perCertRequestAndDown(arg0);
            #endregion

            #region p11加密
            //P11Interface.SignAndVerify ptest = new P11Interface.SignAndVerify();
            //string providerDll = "upkcs11.dll";
            //string keyIndex = "10";
            //string plainData = "<parameters><accountholdername>郑超</accountholdername><accountholderabbre>zc</accountholderabbre><nationality>156</nationality><cardtype1>01</cardtype1><cardnum1>370102198308140017</cardnum1><cardissuingauthority1>济南市公安局历下分局</cardissuingauthority1><cardexpirationtime1>20151027</cardexpirationtime1><headpic></headpic><fingerprint></fingerprint><mobilephone>1370131872</mobilephone><mail>x-hiro@hotmail.com</mail><certtype>1</certtype><timestamp>2013050317300000</timestamp><usbkeyid></usbkeyid><requesttype>02</requesttype><pkcs10>MIICYzCCAUsCADAfMQswCQYDVQQGEwJjbjEQMA4GA1UEAxMHRWFzeXNlYzCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBANKM/8eI7TCvi2zhPFefM+9hAp4WfEtcTULkywUq0xxSCynNgJr+Xr+/nVCaoTqbStjhQgNW7gXlQjiG34BWnCVrXbuHpc6jLgnPmbzMh+YeDPvFPFDVKyZlVNgLC5N4irybAKOfIjn7dPB6XHfXtgJs/cxj0KgpooYLoh7jEby6tWl5g+GKkR5dCF7bR1x61CKJrirqX8yCBQzKpghrohcHCoBcpqlmQAt1Bv30J+nfG8EvnHyPn0k2ZHEnbp+bFuZxgvrnjNK6BxZR/NydarqcD5ncmRdmgVDmYrSBeC+FTbDr6g3B6npOrvQKWsiJdpfVZ/s2PGvQeTBDIiaZZhcCAwEAAaAAMA0GCSqGSIb3DQEBBQUAA4IBAQAHZljIOR3dZlAAgOXkrTmiBUYnrE5XEm/pk8qz32tuaxDW+N9AAwl2eY4+ixUgM6Wa6fQXQmmYo3hiBa/z+BkUFfEgZsy5/Uzke4bcOTJm9fU9KDUptGUpaIiAc+qMjpjrXVOj5cCHl90pW3BOvfhTqoMIhYSwOSeYCtMlcelK211wbe2k/PJDq1gPYz1vhuGpaeIwd23VOuKSgtaaU4QqkTcSpVYDMhRyuB+Gvn1KdhPZeAwvjLQudpiM3qBLYWrY0DHg25f3LFy87aEwextmgKfmSotRWFQuLi31IxJ1xe0AfbsDO5VoKLtAvoVk9qCWjZ6qMJfdaapLwwvtKgzH</pkcs10></parameters>";
            //bool flag = false;
            //String b64SignData = string.Empty;

            ////Console.WriteLine("monsters-pre4");
            //ptest.testSignAndVerify(providerDll);
            //Console.WriteLine(" after testSignAndVerify ");

            //b64SignData = ptest.P11Sign(keyIndex, CKK.RSA, plainData, providerDll);
            //Console.WriteLine("b64SignData = " + b64SignData);

            //flag = ptest.P11Verify(keyIndex, CKK.RSA, plainData, b64SignData, providerDll);
            //Console.WriteLine("flag = " + flag);

            ////Console.WriteLine("monsters-post4");
            #endregion

            #region 提取P7B证书
            //P11Interface.SignAndVerify ptest = new P11Interface.SignAndVerify();
            //X509Certificate2 x509 = ptest.getCertFromP7("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");
            //byte[] rawData = x509.RawData;
            //Console.WriteLine("Content Type: {0}{1}", X509Certificate2.GetCertContentType(rawData), Environment.NewLine);
            //Console.WriteLine("SubjectName: {0}{1}", x509.SubjectName.Name, Environment.NewLine);
            //Console.WriteLine("Serial Number: {0}{1}", x509.SerialNumber, Environment.NewLine);
            //Console.WriteLine("Friendly Name: {0}{1}", x509.FriendlyName, Environment.NewLine);
            //Console.WriteLine("Certificate Verified?: {0}{1}", x509.Verify(), Environment.NewLine);
            //Console.WriteLine("Simple Name: {0}{1}", x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine);
            //Console.WriteLine("Signature Algorithm Name: {0}{1}", x509.SignatureAlgorithm.FriendlyName, Environment.NewLine);
            ////Console.WriteLine("Private Key: {0}{1}", x509.PrivateKey.ToXmlString(false), Environment.NewLine);
            //Console.WriteLine("Public Key: {0}{1}", x509.PublicKey.Key.ToXmlString(false), Environment.NewLine);
            //Console.WriteLine("Certificate Archived?: {0}{1}", x509.Archived, Environment.NewLine);
            //Console.WriteLine("Length of Raw Data: {0}{1}", x509.RawData.Length, Environment.NewLine);
            #endregion

            #region 统计模板没有开通网上交易的营业部
            //DataSet ds = new DataSet();
            //if (CreateAccountLibrary.Rules.BranchRules.GetBranchList(out ds))
            //{
            //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count != 0)
            //    {
            //        shine.OlOpenAcctCtrlService.OlOpenAcctCtrlService openAccService = new shine.OlOpenAcctCtrlService.OlOpenAcctCtrlService();
            //        shine.OlOpenAcctCtrlService.getCustDemoInfo custDemoInfo = new shine.OlOpenAcctCtrlService.getCustDemoInfo();

            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            if (i != 0 && i % 50 == 0)
            //            {
            //                Console.WriteLine(i);
            //            }

            //            custDemoInfo.arg1 = ds.Tables[0].Rows[i]["Settle_Code"].ToString();
            //            custDemoInfo.arg3 = "1";
            //            custDemoInfo.arg4 = "TRUST_MODE";

            //            shine.OlOpenAcctCtrlService.getCustDemoInfoResponse response = openAccService.getCustDemoInfo(custDemoInfo);
            //            if (!response.@return.value.Contains("网上"))
            //            {
            //                Console.WriteLine(ds.Tables[0].Rows[i]["Settle_Code"].ToString() + " -" + ds.Tables[0].Rows[i]["BranchName"].ToString());
            //            }
            //        }
            //    }
            //}
            #endregion

            #region
            //Console.WriteLine(QLKFCommon.PinYinConverter.GetFirst(""));
            //Console.WriteLine(DateTime.Now.ToFileTime());

            //StringBuilder xmlBuilder = new StringBuilder("<?xml version=“1.0” encoding=“UTF-8”?><request><caid>1</caid><transactioncode>");
            //xmlBuilder.Append("1001123443420123410222");
            //xmlBuilder.Append("</transactioncode><parameters><accountholdername>");
            //xmlBuilder.Append("郑超");
            //xmlBuilder.Append("</accountholdername><nationality>156</nationality><cardtype1>01</cardtype1><cardnum1>");
            //xmlBuilder.Append("370102198308140017");
            //xmlBuilder.Append("</cardnum1><certtype>1</certtype><timestamp>");
            //xmlBuilder.Append("20130422152408");
            //xmlBuilder.Append("</timestamp><requesttype>02</requesttype><pkcs10>");
            //xmlBuilder.Append("MIGJAoGBAJsUuq28GuDR1WpJpC/MUMwXVv7j78HE0/jkz2IKwOfljoeA8BcUSevgeWbD/XYbAVzUDgA2q3srNTd4z0FklpGqiK6Fy464tTFgum18UNfTqbHEiiHExB0z/qxeCgyR/LVpNqjXUkQH1hHOTXJDNbJpgn1Ug7ty6kBCJ5ayGJ8JAgMBAAE=");
            //xmlBuilder.Append("</pkcs10></parameters><sign><signAlg>SHA1withRSA</signAlg><signValue>");
            //xmlBuilder.Append("hk2QnDmaRCBXc1f65/N7MM8NFyWp0PC4nqZGdAnCAtHwOfn46jpPIq0KAoEYuN9dZ+SjbSv2FTkoA8dezFfskCXk1gxuurWpHIglvtD1bEk+83sNNIRHoGXQylhocd2K2KZAtLa5nuH331nmxndxT0mlX/HefoHa9Sviyd7YaEw=");
            //xmlBuilder.Append("</signValue><signCertDN>");
            //xmlBuilder.Append("CN=72924634@73924633,OU=Access,O=CSDC Test,C=CN");
            //xmlBuilder.Append("</signCertDN></sign></request>");

            //CreateAccountLibrary.Shine.OlCheckCtrlService.olReturnMsgDTO custCodeReturn = CreateAccountLibrary.Shine.Check.GetUserAccount("QLJNSD0160", "1012", "124.127.129.58", "1088");

            //byte[] bs = Encoding.UTF8.GetBytes(QLKFCommon.Cryptogram.EncryptPassword("65535#7#142857392"));
            //string encryptedID = System.BitConverter.ToString(bs);
            //Encoding.UTF8.GetBytes(encryptedID);
            
            //string oriEncryptedIDs = Encoding.UTF8.GetString(GetPreBytes(encryptedID)).Trim();
            //string s = QLKFCommon.Cryptogram.DecryptPassword(oriEncryptedIDs);

            //Console.WriteLine(encryptedID);
            //Console.WriteLine(s);

            //Console.WriteLine(Cryptogram.DecryptPassword("tPNAOG3dmYfeQQ8SySyCbYOSwqsJ+S1S01prMDmVhfM="));

            //string OriginalInput = QLKFCommon.Cryptogram.EncryptPassword("65535#7#142857392");
            //string OrignalInput = "TESTING1925#=c";
            //string OriginalInput = "r2TRFXxekJRjt5HFVMimy0t+ldDXnDry";
            //Encoding utf8Encoder = System.Text.Encoding.UTF8;
            //byte[] bytes = utf8Encoder.GetBytes(OriginalInput);

            //string bitString = BitConverter.ToString(bytes).Replace("-",string.Empty);
            //Console.WriteLine("input:" + bitString);
            //Console.WriteLine("result:" + QLKFCommon.Cryptogram.DecryptPassword(utf8Encoder.GetString(GetPreBytes(bitString))));

            //Console.WriteLine(AuditAccountLibrary.Facade.UserInfoFacade.GetUserLatestVideoPath(142394));
            
            //CADataAccess.IDInfo info;
            //string errorMessage = string.Empty;
            //CADataAccess.IDAuthCheck.AuthIDCheck("xx", "0101011901001010011", out info, out errorMessage);
            //Console.WriteLine(info.IMage);
            //Console.WriteLine(QLKFCommon.Cryptogram.DecryptPassword("KwmrimNzXPvXC87HhO4T812S9ZVTNwex"));

            //AuditAccountLibrary.Model.UserInfo user = AuditAccountLibrary.Facade.UserInfoFacade.GetUserInfoByID(142394);
            #endregion

            Console.WriteLine("F");
            Console.ReadKey();
        }

        private static byte[] GetPreBytes(string bitString)
        {
            //一个汉字对应三个byte，一个byte三个结果字符,一个汉字对应9个结果字符
            int num = bitString.Length;
            //一个char对应9个结果字符
            //int byteCount = num / 3 + 1;  //原字符串对应的结果字符数


            ////去除一个"-"字符后
            ////byte[] result = new byte[byteCount * 3];
            //byte[] result = new byte[byteCount];
            //for (int i = 0; i < byteCount; i++)
            //{
            //    char c = bitString[i * 3]; //原字符串组的第一个
            //    char c2 = bitString[i * 3 + 1]; //原字符串组的第二个

            //    result[i] = (byte)(HexToDec(c) * 0x10 + HexToDec(c2));  //除数乘以16和余数之合                
            //}

            int byteCount = num / 2;  //原字符串对应的结果字符数


            //去除一个"-"字符后
            //byte[] result = new byte[byteCount * 3];
            byte[] result = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                char c = bitString[i * 2]; //原字符串组的第一个
                char c2 = bitString[i * 2 + 1]; //原字符串组的第二个

                result[i] = (byte)(HexToDec(c) * 0x10 + HexToDec(c2));  //除数乘以16和余数之合                
            }

            return result;
        }

        //十六进制char形式转换成10进制
        private static int HexToDec(char c)
        {
            if (c <= 0x39) //小于 char '9',
            {
                return (int)c - 0x30;
            }

            return ((((int)c) - 0x41) + 10);
        }
    }
}
