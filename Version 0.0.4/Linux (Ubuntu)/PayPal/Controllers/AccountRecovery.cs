using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASodium;
using MySql.Data.MySqlClient;
using System.Text;
using System.IO;
using PriSecFileStorageAPI.Helper;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountRecovery : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("byUserID")]
        public String RequestRecoveryData(String ClientPathID, String SignedUserID)
        {
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedSignedUserID = "";
            Boolean DecodingSignedUserIDChecker = true;
            Boolean ConvertFromBase64SignedUserIDChecker = true;
            Byte[] SignedUserIDByte = new Byte[] { };
            Boolean VerifyUserIDChecker = true;
            Byte[] UserIDByte = new Byte[] { };
            String UserID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    if (DecodingSignedUserIDChecker == true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        if (ConvertFromBase64SignedUserIDChecker == true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            if (VerifyUserIDChecker == true) 
                            {
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (Count == 1) 
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT `Ciphered_Recovery_Data` FROM `User` WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    return MySQLGeneralQuery.ExecuteScalar().ToString();
                                }
                                else 
                                {
                                    return "Error: This user ID does not exists";
                                }
                            }
                            else 
                            {
                                return "Error: Ephemeral ED25519 PK does not match with signed value...";
                            }
                        }
                        else 
                        {
                            return "Error: You didn't pass in base64 encoded parameter value...";
                        }
                    }
                    else 
                    {
                        return "Error: You didn't pass in URL encoded parameter value...";
                    }
                }
                else 
                {
                    return "Error: The client path ID does not exists...";
                }
            }
            else 
            {
                return "Error: The client path ID is null or empty string";
            }
        }

        [HttpGet("DeletebyUserID")]
        public String DeleteAccount(String ClientPathID, String SignedUserID,String SignedECDSAPK,String SealedUserSymmetricKey) 
        {
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            Byte[] ServerECDHSK = new Byte[] { };
            Byte[] ServerECDHPK = new Byte[] { };
            String DecodedSealedUserSymmetricKey = "";
            Boolean DecodingSealedUserSymmetricKeyChecker = true;
            Boolean ConvertFromBase64SealedUserSymmetricKeyChecker = true;
            Byte[] SealedUserSymmetricKeyByte = new Byte[] { };
            Byte[] UserSymmetricKeyByte = new Byte[] { };
            String DecodedSignedECDSAPK = "";
            Boolean DecodingSignedECDSAPKChecker = true;
            Boolean ConvertFromBase64SignedECDSAPKChecker = true;
            Byte[] SignedECDSAPKByte = new Byte[] { };
            Boolean VerifyECDSAPKChecker = true;
            Byte[] ECDSAPKByte = new Byte[] { };
            String DecodedSignedUserID = "";
            Boolean DecodingSignedUserIDChecker = true;
            Boolean ConvertFromBase64SignedUserIDChecker = true;
            Byte[] SignedUserIDByte = new Byte[] { };
            Boolean VerifyUserIDChecker = true;
            Byte[] UserIDByte = new Byte[] { };
            String UserID = "";
            Byte[] ClientECDSAPKByte = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String SignedCipheredRecoveryData = "";
            Byte[] SignedCipheredRecoveryDataByte = new Byte[] { };
            Byte[] CipheredRecoveryDataByte = new Byte[] { };
            Byte[] FilteredCipheredRecoveryDataByte = new Byte[] { };
            Byte[] RecoveryDataByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedECDSAPKChecker, ref DecodedSignedECDSAPK, SignedECDSAPK);
                    decodeDataClass.DecodeDataFunction(ref DecodingSealedUserSymmetricKeyChecker, ref DecodedSealedUserSymmetricKey, SealedUserSymmetricKey);
                    if (DecodingSignedUserIDChecker == true && DecodingSignedECDSAPKChecker==true && DecodingSealedUserSymmetricKeyChecker==true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedECDSAPKChecker, ref SignedECDSAPKByte, DecodedSignedECDSAPK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SealedUserSymmetricKeyChecker, ref SealedUserSymmetricKeyByte, DecodedSealedUserSymmetricKey);
                        if (ConvertFromBase64SignedUserIDChecker == true && ConvertFromBase64SignedECDSAPKChecker==true && ConvertFromBase64SealedUserSymmetricKeyChecker==true)
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyECDSAPKChecker, ref ECDSAPKByte, SignedECDSAPKByte ,ClientECDSAPKByte);
                            if (VerifyUserIDChecker == true && VerifyECDSAPKChecker==true)
                            {
                                ServerECDHSK = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                                ServerECDHPK = System.IO.File.ReadAllBytes(Path + "/" + "ECDHPK.txt");
                                try 
                                {
                                    UserSymmetricKeyByte = SodiumSealedPublicKeyBox.Open(SealedUserSymmetricKeyByte, ServerECDHPK, ServerECDHSK);
                                }
                                catch 
                                {
                                    return "Error: Unable to decrypt user sent symmetric key";
                                }
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (Count == 1)
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT `Ciphered_Recovery_Data` FROM `User` WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    SignedCipheredRecoveryData = MySQLGeneralQuery.ExecuteScalar().ToString();
                                    try 
                                    {
                                        SignedCipheredRecoveryDataByte = Convert.FromBase64String(SignedCipheredRecoveryData);
                                    }
                                    catch 
                                    {
                                        return "Error: Someone messes the data on server side(Signed Ciphered Recovery Data failed to convert from Base64 to bytes)";
                                    }
                                    try 
                                    {
                                        CipheredRecoveryDataByte = SodiumPublicKeyAuth.Verify(SignedCipheredRecoveryDataByte,ECDSAPKByte);                                        
                                    }
                                    catch 
                                    {
                                        return "Error: Someone messes the data on server side(Signed Ciphered Recovery Data failed to verify through given ED25519 PK)";
                                    }
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    Array.Copy(CipheredRecoveryDataByte, 0 , NonceByte, 0 , NonceByte.Length);
                                    FilteredCipheredRecoveryDataByte = new Byte[CipheredRecoveryDataByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredRecoveryDataByte, NonceByte.Length, FilteredCipheredRecoveryDataByte, 0, FilteredCipheredRecoveryDataByte.Length);
                                    try
                                    {
                                        RecoveryDataByte = SodiumSecretBox.Open(FilteredCipheredRecoveryDataByte, NonceByte, UserSymmetricKeyByte);
                                    }
                                    catch 
                                    {
                                        return "Error: Unable to decrypt user recovery data through user sent symmetric key";
                                    }
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "DELETE FROM `User` WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    MySQLGeneralQuery.ExecuteNonQuery();
                                    SodiumSecureMemory.SecureClearBytes(ServerECDHSK);
                                    return "Successed: Corresponding user ID have been deleted";
                                }
                                else
                                {
                                    SodiumSecureMemory.SecureClearBytes(ServerECDHSK);
                                    return "Error: This user ID does not exists";
                                }
                            }
                            else
                            {
                                SodiumSecureMemory.SecureClearBytes(ServerECDHSK);
                                return "Error: Ephemeral ED25519 PK does not match with signed value...";
                            }
                        }
                        else
                        {
                            return "Error: You didn't pass in base64 encoded parameter value...";
                        }
                    }
                    else
                    {
                        return "Error: You didn't pass in URL encoded parameter value...";
                    }
                }
                else
                {
                    return "Error: The client path ID does not exists...";
                }
            }
            else
            {
                return "Error: The client path ID is null or empty string";
            }
        }

        [HttpGet("UpdatebyUserID")]
        public String UpdateAccount(String ClientPathID, String SignedUserID, String SignedECDSAPK, String SignedLoginSignedPK, String SignedLoginPK, String SignedCiphered_Recovery_Data, String SealedUserSymmetricKey) 
        {
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            Byte[] ServerECDHSK = new Byte[] { };
            Byte[] ServerECDHPK = new Byte[] { };
            String DecodedSealedUserSymmetricKey = "";
            Boolean DecodingSealedUserSymmetricKeyChecker = true;
            Boolean ConvertFromBase64SealedUserSymmetricKeyChecker = true;
            Byte[] SealedUserSymmetricKeyByte = new Byte[] { };
            Byte[] UserSymmetricKeyByte = new Byte[] { };
            String DecodedSignedECDSAPK = "";
            Boolean DecodingSignedECDSAPKChecker = true;
            Boolean ConvertFromBase64SignedECDSAPKChecker = true;
            Byte[] SignedECDSAPKByte = new Byte[] { };
            Boolean VerifyECDSAPKChecker = true;
            Byte[] ECDSAPKByte = new Byte[] { };
            String DecodedSignedUserID = "";
            Boolean DecodingSignedUserIDChecker = true;
            Boolean ConvertFromBase64SignedUserIDChecker = true;
            Byte[] SignedUserIDByte = new Byte[] { };
            Boolean VerifyUserIDChecker = true;
            Byte[] UserIDByte = new Byte[] { };
            String UserID = "";
            String DecodedSignedLoginSignedPK = "";
            Boolean DecodingSignedLoginSignedPKChecker = true;
            Boolean ConvertFromBase64SignedLoginSignedPKChecker = true;
            Byte[] SignedLoginSignedPKByte = new Byte[] { };
            Boolean VerifyLoginSignedPKByteChecker = true;
            Byte[] LoginSignedPKByte = new Byte[] { };
            String DecodedSignedLoginPK = "";
            Boolean DecodingSignedLoginPKChecker = true;
            Boolean ConvertFromBase64SignedLoginPKChecker = true;
            Byte[] SignedLoginPKByte = new Byte[] { };
            Boolean VerifyLoginPKByteChecker = true;
            Byte[] LoginPKByte = new Byte[] { };
            String DecodedSignedCiphered_Recovery_Data = "";
            Boolean DecodingSignedCiphered_Recovery_DataChecker = true;
            Boolean ConvertFromBase64SignedCiphered_Recovery_DataChecker = true;
            Byte[] SignedCiphered_Recovery_Data_Byte = new Byte[] { };
            Boolean VerifyCiphered_Recovery_Data_ByteChecker = true;
            Byte[] Ciphered_Recovery_Data_Byte = new Byte[] { };
            Byte[] ClientECDSAPKByte = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String SignedCipheredRecoveryData = "";
            Byte[] SignedCipheredRecoveryDataByte = new Byte[] { };
            Byte[] CipheredRecoveryDataByte = new Byte[] { };
            Byte[] FilteredCipheredRecoveryDataByte = new Byte[] { };
            Byte[] RecoveryDataByte = new Byte[] { };
            Byte[] NonceByte = new Byte[] { };
            String ExceptionString = "";
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            int Count = 0;
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedECDSAPKChecker, ref DecodedSignedECDSAPK, SignedECDSAPK);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedLoginSignedPKChecker, ref DecodedSignedLoginSignedPK, SignedLoginSignedPK);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedLoginPKChecker, ref DecodedSignedLoginPK, SignedLoginPK);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedCiphered_Recovery_DataChecker, ref DecodedSignedCiphered_Recovery_Data, SignedCiphered_Recovery_Data);
                    decodeDataClass.DecodeDataFunction(ref DecodingSealedUserSymmetricKeyChecker, ref DecodedSealedUserSymmetricKey, SealedUserSymmetricKey);
                    if (DecodingSignedUserIDChecker == true && DecodingSignedECDSAPKChecker==true && DecodingSignedLoginSignedPKChecker == true && DecodingSignedLoginPKChecker == true && DecodingSignedCiphered_Recovery_DataChecker == true && DecodingSealedUserSymmetricKeyChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker, ref SignedUserIDByte, DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedECDSAPKChecker, ref SignedECDSAPKByte, DecodedSignedECDSAPK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedLoginSignedPKChecker, ref SignedLoginSignedPKByte, DecodedSignedLoginSignedPK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedLoginPKChecker, ref SignedLoginPKByte, DecodedSignedLoginPK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedCiphered_Recovery_DataChecker, ref SignedCiphered_Recovery_Data_Byte, DecodedSignedCiphered_Recovery_Data);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SealedUserSymmetricKeyChecker, ref SealedUserSymmetricKeyByte, DecodedSealedUserSymmetricKey);
                        if (ConvertFromBase64SignedUserIDChecker == true && ConvertFromBase64SignedECDSAPKChecker==true && ConvertFromBase64SignedLoginSignedPKChecker == true && ConvertFromBase64SignedLoginPKChecker == true && ConvertFromBase64SignedCiphered_Recovery_DataChecker == true && ConvertFromBase64SealedUserSymmetricKeyChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyECDSAPKChecker, ref ECDSAPKByte, SignedECDSAPKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyLoginSignedPKByteChecker, ref LoginSignedPKByte, SignedLoginSignedPKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyLoginPKByteChecker, ref LoginPKByte, SignedLoginPKByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCiphered_Recovery_Data_ByteChecker, ref Ciphered_Recovery_Data_Byte, SignedCiphered_Recovery_Data_Byte, ClientECDSAPKByte);
                            if (VerifyUserIDChecker == true && VerifyECDSAPKChecker==true && VerifyLoginSignedPKByteChecker == true && VerifyLoginPKByteChecker == true && VerifyCiphered_Recovery_Data_ByteChecker == true)
                            {
                                ServerECDHSK = System.IO.File.ReadAllBytes(Path + "/" + "ECDHSK.txt");
                                ServerECDHPK = System.IO.File.ReadAllBytes(Path + "/" + "ECDHPK.txt");
                                try
                                {
                                    UserSymmetricKeyByte = SodiumSealedPublicKeyBox.Open(SealedUserSymmetricKeyByte, ServerECDHPK, ServerECDHSK);
                                }
                                catch
                                {
                                    return "Error: Unable to decrypt user sent symmetric key";
                                }
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                Count = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (Count == 1)
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MySQLGeneralQuery.CommandText = "SELECT `Ciphered_Recovery_Data` FROM `User` WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    SignedCipheredRecoveryData = MySQLGeneralQuery.ExecuteScalar().ToString();
                                    try
                                    {
                                        SignedCipheredRecoveryDataByte = Convert.FromBase64String(SignedCipheredRecoveryData);
                                    }
                                    catch
                                    {
                                        return "Error: Someone messes the data on server side(Signed Ciphered Recovery Data failed to convert from Base64 to bytes)";
                                    }
                                    try
                                    {
                                        CipheredRecoveryDataByte = SodiumPublicKeyAuth.Verify(SignedCipheredRecoveryDataByte, ECDSAPKByte);
                                    }
                                    catch
                                    {
                                        return "Error: Someone messes the data on server side(Signed Ciphered Recovery Data failed to verify through given ED25519 PK)";
                                    }
                                    NonceByte = new Byte[SodiumSecretBox.GenerateNonce().Length];
                                    Array.Copy(CipheredRecoveryDataByte, 0, NonceByte, 0, NonceByte.Length);
                                    FilteredCipheredRecoveryDataByte = new Byte[CipheredRecoveryDataByte.Length - NonceByte.Length];
                                    Array.Copy(CipheredRecoveryDataByte, NonceByte.Length, FilteredCipheredRecoveryDataByte, 0, FilteredCipheredRecoveryDataByte.Length);
                                    try
                                    {
                                        RecoveryDataByte = SodiumSecretBox.Open(FilteredCipheredRecoveryDataByte, NonceByte, UserSymmetricKeyByte);
                                    }
                                    catch
                                    {
                                        return "Error: Unable to decrypt user recovery data through user sent symmetric key";
                                    }
                                    MySQLGeneralQuery = new MySqlCommand();
                                    myMyOwnMySQLConnection.MyMySQLConnection.Close();
                                    myMyOwnMySQLConnection.LoadConnection(ref ExceptionString);
                                    MySQLGeneralQuery.CommandText = "UPDATE `User` SET `Login_Signed_PK`=@Login_Signed_PK,`Login_PK`=@Login_PK,`Ciphered_Recovery_Data`=@Ciphered_Recovery_Data WHERE `User_ID`=@User_ID";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                    MySQLGeneralQuery.Parameters.Add("@Login_Signed_PK", MySqlDbType.Text).Value = Convert.ToBase64String(LoginSignedPKByte);
                                    MySQLGeneralQuery.Parameters.Add("@Login_PK", MySqlDbType.Text).Value = Convert.ToBase64String(LoginPKByte);
                                    MySQLGeneralQuery.Parameters.Add("@Ciphered_Recovery_Data", MySqlDbType.Text).Value = Convert.ToBase64String(Ciphered_Recovery_Data_Byte);
                                    MySQLGeneralQuery.Connection = myMyOwnMySQLConnection.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    MySQLGeneralQuery.ExecuteNonQuery();
                                    SodiumSecureMemory.SecureClearBytes(ServerECDHSK);
                                    return "Successed: Corresponding user info have been updated";
                                }
                                else
                                {
                                    SodiumSecureMemory.SecureClearBytes(ServerECDHSK);
                                    return "Error: This user ID does not exists";
                                }
                            }
                            else
                            {
                                return "Error: Ephemeral ED25519 PK does not match with signed value...";
                            }
                        }
                        else
                        {
                            return "Error: You didn't pass in base64 encoded parameter value...";
                        }
                    }
                    else
                    {
                        return "Error: You didn't pass in URL encoded parameter value...";
                    }
                }
                else
                {
                    return "Error: The client path ID does not exists...";
                }
            }
            else
            {
                return "Error: The client path ID is null or empty string";
            }
        }
    }
}
