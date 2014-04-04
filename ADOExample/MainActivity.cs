using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Mono.Data.Sqlite;
using System.Data;
namespace ADOExample
{
	[Activity (Label = "ADOExample", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private Android.App.AlertDialog.Builder builder;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button btnCreate = FindViewById<Button> (Resource.Id.btnCreate);
			Button btnShow = FindViewById<Button> (Resource.Id.btnShow);
			Button btnUpdate= FindViewById<Button> (Resource.Id.btnUpdate);
			Button btnDelete= FindViewById<Button> (Resource.Id.btnDelete);
			EditText txtMsj = FindViewById<EditText> (Resource.Id.txtMensaje);
			EditText txtID = FindViewById<EditText> (Resource.Id.txtID);
			builder = new AlertDialog.Builder(this);
			AlertDialog alert = builder.Create();
			alert.SetTitle (Resource.String.app_name);
			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "adodemo.db3");
			createDataBase ();
			btnCreate.Click += delegate {
				var connection = new SqliteConnection ("Data Source=" + dbPath);
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandText = "INSERT INTO Items VALUES(@_id, @Symbol)";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@_id", txtID.Text);
				command.Parameters.AddWithValue("@Symbol", txtMsj.Text);
				command.ExecuteNonQuery();
			};

			btnShow.Click += delegate {
				var connection = new SqliteConnection ("Data Source=" + dbPath);
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM Items WHERE _id = @_id";
				command.Parameters.AddWithValue("@_id",txtID.Text);
				command.CommandType = CommandType.Text;
				var r = command.ExecuteReader();
				alert.SetMessage(string.Format("ID: {0}, Value:{1}\n",r["_id"],r["Symbol"]));
				alert.Show();
				connection.Close();
			};

			btnUpdate.Click += delegate {
				var connection = new SqliteConnection ("Data Source=" + dbPath);
				connection.Open ();
				var command = connection.CreateCommand ();
				command.CommandText = "UPDATE Items SET Symbol = @Symbol WHERE _id = @_id";
				command.Parameters.AddWithValue ("@_id", txtID.Text);
				command.Parameters.AddWithValue ("@Symbol", txtMsj.Text);
				command.ExecuteNonQuery ();
			};
				
			btnDelete.Click += delegate {
				var connection = new SqliteConnection ("Data Source=" + dbPath);
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandText = "DELETE FROM Items WHERE _id = @_id";
				command.Parameters.AddWithValue("@_id", txtID.Text);
				command.ExecuteNonQuery();
			};
		}

		public void createDataBase(){
			string dbPath = System.IO.Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "adodemo.db3");
			bool exists = System.IO.File.Exists (dbPath);
			if (!exists) {
				Console.WriteLine("Creating database");
				Mono.Data.Sqlite.SqliteConnection.CreateFile (dbPath);
				var connection = new SqliteConnection ("Data Source=" + dbPath);

				var commands = new[] {
					"CREATE TABLE [Items] (_id ntext, Symbol ntext);",
					"INSERT INTO [Items] ([_id], [Symbol]) VALUES ('1', 'AAPL')",
					"INSERT INTO [Items] ([_id], [Symbol]) VALUES ('2', 'GOOG')",
					"INSERT INTO [Items] ([_id], [Symbol]) VALUES ('3', 'MSFT')"
				};
				connection.Open ();
				foreach (var command in commands) {
					using (var c = connection.CreateCommand ()) {
						c.CommandText = command;
						var rowcount = c.ExecuteNonQuery ();
						Console.WriteLine("\tExecuted " + command);
					}
				}
			} else {
				Console.WriteLine("Database already exists");
				var connection = new SqliteConnection ("Data Source=" + dbPath);
				connection.Open ();
				connection.Close ();
			}
		}
	}
}


