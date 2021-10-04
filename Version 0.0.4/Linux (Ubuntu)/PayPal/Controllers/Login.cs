using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASodium;
using MySql.Data.MySqlClient;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using PriSecFileStorageAPI.Model;
using PriSecFileStorageAPI.Helper;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private CryptographicSecureIDGenerator myCryptographicSecureIDGenerator = new CryptographicSecureIDGenerator();
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet]
        public LoginModels RequestChallenge() 
        {
            LoginModels MyLoginModels = new LoginModels();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            RevampedKeyPair ServerED25519KeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
            Byte[] RandomData = new Byte[128];
            Byte[] SignedRandomData = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            int Count = 0;
            String RequestID = myCryptographicSecureIDGenerator.GenerateMinimumAmountOfUniqueString(24);
            String ExceptionString = "";
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(RandomData);
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
            MySQLGeneralQuery = new MySqlCommand();
            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
            MySQLGeneralQuery.Prepare();
            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
            while (Count != 0)
            {
                RandomData = new Byte[128];
                rngCsp.GetBytes(RandomData);
                MySQLGeneralQuery = new MySqlCommand();
                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                MySQLGeneralQuery.Prepare();
                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
            }
            MySQLGeneralQuery = new MySqlCommand();
            MySQLGeneralQuery.CommandText = "INSERT INTO `Random_Challenge`(`Challenge`, `Valid_Duration`, `ID`) VALUES (@Challenge,@Valid_Duration,@ID)";
            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
            MySQLGeneralQuery.Parameters.Add("@Valid_Duration", MySqlDbType.DateTime).Value = MyUTC8DateTime;
            MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = RequestID;
            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
            MySQLGeneralQuery.Prepare();
            MySQLGeneralQuery.ExecuteNonQuery();
            SignedRandomData = SodiumPublicKeyAuth.Sign(RandomData, ServerED25519KeyPair.PrivateKey);
            MyLoginModels.RequestStatus = "Successed: Requesting server signed random challenge succeeded";
            MyLoginModels.ServerECDSAPKBase64String = Convert.ToBase64String(ServerED25519KeyPair.PublicKey);
            MyLoginModels.SignedRandomChallengeBase64String = Convert.ToBase64String(SignedRandomData);
            myMyOwnMySQLConnection.MyMySQLConnection.Close();
            ServerED25519KeyPair.Clear();
            return MyLoginModels;
        }

        [HttpGet("VerifySignatureBy")]
        public String VerifyStatus(String ClientPathID, String SignedUserID,String SignedSignedRandomChallenge) 
        {
            String VerifyStatus = "";
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            Byte[] ClientECDSAPKByte = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
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
            Boolean VerifyRandomChallengeChecker = true;
            Byte[] RandomChallengeByte = new Byte[] { };
            String Base64AuthenticationSignedECDSAPK = "";
            Boolean ConvertFromBase64SignedAuthenticationECDSAPKChecker = true;
            Byte[] SignedAuthenticationECDSAPKByte = new Byte[] { };
            String Base64AuthenticationECDSAPK = "";
            Boolean ConvertFromBase64AuthenticationECDSAPKChecker = true;
            Byte[] AuthenticationECDSAPKByte = new Byte[] { };
            Boolean VerifySignedAuthenticationECDSAPKByteChecker = true;
            Byte[] TestAuthenticationECDSAPKByte = new Byte[] { };
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            DateTime DBDateTime;
            TimeSpan Duration;
            int Count = 0;
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if(DecodingSignedUserIDChecker==true && DecodingSignedSignedRandomChallengeChecker == true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if(ConvertFromBase64SignedUserIDChecker==true && ConvertFromBase64SignedSignedRandomChallengeChecker == true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if(VerifyUserIDChecker==true && VerifySignedRandomChallengeChecker == true) 
                            {
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if(Count == 1) 
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT `Login_Signed_PK` FROM `User` WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    Base64AuthenticationSignedECDSAPK = MySQLGeneralQuery.ExecuteScalar().ToString();
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT `Login_PK` FROM `User` WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    Base64AuthenticationECDSAPK = MySQLGeneralQuery.ExecuteScalar().ToString();
                                    convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedAuthenticationECDSAPKChecker, ref SignedAuthenticationECDSAPKByte, Base64AuthenticationSignedECDSAPK);
                                    convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64AuthenticationECDSAPKChecker, ref AuthenticationECDSAPKByte, Base64AuthenticationECDSAPK);
                                    if (ConvertFromBase64SignedAuthenticationECDSAPKChecker == true && ConvertFromBase64AuthenticationECDSAPKChecker == true)
                                    {
                                        verifyDataClass.VerifyData(ref VerifySignedAuthenticationECDSAPKByteChecker, ref TestAuthenticationECDSAPKByte, SignedAuthenticationECDSAPKByte, AuthenticationECDSAPKByte);
                                        if (VerifySignedAuthenticationECDSAPKByteChecker == true)
                                        {
                                            if (AuthenticationECDSAPKByte.SequenceEqual(TestAuthenticationECDSAPKByte))
                                            {
                                                verifyDataClass.VerifyData(ref VerifyRandomChallengeChecker, ref RandomChallengeByte, SignedRandomChallengeByte, AuthenticationECDSAPKByte);
                                                if (VerifyRandomChallengeChecker == true)
                                                {
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
                                                            VerifyStatus = "Succeed: This random challenge has been successfully verified and have been deleted from the system";
                                                        }
                                                        else
                                                        {
                                                            MySQLGeneralQuery = new MySqlCommand();
                                                            MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `Challenge`=@Challenge";
                                                            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                            MySQLGeneralQuery.Prepare();
                                                            MySQLGeneralQuery.ExecuteNonQuery();
                                                            VerifyStatus = "Error: This random challenge valid duration is no more valid as 7 minutes have already passed";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        VerifyStatus = "Error: This signed random challenge does not match with plain random challenge after verification";
                                                    }
                                                }
                                                else
                                                {
                                                    VerifyStatus = "Error: Are you an imposter trying to mimic the user?";
                                                }
                                            }
                                            else
                                            {
                                                VerifyStatus = "Error: Verified Public Key unmatch with Public Key in database";
                                            }
                                        }
                                        else
                                        {
                                            VerifyStatus = "Error: Unable to verify signed public key with public key in database";
                                        }
                                    }
                                    else
                                    {
                                        VerifyStatus = "Error: Someone changes the normal/signed public key value in database to non base64 format";
                                    }
                                }
                                else 
                                {
                                    VerifyStatus= "Error: This user ID does not exists";
                                }
                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                            }
                            else 
                            {
                                VerifyStatus = "Error: Cryptographic digital signatures verification failed.. (ED25519 public key verify signature failed..)";
                            }
                        }
                        else 
                        {
                            VerifyStatus = "Error: You didn't pass in correct base64 encoded parameter value";
                        }
                    }
                    else 
                    {
                        VerifyStatus = "Error: You didn't pass in correct URL encoded parameter value";
                    }
                }
                else
                {
                    VerifyStatus = "Error: Client Path ID does not exists";
                }
            }
            else
            {
                VerifyStatus = "Error: Client Path ID was null or empty";
            }
            return VerifyStatus;
        }
    }
}
