using System;

namespace Pharmacy_Manager
{
	public class Medicine
	{
		public int? ID { get; protected set; }
		public string Name { get; protected set; }
		public string Manufacturer { get; protected set; }
		public double Price { get; protected set; }
		public int Amount { get; protected set; }
		public bool WithPrescription { get; protected set; }

		public Medicine(int? id, string name, string manufacturer, double price, int amount, bool withPrescription)
		{
			this.ID = id;
			this.Name = name;
			this.Manufacturer = manufacturer;
			this.Price = price;
			this.Amount = amount;
			this.WithPrescription = withPrescription;
		}

		/// <summary>
		/// 	Pokazanie leku na konsoli
		/// </summary>

		public void ShowMedicine()
		{
			Console.WriteLine($"-> Nazwa: {this.Name}  |  Dostawca: {this.Manufacturer}\n" +
				$"   [ ID-{this.ID.ToString()}  |  Cena: {(this.Price + " zł.")}  |  Ilość: {(this.Amount + " szt.")}  |  Na receptę: {this.WithPrescription} ]\n");
		}
	}
}
