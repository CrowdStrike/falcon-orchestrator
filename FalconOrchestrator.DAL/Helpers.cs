//<Falcon Orchestrator provides automated workflow and response capabilities>
//    Copyright(C) 2016 CrowdStrike

//   This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or(at your option) any later version.

//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with this program.If not, see<http://www.gnu.org/licenses/>.


using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace FalconOrchestrator.DAL
{
    public class DatabaseHelpers
    {
        public static bool TestConnection()
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                try
                {
                    db.Database.Connection.Open();
                    return true;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    return false;
                }
            }
        }

        public static string GetConfigValue(string key)
        {

                using (FalconOrchestratorDB db = new FalconOrchestratorDB())
                {
                    return db.Configurations.Where(x => x.Key.Equals(key)).Select(x => x.Value).FirstOrDefault();
                }
        }

        public static bool AccountExists(string accountName)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Accounts.Any(x => x.AccountName.Equals((accountName)));
            }
        }

        public static void SetConfigValue(string key, string value)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                if(!db.Configurations.Any(x => x.Key.Equals(key)))
                {
                    Configuration config = new Configuration
                    {
                        Key = key,
                        Value = value
                    };
                    db.Configurations.Add(config);
                }
                else
                {
                    Configuration config = db.Configurations.Where(x => x.Key.Equals(key)).FirstOrDefault();
                    config.Value = value;
                }
                db.SaveChanges();
            }
        } 

        public static int GetNextDetectionID()
        {
            try
            {
                using (FalconOrchestratorDB db = new FalconOrchestratorDB())
                {
                    Detection result = db.Detections.OrderByDescending(x => x.DetectionId).FirstOrDefault();
                    if (result == null)
                        return 1;
                    else
                        return (int)result.DetectionId + 1;
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public static DateTime GetAccountTimestamp(string accountName)
        {
            using (FalconOrchestratorDB db = new FalconOrchestratorDB())
            {
                return db.Accounts.Where(x => x.AccountName == accountName).Select(y => y.Timestamp).FirstOrDefault();
            }
        }

        public static string GetResponderEmailAddress(int? responderId, AppConfiguration config)
        {
            try
            {
                if (responderId == null)
                    return config.EMAIL_CC;

                using (FalconOrchestratorDB db = new FalconOrchestratorDB())
                {
                    return db.Responders.Where(x => x.ResponderId == responderId).Select(y => y.EmailAddress).FirstOrDefault();
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

    }

    public class Crypto
    {
        public static string EncryptText(string input, string password)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);
            string result = Convert.ToBase64String(bytesEncrypted);

            return result;
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 256;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static string DecryptText(string input, string password)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 256;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

    }

}
