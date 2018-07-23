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

			try
			{
				using (SqlConnection sqlConnection = new SqlConnection(ConnectionString.connectionString))
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
					ConsoleEx.WriteLineInGreen("\nSukces!");
				}
			}
			catch (ZeroException)
			{
				ConsoleEx.WriteLineInRed("Nastąpił wyjątek w transakcji: Transakcja została wykonana, ale nie zmieniono żadnego wiersza!!");

				try
				{
					transaction.Rollback();
				}
				catch (Exception e2)
				{
					ConsoleEx.WriteLineInRed($"Nastąpił wyjątek przy anulowaniu transakcji: {e2.GetType().ToString()}: {e2.Message}!!");
				}
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w transakcji: {e.GetType().ToString()}: {e.Message}!!");

				try
				{
					transaction.Rollback();
				}
				catch (Exception e2)
				{
					ConsoleEx.WriteLineInRed($"Nastąpił wyjątek przy anulowaniu transakcji: {e2.GetType().ToString()}: {e2.Message}!!");
				}
			}
		}
	}
}
