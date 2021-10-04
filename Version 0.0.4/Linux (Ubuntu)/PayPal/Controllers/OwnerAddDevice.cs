using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASodium;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using PriSecFileStorageAPI.Helper;
using PriSecFileStorageAPI.Model;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerAddDevice : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("GetMFADeviceCount")]
        public String GetMFADeviceCount(String DirectoryID)
        {
            String FileStoragePath = "{Path to File Storage}";
            String ResultString = "";
            int Result = 0;
            if (DirectoryID != null)
            {
                FileStoragePath += DirectoryID;
                FileStoragePath += "/MFA_Device";
                if (Directory.Exists(FileStoragePath) == true)
                {
                    Result = Directory.GetDirectories(FileStoragePath).Length;
                    return Result.ToString();
                }
                else
                {
                    ResultString = "Error: The folder ID you specified does not exists";
                    return ResultString;
                }
            }
            else
            {
                ResultString = "Error:You didn't pass in a folder ID";
                return ResultString;
            }
        }

        [HttpGet("UploadFirstDevicePK")]
        public String OwnerUploadFirstDevicePK(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String CipheredSignedED25519PK, String UniqueDeviceID)
        {
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
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            String UniqueUserFileStorageID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryValidDateTime = new DateTime();
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            Byte[] SignedFileED25519PK = new Byte[] { };
            Byte[] TestFileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            int DeviceCount = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedED25519PKChecker, ref DecodedCipheredSignedED25519PK, CipheredSignedED25519PK);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingCipheredSignedED25519PKChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedED25519PKChecker, ref CipheredSignedED25519PKByte, DecodedCipheredSignedED25519PK);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64CipheredSignedED25519PKChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredED25519PKByteChecker, ref CipheredED25519PKByte, CipheredSignedED25519PKByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyCipheredED25519PKByteChecker == true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
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
                                    return "Error: Unable to decrypt the encrypted directory ID";
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
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    return "Error: The specified main directory does not contain ED25519 PK";
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
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    return "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established between server and client?(Unable to decrypt sent ED25519PK)";
                                }
                                if (PlainText.Length != 128)
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: You can only pass in public key that is 128 bytes, the first 32 bytes represents the public key, the latter 96 bytes represents the signed public key";
                                }
                                ED25519PKByte = new Byte[32];
                                SignedED25519PKByte = new Byte[96];
                                Array.Copy(PlainText, 0, ED25519PKByte, 0, 32);
                                Array.Copy(PlainText, 32, SignedED25519PKByte, 0, 96);
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    return "Error: The specified main directory does not exists..";
                                }
                                else
                                {
                                    DirectoryValidDateTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                    if (MyUTC8DateTime.CompareTo(DirectoryValidDateTime) <= 0)
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
                                            return "Error: Are you an imposter trying to mimic the directory's owner?(Failed to verify the challenge through ED25519PK resides in the directory)";
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
                                                //Do something here...
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device") == false)
                                                {
                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device");
                                                }
                                                else
                                                {
                                                    DeviceCount = Directory.GetDirectories(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device").Length;
                                                    if (DeviceCount == 1)
                                                    {
                                                        return "Error: You can't upload second device PK through this API endpoint";
                                                    }
                                                }
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID) == false)
                                                {
                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID);
                                                }
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID + "/PK.txt", ED25519PKByte);
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID + "/SPK.txt", SignedED25519PKByte);
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Success: The PK along with the device ID has been stored";
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
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            return "Error: The specified random challenge does not exist in the system";
                                        }
                                    }
                                    else
                                    {
                                        return "Error: You no longer own this rent directory/folder";
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                return "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            return "Error: Please pass in correct Base64 encoded String in parameter";
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        return "Error: Please pass in correct URL encoded String in parameter";
                    }
                }
                else
                {
                    return "Error: The specified ETLS ID does not exists..";
                }
            }
            else
            {
                return "Error: Client did not specify an ETLS ID";
            }
        }

        [HttpGet("UploadOtherDevicePK")]
        public String OwnerUploadOtherDevicePK(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String CipheredSignedED25519PK, String OtherUniqueDeviceID, String UniqueDeviceID)
        {
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
            String DecodedSignedSignedRandomChallenge = "";
            Boolean DecodingSignedSignedRandomChallengeChecker = true;
            Boolean ConvertFromBase64SignedSignedRandomChallengeChecker = true;
            Byte[] SignedSignedRandomChallengeByte = new Byte[] { };
            Boolean VerifySignedRandomChallengeChecker = true;
            Byte[] SignedRandomChallengeByte = new Byte[] { };
            Byte[] VerifiedSignedRandomChallengeByte = new Byte[] { };
            Byte[] RandomChallengeByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            String UniqueUserFileStorageID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryValidDateTime = new DateTime();
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            Byte[] SignedFileED25519PK = new Byte[] { };
            Byte[] TestFileED25519PK = new Byte[] { };
            Byte[] OtherDeviceFileED25519PK = new Byte[] { };
            Byte[] OtherDeviceSignedFileED25519PK = new Byte[] { };
            Byte[] OtherDeviceTestFileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedED25519PKChecker, ref DecodedCipheredSignedED25519PK, CipheredSignedED25519PK);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingCipheredSignedED25519PKChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedED25519PKChecker, ref CipheredSignedED25519PKByte, DecodedCipheredSignedED25519PK);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64CipheredSignedED25519PKChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredED25519PKByteChecker, ref CipheredED25519PKByte, CipheredSignedED25519PKByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyCipheredED25519PKByteChecker == true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
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
                                    return "Error: Unable to decrypt the encrypted directory ID";
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
                                    OtherDeviceFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + OtherUniqueDeviceID + "/" + "PK.txt");
                                    OtherDeviceSignedFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + OtherUniqueDeviceID + "/" + "SPK.txt");
                                    OtherDeviceTestFileED25519PK = SodiumPublicKeyAuth.Verify(OtherDeviceSignedFileED25519PK, OtherDeviceFileED25519PK);
                                    if (OtherDeviceFileED25519PK.SequenceEqual(OtherDeviceTestFileED25519PK) == false)
                                    {
                                        throw new ArgumentException("Verified Other Device Signed ED25519PK does not match with normal other device ED25519PK");
                                    }
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    return "Error: The specified main directory or MFA_Device do not contain ED25519 PK";
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
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    return "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established between server and client?(Unable to decrypt sent ED25519PK)";
                                }
                                if (PlainText.Length != 128)
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: You can only pass in public key that is 128 bytes, the first 32 bytes represents the public key, the latter 96 bytes represents the signed public key";
                                }
                                ED25519PKByte = new Byte[32];
                                SignedED25519PKByte = new Byte[96];
                                Array.Copy(PlainText, 0, ED25519PKByte, 0, 32);
                                Array.Copy(PlainText, 32, SignedED25519PKByte, 0, 96);
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    return "Error: The specified main directory does not exists..";
                                }
                                else
                                {
                                    DirectoryValidDateTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                    if (MyUTC8DateTime.CompareTo(DirectoryValidDateTime) <= 0)
                                    {
                                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                        try
                                        {
                                            VerifiedSignedRandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, OtherDeviceFileED25519PK);
                                        }
                                        catch
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: Are you an imposter trying to mimic the directory's owner?(Failed to verify the challenge through ED25519PK resides in the directory)";
                                        }
                                        try
                                        {
                                            RandomChallengeByte = SodiumPublicKeyAuth.Verify(VerifiedSignedRandomChallengeByte, FileED25519PK);
                                        }
                                        catch
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: Do you use the wrong MFA device to login?(Failed to verify the challenge through ED25519PK resides in the directory)";
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
                                                //Do something here...
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device") == false)
                                                {
                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device");
                                                }
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID) == false)
                                                {
                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID);
                                                }
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID + "/PK.txt", ED25519PKByte);
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID + "/SPK.txt", SignedED25519PKByte);
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Success: The PK along with the device ID has been stored";
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
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            return "Error: The specified random challenge does not exist in the system";
                                        }
                                    }
                                    else
                                    {
                                        return "Error: You no longer own this rent directory/folder";
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                return "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            return "Error: Please pass in correct Base64 encoded String in parameter";
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        return "Error: Please pass in correct URL encoded String in parameter";
                    }
                }
                else
                {
                    return "Error: The specified ETLS ID does not exists..";
                }
            }
            else
            {
                return "Error: Client did not specify an ETLS ID";
            }
        }

        [HttpGet("RemoveFirstDevicePK")]
        public String OwnerRemoveFirstDevicePK(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge)
        {
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedDirectoryID = "";
            Boolean DecodingCipheredSignedDirectoryIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedDirectoryIDChecker = true;
            Byte[] CipheredSignedDirectoryIDByte = new Byte[] { };
            Boolean VerifyCipheredDirectoryIDByteChecker = true;
            Byte[] CipheredDirectoryIDByte = new Byte[] { };
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
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            String UniqueUserFileStorageID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryValidDateTime = new DateTime();
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            Byte[] SignedFileED25519PK = new Byte[] { };
            Byte[] TestFileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            int DeviceCount = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
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
                                    return "Error: Unable to decrypt the encrypted directory ID";
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
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    return "Error: The specified main directory does not contain ED25519 PK";
                                }
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    return "Error: The specified main directory does not exists..";
                                }
                                else
                                {
                                    DirectoryValidDateTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                    if (MyUTC8DateTime.CompareTo(DirectoryValidDateTime) <= 0)
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
                                            return "Error: Are you an imposter trying to mimic the directory's owner?(Failed to verify the challenge through ED25519PK resides in the directory)";
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
                                                //Do something here...
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device") == false)
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Error: You haven't upload the public key of the allowed user";
                                                }
                                                else
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    DeviceCount = Directory.GetDirectories(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device").Length;
                                                    if (DeviceCount != 1)
                                                    {
                                                        SodiumSecureMemory.SecureClearBytes(PlainText);
                                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                        SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                        return "Error: You must clear other MFA device and their PK before you can clear the final MFA device and its PK";
                                                    }
                                                    else
                                                    {
                                                        Directory.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/", true);
                                                    }
                                                }
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Successed: You have deleted the final/first MFA device and its PK";
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
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            return "Error: The specified random challenge does not exist in the system";
                                        }
                                    }
                                    else
                                    {
                                        return "Error: You no longer own this rent directory/folder";
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                return "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            return "Error: Please pass in correct Base64 encoded String in parameter";
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        return "Error: Please pass in correct URL encoded String in parameter";
                    }
                }
                else
                {
                    return "Error: The specified ETLS ID does not exists..";
                }
            }
            else
            {
                return "Error: Client did not specify an ETLS ID";
            }
        }

        [HttpGet("RemoveOtherDevicePK")]
        public String OwnerRemoveOtherDevicePK(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String OtherUniqueDeviceID, String UniqueDeviceID)
        {
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedDirectoryID = "";
            Boolean DecodingCipheredSignedDirectoryIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedDirectoryIDChecker = true;
            Byte[] CipheredSignedDirectoryIDByte = new Byte[] { };
            Boolean VerifyCipheredDirectoryIDByteChecker = true;
            Byte[] CipheredDirectoryIDByte = new Byte[] { };
            String DecodedSignedSignedRandomChallenge = "";
            Boolean DecodingSignedSignedRandomChallengeChecker = true;
            Boolean ConvertFromBase64SignedSignedRandomChallengeChecker = true;
            Byte[] SignedSignedRandomChallengeByte = new Byte[] { };
            Boolean VerifySignedRandomChallengeChecker = true;
            Byte[] SignedRandomChallengeByte = new Byte[] { };
            Byte[] VerifiedSignedRandomChallengeByte = new Byte[] { };
            Byte[] RandomChallengeByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            Byte[] CipheredText = new Byte[] { };
            Byte[] PlainText = new Byte[] { };
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            String UniqueUserFileStorageID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryValidDateTime = new DateTime();
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            Byte[] SignedFileED25519PK = new Byte[] { };
            Byte[] TestFileED25519PK = new Byte[] { };
            Byte[] OtherDeviceFileED25519PK = new Byte[] { };
            Byte[] OtherDeviceSignedFileED25519PK = new Byte[] { };
            Byte[] OtherDeviceTestFileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
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
                                    return "Error: Unable to decrypt the encrypted directory ID";
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
                                    OtherDeviceFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + OtherUniqueDeviceID + "/" + "PK.txt");
                                    OtherDeviceSignedFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + OtherUniqueDeviceID + "/" + "SPK.txt");
                                    OtherDeviceTestFileED25519PK = SodiumPublicKeyAuth.Verify(OtherDeviceSignedFileED25519PK, OtherDeviceFileED25519PK);
                                    if (OtherDeviceFileED25519PK.SequenceEqual(OtherDeviceTestFileED25519PK) == false)
                                    {
                                        throw new ArgumentException("Verified Other Device Signed ED25519PK does not match with normal other device ED25519PK");
                                    }
                                }
                                catch
                                {
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    return "Error: The specified main directory or MFA_Device do not contain ED25519 PK";
                                }
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    return "Error: The specified main directory does not exists..";
                                }
                                else
                                {
                                    DirectoryValidDateTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                    if (MyUTC8DateTime.CompareTo(DirectoryValidDateTime) <= 0)
                                    {
                                        myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                        try
                                        {
                                            VerifiedSignedRandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, OtherDeviceFileED25519PK);
                                        }
                                        catch
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: Are you an imposter trying to mimic the directory's owner?(Failed to verify the challenge through ED25519PK resides in the directory)";
                                        }
                                        try
                                        {
                                            RandomChallengeByte = SodiumPublicKeyAuth.Verify(VerifiedSignedRandomChallengeByte, FileED25519PK);
                                        }
                                        catch
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: Do you use the wrong MFA device to login?(Failed to verify the challenge through ED25519PK resides in the directory)";
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
                                                //Do something here...
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device") == false)
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Error: You have not yet upload MFA device and its PK";
                                                }
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID) == false)
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Error: This MFA device(Unique MFA Device) you stated does not exist";
                                                }
                                                else
                                                {
                                                    Directory.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + UniqueDeviceID, true);
                                                    System.IO.File.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + "Lock_" + UniqueDeviceID + ".txt");
                                                }
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Success: The PK along with the MFA device has been deleted";
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
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            return "Error: The specified random challenge does not exist in the system";
                                        }
                                    }
                                    else
                                    {
                                        return "Error: You no longer own this rent directory/folder";
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                return "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            return "Error: Please pass in correct Base64 encoded String in parameter";
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        return "Error: Please pass in correct URL encoded String in parameter";
                    }
                }
                else
                {
                    return "Error: The specified ETLS ID does not exists..";
                }
            }
            else
            {
                return "Error: Client did not specify an ETLS ID";
            }
        }
        
        [HttpGet("GetMFADevice")]
        public MFADeviceListModel OwnerGetMFADevice(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedDirectoryID = "";
            Boolean DecodingCipheredSignedDirectoryIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedDirectoryIDChecker = true;
            Byte[] CipheredSignedDirectoryIDByte = new Byte[] { };
            Boolean VerifyCipheredDirectoryIDByteChecker = true;
            Byte[] CipheredDirectoryIDByte = new Byte[] { };
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
            Byte[] ClientECDSAPKByte = new Byte[] { };
            Byte[] SharedSecret = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to File Storage}";
            String UniqueUserFileStorageID = "";
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DirectoryValidDateTime = new DateTime();
            DateTime DBDateTime;
            Byte[] FileED25519PK = new Byte[] { };
            Byte[] SignedFileED25519PK = new Byte[] { };
            Byte[] TestFileED25519PK = new Byte[] { };
            TimeSpan Duration;
            int Count = 0;
            int LoopCount = 0;
            int DirectoryRootCount = 0;
            String[] MFADeviceID = new String[] { };
            String[] ActualMFADeviceID = new String[] { };
            String Status = "";
            MFADeviceListModel MyMFADeviceList = new MFADeviceListModel();
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true)
                            {
                                NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
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
                                    Status = "Error: Unable to decrypt the encrypted directory ID";
                                    MyMFADeviceList.Status = Status;
                                    MyMFADeviceList.MFADeviceID = MFADeviceID;
                                    return MyMFADeviceList;
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
                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                    Status = "Error: The specified main directory does not contain ED25519 PK";
                                    MyMFADeviceList.Status = Status;
                                    MyMFADeviceList.MFADeviceID = MFADeviceID;
                                    return MyMFADeviceList;
                                }
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    Status = "Error: The specified main directory does not exists..";
                                    MyMFADeviceList.Status = Status;
                                    MyMFADeviceList.MFADeviceID = MFADeviceID;
                                    return MyMFADeviceList;
                                }
                                else
                                {
                                    DirectoryValidDateTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                    if (MyUTC8DateTime.CompareTo(DirectoryValidDateTime) <= 0)
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
                                            Status = "Error: Are you an imposter trying to mimic the directory's owner?(Failed to verify the challenge through ED25519PK resides in the directory)";
                                            MyMFADeviceList.Status = Status;
                                            MyMFADeviceList.MFADeviceID = MFADeviceID;
                                            return MyMFADeviceList;
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
                                                //Do something here...
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device") == false)
                                                {
                                                    Status = "Error: You have not yet add a MFA Device";
                                                    MyMFADeviceList.Status = Status;
                                                    MyMFADeviceList.MFADeviceID = MFADeviceID;
                                                    return MyMFADeviceList;
                                                }
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                MFADeviceID = Directory.GetDirectories(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device");
                                                DirectoryRootCount = (FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/").Length;
                                                ActualMFADeviceID = new String[MFADeviceID.Length];
                                                while (LoopCount < MFADeviceID.Length)
                                                {
                                                    ActualMFADeviceID[LoopCount] = MFADeviceID[LoopCount].Remove(0, DirectoryRootCount);
                                                    LoopCount += 1;
                                                }
                                                Status = "Success: A list of MFA device have been returned";
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                MyMFADeviceList.Status = Status;
                                                MyMFADeviceList.MFADeviceID = ActualMFADeviceID;
                                                return MyMFADeviceList;
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
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                Status = "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                                MyMFADeviceList.Status = Status;
                                                MyMFADeviceList.MFADeviceID = MFADeviceID;
                                                return MyMFADeviceList;
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            Status = "Error: The specified random challenge does not exist in the system";
                                            MyMFADeviceList.Status = Status;
                                            MyMFADeviceList.MFADeviceID = MFADeviceID;
                                            return MyMFADeviceList;
                                        }
                                    }
                                    else
                                    {
                                        Status = "Error: You no longer own this rent directory/folder";
                                        MyMFADeviceList.Status = Status;
                                        MyMFADeviceList.MFADeviceID = MFADeviceID;
                                        return MyMFADeviceList;
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                Status = "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
                                MyMFADeviceList.Status = Status;
                                MyMFADeviceList.MFADeviceID = MFADeviceID;
                                return MyMFADeviceList;
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            Status = "Error: Please pass in correct Base64 encoded String in parameter";
                            MyMFADeviceList.Status = Status;
                            MyMFADeviceList.MFADeviceID = MFADeviceID;
                            return MyMFADeviceList;
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        Status = "Error: Please pass in correct URL encoded String in parameter";
                        MyMFADeviceList.Status = Status;
                        MyMFADeviceList.MFADeviceID = MFADeviceID;
                        return MyMFADeviceList;
                    }
                }
                else
                {
                    Status = "Error: The specified ETLS ID does not exists..";
                    MyMFADeviceList.Status = Status;
                    MyMFADeviceList.MFADeviceID = MFADeviceID;
                    return MyMFADeviceList;
                }
            }
            else
            {
                Status = "Error: Client did not specify an ETLS ID";
                MyMFADeviceList.Status = Status;
                MyMFADeviceList.MFADeviceID = MFADeviceID;
                return MyMFADeviceList;
            }
        }
    }
}
