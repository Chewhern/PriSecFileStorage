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
using PriSecFileStorageAPI.Model;
using PriSecFileStorageAPI.Helper;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MFAOwnerUploadFiles : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpPost]
        public String UploadFile(MFAUploadFilesModel FilesModel)
        {
            if (FilesModel != null)
            {
                CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
                VerifyDataClass verifyDataClass = new VerifyDataClass();
                DecodeDataClass decodeDataClass = new DecodeDataClass();
                ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
                String DecodedSignedUniqueFilename = "";
                Boolean DecodingSignedUniqueFileNameChecker = true;
                Boolean ConvertFromBase64SignedUniqueFileNameChecker = true;
                Byte[] SignedUniqueFilenameByte = new Byte[] { };
                Boolean VerifyUniqueFilenameByteChecker = true;
                Byte[] UniqueFilenameByte = new Byte[] { };
                String UniqueFilename = "";
                Byte[] CipheredSignedFileContentByte = new Byte[] { };
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
                Path += FilesModel.ClientPathID;
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
                int FileCount = 0;
                if (FilesModel.ClientPathID != null && FilesModel.ClientPathID.CompareTo("") != 0)
                {
                    if (Directory.Exists(Path))
                    {
                        ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                        SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                        decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, FilesModel.CipheredSignedDirectoryID);
                        decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, FilesModel.SignedSignedRandomChallenge);
                        decodeDataClass.DecodeDataFunction(ref DecodingSignedUniqueFileNameChecker, ref DecodedSignedUniqueFilename, FilesModel.SignedUniqueFileName);
                        if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingSignedUniqueFileNameChecker == true)
                        {
                            convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                            convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                            convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUniqueFileNameChecker, ref SignedUniqueFilenameByte, DecodedSignedUniqueFilename);
                            if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64SignedUniqueFileNameChecker == true)
                            {
                                verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                                verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                                verifyDataClass.VerifyData(ref VerifyUniqueFilenameByteChecker, ref UniqueFilenameByte, SignedUniqueFilenameByte, ClientECDSAPKByte);
                                if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyUniqueFilenameByteChecker == true)
                                {
                                    CipheredSignedFileContentByte = Convert.FromBase64String(FilesModel.CipheredSignedFileContent);
                                    UniqueFilename = Encoding.UTF8.GetString(UniqueFilenameByte);
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
                                        return "Error: Unable to decrypt directory ID";
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
                                        return "Error: The specified main directory ED25519 PK does not exist in the system";
                                    }
                                    if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                    {
                                        return "Error: The specified main directory does not exist in the system";
                                    }
                                    else
                                    {
                                        DirectoryValidDateTime = System.IO.File.GetLastWriteTime(FileStoragePath + UniqueUserFileStorageID + "/" + "rootPK.txt");
                                        if (MyUTC8DateTime.CompareTo(DirectoryValidDateTime) <= 0)
                                        {
                                            if ((GetDirectorySizeClass.GetDirectorySize(FileStoragePath + UniqueUserFileStorageID) - 128) <= 1073741824)
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
                                                    return "Error: The signed random challenge can't be verified by the specified directory ED25519 PK";
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
                                                        if (System.IO.File.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + FilesModel.FirstMFADeviceID + "/LoginStatus.txt")==true && System.IO.File.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + FilesModel.SecondMFADeviceID + "/LoginStatus.txt") == true) 
                                                        {
                                                            if (FilesModel.FirstMFADeviceID.CompareTo(FilesModel.SecondMFADeviceID) != 0) 
                                                            {
                                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename) == false)
                                                                {
                                                                    Directory.CreateDirectory(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename);
                                                                }
                                                                FileCount = Directory.GetFiles(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename).Length;
                                                                FileCount += 1;
                                                                if (FileCount == FilesModel.LastFileIndex) 
                                                                {
                                                                    System.IO.File.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + FilesModel.FirstMFADeviceID + "/LoginStatus.txt");
                                                                    System.IO.File.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + FilesModel.SecondMFADeviceID + "/LoginStatus.txt");
                                                                }
                                                                System.IO.File.WriteAllBytes(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename + "/EndpointEncryptedFileContent" + FileCount.ToString() + ".txt", CipheredSignedFileContentByte);
                                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                                return "Successed: File has been uploaded..";
                                                            }
                                                            else 
                                                            {
                                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                                return "Error: MFA device can't be the same";
                                                            }
                                                        }
                                                        else 
                                                        {
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                            return "Error: You haven't yet add MFA device";
                                                        }
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
                                                    return "Error: The specified random challenge does not exist in the system";
                                                }
                                            }
                                            else
                                            {
                                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                return "Error: The directory have used up 1 GB";
                                            }
                                        }
                                        else
                                        {
                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                            return "Error: You no longer own this rent directory/folder";
                                        }
                                    }
                                }
                                else
                                {
                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                    return "Error: Man in the middle spotted do you intend to mimic the ETLS Session that established between server and client?";
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
                            return "Error: Please pass in correct URL encoded String in parameter..";
                        }
                    }
                    else
                    {
                        return "Error: The specified ETLS ID does not exist in the system";
                    }
                }
                else
                {
                    return "Error: Client did not specify an ETLS ID";
                }
            }
            else
            {
                return "Error: Please pass the Files Model correctly through Post";
            }
        }

        [HttpGet("DeleteEndpointEncryptedFile")]
        public String DeleteFile(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String SignedUniqueFileName, String FirstMFADeviceID, String SecondMFADeviceID)
        {
            CryptographicSecureIDGenerator IDGenerator = new CryptographicSecureIDGenerator();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedSignedUniqueFilename = "";
            Boolean DecodingSignedUniqueFileNameChecker = true;
            Boolean ConvertFromBase64SignedUniqueFileNameChecker = true;
            Byte[] SignedUniqueFilenameByte = new Byte[] { };
            Boolean VerifyUniqueFilenameByteChecker = true;
            Byte[] UniqueFilenameByte = new Byte[] { };
            String UniqueFilename = "";
            Byte[] CipheredSignedFileContentByte = new Byte[] { };
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
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUniqueFileNameChecker, ref DecodedSignedUniqueFilename, SignedUniqueFileName);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingSignedUniqueFileNameChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUniqueFileNameChecker, ref SignedUniqueFilenameByte, DecodedSignedUniqueFilename);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64SignedUniqueFileNameChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyUniqueFilenameByteChecker, ref UniqueFilenameByte, SignedUniqueFilenameByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyUniqueFilenameByteChecker == true)
                            {
                                UniqueFilename = Encoding.UTF8.GetString(UniqueFilenameByte);
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
                                    return "Error: Unable to decrypt the specified main directory through ETLS shared secret";
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
                                    return "Error: The ED25519PK does not exists in the specified directory..";
                                }
                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID) == false)
                                {
                                    return "Error: The main directory that you specified does not exist in the system";
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
                                            return "Error: Failed to verify the random challenge with the specified directory ID ED25519 PK";
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
                                                if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename) == false)
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    return "Error: This unique random generated file name does not exists in the server..";
                                                }
                                                else
                                                {
                                                    if (System.IO.File.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + FirstMFADeviceID + "/LoginStatus.txt") == true && System.IO.File.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + SecondMFADeviceID + "/LoginStatus.txt") == true) 
                                                    {
                                                        if (FirstMFADeviceID.CompareTo(SecondMFADeviceID) != 0)
                                                        {
                                                            Directory.Delete(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename, true);
                                                            System.IO.File.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + FirstMFADeviceID + "/LoginStatus.txt");
                                                            System.IO.File.Delete(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + SecondMFADeviceID + "/LoginStatus.txt");
                                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                            return "Successed: File successfully deleted";
                                                        }
                                                        else 
                                                        {
                                                            myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                            return "Error: MFA device can't be the same";
                                                        }
                                                    }
                                                    else 
                                                    {
                                                        myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                        return "Error: You haven't yet add MFA device";
                                                    }
                                                }
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
                                            return "Error: This random challenge does not exist in server";
                                        }
                                    }
                                    else
                                    {
                                        myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                        return "Error: You no longer own this rent directory/folder";
                                    }
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                return "Error: Man in the middle spotted.., are you an imposter trying to mimic the ETLS established between client and server?";
                            }
                        }
                        else
                        {
                            SodiumSecureMemory.SecureClearBytes(SharedSecret);
                            return "Error: Client didn't pass in correct Base64 encoded string in parameter";
                        }
                    }
                    else
                    {
                        SodiumSecureMemory.SecureClearBytes(SharedSecret);
                        return "Error: Client didn't pass in correct URL encoded string in parameter";
                    }
                }
                else
                {
                    return "Error: The specified ETLS ID does not exist";
                }
            }
            else
            {
                return "Error: Client didn't specify ETLS ID";
            }
        }
    }
}
