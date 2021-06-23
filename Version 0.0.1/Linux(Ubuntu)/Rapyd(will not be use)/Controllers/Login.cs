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
using System.Runtime.InteropServices;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Login : ControllerBase
    {
        private CryptographicSecureIDGenerator myCryptographicSecureIDGenerator = new CryptographicSecureIDGenerator();
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("RequestBy")]
        public LoginModels RequestChallenge(String ClientPathID, String SignedUserID , String SignedAuthenticationType) 
        {
            LoginModels MyLoginModels = new LoginModels();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            RevampedKeyPair ServerED25519KeyPair = SodiumPublicKeyAuth.GenerateRevampedKeyPair();
            String DecodedSignedUserID = "";
            Boolean DecodingSignedUserIDChecker = true;
            Boolean ConvertFromBase64SignedUserIDChecker = true;
            Byte[] SignedUserIDByte = new Byte[] { };
            Boolean VerifyUserIDChecker = true;
            Byte[] UserIDByte = new Byte[] { };
            String UserID = "";
            String DecodedSignedAuthenticationType = "";
            Boolean DecodingSignedAuthenticationTypeChecker = true;
            Boolean ConvertFromBase64SignedAuthenticationTypeChecker = true;
            Byte[] SignedAuthenticationTypeByte = new Byte[] { };
            Boolean VerifyAuthenticationTypeChecker = true;
            Byte[] AuthenticationTypeByte = new Byte[] { };
            String AuthenticationType = "";
            Byte[] RandomData = new Byte[128];
            Byte[] SignedRandomData = new Byte[] { };
            Byte[] ClientECDSAPKByte = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            int Count = 0;
            String RequestID = myCryptographicSecureIDGenerator.GenerateMinimumAmountOfUniqueString(24);
            String ExceptionString = "";
            String Path = "{Path that stores ephemeral TLS data}";
            Path += ClientPathID;
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(RandomData);
            DateTime MyUTC8DateTime = DateTime.UtcNow.AddHours(8);
            if(ClientPathID!=null && ClientPathID.CompareTo("") != 0) 
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker,ref DecodedSignedUserID,SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedAuthenticationTypeChecker, ref DecodedSignedAuthenticationType, SignedAuthenticationType);
                    if(DecodingSignedAuthenticationTypeChecker==true && DecodingSignedUserIDChecker == true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedAuthenticationTypeChecker, ref SignedAuthenticationTypeByte, DecodedSignedAuthenticationType);
                        if(ConvertFromBase64SignedAuthenticationTypeChecker==true && ConvertFromBase64SignedUserIDChecker == true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyAuthenticationTypeChecker, ref AuthenticationTypeByte, SignedAuthenticationTypeByte, ClientECDSAPKByte);
                            if(VerifyUserIDChecker==true && VerifyAuthenticationTypeChecker == true) 
                            {
                                AuthenticationType = Encoding.UTF8.GetString(AuthenticationTypeByte);
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (Count == 1) 
                                {
                                    if (AuthenticationType.Equals("Login",StringComparison.OrdinalIgnoreCase)) 
                                    {
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge AND `User_ID`=@User_ID";
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                        MySQLGeneralQuery.Parameters.Add("@User_ID",MySqlDbType.Text).Value=UserID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        while (Count != 0) 
                                        {
                                            RandomData = new Byte[128];
                                            rngCsp.GetBytes(RandomData);
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge AND `User_ID`=@User_ID";
                                            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                            Count += 1;
                                        }
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `Random_Challenge`(`User_ID`, `Challenge`, `Authentication_Type`, `Valid_Duration`, `ID`) VALUES (@User_ID,@Challenge,@Authentication_Type,@Valid_Duration,@ID)";
                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                        MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
                                        MySQLGeneralQuery.Parameters.Add("@Valid_Duration", MySqlDbType.DateTime).Value = MyUTC8DateTime;
                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = RequestID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        SignedRandomData = SodiumPublicKeyAuth.Sign(RandomData, ServerED25519KeyPair.PrivateKey);
                                        MyLoginModels.RequestStatus = "Successed: Requesting server signed random challenge succeeded";
                                        MyLoginModels.UserIDChecker = "Successed: This user ID exists...";
                                        MyLoginModels.ServerECDSAPKBase64String = Convert.ToBase64String(ServerED25519KeyPair.PublicKey);
                                        MyLoginModels.SignedRandomChallengeBase64String = Convert.ToBase64String(SignedRandomData);
                                    }
                                    else if (AuthenticationType.Equals("Renew Payment", StringComparison.OrdinalIgnoreCase))
                                    {
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge AND `User_ID`=@User_ID";
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        while (Count != 0)
                                        {
                                            RandomData = new Byte[128];
                                            rngCsp.GetBytes(RandomData);
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge AND `User_ID`=@User_ID";
                                            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                            Count += 1;
                                        }
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `Random_Challenge`(`User_ID`, `Challenge`, `Authentication_Type`, `Valid_Duration`, `ID`) VALUES (@User_ID,@Challenge,@Authentication_Type,@Valid_Duration,@ID)";
                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                        MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
                                        MySQLGeneralQuery.Parameters.Add("@Valid_Duration", MySqlDbType.DateTime).Value = MyUTC8DateTime;
                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = RequestID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        SignedRandomData = SodiumPublicKeyAuth.Sign(RandomData, ServerED25519KeyPair.PrivateKey);
                                        MyLoginModels.RequestStatus = "Successed: Requesting server signed random challenge succeeded";
                                        MyLoginModels.UserIDChecker = "Successed: This user ID exists...";
                                        MyLoginModels.ServerECDSAPKBase64String = Convert.ToBase64String(ServerED25519KeyPair.PublicKey);
                                        MyLoginModels.SignedRandomChallengeBase64String = Convert.ToBase64String(SignedRandomData);
                                    }
                                    else if (AuthenticationType.Equals("Miscellaneous", StringComparison.OrdinalIgnoreCase))
                                    {
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge AND `User_ID`=@User_ID";
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                        while (Count != 0)
                                        {
                                            RandomData = new Byte[128];
                                            rngCsp.GetBytes(RandomData);
                                            MySQLGeneralQuery = new MySqlCommand();
                                            MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `Challenge`=@Challenge AND `User_ID`=@User_ID";
                                            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                            MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                            MySQLGeneralQuery.Prepare();
                                            Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                            Count += 1;
                                        }
                                        MySQLGeneralQuery = new MySqlCommand();
                                        MySQLGeneralQuery.CommandText = "INSERT INTO `Random_Challenge`(`User_ID`, `Challenge`, `Authentication_Type`, `Valid_Duration`, `ID`) VALUES (@User_ID,@Challenge,@Authentication_Type,@Valid_Duration,@ID)";
                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomData);
                                        MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
                                        MySQLGeneralQuery.Parameters.Add("@Valid_Duration", MySqlDbType.DateTime).Value = MyUTC8DateTime;
                                        MySQLGeneralQuery.Parameters.Add("@ID", MySqlDbType.Text).Value = RequestID;
                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                        MySQLGeneralQuery.Prepare();
                                        MySQLGeneralQuery.ExecuteNonQuery();
                                        SignedRandomData = SodiumPublicKeyAuth.Sign(RandomData, ServerED25519KeyPair.PrivateKey);
                                        MyLoginModels.RequestStatus = "Successed: Requesting server signed random challenge succeeded";
                                        MyLoginModels.UserIDChecker = "Successed: This user ID exists...";
                                        MyLoginModels.ServerECDSAPKBase64String = Convert.ToBase64String(ServerED25519KeyPair.PublicKey);
                                        MyLoginModels.SignedRandomChallengeBase64String = Convert.ToBase64String(SignedRandomData);
                                    }
                                    else 
                                    {
                                        MyLoginModels.RequestStatus = "Error: Wrong API";
                                        MyLoginModels.UserIDChecker = "Error: Not Valid";
                                        MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                                        MyLoginModels.SignedRandomChallengeBase64String = "Error: Not Valid";
                                    }
                                }
                                else 
                                {
                                    MyLoginModels.RequestStatus = "Error: Not Valid";
                                    MyLoginModels.UserIDChecker = "Error: This user ID does not exists";
                                    MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                                    MyLoginModels.SignedRandomChallengeBase64String = "Error: Not Valid";
                                }
                                myMyOwnMySQLConnection.MyMySQLConnection.Close();
                            }
                            else 
                            {
                                MyLoginModels.RequestStatus = "Error: Ephemeral ED25519 PK does not match with signed value...";
                                MyLoginModels.UserIDChecker = "Error: Not Valid";
                                MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                                MyLoginModels.SignedRandomChallengeBase64String = "Error: Not Valid";
                            }
                        }
                        else 
                        {
                            MyLoginModels.RequestStatus = "Error: You didn't pass in base64 encoded parameter value...";
                            MyLoginModels.UserIDChecker = "Error: Not Valid";
                            MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                            MyLoginModels.SignedRandomChallengeBase64String = "Error: Not Valid";
                        }
                    }
                    else 
                    {
                        MyLoginModels.RequestStatus = "Error: You didn't pass in URL encoded parameter value...";
                        MyLoginModels.UserIDChecker = "Error: Not Valid";
                        MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                        MyLoginModels.SignedRandomChallengeBase64String = "Error: Not Valid";
                    }
                }
                else 
                {
                    MyLoginModels.RequestStatus = "Error: The client path ID does not exists...";
                    MyLoginModels.UserIDChecker = "Error: Not Valid";
                    MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                    MyLoginModels.SignedRandomChallengeBase64String = "Error:Not Valid";
                }
            }
            else 
            {
                MyLoginModels.RequestStatus = "Error: The client path ID is null or empty string";
                MyLoginModels.UserIDChecker = "Error: Not Valid";
                MyLoginModels.ServerECDSAPKBase64String = "Error: Not Valid";
                MyLoginModels.SignedRandomChallengeBase64String = "Error:Not Valid";
            }
            ServerED25519KeyPair.Clear();
            GCHandle MyGeneralGCHandle = GCHandle.Alloc(ClientPathID, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientPathID.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(Path, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), Path.Length);
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
            return MyLoginModels;
        }

        [HttpGet("VerifySignatureBy")]
        public String VerifyStatus(String ClientPathID, String SignedUserID, String SignedAuthenticationType,String SignedSignedRandomChallenge) 
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
            String DecodedSignedAuthenticationType = "";
            Boolean DecodingSignedAuthenticationTypeChecker = true;
            Boolean ConvertFromBase64SignedAuthenticationTypeChecker = true;
            Byte[] SignedAuthenticationTypeByte = new Byte[] { };
            Boolean VerifyAuthenticationTypeChecker = true;
            Byte[] AuthenticationTypeByte = new Byte[] { };
            String AuthenticationType = "";
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
            String Path = "{Path that stores ephemeral TLS data}";
            Path += ClientPathID;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedAuthenticationTypeChecker, ref DecodedSignedAuthenticationType, SignedAuthenticationType);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    if(DecodingSignedUserIDChecker==true && DecodingSignedAuthenticationTypeChecker==true && DecodingSignedSignedRandomChallengeChecker == true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedAuthenticationTypeChecker, ref SignedAuthenticationTypeByte, DecodedSignedAuthenticationType);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        if(ConvertFromBase64SignedUserIDChecker==true && ConvertFromBase64SignedAuthenticationTypeChecker==true && ConvertFromBase64SignedSignedRandomChallengeChecker == true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyAuthenticationTypeChecker, ref AuthenticationTypeByte, SignedAuthenticationTypeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            if(VerifyUserIDChecker==true && VerifyAuthenticationTypeChecker==true && VerifySignedRandomChallengeChecker == true) 
                            {
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                AuthenticationType = Encoding.UTF8.GetString(AuthenticationTypeByte);
                                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if(Count == 1) 
                                {
                                    if (AuthenticationType.Equals("Login", StringComparison.OrdinalIgnoreCase)) 
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
                                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedAuthenticationECDSAPKChecker,ref SignedAuthenticationECDSAPKByte, Base64AuthenticationSignedECDSAPK);
                                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64AuthenticationECDSAPKChecker, ref AuthenticationECDSAPKByte, Base64AuthenticationECDSAPK);
                                        if(ConvertFromBase64SignedAuthenticationECDSAPKChecker==true && ConvertFromBase64AuthenticationECDSAPKChecker == true) 
                                        {
                                            verifyDataClass.VerifyData(ref VerifySignedAuthenticationECDSAPKByteChecker, ref TestAuthenticationECDSAPKByte, SignedAuthenticationECDSAPKByte, AuthenticationECDSAPKByte);
                                            if (VerifySignedAuthenticationECDSAPKByteChecker == true) 
                                            {
                                                if (AuthenticationECDSAPKByte.SequenceEqual(TestAuthenticationECDSAPKByte)) 
                                                {
                                                    verifyDataClass.VerifyData(ref VerifyRandomChallengeChecker,ref RandomChallengeByte, SignedRandomChallengeByte, AuthenticationECDSAPKByte);
                                                    if (VerifyRandomChallengeChecker == true) 
                                                    {
                                                        MySQLGeneralQuery = new MySqlCommand();
                                                        MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                        MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                        MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                        MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
                                                        MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                        MySQLGeneralQuery.Prepare();
                                                        Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                                        if (Count == 1) 
                                                        {
                                                            MySQLGeneralQuery = new MySqlCommand();
                                                            MySQLGeneralQuery.CommandText = "SELECT `Valid_Duration` FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                            MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                            MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                            MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
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
                                                                MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
                                                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                                                MySQLGeneralQuery.Prepare();
                                                                MySQLGeneralQuery.ExecuteNonQuery();
                                                                VerifyStatus = "Succeed: This random challenge has been successfully verified and have been deleted from the system";
                                                            }
                                                            else 
                                                            {
                                                                MySQLGeneralQuery = new MySqlCommand();
                                                                MySQLGeneralQuery.CommandText = "DELETE FROM `Random_Challenge` WHERE `User_ID`=@User_ID AND `Challenge`=@Challenge AND `Authentication_Type`=@Authentication_Type";
                                                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                                                MySQLGeneralQuery.Parameters.Add("@Challenge", MySqlDbType.Text).Value = Convert.ToBase64String(RandomChallengeByte);
                                                                MySQLGeneralQuery.Parameters.Add("@Authentication_Type", MySqlDbType.Text).Value = AuthenticationType;
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
                                        VerifyStatus = "Error: Wrong API";
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
            GCHandle MyGeneralGCHandle = GCHandle.Alloc(ClientPathID, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientPathID.Length);
            MyGeneralGCHandle.Free();
            MyGeneralGCHandle = GCHandle.Alloc(Path, GCHandleType.Pinned);
            SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), Path.Length);
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
            return VerifyStatus;
        }
    }
}
