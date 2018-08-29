using System;
using System.Data.SqlClient;

namespace Pharmacy_Manager
{
	public static class SqlTransactionTool
	{
		/// <summary>
		/// 	Transakcja -> wymaga obiektu SqlCommand z wpisaną komendą i podpiętymi parametrami!
		/// </summary>
		/// <param name="sqlCommand"></param>

		public static void Transaction(SqlCommand sqlCommand)
		{
			int queryValue;
			SqlTransaction transaction = null;

			using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.connectionString))
			{
				try
				{
					// Dodanie komendy do połączenia
					sqlCommand.Connection = sqlConnection;
					// Otwarcie połączenia
					sqlConnection.Open();
					// Rozpoczęcie transakcji
					transaction = sqlConnection.BeginTransaction();
					// Dodanie transakcji do komendy
					sqlCommand.Transaction = transaction;
					// Wykonanie zapytania
					queryValue = sqlCommand.ExecuteNonQuery();
					// Exception jeżeli zapytanie zwróci 0 lub 1 dla zmienionych wierszy
					// 1 - dlatego że mamy bazę logów - a tam zapytanie wykona się zawsze
					// Więc - poprawna transakcja zwróci zawsze 2 wiersze - 1 dla usunięcia np. nieistniejącej wartości - 0 dla błędu
					if (queryValue == 0 || queryValue == 1)
						throw new ZeroException();
					// Commit
					transaction.Commit();
					ConsoleEx.WriteLineWithColor("Sukces!", ConsoleColor.Green);
				}
				catch (ZeroException)
				{
					ConsoleEx.WriteLineWithColor("Nastąpił wyjątek w transakcji: Transakcja została wykonana, ale nie zmieniono żadnego wiersza!!", ConsoleColor.Red);

					try
					{
						transaction.Rollback();
					}
					catch (Exception e2)
					{
						ConsoleEx.WriteLineWithColor($"Nastąpił wyjątek przy anulowaniu transakcji: {e2.GetType().ToString()}: {e2.Message}!!", ConsoleColor.Red);
					}
				}
				catch (Exception e)
				{
					ConsoleEx.WriteLineWithColor($"Nastąpił wyjątek w transakcji: {e.GetType().ToString()}: {e.Message}!!", ConsoleColor.Red);

					try
					{
						transaction.Rollback();
					}
					catch (Exception e2)
					{
						ConsoleEx.WriteLineWithColor($"Nastąpił wyjątek przy anulowaniu transakcji: {e2.GetType().ToString()}: {e2.Message}!!", ConsoleColor.Red);
					}
				}
			} // Zamknięcie połączenia
		}

	}
}
