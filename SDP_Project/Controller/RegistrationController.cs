﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SDP_Project.Controller
{
    class RegistrationController
    {
        public static bool validateStaffID(String staffID)
        {
            string SQL = "select * from staff where staffID= '" + staffID + "'";
            MySqlCommand cmd = new MySqlCommand(SQL, FormContainer.conn);
            MySqlDataReader myData = cmd.ExecuteReader();
            if (!myData.HasRows)
            {
                myData.Close();
                return true;
            }
            else
            {
                myData.Close();
                return false;
            }
        }
        public static bool confirmPassword(String pwd, String cpwd)
        {
            if (pwd.Equals(cpwd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void register(String staffID, String password, String name, String rank)
        {
            // SQL 
            string SQL;
            MySqlCommand cmd;
            try
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    password = GetMd5Hash(md5Hash, password);
                }
                SQL = "insert into staff (staffid, password, name, rank) values ('" + staffID + "', '" + password + "', '" + name + "', '" + rank + "');";
                cmd = new MySqlCommand(SQL, FormContainer.conn);
                cmd.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Error " + ex.Number + " : " + ex.Message);
            }
        }

        #region Md5Hash method
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        // Verify a hash against a string.
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
