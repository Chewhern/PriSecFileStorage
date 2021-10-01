using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASodium;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using PriSecFileStorageAPI.Helper;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerAddUser : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();
        private CryptographicSecureIDGenerator MyIDGenerator = new CryptographicSecureIDGenerator();
        
        [HttpGet("UploadPK")]
        public String OwnerUploadPK(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String CipheredSignedED25519PK)
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
            String UniqueUserID = MyIDGenerator.GenerateUniqueString();
            if (UniqueUserID.Length > 16)
            {
                UniqueUserID = UniqueUserID.Substring(0, 16);
            }
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
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User") == false)
                                                {
                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User");
                                                }
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + UniqueUserID) == false)
                                                {
                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + UniqueUserID);
                                                }
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + UniqueUserID + "/PK.txt", ED25519PKByte);
                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + UniqueUserID + "/SPK.txt", SignedED25519PKByte);
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "ID: " + UniqueUserID;
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

        [HttpGet("RemovePK")]
        public String OwnerRemovePK(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String CipheredSignedAnotherUserID)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedCipheredSignedAnotherUserID = "";
            Boolean DecodingCipheredSignedAnotherUserIDChecker = true;
            Boolean ConvertFromBase64CipheredSignedAnotherUserIDChecker = true;
            Byte[] CipheredSignedAnotherUserIDByte = new Byte[] { };
            Boolean VerifyCipheredAnotherUserIDChecker = true;
            Byte[] CipheredAnotherUserIDByte = new Byte[] { };
            String AnotherUserID = "";
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
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedAnotherUserIDChecker, ref DecodedCipheredSignedAnotherUserID, CipheredSignedAnotherUserID);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingCipheredSignedAnotherUserIDChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedAnotherUserIDChecker, ref CipheredSignedAnotherUserIDByte, DecodedCipheredSignedAnotherUserID);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64CipheredSignedAnotherUserIDChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredAnotherUserIDChecker, ref CipheredAnotherUserIDByte, CipheredSignedAnotherUserIDByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyCipheredAnotherUserIDChecker == true)
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
                                CipheredText = new Byte[CipheredAnotherUserIDByte.Length - NonceByte.Length];
                                Array.Copy(CipheredAnotherUserIDByte, 0, NonceByte, 0, NonceByte.Length);
                                Array.Copy(CipheredAnotherUserIDByte, NonceByte.Length, CipheredText, 0, CipheredText.Length);
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
                                AnotherUserID = Encoding.UTF8.GetString(PlainText);
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
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User") == false)
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Error: You haven't upload the public key of the allowed user";
                                                }
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + AnotherUserID) == false)
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Error: This allowed user does not exists";
                                                }
                                                Directory.Delete(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + AnotherUserID, true);
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                return "Successed: You have deleted the user from your permission list";
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

        [HttpGet("GetAllowedUsers")]
        public AllowedUserListModel GetAllowedUsers(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge)
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
            String[] AllowedUserID = new String[] { };
            String[] ActualAllowedUserID = new String[] { };
            String Status = "";
            AllowedUserListModel MyUserList = new AllowedUserListModel();
            int LoopCount = 0;
            int DirectoryRootCount = 0;
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
                                    Status= "Error: Unable to decrypt the encrypted directory ID";
                                    MyUserList.Status = Status;
                                    MyUserList.AllowedUserID = AllowedUserID;
                                    return MyUserList;
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
                                    MyUserList.Status = Status;
                                    MyUserList.AllowedUserID = AllowedUserID;
                                    return MyUserList;
                                }
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    Status = "Error: The specified main directory does not exists..";
                                    MyUserList.Status = Status;
                                    MyUserList.AllowedUserID = AllowedUserID;
                                    return MyUserList;
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
                                            MyUserList.Status = Status;
                                            MyUserList.AllowedUserID = AllowedUserID;
                                            return MyUserList;
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
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User") == false)
                                                {
                                                    Status = "Error: You have not yet add a user";
                                                    MyUserList.Status = Status;
                                                    MyUserList.AllowedUserID = AllowedUserID;
                                                    return MyUserList;
                                                }
                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                SodiumSecureMemory.SecureClearBytes(PlainText);
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                AllowedUserID = Directory.GetDirectories(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User");
                                                DirectoryRootCount = (FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/").Length;
                                                ActualAllowedUserID = new String[AllowedUserID.Length];
                                                while (LoopCount < AllowedUserID.Length) 
                                                {
                                                    ActualAllowedUserID[LoopCount] = AllowedUserID[LoopCount].Remove(0,DirectoryRootCount);
                                                    LoopCount += 1;
                                                }
                                                Status = "Success: A list of allowed users have been returned";
                                                SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                MyUserList.Status = Status;
                                                MyUserList.AllowedUserID = ActualAllowedUserID;
                                                return MyUserList;
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
                                                MyUserList.Status = Status;
                                                MyUserList.AllowedUserID = AllowedUserID;
                                                return MyUserList;
                                            }
                                        }
                                        else
                                        {
                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                            Status = "Error: The specified random challenge does not exist in the system";
                                            MyUserList.Status = Status;
                                            MyUserList.AllowedUserID = AllowedUserID;
                                            return MyUserList;
                                        }
                                    }
                                    else
                                    {
                                        Status = "Error: You no longer own this rent directory/folder";
                                        MyUserList.Status = Status;
                                        MyUserList.AllowedUserID = AllowedUserID;
                                        return MyUserList;
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                Status = "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
                                MyUserList.Status = Status;
                                MyUserList.AllowedUserID = AllowedUserID;
                                return MyUserList;
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            Status = "Error: Please pass in correct Base64 encoded String in parameter";
                            MyUserList.Status = Status;
                            MyUserList.AllowedUserID = AllowedUserID;
                            return MyUserList;
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        Status = "Error: Please pass in correct URL encoded String in parameter";
                        MyUserList.Status = Status;
                        MyUserList.AllowedUserID = AllowedUserID;
                        return MyUserList;
                    }
                }
                else
                {
                    Status = "Error: The specified ETLS ID does not exists..";
                    MyUserList.Status = Status;
                    MyUserList.AllowedUserID = AllowedUserID;
                    return MyUserList;
                }
            }
            else
            {
                Status = "Error: Client did not specify an ETLS ID";
                MyUserList.Status = Status;
                MyUserList.AllowedUserID = AllowedUserID;
                return MyUserList;
            }
        }
    }
}
