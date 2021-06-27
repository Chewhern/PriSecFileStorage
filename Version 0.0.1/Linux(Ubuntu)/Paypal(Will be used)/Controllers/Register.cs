using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;

namespace PriSecFileStorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Register : ControllerBase
    {
        private MyOwnMySQLConnection MyMyOwnMySQLConnectionClass = new MyOwnMySQLConnection();

        [HttpGet("byValues")]
        public RegisterModel RegisterAccount(String ClientPathID,String SignedUserID,String SignedLoginSignedPK,String SignedLoginPK,String SignedCiphered_Recovery_Data) 
        {
            RegisterModel MyRegisterModel = new RegisterModel();
            VerifyDataClass verifyDataClass = new VerifyDataClass();
            DecodeDataClass decodeDataClass = new DecodeDataClass();
            ConvertFromBase64StringClass convertFromBase64StringClass = new ConvertFromBase64StringClass();
            String DecodedSignedUserID = "";
            Boolean DecodingSignedUserIDChecker = true;
            Boolean ConvertFromBase64SignedUserIDChecker = true;
            Byte[] SignedUserIDByte = new Byte[] { };
            Boolean VerifyUserIDByteChecker = true;
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
            Byte[] ClientECDSAPK = new Byte[] { };
            MySqlCommand MySQLGeneralQuery = new MySqlCommand();
            String ExceptionString = "";
            int UserIDChecker = 0;
            String Path = "{Path to store ETLS information}";
            Path += ClientPathID;
            if(ClientPathID!=null && ClientPathID.CompareTo("") != 0) 
            {
                if(Directory.Exists(Path)) 
                {
                    ClientECDSAPK = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUserIDChecker, ref DecodedSignedUserID, SignedUserID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedLoginSignedPKChecker, ref DecodedSignedLoginSignedPK, SignedLoginSignedPK);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedLoginPKChecker, ref DecodedSignedLoginPK, SignedLoginPK);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedCiphered_Recovery_DataChecker, ref DecodedSignedCiphered_Recovery_Data, SignedCiphered_Recovery_Data);
                    if(DecodingSignedUserIDChecker==true && DecodingSignedLoginSignedPKChecker==true && DecodingSignedLoginPKChecker==true && DecodingSignedCiphered_Recovery_DataChecker==true) 
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUserIDChecker,ref SignedUserIDByte,DecodedSignedUserID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedLoginSignedPKChecker, ref SignedLoginSignedPKByte, DecodedSignedLoginSignedPK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedLoginPKChecker, ref SignedLoginPKByte, DecodedSignedLoginPK);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedCiphered_Recovery_DataChecker, ref SignedCiphered_Recovery_Data_Byte, DecodedSignedCiphered_Recovery_Data);
                        if(ConvertFromBase64SignedUserIDChecker==true && ConvertFromBase64SignedLoginSignedPKChecker==true && ConvertFromBase64SignedLoginPKChecker==true && ConvertFromBase64SignedCiphered_Recovery_DataChecker == true) 
                        {
                            verifyDataClass.VerifyData(ref VerifyUserIDByteChecker, ref UserIDByte, SignedUserIDByte, ClientECDSAPK);
                            verifyDataClass.VerifyData(ref VerifyLoginSignedPKByteChecker, ref LoginSignedPKByte, SignedLoginSignedPKByte, ClientECDSAPK);
                            verifyDataClass.VerifyData(ref VerifyLoginPKByteChecker, ref LoginPKByte, SignedLoginPKByte, ClientECDSAPK);
                            verifyDataClass.VerifyData(ref VerifyCiphered_Recovery_Data_ByteChecker, ref Ciphered_Recovery_Data_Byte, SignedCiphered_Recovery_Data_Byte, ClientECDSAPK);
                            if(VerifyUserIDByteChecker==true && VerifyLoginSignedPKByteChecker==true && VerifyLoginPKByteChecker==true && VerifyCiphered_Recovery_Data_ByteChecker == true) 
                            {
                                UserID = Encoding.UTF8.GetString(UserIDByte);
                                MyMyOwnMySQLConnectionClass.LoadConnection(ref ExceptionString);
                                MySQLGeneralQuery.CommandText = "SELECT COUNT(*) FROM `User` WHERE `User_ID`=@User_ID";
                                MySQLGeneralQuery.Parameters.Add("@User_ID", MySqlDbType.Text).Value = UserID;
                                MySQLGeneralQuery.Connection = MyMyOwnMySQLConnectionClass.MyMySQLConnection;
                                MySQLGeneralQuery.Prepare();
                                UserIDChecker = int.Parse(MySQLGeneralQuery.ExecuteScalar().ToString());
                                if (UserIDChecker == 0) 
                                {
                                    MySQLGeneralQuery = new MySqlCommand();
                                    MyMyOwnMySQLConnectionClass.MyMySQLConnection.Close();
                                    MyMyOwnMySQLConnectionClass.LoadConnection(ref ExceptionString);
                                    MySQLGeneralQuery.CommandText = "INSERT INTO `User`(`User_ID`, `Login_Signed_PK`, `Login_PK`, `Ciphered_Recovery_Data`) VALUES (@User_ID, @Login_Signed_PK, @Login_PK, @Ciphered_Recovery_Data)";
                                    MySQLGeneralQuery.Parameters.Add("@User_ID",MySqlDbType.Text).Value=UserID;
                                    MySQLGeneralQuery.Parameters.Add("@Login_Signed_PK", MySqlDbType.Text).Value = Convert.ToBase64String(LoginSignedPKByte);
                                    MySQLGeneralQuery.Parameters.Add("@Login_PK", MySqlDbType.Text).Value = Convert.ToBase64String(LoginPKByte);
                                    MySQLGeneralQuery.Parameters.Add("@Ciphered_Recovery_Data", MySqlDbType.Text).Value = Convert.ToBase64String(Ciphered_Recovery_Data_Byte);
                                    MySQLGeneralQuery.Connection = MyMyOwnMySQLConnectionClass.MyMySQLConnection;
                                    MySQLGeneralQuery.Prepare();
                                    MySQLGeneralQuery.ExecuteNonQuery();
                                    MyRegisterModel.Error = "Congratulations: Your account has been registered ...";
                                    MyRegisterModel.UserIDChecker = "Congratulations: This ID does not exists and this ID has been registered into our system";
                                    MyRegisterModel.UserIDCount = "Congratulations: This ID has count of 0 in database rows";
                                }
                                else 
                                {
                                    MyRegisterModel.Error = "Error: The user ID have already existed ... Use another one instead";
                                    MyRegisterModel.UserIDChecker = "Error: Existed";
                                    MyRegisterModel.UserIDCount = "Error: User ID count ="+UserIDChecker.ToString();
                                }
                                MyMyOwnMySQLConnectionClass.MyMySQLConnection.Close();
                            }
                            else 
                            {
                                MyRegisterModel.Error = "Error: Are you an imposter? The ECDSA(ED25519) public key does not match with the values you sent..";
                                MyRegisterModel.UserIDChecker = "Error: Not Valid";
                                MyRegisterModel.UserIDCount = "Error: Not Valid";
                            }
                        }
                        else 
                        {
                            MyRegisterModel.Error = "Error: You didn't pass in correct Base 64 encoded paramter value...";
                            MyRegisterModel.UserIDChecker = "Error: Not Valid";
                            MyRegisterModel.UserIDCount = "Error: Not Valid";
                        }
                    }
                    else 
                    {
                        MyRegisterModel.Error = "Error: You didn't pass in correct URL encoded parameter value...";
                        MyRegisterModel.UserIDChecker = "Error: Not Valid";
                        MyRegisterModel.UserIDCount = "Error: Not Valid";
                    }
                }
                else 
                {
                    MyRegisterModel.Error = "Error: Path does not exists";
                    MyRegisterModel.UserIDChecker = "Error: Not Valid";
                    MyRegisterModel.UserIDCount = "Error: Not Valid";
                }
            }
            else 
            {
                MyRegisterModel.Error = "Error: You didn't specify a path";
                MyRegisterModel.UserIDChecker = "Error: Not Valid";
                MyRegisterModel.UserIDCount = "Error: Not Valid";
            }
            return MyRegisterModel;
        }
    }
}
