namespace Pharmacy_Manager
{
	public static class ConnectionString
	{
		/// <summary>
		/// 	String odpowiadający za połączenie z bazą.
		/// </summary>

		public static string connectionString { get; } = "Integrated Security = SSPI;Data Source=.\\SQLEXPRESS;Initial Catalog=Pharmacy;";
	}
}
