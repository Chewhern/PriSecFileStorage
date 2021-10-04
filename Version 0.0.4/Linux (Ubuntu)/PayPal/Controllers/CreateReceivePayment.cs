using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using ASodium;
using System.Text;
using MySql.Data.MySqlClient;
using PayPalCheckoutSdk.Orders;
using PriSecFileStorageAPI.Helper;
using PriSecFileStorageAPI.Model;


namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateReceivePayment : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();
        private CryptographicSecureIDGenerator MyIDGenerator = new CryptographicSecureIDGenerator();

        [HttpGet]
        public CheckOutPageHolderModel CreateCheckOutPage() 
        {
            Double OriginalPrice = 0.00;
            Double HandlingPrice = 1.00;
            Double TotalPrice = 0.00;
            Double ItemPrice = 0.00;
            String ItemDescription = "";
            String MyInvoiceID = MyIDGenerator.GenerateUniqueString();
            String OrderID = "";
            String RedirectLink = "";
            OriginalPrice = 4.00;
            ItemPrice = OriginalPrice;
            TotalPrice = OriginalPrice + HandlingPrice;
            ItemDescription = "1 GB storage";
            var CreateOrderResponse = PayPalCreateOrder.CreateOrder(MyInvoiceID, TotalPrice, OriginalPrice, HandlingPrice, ItemPrice, ItemDescription).Result;
            var createOrderResult = CreateOrderResponse.Result<Order>();
            OrderID = createOrderResult.Id;
            foreach (LinkDescription link in createOrderResult.Links)
            {
                if (link.Rel.CompareTo("approve") == 0)
                {
                    RedirectLink = link.Href;
                }
            }
            CheckOutPageHolderModel PageHolder = new CheckOutPageHolderModel();
            PageHolder.PayPalOrderID = OrderID;
            PageHolder.CheckOutPageUrl = RedirectLink;
            PageHolder.InvoiceID = MyInvoiceID;
            return PageHolder;
        }

        [HttpGet("CheckPayment")]
        public FileCreationModel CheckPaymentAndCreateFolder(String ClientPathID, String CipheredSignedOrderID, String CipheredSignedED25519PK)
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
            Byte[] ED25519PKByte = new Byte[] { };
            Byte[] SignedED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedOrderID = "";
            Boolean DecodingCipheredSignedOrderIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedOrderIDChecker = true;
            Byte[] CipheredSignedOrderIDByte = new Byte[] { };
            Boolean VerifyCipheredOrderIDByteChecker = true;
            Byte[] CipheredOrderIDByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            String OrderID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            Boolean PaymentMade = true;
            String UniqueUserFileStorageID = IDGenerator.GenerateUniqueString();
            DateTime DirectoryExpirationTime = DateTime.UtcNow.AddHours(8).AddDays(30);
            DateTime DBExpirationTime = DirectoryExpirationTime;
            FileCreationModel FileCreationDataHolder = new FileCreationModel();
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedOrderIDChecker, ref DecodedCipheredSignedOrderID, CipheredSignedOrderID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedED25519PKChecker, ref DecodedCipheredSignedED25519PK, CipheredSignedED25519PK);
                    if (DecodingCipheredSignedOrderIDChecker == true && DecodingCipheredSignedED25519PKChecker==true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedOrderIDChecker, ref CipheredSignedOrderIDByte, DecodedCipheredSignedOrderID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedED25519PKChecker, ref CipheredSignedED25519PKByte, DecodedCipheredSignedED25519PK);
                        if (ConvertFromBase64CipheredSignedOrderIDChecker == true && ConvertFromBase64CipheredSignedED25519PKChecker==true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredOrderIDByteChecker, ref CipheredOrderIDByte, CipheredSignedOrderIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredED25519PKByteChecker, ref CipheredED25519PKByte, CipheredSignedED25519PKByte, ClientECDSAPKByte);
                            if (VerifyCipheredOrderIDByteChecker == true && VerifyCipheredED25519PKByteChecker==true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredOrderIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredOrderIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredOrderIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    FileCreationDataHolder.FolderID = "Error: Not Valid";
                                    FileCreationDataHolder.Status = "Error: Unable to decrypt specified order ID..";
                                    return FileCreationDataHolder;
                                }
                                OrderID = Encoding.UTF8.GetString(PlainText);
                                try 
                                {
                                    var captureOrderResponse = PaypalCaptureOrder.CaptureOrder(OrderID).Result;
                                    var captureOrderResult = captureOrderResponse.Result<Order>();
                                    var captureId = "";
                                    foreach (PurchaseUnit purchaseUnit in captureOrderResult.PurchaseUnits)
                                    {
                                        foreach (Capture capture in purchaseUnit.Payments.Captures)
                                        {
                                            captureId = capture.Id;
                                        }
                                    }
                                    if (captureId.CompareTo("") == 0) 
                                    {
                                        PaymentMade = false;
                                    }
                                }
                                catch 
                                {
                                    PaymentMade = false;
                                }
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
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        FileCreationDataHolder.FolderID = "Error: Not Valid";
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
                                    if (PlainText.Length != 128) 
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        FileCreationDataHolder.FolderID = "Error: Not Valid";
                                        FileCreationDataHolder.Status = "Error: You can only pass in public key that is 128 bytes, the first 32 bytes represents the public key, the latter 96 bytes represents the signed public key";
                                        return FileCreationDataHolder;
                                    }
                                    ED25519PKByte = new Byte[32];
                                    SignedED25519PKByte = new Byte[96];
                                    Array.Copy(PlainText, 0, ED25519PKByte, 0, 32);
                                    Array.Copy(PlainText, 32, SignedED25519PKByte, 0, 96);
                                    System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", ED25519PKByte);
                                    System.IO.File.SetCreationTime(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", DirectoryExpirationTime);
                                    System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/rootSPK.txt", SignedED25519PKByte);
                                    System.IO.File.SetCreationTime(FileStoragePath + UniqueUserFileStorageID + "/rootSPK.txt", DirectoryExpirationTime);
                                    myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                    FileCreationDataHolder.FolderID = UniqueUserFileStorageID;
                                    FileCreationDataHolder.Status = "Successed: Payment has been made and corresponding IDs have been sent back";
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return FileCreationDataHolder;
                                }
                                else
                                {
                                    FileCreationDataHolder.FolderID = "Error: Not Valid";
                                    FileCreationDataHolder.Status = "Error: Have you really made payment?";
                                    return FileCreationDataHolder;
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                FileCreationDataHolder.FolderID = "Error: Not Valid";
                                FileCreationDataHolder.Status = "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established being server and client?";
                                return FileCreationDataHolder;
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            FileCreationDataHolder.FolderID = "Error: Not Valid";
                            FileCreationDataHolder.Status = "Error: Client did not pass in correct Base64 encoded String in parameter..";
                            return FileCreationDataHolder;
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        FileCreationDataHolder.FolderID = "Error: Not Valid";
                        FileCreationDataHolder.Status = "Error: Client did not pass in correct URL encoded String in parameter..";
                        return FileCreationDataHolder;
                    }
                }
                else
                {
                    FileCreationDataHolder.FolderID = "Error: Not Valid";
                    FileCreationDataHolder.Status = "Error: The specified ETLS ID does not exists...";
                    return FileCreationDataHolder;
                }
            }
            else
            {
                FileCreationDataHolder.FolderID = "Error: Not Valid";
                FileCreationDataHolder.Status = "Error: Client did not specify the ETLS ID...";
                return FileCreationDataHolder;
            }
        }

        [HttpGet("RenewPayment")]
        public String RenewPayment(String ClientPathID, String CipheredSignedOrderID, String CipheredSignedDirectoryID , String SignedSignedRandomChallenge ,String CipheredSignedED25519PK)
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
            Byte[] ED25519PKByte = new Byte[] { };
            Byte[] SignedED25519PKByte = new Byte[] { };
            String DecodedCipheredSignedDirectoryID = "";
            Boolean DecodingCipheredSignedDirectoryIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedDirectoryIDChecker = true;
            Byte[] CipheredSignedDirectoryIDByte = new Byte[] { };
            Boolean VerifyCipheredDirectoryIDByteChecker = true;
            Byte[] CipheredDirectoryIDByte = new Byte[] { };
            String DecodedCipheredSignedOrderID = "";
            Boolean DecodingCipheredSignedOrderIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedOrderIDChecker = true;
            Byte[] CipheredSignedOrderIDByte = new Byte[] { };
            Boolean VerifyCipheredOrderIDByteChecker = true;
            Byte[] CipheredOrderIDByte = new Byte[] { };
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
            String OrderID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            Boolean PaymentMade = true;
            String UniqueUserFileStorageID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryExpirationTime = new DateTime();
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            Byte[] SignedFileED25519PK = new Byte[] { };
            Byte[] TestFileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedOrderIDChecker, ref DecodedCipheredSignedOrderID, CipheredSignedOrderID);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedED25519PKChecker, ref DecodedCipheredSignedED25519PK, CipheredSignedED25519PK);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if (DecodingCipheredSignedOrderIDChecker == true && DecodingCipheredSignedED25519PKChecker == true && DecodingCipheredSignedDirectoryIDChecker==true && DecodingSignedSignedRandomChallengeChecker==true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedOrderIDChecker, ref CipheredSignedOrderIDByte, DecodedCipheredSignedOrderID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedED25519PKChecker, ref CipheredSignedED25519PKByte, DecodedCipheredSignedED25519PK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if (ConvertFromBase64CipheredSignedOrderIDChecker == true && ConvertFromBase64CipheredSignedED25519PKChecker == true && ConvertFromBase64CipheredSignedDirectoryIDChecker==true && ConvertFromBase64SignedSignedRandomChallengeChecker==true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredOrderIDByteChecker, ref CipheredOrderIDByte, CipheredSignedOrderIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredED25519PKByteChecker, ref CipheredED25519PKByte, CipheredSignedED25519PKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if (VerifyCipheredOrderIDByteChecker == true && VerifyCipheredED25519PKByteChecker == true && VerifyCipheredDirectoryIDByteChecker==true && VerifySignedRandomChallengeChecker==true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                CipheredText = new Byte[CipheredOrderIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredOrderIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredOrderIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
                                try
                                {
                                    PlainText = SodiumSecretBox.Open(CipheredText, NonceByte, SharedSecret);
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Unable to decrypt ETLS signed order ID..";
                                }
                                OrderID = Encoding.UTF8.GetString(PlainText);
                                try
                                {
                                    var captureOrderResponse = PaypalCaptureOrder.CaptureOrder(OrderID).Result;
                                    var captureOrderResult = captureOrderResponse.Result<Order>();
                                    var captureId = "";
                                    foreach (PurchaseUnit purchaseUnit in captureOrderResult.PurchaseUnits)
                                    {
                                        foreach (Capture capture in purchaseUnit.Payments.Captures)
                                        {
                                            captureId = capture.Id;
                                        }
                                    }
                                    if (captureId.CompareTo("") == 0)
                                    {
                                        PaymentMade = false;
                                    }
                                }
                                catch
                                {
                                    PaymentMade = false;
                                }
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
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS signed encrypted directory ID..";
                                    }
                                    UniqueUserFileStorageID = Encoding.UTF8.GetString(PlainText);
                                    try 
                                    {
                                        FileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                        SignedFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/" + "rootSPK.txt");
                                        TestFileED25519PK = SodiumPublicKeyAuth.Verify(SignedFileED25519PK, FileED25519PK);
                                        if (FileED25519PK.SequenceEqual(TestFileED25519PK) == false) 
                                        {
                                            throw new ArgumentException("Verified Signed ED25519PK does not match with normal ED25519PK");
                                        }
                                    }
                                    catch
                                    {
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
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
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: Unable to decrypt ETLS signed encrypted new ED25519 PK..";
                                    }
                                    if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                    {
                                        return "Error: Unable to find corresponding user directory...";
                                    }
                                    else
                                    {
                                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                        try
                                        {
                                            RandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, FileED25519PK);
                                        }
                                        catch
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: The signed random challenge can't be verified with the specific directory ED25519 PK";
                                        }
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        if (Count == 1)
                                        {
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "SELECT `Valid_Duration` FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            DBDateTime = DateTime.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                            Duration = MyUTC8DateTime.Subtract(DBDateTime);
                                            if (Duration.TotalMinutes < 8)
                                            {
                                                MySQLGeneralQuery = new MySqlCommand();
                                                MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                                MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                MySQLGeneralQuery.Prepare();
                                                MySQLGeneralQuery.ExecuteNonQuery();
                                                DirectoryExpirationTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                                if (MyUTC8DateTime > DirectoryExpirationTime)
                                                {
                                                    DirectoryExpirationTime = MyUTC8DateTime;
                                                }
                                                DirectoryExpirationTime = DirectoryExpirationTime.AddDays(30);
                                                if (PlainText.Length != 128)
                                                {
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    return "Error: You can only pass in public key that is 128 bytes, the first 32 bytes represents the public key, the latter 96 bytes represents the signed public key";
                                                }
                                                ED25519PKByte = new Byte[32];
                                                SignedED25519PKByte = new Byte[96];
                                                Array.Copy(PlainText, 0, ED25519PKByte, 0, 32);
                                                Array.Copy(PlainText, 32, SignedED25519PKByte, 0, 96);
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", ED25519PKByte);
                                                System.IO.File.SetCreationTime(FileStoragePath + UniqueUserFileStorageID + "/rootPK.txt", DirectoryExpirationTime);
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/rootSPK.txt", SignedED25519PKByte);
                                                System.IO.File.SetCreationTime(FileStoragePath + UniqueUserFileStorageID + "/rootSPK.txt", DirectoryExpirationTime);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                return "Successed: Payment has been renewed.. and ED25519 PK of the directory has been updated";
                                            }
                                            else
                                            {
                                                MySQLGeneralQuery = new MySqlCommand();
                                                MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                                MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                MySQLGeneralQuery.Prepare();
                                                MySQLGeneralQuery.ExecuteNonQuery();
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                return "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: This random challenge does not exists in the system..";
                                        }
                                    }
                                }
                                else
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: You haven't made payment.";
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                return "Error: Man in the middle spotted, are you an imposter trying to get over the ETLS?";
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            return "Error: Please pass in correct Base64 Encoded String in the parameter..";
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
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
