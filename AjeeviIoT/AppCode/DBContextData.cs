using Microsoft.AspNetCore.Hosting;
using System;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using MySqlConnector;
using System.Xml;
using Newtonsoft.Json.Linq;


public class DBContextData
{



    public static string ExecuteRowSqlCommand(string query)
    {
        try
        {
            using (var connection = new MySqlConnection(GlobalModel.ConnectionString))
            {
                using (var command = new MySqlCommand(query, connection))
                {


                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new StringBuilder();
                        while (reader.Read())
                        {
                            // Process each row here
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Append(reader[i].ToString() + ", ");
                            }
                            result.AppendLine();
                        }
                        return result.ToString(); // Return your result here
                    }
                }
            }
        }
        catch
        {
            return null;
        }
    }

    public static string ExecuteQueryDynamicDataset(string spQuery)
    {
        try
        {
            using (MySqlConnection con = new MySqlConnection(GlobalModel.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(spQuery, con))
                {

                    //cmd.CommandTimeout = Int32.MaxValue;
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        DataSet dt = new DataSet();
                        sda.Fill(dt);

                        return DataSetToJSONWithJSONNet(dt);
                    }
                }
            }
        }
        catch
        {
            return null;
        }


    }


    public static int ExecuteQueryTotalCount(string spQuery)
    {
        try
        {
            using (MySqlConnection con = new MySqlConnection(GlobalModel.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(spQuery, con))
                {

                    //cmd.CommandTimeout = Int32.MaxValue;
                    cmd.CommandType = CommandType.Text;

                    using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                    {
                        DataSet dt = new DataSet();
                        sda.Fill(dt);

                        return DataSetTointWithJSONNet(dt);
                    }
                }
            }
        }
        catch
        {
            return 0;
        }


    }

    public static string DataSetToJSONWithJSONNet(DataSet table)
    {
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);
        var jsonObject = JObject.Parse(JSONString);
        JSONString = jsonObject["Table"].ToString();
        return JSONString;
    }

    public static int DataSetTointWithJSONNet(DataSet table)
    {
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);
        var jsonObject = JObject.Parse(JSONString);
        JSONString = jsonObject["Table"][0]["TotalCount"].ToString();
        return Convert.ToInt32(JSONString);
    }

}

