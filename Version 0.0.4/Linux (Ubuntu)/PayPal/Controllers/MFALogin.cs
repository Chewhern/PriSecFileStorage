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

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MFALogin : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("MFADeviceLogin")]
        public String LoginMFADevice(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String OtherUniqueDeviceID)
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
            Boolean VerifyRandomChallengeChecker = true;
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
                            verifyDataClass.VerifyData(ref VerifyRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifyRandomChallengeChecker == true)
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
                                            RandomChallengeByte = SodiumPublicKeyAuth.Verify(SignedRandomChallengeByte, OtherDeviceFileED25519PK);
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
                                                if (System.IO.File.Exists(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + OtherUniqueDeviceID + "/LoginStatus.txt")==false) 
                                                {
                                                    System.IO.File.Create(FileStoragePath + UniqueUserFileStorageID + "/MFA_Device/" + OtherUniqueDeviceID + "/LoginStatus.txt");
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Success: This MFA Device have been logged in";                                                
                                                }
                                                else 
                                                {
                                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                                    SodiumSecureMemory.SecureClearBytes(PlainText);
                                                    SodiumSecureMemory.SecureClearBytes(SharedSecret);
                                                    SodiumSecureMemory.SecureClearString(UniqueUserFileStorageID);
                                                    return "Success: This MFA Device had already logged in";
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
    }
}
