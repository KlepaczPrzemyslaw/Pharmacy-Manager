using System;
using System.Data.SqlClient;

namespace Pharmacy_Manager
{
	public class OrderSqlTransaction
	{
		SqlConnection sqlConnection_ = new SqlConnection(ConnectionString.connectionString);
		SqlTransaction transaction;
		SqlCommand sqlCommand = new SqlCommand();

		public OrderSqlTransaction()
		{
			sqlConnection_.Open();
			this.transaction = sqlConnection_.BeginTransaction();
			sqlCommand.Connection = sqlConnection_;
			sqlCommand.Transaction = transaction;
		}

		public void TransactionCommit()
		{
			transaction.Commit();
			sqlConnection_.Close();
		}

		public void TransactionRollback()
		{
			transaction.Rollback();
			sqlConnection_.Close();
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
		/// 	Wykonuje INSERT na tabeli Orders
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
		/// 	Wykonuje INSERT na tabeli Orders
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
