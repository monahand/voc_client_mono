//Request library
using System.Net;
using System.IO;
using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;
using Newtonsoft.Json;

//sqllite
using Mono.Data.Sqlite;

public class VocSyncRequestClient
{
    public static bool Validator (object sender, X509Certificate certificate, X509Chain chain,
                                      SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
    public static bool GetRequest()
    {

	string html = string.Empty;
        string url = @"https://catalog.data.gov/api/3";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.AutomaticDecompression = DecompressionMethods.GZip;

        // Ignore certificates for now muhahaha
        ServicePointManager.ServerCertificateValidationCallback = Validator;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
                html = reader.ReadToEnd();
        }

        Console.WriteLine(html);
        Console.WriteLine(JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(html));
        return true;
    }
}

public class DatabaseLib
{
    public static bool execute_query(IDbConnection dbcon, string sql)
    {
        IDbCommand dbcmd = dbcon.CreateCommand();
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = sql;
        dbcmd.ExecuteNonQuery();
        dbcmd.Dispose();
        return true;
    }

    public static void create_tables()
    {
        const string connectionString = "URI=file:SqliteTest.db";
        IDbConnection dbconn = new SqliteConnection(connectionString);

        dbconn.Open();
        string sql = "create table if not exists voc_user" +
                          "(userid text, password text," +
                          "device_id text, platform text," +
                          "device_type text, access_token text," +
                          "refresh_token text, voc_id text," +
                          "congestion_detection text, ads_frequency text," +
                          "daily_quota integer, daily_manifest integer," +
                          "daily_download_wifi integer, daily_download_cellular integer," +
                          "congestion text, sdk_capabilities text," +
                          "max_content_duration integer, play_ads text," +
                          "skip_policy_first_time text, tod_policy text," +
                          "token_expiration integer, server text," +
                          "server_state text, my_row integer primary key autoincrement)";
        execute_query(dbconn, sql);

        sql =  "create table if not exists provider " +
                        " (name text unique, contentprovider text, subscribed integer)";
        execute_query(dbconn, sql);


	sql = "create table if not exists category (name text unique,subscribed integer)";
        execute_query(dbconn, sql);

        sql = "create table if not exists uuid_table (uuid text)";
        execute_query(dbconn, sql);
        
        sql = "create table if not exists playing (unique_Id text,timestamp DATETIME DEFAULT CURRENT_TIMESTAMP)";
        execute_query(dbconn, sql);

        sql = "create table if not exists content_status " + 
                " (download_time text,download length integer,download_duration real,eviction_info text,user_rating int,unique_id text, my_row integer primary key autoincrement)";
        execute_query(dbconn, sql);
		
        sql = "create table if not exists consumption_status (watch_time int,watchstart integer,watchend int,my_row integer primary key autoincrement)";
        execute_query(dbconn, sql);

        sql = " create table if not exists ad_consumption_status (adurl text,duration int, starttime integer,stopposition int, clicked int,unique_id text, my_row integer primary key autoincrement)";
        execute_query(dbconn, sql);

        sql = " create table if not exists cache_manifest " +
            "( local_file text, local_thumbnail text, " +
            " local_info text, video_size integer, " +
            " thumbnail_size integer, download_date integer, " +
            " content_provider text, category text, " +
            " unique_id text, summary text, " +
            " title text, duration integer, " +
            " timestamp integer, sdk_metadata text, " +
            " streams text,   ad_server_url text, " +
            " tags text, priority integer, " +
            " object_type text, thumb_attrs text, " +
            " object_attrs text, children text, " +
            " policy_name text, key_server_url text, " +
            " save integer default 0, my_row integer primary key autoincrement)";
        execute_query(dbconn, sql);

        // clean up
        dbconn.Close();

    } 

}


public class VocClient 
{


    static public void Main ()
    {
        Console.WriteLine ("Hello Mono World");
        DatabaseLib.create_tables();
    }

}