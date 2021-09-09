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
    public class UserFileOperation : ControllerBase
    {
        private MyOwnMySQLConnection myMyOwnMySQLConnection = new MyOwnMySQLConnection();

        [HttpGet("GetEndpointEncryptedFileContentCount")]
        public String GetFileContentCount(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String SignedUniqueFileName, String CipheredSignedAnotherUserID)
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
            GCHandle MyGeneralGCHandle = new GCHandle();
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to User File Storage}";
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
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedAnotherUserIDChecker, ref DecodedCipheredSignedAnotherUserID, CipheredSignedAnotherUserID);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingSignedUniqueFileNameChecker == true && DecodingCipheredSignedAnotherUserIDChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUniqueFileNameChecker, ref SignedUniqueFilenameByte, DecodedSignedUniqueFilename);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedAnotherUserIDChecker, ref CipheredSignedAnotherUserIDByte, DecodedCipheredSignedAnotherUserID);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64SignedUniqueFileNameChecker == true && ConvertFromBase64CipheredSignedAnotherUserIDChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyUniqueFilenameByteChecker, ref UniqueFilenameByte, SignedUniqueFilenameByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredAnotherUserIDChecker, ref CipheredAnotherUserIDByte, CipheredSignedAnotherUserIDByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyUniqueFilenameByteChecker == true && VerifyCipheredAnotherUserIDChecker == true)
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
                                    MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                    MyGeneralGCHandle.Free();
                                    return "Error: Unable to decrypt the encrypted directory ID";
                                }
                                UniqueUserFileStorageID = Encoding.UTF8.GetString(PlainText);
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
                                    return "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established between server and client?(Unable to decrypt sent ED25519PK)";
                                }
                                AnotherUserID = Encoding.UTF8.GetString(PlainText);
                                try
                                {
                                    FileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + AnotherUserID +"/" + "PK.txt");
                                    SignedFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + AnotherUserID + "/" + "SPK.txt");
                                    TestFileED25519PK = SodiumPublicKeyAuth.Verify(SignedFileED25519PK, FileED25519PK);
                                    if (FileED25519PK.SequenceEqual(TestFileED25519PK) == false)
                                    {
                                        throw new ArgumentException("Verified Signed ED25519PK does not match with normal ED25519PK");
                                    }
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
                                    return "Error: The specified main directory does not contain ED25519 PK";
                                }
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
                                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                        MyGeneralGCHandle.Free();
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
                                            if (Directory.Exists(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename) == false)
                                            {
                                                return "Error: The specified encrypted file content folder does not exists in the server..";
                                            }
                                            else
                                            {
                                                return Directory.GetFiles(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename).Length.ToString();
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
                                        return "Error: The specified random challenge does not exist in the system";
                                    }
                                }
                                else
                                {
                                    return "Error: You no longer own this rent directory/folder";
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
                                return "Error: Man in the middle spotted, do you intend to mimic the ETLS that established between client and server?";
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
                            return "Error: Please pass in correct Base64 encoded String in parameter";
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

        [HttpGet("GetEndpointEncryptedFile")]
        public String GetFile(String ClientPathID, String CipheredSignedDirectoryID, String SignedSignedRandomChallenge, String SignedUniqueFileName, String FileContentCount, String CipheredSignedAnotherUserID)
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
            GCHandle MyGeneralGCHandle = new GCHandle();
            String Path = "{Path to ETLS}";
            Path += ClientPathID;
            String FileStoragePath = "{Path to User File Storage}";
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
            try
            {
                FileCount = int.Parse(FileContentCount);
            }
            catch
            {
                return "Error: Failed to get file content count correctly..";
            }
            if (ClientPathID != null && ClientPathID.CompareTo("") != 0)
            {
                if (Directory.Exists(Path))
                {
                    ClientECDSAPKByte = System.IO.File.ReadAllBytes(Path + "/" + "ClientECDSAPK.txt");
                    SharedSecret = System.IO.File.ReadAllBytes(Path + "/" + "SharedSecret.txt");
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedDirectoryIDChecker, ref DecodedCipheredSignedDirectoryID, CipheredSignedDirectoryID);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedSignedRandomChallengeChecker, ref DecodedSignedSignedRandomChallenge, SignedSignedRandomChallenge);
                    decodeDataClass.DecodeDataFunction(ref DecodingSignedUniqueFileNameChecker, ref DecodedSignedUniqueFilename, SignedUniqueFileName);
                    decodeDataClass.DecodeDataFunction(ref DecodingCipheredSignedAnotherUserIDChecker, ref DecodedCipheredSignedAnotherUserID, CipheredSignedAnotherUserID);
                    if (DecodingCipheredSignedDirectoryIDChecker == true && DecodingSignedSignedRandomChallengeChecker == true && DecodingSignedUniqueFileNameChecker == true && DecodingCipheredSignedAnotherUserIDChecker == true)
                    {
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedDirectoryIDChecker, ref CipheredSignedDirectoryIDByte, DecodedCipheredSignedDirectoryID);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedSignedRandomChallengeChecker, ref SignedSignedRandomChallengeByte, DecodedSignedSignedRandomChallenge);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64SignedUniqueFileNameChecker, ref SignedUniqueFilenameByte, DecodedSignedUniqueFilename);
                        convertFromBase64StringClass.ConvertFromBase64StringFunction(ref ConvertFromBase64CipheredSignedAnotherUserIDChecker, ref CipheredSignedAnotherUserIDByte, DecodedCipheredSignedAnotherUserID);
                        if (ConvertFromBase64CipheredSignedDirectoryIDChecker == true && ConvertFromBase64SignedSignedRandomChallengeChecker == true && ConvertFromBase64SignedUniqueFileNameChecker == true && ConvertFromBase64CipheredSignedAnotherUserIDChecker == true)
                        {
                            verifyDataClass.VerifyData(ref VerifyCipheredDirectoryIDByteChecker, ref CipheredDirectoryIDByte, CipheredSignedDirectoryIDByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifySignedRandomChallengeChecker, ref SignedRandomChallengeByte, SignedSignedRandomChallengeByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyUniqueFilenameByteChecker, ref UniqueFilenameByte, SignedUniqueFilenameByte, ClientECDSAPKByte);
                            verifyDataClass.VerifyData(ref VerifyCipheredAnotherUserIDChecker, ref CipheredAnotherUserIDByte, CipheredSignedAnotherUserIDByte, ClientECDSAPKByte);
                            if (VerifyCipheredDirectoryIDByteChecker == true && VerifySignedRandomChallengeChecker == true && VerifyUniqueFilenameByteChecker == true && VerifyCipheredAnotherUserIDChecker == true)
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
                                    MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                    MyGeneralGCHandle.Free();
                                    MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                    SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                    MyGeneralGCHandle.Free();
                                    return "Error: Unable to decrypt main directory through specified ETLS shared secret";
                                }
                                UniqueUserFileStorageID = Encoding.UTF8.GetString(PlainText);
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
                                    return "Error: Man in the middle spotted, are you an imposter trying to mimic the ETLS that established between server and client?(Unable to decrypt sent ED25519PK)";
                                }
                                AnotherUserID = Encoding.UTF8.GetString(PlainText);
                                try
                                {
                                    FileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + AnotherUserID + "/" + "PK.txt");
                                    SignedFileED25519PK = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/Allowed_User/" + AnotherUserID + "/" + "SPK.txt");
                                    TestFileED25519PK = SodiumPublicKeyAuth.Verify(SignedFileED25519PK, FileED25519PK);
                                    if (FileED25519PK.SequenceEqual(TestFileED25519PK) == false)
                                    {
                                        throw new ArgumentException("Verified Signed ED25519PK does not match with normal ED25519PK");
                                    }
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
                                    return "Error: Unable to find corresponding directory's ED25519PK";
                                }
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
                                        MyGeneralGCHandle = GCHandle.Alloc(SharedSecret, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), SharedSecret.Length);
                                        MyGeneralGCHandle.Free();
                                        MyGeneralGCHandle = GCHandle.Alloc(ClientECDSAPKByte, GCHandleType.Pinned);
                                        SodiumSecureMemory.MemZero(MyGeneralGCHandle.AddrOfPinnedObject(), ClientECDSAPKByte.Length);
                                        MyGeneralGCHandle.Free();
                                        return "Error: Are you an imposter trying to represents this directory owner? (ED25519 failed to verify challenge)";
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
                                                return "Error: The specified main folder/encrypted file folder does not exists in the system";
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    CipheredSignedFileContentByte = System.IO.File.ReadAllBytes(FileStoragePath + UniqueUserFileStorageID + "/" + UniqueFilename + "/EndpointEncryptedFileContent" + FileCount.ToString() + ".txt");
                                                }
                                                catch
                                                {
                                                    return "Error:Failed to read file, is the file name or directory ID correct?";
                                                }
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
                                                return Convert.ToBase64String(CipheredSignedFileContentByte);
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
                                        return "Error: This challenge does not exist in system";
                                    }
                                }
                                else
                                {
                                    return "Error: You no longer own this rent directory/folder";
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
                                return "Error: Are you an imposter trying to mimic the client?";
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
                            return "Error: Please pass in correct Base64 encoded String in parameter";
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
                return "Error: Client didn't specify an ETLS ID";
            }
        }
    }
}
