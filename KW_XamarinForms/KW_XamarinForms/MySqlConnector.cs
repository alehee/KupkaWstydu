using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using Plugin.Toast;

namespace KW_XamarinForms
{
    class MySqlConnector
    {
        public MySqlConnection getConnection()
        {
            const string CONNECTION_STRING = "server=aleksanderheese.pl;port=3306;user=u986763087_kwadmin;password=KwP@ssw0rd;database=u986763087_kupkawstydu;";
            return new MySqlConnection(CONNECTION_STRING);
        }

        public string[] getUserData(int userId)
        {
            if (userId > 0)
            {
                string[] returnTab = new string[] { "", "", "", "" };

                /* 
                 * 0. Pseudonym
                 * 1. Date
                 * 2. ArrearsCompleted
                 * 3. ArrearsNotCompleted
                 */

                MySqlConnection conn = getConnection();
                string sql = "SELECT Pseudonym, Date, ArrearsCompleted, ArrearsNotCompleted FROM users WHERE ID=" + userId + " LIMIT 1";
                MySqlCommand query = new MySqlCommand(sql, conn);
                query.CommandTimeout = 30;
                try
                {
                    conn.Open();
                    MySqlDataReader mySqlDataReader = query.ExecuteReader();

                    if (mySqlDataReader.HasRows)
                    {
                        while (mySqlDataReader.Read())
                        {
                            returnTab[0] = mySqlDataReader.GetValue(0).ToString();
                            returnTab[1] = mySqlDataReader.GetValue(1).ToString();
                            returnTab[2] = mySqlDataReader.GetValue(2).ToString();
                            returnTab[3] = mySqlDataReader.GetValue(3).ToString();
                        }
                    }

                    conn.Close();
                    return returnTab;
                }
                catch (MySqlException e)
                {
                    CrossToastPopUp.Current.ShowToastError("Connection Error! Check internet connection!");
                }

                return null;
            }

            else
                return null;
        }

        public void modifyUserData(int userId, string newName)
        {
            MySqlConnection conn = getConnection();
            string sql = "UPDATE users SET Pseudonym='" + newName + "' WHERE ID=" + userId;
            MySqlCommand query = new MySqlCommand(sql, conn);
            query.CommandTimeout = 30;
            try
            {
                conn.Open();

                MySqlDataReader mySqlDataReader = query.ExecuteReader();

                conn.Close();
            }
            catch (MySqlException e)
            {
                CrossToastPopUp.Current.ShowToastError("Connection Error! Check internet connection!");
            }
        }

        public int createUser()
        {
            int userId = 0;

            MySqlConnection conn = getConnection();
            string sql = "INSERT INTO users(ID, Pseudonym, Date, ArrearsCompleted, ArrearsNotCompleted) VALUES(NULL, 'Nowy Członek', CURRENT_TIMESTAMP, 0, 0)";
            MySqlCommand query = new MySqlCommand(sql, conn);
            query.CommandTimeout = 30;
            try
            {
                conn.Open();

                MySqlDataReader mySqlDataReader = query.ExecuteReader();

                conn.Close();
                conn.Open();

                sql = "SELECT ID FROM users ORDER BY ID DESC LIMIT 1";
                query = new MySqlCommand(sql, conn);
                mySqlDataReader = query.ExecuteReader();

                if (mySqlDataReader.HasRows)
                {
                    while (mySqlDataReader.Read())
                    {
                        userId = mySqlDataReader.GetInt32(0);
                    }
                }

                conn.Close();

                return userId;
            }
            catch(MySqlException e)
            {
                CrossToastPopUp.Current.ShowToastError("Connection Error! Check internet connection! " + e.ToString(), Plugin.Toast.Abstractions.ToastLength.Long);
                return 0;
            }
        }

        public void modifyCompletedArrears(int userId, int modificator)
        {
            MySqlConnection conn = getConnection();
            string sql = "";
            if(modificator == 1)
                sql = "UPDATE users SET ArrearsCompleted = ArrearsCompleted + 1 WHERE ID=" + userId;
            else
                sql = "UPDATE users SET ArrearsCompleted = ArrearsCompleted - 1 WHERE ID=" + userId;
            MySqlCommand query = new MySqlCommand(sql, conn);
            query.CommandTimeout = 30;
            try
            {
                conn.Open();

                MySqlDataReader mySqlDataReader = query.ExecuteReader();

                conn.Close();
            }
            catch (MySqlException e)
            {
                CrossToastPopUp.Current.ShowToastError("Connection Error! Check internet connection!");
            }
        }

        public void modifyNotCompletedArrears(int userId, int modificator)
        {
            MySqlConnection conn = getConnection();
            string sql = "";
            if (modificator == 1)
                sql = "UPDATE users SET ArrearsNotCompleted = ArrearsNotCompleted + 1 WHERE ID=" + userId;
            else
                sql = "UPDATE users SET ArrearsNotCompleted = ArrearsNotCompleted - 1 WHERE ID=" + userId;
            MySqlCommand query = new MySqlCommand(sql, conn);
            query.CommandTimeout = 30;
            try
            {
                conn.Open();

                MySqlDataReader mySqlDataReader = query.ExecuteReader();

                conn.Close();
            }
            catch (MySqlException e)
            {
                CrossToastPopUp.Current.ShowToastError("Connection Error! Check internet connection!");
            }
        }

        public string[,] getHighscoreArray(int option)
        {
            string[,] returnTab = new string[100, 3];
            string sql = "";

            // Najwięcej wykonanych
            if (option == 1)
            {
                sql = "SELECT ID, Pseudonym, ArrearsCompleted FROM users ORDER BY ArrearsCompleted DESC LIMIT 100";
            }

            // Najwięcej do zrobienia
            else if(option == 2)
            {
                sql = "SELECT ID, Pseudonym, ArrearsNotCompleted FROM users ORDER BY ArrearsNotCompleted DESC LIMIT 100";
            }

            MySqlConnection conn = getConnection();
            MySqlCommand query = new MySqlCommand(sql, conn);
            query.CommandTimeout = 30;
            try
            {
                conn.Open();
                MySqlDataReader mySqlDataReader = query.ExecuteReader();

                if (mySqlDataReader.HasRows)
                {
                    int iterator = 0;
                    while (mySqlDataReader.Read())
                    {
                        returnTab[iterator, 0] = mySqlDataReader.GetValue(0).ToString();
                        returnTab[iterator, 1] = mySqlDataReader.GetValue(1).ToString();
                        returnTab[iterator, 2] = mySqlDataReader.GetValue(2).ToString();
                        iterator++;
                    }
                }

                conn.Close();
                return returnTab;
            }
            catch (MySqlException e)
            {
                CrossToastPopUp.Current.ShowToastError("Connection Error! Check internet connection!");
            }

            return null;
        }
    }
}