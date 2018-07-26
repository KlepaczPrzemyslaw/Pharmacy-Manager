using System;
using System.Data.SqlClient;

namespace Pharmacy_Manager
{
	/// <summary>
	/// 	Klasa dla zamówień - Podczas zamówienia dodajemy 2, lub 3 rzeczy -> a to mechanizm do cofania zmian, gdyby coś poszło nie tak.
	/// </summary>

	public class OrderSqlTransaction
	{
		SqlConnection sqlConnection = new SqlConnection(ConnectionString.connectionString);
		SqlTransaction transaction;
		SqlCommand sqlCommand = new SqlCommand();

		/// <summary>
		/// 	Konstruktor -> otwiera transakcję
		/// </summary>

		public OrderSqlTransaction()
		{
			sqlConnection.Open();
			this.transaction = sqlConnection.BeginTransaction();
			sqlCommand.Connection = sqlConnection;
			sqlCommand.Transaction = transaction;
		}

		/// <summary>
		/// 	Potwierdzenie transakcji
		/// </summary>

		public void TransactionCommit()
		{
			transaction.Commit();
			sqlConnection.Close();
		}

		/// <summary>
		/// 	Anulowanie transakcji
		/// </summary>

		public void TransactionRollback()
		{
			transaction.Rollback();
			sqlConnection.Close();
		}

		/// <summary>
		/// 	Dodaje receptę do bazy danych - dodając do obiektu ID. -> Obiekt prescription wraca z uzupełnionym ID.
		/// </summary>
		/// <param name="prescription"></param>

		public void InsertPrescription(Prescription prescription)
		{
			sqlCommand.CommandText = "INSERT INTO Prescriptions VALUES (@CustomerName, @PESEL, @PrescriptionNumber); SELECT CAST(scope_identity() AS int)";

			SqlParameter sqlCustomerNameParam = new SqlParameter { ParameterName = "@CustomerName", Value = prescription.CustomerName, DbType = System.Data.DbType.String};
			sqlCommand.Parameters.Add(sqlCustomerNameParam);

			SqlParameter sqlPESELParam = new SqlParameter { ParameterName = "@PESEL", Value = prescription.PESEL, DbType = System.Data.DbType.Int64 };
			sqlCommand.Parameters.Add(sqlPESELParam);

			SqlParameter sqlPrescriptionNumberParam = new SqlParameter { ParameterName = "@PrescriptionNumber", Value = prescription.PrescriptionNumber,
				DbType = System.Data.DbType.Int64 };
			sqlCommand.Parameters.Add(sqlPrescriptionNumberParam);

			// Przypisanie ID dodanego rekordu
			prescription.SaveID((int) sqlCommand.ExecuteScalar());

			sqlCommand.Parameters.Remove(sqlCustomerNameParam);
			sqlCommand.Parameters.Remove(sqlPESELParam);
			sqlCommand.Parameters.Remove(sqlPrescriptionNumberParam);
		}

		/// <summary>
		/// 	Wykonuje UPDATE na ilości leku
		/// </summary>
		/// <param name="medicine"></param>
		/// <param name="newAmount"></param>

		public void UpdateMedicineAmount(Medicine medicine, int newAmount)
		{
			sqlCommand.CommandText = $"UPDATE Medicines SET Amount = @Amount WHERE ID = @ID;";

			SqlParameter sqlAmountParam = new SqlParameter {ParameterName = "@Amount", Value = newAmount, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlAmountParam);

			SqlParameter sqlIDParam = new SqlParameter {ParameterName = "@ID", Value = medicine.ID, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlIDParam);

			sqlCommand.ExecuteNonQuery();

			sqlCommand.Parameters.Remove(sqlAmountParam);
			sqlCommand.Parameters.Remove(sqlIDParam);
		}

		/// <summary>
		/// 	Wykonuje INSERT na tabeli Orders -> Dla recepty
		/// </summary>
		/// <param name="order"></param>

		public void InsertOrderWithPrescription(Order order)
		{
			sqlCommand.CommandText = $"INSERT INTO Orders VALUES (@PrescriptionID, @MedicineID, @Date, @Amount);";

			SqlParameter sqlPrescriptionIDParam = new SqlParameter { ParameterName = "@PrescriptionID", Value = order.PrescriptionID, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlPrescriptionIDParam);

			SqlParameter sqlMedicineIDParam = new SqlParameter { ParameterName = "@MedicineID", Value = order.MedicineID, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlMedicineIDParam);

			SqlParameter sqlDateParam = new SqlParameter { ParameterName = "@Date", Value = order.Date, DbType = System.Data.DbType.DateTime };
			sqlCommand.Parameters.Add(sqlDateParam);

			SqlParameter sqlAmountParam = new SqlParameter { ParameterName = "@Amount", Value = order.Amount, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlAmountParam);

			sqlCommand.ExecuteNonQuery();

			sqlCommand.Parameters.Remove(sqlPrescriptionIDParam);
			sqlCommand.Parameters.Remove(sqlMedicineIDParam);
			sqlCommand.Parameters.Remove(sqlDateParam);
			sqlCommand.Parameters.Remove(sqlAmountParam);
		}

		/// <summary>
		/// 	Wykonuje INSERT na tabeli Orders -> Bez recepty
		/// </summary>
		/// <param name="order"></param>

		public void InsertOrderWithoutPrescription(Order order)
		{
			sqlCommand.CommandText = $"INSERT INTO Orders VALUES (null, @MedicineID, @Date, @Amount);";

			SqlParameter sqlMedicineIDParam = new SqlParameter { ParameterName = "@MedicineID", Value = order.MedicineID, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlMedicineIDParam);

			SqlParameter sqlDateParam = new SqlParameter { ParameterName = "@Date", Value = order.Date, DbType = System.Data.DbType.DateTime };
			sqlCommand.Parameters.Add(sqlDateParam);

			SqlParameter sqlAmountParam = new SqlParameter { ParameterName = "@Amount", Value = order.Amount, DbType = System.Data.DbType.Int32 };
			sqlCommand.Parameters.Add(sqlAmountParam);

			sqlCommand.ExecuteNonQuery();

			sqlCommand.Parameters.Remove(sqlMedicineIDParam);
			sqlCommand.Parameters.Remove(sqlDateParam);
			sqlCommand.Parameters.Remove(sqlAmountParam);
		}
	}
}
