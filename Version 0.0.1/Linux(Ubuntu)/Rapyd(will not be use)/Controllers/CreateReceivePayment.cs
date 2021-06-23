using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Runtime.InteropServices;
using ASodium;
using System.Text;
using MySql.Data.MySqlClient;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateReceivePayment : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("CreatePaymentRequest")]
        public CheckOutPageHolderModel CreateCheckOutPage(String ClientPathID,String CipheredSignedCountryCode) 
        {
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedCountryCode = "";
            Boolean DecodingCipheredSignedCountryCodeChecker = true;
            Boolean ConvertFromBase64CipheredSignedCountryCodeChecker = true;
            Byte[] CipheredSignedCountryCodeByte = new Byte[] { };
            Boolean VerifyCipheredCountryCodeByteChecker = true;
            Byte[] CipheredCountryCodeByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            String CountryCode = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            Boolean AbleToDecryptCipheredCountryCode = true;
            CheckOutPageHolderModel PageHolder = new CheckOutPageHolderModel();
            RapydApiRequest.RapydRequiredDataModel RequiredData = new RapydApiRequest.RapydRequiredDataModel();
            GCHandle MyGeneralGCHandle = new GCHandle();
            String Path = "{Path that stores ephemeral TLS data}";
            Path += ClientPathID;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedCountryCodeChecker,ref DecodedCipheredSignedCountryCode,CipheredSignedCountryCode);
                    if (DecodingCipheredSignedCountryCodeChecker == true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedCountryCodeChecker, ref CipheredSignedCountryCodeByte, DecodedCipheredSignedCountryCode);
                        if (ConvertFromBase64CipheredSignedCountryCodeChecker == true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredCountryCodeByteChecker, ref CipheredCountryCodeByte, CipheredSignedCountryCodeByte, ClientECDSAPKByte);
                            if (VerifyCipheredCountryCodeByteChecker == true) 
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredCountryCodeByte.Length - NonceByte.Length];
                                Array.Copy(CipheredCountryCodeByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredCountryCodeByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try 
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    AbleToDecryptCipheredCountryCode = false;
                                    MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                    MyGeneralGCHandle.Free();
                                    PageHolder.Status = "Error: Unable to decrypt data with established ETLS shared secret...(unable to decrypt country code)";
                                    PageHolder.CheckOutPageUrl = "Not Valid";
                                    PageHolder.CheckOutPageID = "Not Valid";
                                    return PageHolder;
                                }
                                if (AbleToDecryptCipheredCountryCode == true) 
                                {
                                    CountryCode = Encoding.UTF8.GetString(PlainText);
                                    RequiredData = RapydApiRequest.RapydCreatePayment.CreatePaymentRequest(CountryCode);
                                    if (RequiredData == null)
                                    {
                                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                        MyGeneralGCHandle.Free();
                                        PageHolder.Status = "Error: Unable to create checkout page URL and ID";
                                        PageHolder.CheckOutPageUrl = "Not Valid";
                                        PageHolder.CheckOutPageID = "Not Valid";
                                        return PageHolder;
                                    }
                                    else
                                    {
                                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                        MyGeneralGCHandle.Free();
                                        PageHolder.Status = "Successed: Successfully created Checkout page ID and URL";
                                        PageHolder.CheckOutPageID = RequiredData.ID;
                                        PageHolder.CheckOutPageUrl = RequiredData.Redirect_Url;
                                        return PageHolder;
                                    }
                                }
                                else 
                                {
                                    PageHolder.Status = "Error: Unable to decrypt data with established ETLS shared secret...";
                                    PageHolder.CheckOutPageUrl = "Not Valid";
                                    PageHolder.CheckOutPageID = "Not Valid";
                                    return PageHolder;
                                }                              
                            }
                            else
                            {
                                MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                MyGeneralGCHandle.Free();
                                PageHolder.Status = "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established between client and server?";
                                PageHolder.CheckOutPageUrl = "Not Valid";
                                PageHolder.CheckOutPageID = "Not Valid";
                                return PageHolder;
                            }
                        }
                        else
                        {
                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                            MyGeneralGCHandle.Free();
                            PageHolder.Status = "Error: Please pass in correct Base64 encoded String in parameter";
                            PageHolder.CheckOutPageUrl = "Not Valid";
                            PageHolder.CheckOutPageID = "Not Valid";
                            return PageHolder;
                        }
                    }
                    else 
                    {
                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                        MyGeneralGCHandle.Free();
                        PageHolder.Status = "Error: Please pass in correct URL encoded String in parameter";
                        PageHolder.CheckOutPageUrl = "Not Valid";
                        PageHolder.CheckOutPageID = "Not Valid";
                        return PageHolder;
                    }
                }
                else 
                {
                    PageHolder.Status = "Error: The specified corresponding ETLS ID does not exist";
                    PageHolder.CheckOutPageUrl = "Not Valid";
                    PageHolder.CheckOutPageID = "Not Valid";
                    return PageHolder;
                }
            }
            else 
            {
                PageHolder.Status = "Error: Client did not specify an ETLS ID";
                PageHolder.CheckOutPageUrl = "Not Valid";
                PageHolder.CheckOutPageID = "Not Valid";
                return PageHolder;
            }
        }

        [HttpGet("CheckPayment")]
        public FileCreationModel CheckPaymentAndCreateFolder(String ClientPathID, String CipheredSignedCheckOutPageID, String CipheredSignedED25519PK)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedED25519PK = "";
            Boolean DecodingCipheredSignedED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedED25519PKChecker = true;
            Byte[] CipheredSignedED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredED25519PKByteChecker = true;
            Byte[] CipheredED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedCheckOutPageID = "";
            Boolean DecodingCipheredSignedCheckOutPageIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedCheckOutPageIDChecker = true;
            Byte[] CipheredSignedCheckOutPageIDByte = new Byte[] { };
            Boolean VerifyCipheredCheckOutPageIDByteChecker = true;
            Byte[] CipheredCheckOutPageIDByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            String CheckOutPageID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            GCHandle MyGeneralGCHandle = new GCHandle();
            String Path = "{Path that stores ephemeral TLS data}";
            Path += ClientPathID;
            String FileStoragePath = "{Path that stores user subscribed file storage folder}";
            String CheckOutPagePath = "{Path that stores user paid Rapyd CheckOutPageID}";
            Boolean PaymentMade = true;
            String UniqueUserFileStorageID = IDGenerator.GenerateUniqueString();
            String UniquePaymentID = IDGenerator.GenerateUniqueString();
            DateTime DirectoryExpirationTime = DateTime.UtcNow.AddHours(8).AddDays(30);
            DateTime DBExpirationTime = DirectoryExpirationTime;
            int Count = 0;
            FileCreationModel FileCreationDataHolder = new FileCreationModel();
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedCheckOutPageIDChecker, ref DecodedCipheredSignedCheckOutPageID, CipheredSignedCheckOutPageID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedED25519PKChecker, ref DecodedCipheredSignedED25519PK, CipheredSignedED25519PK);
                    if (DecodingCipheredSignedCheckOutPageIDChecker == true && DecodingCipheredSignedED25519PKChecker==true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedCheckOutPageIDChecker, ref CipheredSignedCheckOutPageIDByte, DecodedCipheredSignedCheckOutPageID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedED25519PKChecker, ref CipheredSignedED25519PKByte, DecodedCipheredSignedED25519PK);
                        if (ConvertFromBase64CipheredSignedCheckOutPageIDChecker == true && ConvertFromBase64CipheredSignedED25519PKChecker==true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredCheckOutPageIDByteChecker, ref CipheredCheckOutPageIDByte, CipheredSignedCheckOutPageIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredED25519PKByteChecker, ref CipheredED25519PKByte, CipheredSignedED25519PKByte, ClientECDSAPKByte);
                            if (VerifyCipheredCheckOutPageIDByteChecker == true && VerifyCipheredED25519PKByteChecker==true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredCheckOutPageIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredCheckOutPageIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredCheckOutPageIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                    MyGeneralGCHandle.Free();
                                    FileCreationDataHolder.FolderID = "Error: Not Valid";
                                    FileCreationDataHolder.PaymentID = "Error: Not Valid";
                                    FileCreationDataHolder.Status = "Error: Unable to decrypt specified checkout page..";
                                    return FileCreationDataHolder;
                                }
                                CheckOutPageID = Encoding.UTF8.GetString(PlainText);
                                if (Directory.Exists(CheckOutPagePath + CheckOutPageID) == true)
                                {
                                    FileCreationDataHolder.FolderID = "Error: Not Valid";
                                    FileCreationDataHolder.PaymentID = "Error: Not Valid";
                                    FileCreationDataHolder.Status = "Error: The specified checkout page have been created..";
                                    return FileCreationDataHolder;
                                }
                                else
                                {
                                    PaymentMade = RapydApiRequest.RapydCheckPaymentHasMade.CheckPaymentHasMade(CheckOutPageID);
                                    if (PaymentMade == true)
                                    {
                                        PlainText = new Byte[] { };
                                        NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                        CipheredText = new Byte[CipheredED25519PKByte.Length - NonceByte.Length];
                                        Array.Copy(CipheredED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                        Array.Copy(CipheredED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                        try
                                        {
                                            PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                        }
                                        catch
                                        {
                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                            MyGeneralGCHandle.Free();
                                            FileCreationDataHolder.FolderID = "Error: Not Valid";
                                            FileCreationDataHolder.PaymentID = "Error: Not Valid";
                                            FileCreationDataHolder.Status = "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established between server and client?(Unable to decrypt new ED25519PK)";
                                            return FileCreationDataHolder;
                                        }
                                        if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                        {
                                            Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID);
                                        }
                                        else
                                        {
                                            while (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == true)
                                            {
                                                UniqueUserFileStorageID = "";
                                                UniqueUserFileStorageID = IDGenerator.GenerateUniqueString();
                                            }
                                            Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID);
                                        }
                                        System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", PlainText);
                                        System.IO.File.SetCreationTime(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", DirectoryExpirationTime);
                                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Purchase_Records` WHERE `ID`=@ID";
                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        while (Count != 0)
                                        {
                                            UniquePaymentID = "";
                                            UniquePaymentID = IDGenerator.GenerateUniqueString();
                                            Count = 0;
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Purchase_Records` WHERE `ID`=@ID";
                                            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        }
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `Purchase_Records`(`ID`, `Expiration_Date`) VALUES (@ID,@Expiration_Date)";
                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                        MySQLGeneralQuery.Parameters.Add("@Expiration_Date", MySqlDbType.DateTime).Value = DBExpirationTime;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        FileCreationDataHolder.FolderID = UniqueUserFileStorageID;
                                        FileCreationDataHolder.PaymentID = UniquePaymentID;
                                        FileCreationDataHolder.Status = "Successed: Payment has been made and corresponding IDs have been sent back";
                                        MyGeneralGCHandle = GCHandle.Alloc(PlainText, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), PlainText.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                        MyGeneralGCHandle.Free();
                                        Directory.CreateDirectory(CheckOutPagePath + CheckOutPageID);
                                        return FileCreationDataHolder;
                                    }
                                    else
                                    {
                                        FileCreationDataHolder.FolderID = "Error: Not Valid";
                                        FileCreationDataHolder.PaymentID = "Error: Not Valid";
                                        FileCreationDataHolder.Status = "Error: Have you really made payment?";
                                        return FileCreationDataHolder;
                                    }
                                }
                            }
                            else
                            {
                                MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                MyGeneralGCHandle.Free();
                                FileCreationDataHolder.FolderID = "Error: Not Valid";
                                FileCreationDataHolder.PaymentID = "Error: Not Valid";
                                FileCreationDataHolder.Status = "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established being server and client?";
                                return FileCreationDataHolder;
                            }
                        }
                        else
                        {
                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                            MyGeneralGCHandle.Free();
                            FileCreationDataHolder.FolderID = "Error: Not Valid";
                            FileCreationDataHolder.PaymentID = "Error: Not Valid";
                            FileCreationDataHolder.Status = "Error: Client did not pass in correct Base64 encoded String in parameter..";
                            return FileCreationDataHolder;
                        }
                    }
                    else
                    {
                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                        MyGeneralGCHandle.Free();
                        FileCreationDataHolder.FolderID = "Error: Not Valid";
                        FileCreationDataHolder.PaymentID = "Error: Not Valid";
                        FileCreationDataHolder.Status = "Error: Client did not pass in correct URL encoded String in parameter..";
                        return FileCreationDataHolder;
                    }
                }
                else
                {
                    FileCreationDataHolder.FolderID = "Error: Not Valid";
                    FileCreationDataHolder.PaymentID = "Error: Not Valid";
                    FileCreationDataHolder.Status = "Error: The specified ETLS ID does not exists...";
                    return FileCreationDataHolder;
                }
            }
            else
            {
                FileCreationDataHolder.FolderID = "Error: Not Valid";
                FileCreationDataHolder.PaymentID = "Error: Not Valid";
                FileCreationDataHolder.Status = "Error: Client did not specify the ETLS ID...";
                return FileCreationDataHolder;
            }
        }

        [HttpGet("RenewPayment")]
        public String RenewPayment(String ClientPathID, String SignedUserID ,String CipheredSignedCheckOutPageID, String CipheredSignedDirectoryID , String CipheredSignedPaymentID , String SignedSignedRandomChallenge ,String CipheredSignedED25519PK)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedED25519PK = "";
            Boolean DecodingCipheredSignedED25519PKChecker = true;
            Boolean ConvertFromBase64CipheredSignedED25519PKChecker = true;
            Byte[] CipheredSignedED25519PKByte = new Byte[] { };
            Boolean VerifyCipheredED25519PKByteChecker = true;
            Byte[] CipheredED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedDirectoryID = "";
            Boolean DecodingCipheredSignedDirectoryIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedDirectoryIDChecker = true;
            Byte[] CipheredSignedDirectoryIDByte = new Byte[] { };
            Boolean VerifyCipheredDirectoryIDByteChecker = true;
            Byte[] CipheredDirectoryIDByte = new Byte[] { };
            String DecodedCipheredSignedPaymentID = "";
            Boolean DecodingCipheredSignedPaymentIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedPaymentIDChecker = true;
            Byte[] CipheredSignedPaymentIDByte = new Byte[] { };
            Boolean VerifyCipheredPaymentIDByteChecker = true;
            Byte[] CipheredPaymentIDByte = new Byte[] { };
            String DecodedCipheredSignedCheckOutPageID = "";
            Boolean DecodingCipheredSignedCheckOutPageIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedCheckOutPageIDChecker = true;
            Byte[] CipheredSignedCheckOutPageIDByte = new Byte[] { };
            Boolean VerifyCipheredCheckOutPageIDByteChecker = true;
            Byte[] CipheredCheckOutPageIDByte = new Byte[] { };
            String DecodedSignedUserID = "";
            Boolean DecodingSignedUserIDChecker = true;
            Boolean ConvertFromBase64SignedUserIDChecker = true;
            Byte[] SignedUserIDByte = new Byte[] { };
            Boolean VerifyUserIDChecker = true;
            Byte[] UserIDByte = new Byte[] { };
            String UserID = "";
            String DecodedSignedSignedRandomChallenge = "";
            Boolean DecodingSignedSignedRandomChallengeChecker = true;
            Boolean ConvertFromBase64SignedSignedRandomChallengeChecker = true;
            Byte[] SignedSignedRandomChallengeByte = new Byte[] { };
            Boolean VerifySignedRandomChallengeChecker = true;
            Byte[] SignedRandomChallengeByte = new Byte[] { };
            Byte[] RandomChallengeByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            String CheckOutPageID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            GCHandle MyGeneralGCHandle = new GCHandle();
            String Path = "{Path that stores ephemeral TLS data}";
            Path += ClientPathID;
            String FileStoragePath = "{Path that stores user subscribed file storage folder}";
            String CheckOutPagePath = "{Path that stores user paid Rapyd CheckOutPageID}";
            Boolean PaymentMade = true;
            String UniqueUserFileStorageID = "";
            String UniquePaymentID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryExpirationTime = new DateTime();
            DateTime DBExpirationTime = DirectoryExpirationTime;
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedCheckOutPageIDChecker, ref DecodedCipheredSignedCheckOutPageID, CipheredSignedCheckOutPageID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedED25519PKChecker, ref DecodedCipheredSignedED25519PK, CipheredSignedED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedPaymentIDChecker, ref DecodedCipheredSignedPaymentID, CipheredSignedPaymentID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if (DecodingSignedUserIDChecker == true && DecodingCipheredSignedCheckOutPageIDChecker == true && DecodingCipheredSignedED25519PKChecker == true && DecodingCipheredSignedDirectoryIDChecker==true && DecodingCipheredSignedPaymentIDChecker==true && DecodingSignedSignedRandomChallengeChecker==true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedCheckOutPageIDChecker, ref CipheredSignedCheckOutPageIDByte, DecodedCipheredSignedCheckOutPageID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedED25519PKChecker, ref CipheredSignedED25519PKByte, DecodedCipheredSignedED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedPaymentIDChecker, ref CipheredSignedPaymentIDByte, DecodedCipheredSignedPaymentID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if (ConvertFromBase64SignedUserIDChecker == true && ConvertFromBase64CipheredSignedCheckOutPageIDChecker == true && ConvertFromBase64CipheredSignedED25519PKChecker == true && ConvertFromBase64CipheredSignedDirectoryIDChecker==true && ConvertFromBase64CipheredSignedPaymentIDChecker==true && ConvertFromBase64SignedSignedRandomChallengeChecker==true)
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredCheckOutPageIDByteChecker, ref CipheredCheckOutPageIDByte, CipheredSignedCheckOutPageIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredED25519PKByteChecker, ref CipheredED25519PKByte, CipheredSignedED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredPaymentIDByteChecker, ref CipheredPaymentIDByte, CipheredSignedPaymentIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if (VerifyUserIDChecker == true && VerifyCipheredCheckOutPageIDByteChecker == true && VerifyCipheredED25519PKByteChecker == true && VerifyCipheredDirectoryIDByteChecker==true && VerifyCipheredPaymentIDByteChecker==true && VerifySignedRandomChallengeChecker==true)
                            {
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredCheckOutPageIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredCheckOutPageIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredCheckOutPageIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                    MyGeneralGCHandle.Free();
                                    return "Error: Unable to decrypt ETLS signed encrypted checkout page..";
                                }
                                CheckOutPageID = Encoding.UTF8.GetString(PlainText);
                                if (Directory.Exists(CheckOutPagePath + CheckOutPageID) == true)
                                {
                                    return "Error: This checkout page have been used...";
                                }
                                else
                                {
                                    PaymentMade = RapydApiRequest.RapydCheckPaymentHasMade.CheckPaymentHasMade(CheckOutPageID);
                                    if (PaymentMade == true)
                                    {
                                        PlainText = new Byte[] { };
                                        NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                        CipheredText = new Byte[CipheredDirectoryIDByte.Length - NonceByte.Length];
                                        Array.Copy(CipheredDirectoryIDByte, 0, NonceByte, 0, NonceByte.Length);
                                        Array.Copy(CipheredDirectoryIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                        try
                                        {
                                            PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                        }
                                        catch
                                        {
                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                            MyGeneralGCHandle.Free();
                                            return "Error: Unable to decrypt ETLS signed encrypted directory ID..";
                                        }
                                        UniqueUserFileStorageID = Encoding.UTF8.GetString(PlainText);
                                        PlainText = new Byte[] { };
                                        NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                        CipheredText = new Byte[CipheredPaymentIDByte.Length - NonceByte.Length];
                                        Array.Copy(CipheredPaymentIDByte, 0, NonceByte, 0, NonceByte.Length);
                                        Array.Copy(CipheredPaymentIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                        try
                                        {
                                            PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                        }
                                        catch
                                        {
                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                            MyGeneralGCHandle.Free();
                                            return "Error: Unable to decrypt ETLS signed encrypted payment ID..";
                                        }
                                        UniquePaymentID = Encoding.UTF8.GetString(PlainText);
                                        try 
                                        {
                                            FileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                        }
                                        catch
                                        {
                                            MyGeneralGCHandle = GCHandle.Alloc(PlainText, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), PlainText.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(SignedUserID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedUserID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedUserID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(SignedUserIDByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserIDByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(UserIDByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserIDByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(UserID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedPaymentID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedPaymentID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentIDByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentIDByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredPaymentIDByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredPaymentIDByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(UniquePaymentID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniquePaymentID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedDirectoryID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedDirectoryID.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryIDByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryIDByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredDirectoryIDByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDirectoryIDByte.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(UniqueUserFileStorageID, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniqueUserFileStorageID.Length);
                                            MyGeneralGCHandle.Free();
                                            return "Error: Unable to find corresponding ED25519 PK for this particular directory..";
                                        }
                                        PlainText = new Byte[] { };
                                        NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                        CipheredText = new Byte[CipheredED25519PKByte.Length - NonceByte.Length];
                                        Array.Copy(CipheredED25519PKByte, 0, NonceByte, 0, NonceByte.Length);
                                        Array.Copy(CipheredED25519PKByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                        try
                                        {
                                            PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                        }
                                        catch
                                        {
                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                            MyGeneralGCHandle.Free();
                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                            MyGeneralGCHandle.Free();
                                            return "Error: Unable to decrypt ETLS signed encrypted new ED25519 PK..";
                                        }
                                        if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                        {
                                            return "Error: Unable to find corresponding user directory...";
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                            if (Count == 1) 
                                            {
                                                try 
                                                {
                                                    RandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, FileED25519PK);
                                                }
                                                catch 
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                                    MyGeneralGCHandle.Free();
                                                    MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                                    MyGeneralGCHandle.Free();
                                                    return "Error: The signed random challenge can't be verified with the specific directory ED25519 PK";
                                                }
                                                MySQLGeneralQuery = new MySqlCommand();
                                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = "Renew Payment";
                                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                MySQLGeneralQuery.Prepare();
                                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                                if (Count == 1) 
                                                {
                                                    MySQLGeneralQuery = new MySqlCommand();
                                                    MySQLGeneralQuery.CommandText = "SELECT `Valid_Duration` FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                    MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                    MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = "Renew Payment";
                                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                    MySQLGeneralQuery.Prepare();
                                                    DBDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                                    Duration = MyUTC8DateTime.Subtract(DBDateTime);
                                                    if (Duration.TotalMinutes < 8)
                                                    {
                                                        MySQLGeneralQuery = new MySqlCommand();
                                                        MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                        MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = "Renew Payment";
                                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                        MySQLGeneralQuery.Prepare();
                                                        MySQLGeneralQuery.ExecuteNonQuery();
                                                        MySQLGeneralQuery = new MySqlCommand();
                                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Purchase_Records` WHERE `ID`=@ID";
                                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                        MySQLGeneralQuery.Prepare();
                                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                                        if (Count == 1)
                                                        {
                                                            MySQLGeneralQuery = new MySqlCommand();
                                                            MySQLGeneralQuery.CommandText = "SELECT `Expiration_Date` FROM `Purchase_Records` WHERE `ID`=@ID";
                                                            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                            MySQLGeneralQuery.Prepare();
                                                            DirectoryExpirationTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                                            if (MyUTC8DateTime > DirectoryExpirationTime) 
                                                            {
                                                                DirectoryExpirationTime = MyUTC8DateTime;
                                                            }
                                                            DirectoryExpirationTime = DirectoryExpirationTime.AddDays(30);
                                                            System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", PlainText);
                                                            System.IO.File.SetCreationTime(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", DirectoryExpirationTime);
                                                            DBExpirationTime = DirectoryExpirationTime;
                                                            MySQLGeneralQuery = new MySqlCommand();
                                                            MySQLGeneralQuery.CommandText = "UPDATE `Purchase_Records` SET `Expiration_Date`=@Expiration_Date WHERE `ID`=@ID";
                                                            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = UniquePaymentID;
                                                            MySQLGeneralQuery.Parameters.Add("@Expiration_Date", MySqlDbType.DateTime).Value = DBExpirationTime;
                                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                            MySQLGeneralQuery.Prepare();
                                                            MySQLGeneralQuery.ExecuteNonQuery();
                                                            MyGeneralGCHandle = GCHandle.Alloc(PlainText, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), PlainText.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedUserID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedUserID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedUserID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedUserIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UserIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UserID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedPaymentID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedPaymentID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredPaymentIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredPaymentIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UniquePaymentID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniquePaymentID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedDirectoryID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedDirectoryID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredDirectoryIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDirectoryIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UniqueUserFileStorageID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniqueUserFileStorageID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                            Directory.CreateDirectory(CheckOutPagePath + CheckOutPageID);
                                                            return "Successed: Payment has been renewed.. and ED25519 PK of the directory has been updated";
                                                        }
                                                        else
                                                        {
                                                            MyGeneralGCHandle = GCHandle.Alloc(PlainText, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), PlainText.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedUserID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedUserID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedUserID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(SignedUserIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UserIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UserID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedPaymentID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedPaymentID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredPaymentIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredPaymentIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UniquePaymentID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniquePaymentID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedDirectoryID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedDirectoryID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(CipheredDirectoryIDByte, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDirectoryIDByte.Length);
                                                            MyGeneralGCHandle.Free();
                                                            MyGeneralGCHandle = GCHandle.Alloc(UniqueUserFileStorageID, GCHandleType.Pinned);
                                                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniqueUserFileStorageID.Length);
                                                            MyGeneralGCHandle.Free();
                                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                            return "Error: This payment ID does not exists...";
                                                        }
                                                    }
                                                    else 
                                                    {
                                                        MySQLGeneralQuery = new MySqlCommand();
                                                        MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                        MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = "Renew Payment";
                                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                        MySQLGeneralQuery.Prepare();
                                                        MySQLGeneralQuery.ExecuteNonQuery();
                                                        myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                        MyGeneralGCHandle = GCHandle.Alloc(PlainText, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), PlainText.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(SignedUserID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedUserID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedUserID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(SignedUserIDByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedUserIDByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(UserIDByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserIDByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(UserID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UserID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedPaymentID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedPaymentID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedPaymentIDByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedPaymentIDByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredPaymentIDByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredPaymentIDByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(UniquePaymentID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniquePaymentID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(DecodedCipheredSignedDirectoryID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedCipheredSignedDirectoryID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredSignedDirectoryIDByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredSignedDirectoryIDByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(CipheredDirectoryIDByte, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), CipheredDirectoryIDByte.Length);
                                                        MyGeneralGCHandle.Free();
                                                        MyGeneralGCHandle = GCHandle.Alloc(UniqueUserFileStorageID, GCHandleType.Pinned);
                                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), UniqueUserFileStorageID.Length);
                                                        MyGeneralGCHandle.Free();
                                                        return "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                                    }
                                                }
                                                else 
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    return "Error: This random challenge does not exists in the system..";
                                                }
                                            }
                                            else 
                                            {
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                return "Error: This user ID does not exist..";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return "Error: You haven't made payment.";
                                    }
                                }
                            }
                            else
                            {
                                MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                                MyGeneralGCHandle.Free();
                                MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                                SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                                MyGeneralGCHandle.Free();
                                return "Error: Man in the middle spotted, are you an imposter trying to get over the ETLS?";
                            }
                        }
                        else
                        {
                            MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                            MyGeneralGCHandle.Free();
                            MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                            MyGeneralGCHandle.Free();
                            return "Error: Please pass in correct Base64 Encoded String in the parameter..";
                        }
                    }
                    else
                    {
                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallenge, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallenge.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(DecodedSignedSignedRandomChallenge, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), DecodedSignedSignedRandomChallenge.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(SignedSignedRandomChallengeByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedSignedRandomChallengeByte.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(SignedRandomChallengeByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SignedRandomChallengeByte.Length);
                        MyGeneralGCHandle.Free();
                        MyGeneralGCHandle = GCHandle.Alloc(RandomChallengeByte, GCHandleType.Pinned);
                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), RandomChallengeByte.Length);
                        MyGeneralGCHandle.Free();
                        return "Error: Please pass in correct URL Encoded String in the parameter..";
                    }
                }
                else
                {
                    return "Error: The corresponding ETLS ID does not exists in the server..";
                }
            }
            else
            {
                return "Error: The ETLS ID mustn't be null";
            }
        }
    }
}
