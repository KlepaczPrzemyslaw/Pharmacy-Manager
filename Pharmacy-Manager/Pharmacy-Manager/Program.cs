﻿using System;
using System.Collections.Generic;

namespace Pharmacy_Manager
{
	class Program
	{
		static void Main(string[] args)
		{
			string command = "";
			ConsoleEx.WriteLineInYellow("------ Pharmacy Manager ------");

			while (true)
			{
				Console.Write("Podaj komendę (lub wpisz help): ");
				command = Console.ReadLine().Trim().ToLower();
				Console.Clear();

				if (command == "exit")
					break;

				switch (command)
				{
					case "help":
						HelpCommand();
						break;
					case "madd":
						AddMedicineCommand();
						break;
					case "mshowall":
						ShowAllMedicinesCommand();
						break;
					case "mshowselected":
						ShowChosenMedicinesCommand();
						break;
					case "mupdate":
						UpdateMedicineCommand();
						break;
					case "mupdateamount":
						UpdateMedicineAmountCommand();
						break;
					case "mdelete":
						DeleteMedicineCommand();
						break;
					case "order":
						AddToOrder();
						break;
					default:
						ConsoleEx.WriteLineInRed("Niepoprawna komenda !!");
						break;
				}

				ConsoleEx.WriteLineInYellow(" Nowa Komenda ".PadLeft(22, '-').PadRight(30, '-'));
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// "DRY" //
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Pobiera lek od użytkownika
		/// 	Wykorzystuje: AddMedicineCommand() / UpdateMedicineCommand()
		/// </summary>
		/// <returns>
		/// 	Medicine / null
		/// </returns>

		private static Medicine GetMedicineFromUser_()
		{
			try
			{
				Console.Write("Podaj nazwę leku: ");
				string name = Console.ReadLine().Trim();

				Console.Write("Podaj producenta leku: ");
				string manufacturer = Console.ReadLine().Trim();

				Console.Write("Podaj cenę leku: ");
				double price = double.Parse(Console.ReadLine().Trim().Replace('.', ','));

				Console.Write("Podaj dostępną ilość leku: ");
				int amount = int.Parse(Console.ReadLine().Trim());

				bool withPrescription;
				Console.Write("Czy to lek na receptę (Y/N): ");
				if (Console.ReadLine().Trim() == "Y")
					withPrescription = true;
				else
					withPrescription = false;

				return new Medicine(null, name, manufacturer, price, amount, withPrescription);
			}
			catch (FormatException)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił błąd: Podałeś daną o nieprawidłowym formacie!!");
				return null;
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w pobieraniu leku od użytkownika: {e.GetType().ToString()}: {e.Message}!!");
				return null;
			}
		}

		/// <summary>
		/// 	Pokazuje listę leków 
		/// 	Wykorzystuje: ShowChosenMedicinesCommand() / ShowAllMedicinesCommand()
		/// </summary>
		/// <param name="byNamePart"></param>

		private static void ShowMedicines_(string byNamePart = "")
		{
			List<Medicine> medicines = MedicineDAO.SelectMedicines(byNamePart);

			if (medicines == null)
				return;

			foreach (Medicine medicine in medicines)
			{
				medicine.ShowMedicine();
			}

			ConsoleEx.WriteLineInGreen("Sukces!");
		}

		/// <summary>
		/// 	Zwraca listę leków z indeksami
		/// 	Wykorzystuje: GetMedicineByIndex_()
		/// </summary>
		/// <param name="byNamePart"></param>

		private static List<Medicine> GetMedicinesForIndex_(string byNamePart = "")
		{
			// Pobranie listy 
			List<Medicine> medicines = MedicineDAO.SelectMedicines(byNamePart);

			if (medicines == null)
				return null;

			return medicines;
		}

		/// <summary>
		/// 	Zwraca wybrany lek po indeksie bez upewniania się, czy rekord modyfikować 
		/// 	Wykorzystuje: GetMedicineForEditWithConfirmation_() / AddToOrder()
		/// </summary>
		/// <returns>
		/// 	Medicine / null
		/// </returns>

		private static Medicine GetMedicineByIndex_()
		{
			// Dodatkowa przeszukiwarka
			Console.Write("Jakiego leku szukasz (możesz zawęzić poszukiwania / wciśnij enter, aby pokazać wszystko): ");
			string text = Console.ReadLine().Trim();
			Console.WriteLine();
			List<Medicine> medList = GetMedicinesForIndex_(text);

			if (medList == null)
				return null;

			// Wypisanie lekarstw z indeksami
			for (int i = 0; i < medList.Count; i++)
			{
				ConsoleEx.WriteInGreen($"Index [{i}]:");
				medList[i].ShowMedicine();
			}

			// Pobranie indeksu
			int index;
			Console.Write("Wybierz index: ");
			try
			{
				index = int.Parse(Console.ReadLine().Trim());
			}
			catch
			{
				ConsoleEx.WriteLineInRed("Niepoprawny index!");
				return null;
			}

			// Pokazanie leku
			ConsoleEx.WriteLineInRed("\nWybrany lek: ");
			medList[index].ShowMedicine();

			return medList[index];
		}

		/// <summary>
		/// 	Zwraca wybrany lek po indeksie, wraz z upewnieniem się czy chcesz zmiany dokonać 
		/// 	Wykorzystuje: UpdateMedicineCommand() / UpdateMedicineAmountCommand() / DeleteMedicineCommand()
		/// </summary>
		/// <returns>
		/// 	Medicine / null
		/// </returns>

		private static Medicine GetMedicineForEditWithConfirmation_()
		{
			Medicine medicine = GetMedicineByIndex_();

			if (medicine == null)
				return null;

			// Upewnienie się 
			ConsoleEx.ImportantQuestion("Na pewno? (Y/N): ");
			if (Console.ReadLine().Trim() != "Y")
			{
				ConsoleEx.WriteLineInGreen("\nNiczego nie usunięto!");
				return null;
			}

			return medicine;
		}

		/// <summary>
		/// 	Pobiera receptę od użytkownika
		/// 	Wykorzystuje: AddToOrder()
		/// </summary>
		/// <returns>
		/// 	Prescription / null
		/// </returns>

		private static Prescription GetPrescriptionFromUser_()
		{
			try
			{
				Console.Write("\nPodaj imię i nazwisko klienta: ");
				string customerName = Console.ReadLine().Trim();

				Console.Write("Podaj PESEL klienta (11 znaków): ");
				long pesel = long.Parse(Console.ReadLine().Trim());

				if (pesel < 9999999999 || pesel > 100000000000)
					throw new FormatException();

				Console.Write("Podaj numer recepty: ");
				long prescriptionNumber = long.Parse(Console.ReadLine().Trim());

				return new Prescription(null, customerName, pesel, prescriptionNumber);
			}
			catch (FormatException)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił błąd: Podałeś daną o nieprawidłowym formacie!!");
				return null;
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w pobieraniu leku od użytkownika: {e.GetType().ToString()}: {e.Message}!!");
				return null;
			}
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// "Funkcje" //
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Help - pomoc dla użytkownika
		/// </summary>

		private static void HelpCommand()
		{
			ConsoleEx.WriteLineInCyan(" Help ".PadLeft(18, '-').PadRight(30, '-'));

			Console.WriteLine("Oto Lista komend:");
			Console.WriteLine("Komenda: help          -> Wyświetla spis komend wraz z wyjaśnieniami.");
			Console.WriteLine("Komenda: exit          -> Wychodzi z programu.\n");

			Console.WriteLine("Komenda: mshowall      -> Pokazuje listę wszystkich dostępnych leków.");
			Console.WriteLine("Komenda: mshowselected -> Pokazuje listę leków, które można przeszukać po nazwie.");
			Console.WriteLine("Komenda: madd          -> Dodaje lek.");
			Console.WriteLine("Komenda: mupdate       -> Pokazuje listę leków i pozwala jeden z nich zmienić.");
			Console.WriteLine("Komenda: mupdateamount -> Pokazuje listę leków i pozwala w jednym z nich zmienić ilość.");
			Console.WriteLine("Komenda: mdelete       -> Pokazuje listę leków i pozwala jeden z nich usunąć.\n");

			Console.WriteLine("Komenda: order         -> Przygotowuje zamówienie.\n");

			ConsoleEx.WriteLineInGreen("Sukces!");
		}

		/// <summary>
		/// 	Dodawanie leku do bazy
		/// </summary>

		private static void AddMedicineCommand()
		{
			ConsoleEx.WriteLineInCyan(" Dodawanie Leku ".PadLeft(23, '-').PadRight(30, '-'));

			Medicine medicine = GetMedicineFromUser_();

			if (medicine == null)
				return;

			// Insert
			MedicineDAO.InsertMedicineByTransaction(medicine);
		}

		/// <summary>
		/// 	Pokazuje dostępne lekarstwa z filtrem po nazwie
		/// </summary>

		private static void ShowChosenMedicinesCommand()
		{
			ConsoleEx.WriteLineInCyan(" Lista Lekarstw ".PadLeft(23, '-').PadRight(30, '-'));

			Console.Write("Jakiego leku szukasz: ");
			string text = Console.ReadLine().Trim();
			Console.WriteLine();

			// Odpala metodę szukającą z podanym parametrem
			ShowMedicines_(text);
		}

		/// <summary>
		/// 	Pokazuje wszyskie dostępne lekarstwa
		/// </summary>

		public static void ShowAllMedicinesCommand()
		{
			ConsoleEx.WriteLineInCyan(" Lista Lekarstw ".PadLeft(23, '-').PadRight(30, '-'));

			// Odpala metodę szukającą bez parametru
			ShowMedicines_();
		}

		/// <summary>
		/// 	Zmiana całego leku z bazy
		/// </summary>

		private static void UpdateMedicineCommand()
		{
			ConsoleEx.WriteLineInCyan(" Zmiana Lekarstwa ".PadLeft(24, '-').PadRight(30, '-'));

			// Pobranie leku do zmiany
			Medicine oldMedicine = GetMedicineForEditWithConfirmation_();
			Console.WriteLine();

			if (oldMedicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Pobranie nowego leku
			Medicine newMedicine = GetMedicineFromUser_();

			if (newMedicine == null)
				return;

			// Update - Transakcja
			MedicineDAO.UpdateMedicineByTransaction(newMedicine, oldMedicine);
		}

		/// <summary>
		/// 	Zmiana ilości leku
		/// </summary>

		private static void UpdateMedicineAmountCommand()
		{
			ConsoleEx.WriteLineInCyan(" Zmiana Ilości Leku ".PadLeft(25, '-').PadRight(30, '-'));

			// Pobranie leku
			Medicine oldMedicine = GetMedicineForEditWithConfirmation_();
			Console.WriteLine();

			if (oldMedicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Pobranie ilości
			int newAmount;
			Console.Write("Podaj nową ilość: ");
			try
			{
				newAmount = int.Parse(Console.ReadLine().Trim());
			}
			catch
			{
				ConsoleEx.WriteLineInRed("Niepoprawny index!");
				return;
			}

			// Update Amount - Transakcja
			MedicineDAO.UpdateMedicineAmountByTransaction(oldMedicine, newAmount);
		}

		/// <summary>
		/// 	Usuwanie leku z bazy
		/// </summary>

		private static void DeleteMedicineCommand()
		{
			ConsoleEx.WriteLineInCyan(" Usuwanie Lekarstwa ".PadLeft(25, '-').PadRight(30, '-'));

			// Pobranie leku
			Medicine medicine = GetMedicineForEditWithConfirmation_();

			if (medicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Delete - Transakcja
			MedicineDAO.DeleteMedicineByTransaction(medicine);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// "Zamówienie" //
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// 	Dodaje lek do zamówienia
		/// </summary>

		private static void AddToOrder()
		{
			OrderSqlTransaction orderSqlTransaction = new OrderSqlTransaction();

			ConsoleEx.WriteLineInCyan(" Zamówienie ".PadLeft(21, '-').PadRight(30, '-'));

			// Pobranie leku
			Medicine medicine = GetMedicineByIndex_();

			if (medicine == null)
			{
				ConsoleEx.WriteLineInRed("Anulowano akcję!");
				return;
			}

			// Pobranie ilości
			int quantity;
			Console.Write("Podaj ilość: ");
			try
			{
				quantity = int.Parse(Console.ReadLine().Trim());

				if (quantity > medicine.Amount)
					throw new QuantityException();
			}
			catch (QuantityException)
			{
				ConsoleEx.WriteLineInRed($"Brak wystarczającej ilości leku w bazie!!");
				return;
			}
			catch (Exception e)
			{
				ConsoleEx.WriteLineInRed($"Nastąpił wyjątek w pobieraniu ilości: {e.GetType().ToString()}: {e.Message}!!");
				return;
			}

			// Scieżka recepty
			if (medicine.WithPrescription == true)
			{
				ConsoleEx.WriteLineInRed($"\nTen lek jest na receptę!");

				// musimy takową pobrać
				Console.Write("Czy klient posiada receptę? (Y/N): ");
				if (Console.ReadLine() != "Y")
				{
					ConsoleEx.WriteLineInRed("Anulowano akcję!");
					return;
				}

				Prescription prescription = GetPrescriptionFromUser_();
				if (prescription == null)
					return;

				try
				{
					orderSqlTransaction.InsertPrescription(prescription);
					orderSqlTransaction.UpdateMedicineAmount(medicine, (medicine.Amount - quantity));
					orderSqlTransaction.InsertOrderWithPrescription(new Order(null, prescription.ID, medicine.ID, DateTime.Now, quantity));

					Console.WriteLine($"\nSuma zamówienia: {medicine.Price * quantity}zł. Czy klient zapłacił? (Y/N): ");
					if (Console.ReadLine() != "Y")
					{
						ConsoleEx.WriteLineInRed("Anulowano akcję!");
						orderSqlTransaction.TransactionRollback();
						return;
					}

					orderSqlTransaction.TransactionCommit();
					ConsoleEx.WriteLineInGreen("Sukces!");
				}
				catch (Exception e)
				{
					ConsoleEx.WriteLineInRed($"Wyjątek przy finalizacji: {e.GetType()}: {e.Message}");
					orderSqlTransaction.TransactionRollback();
				}
			}
			// Bez recepty
			else
			{
				try
				{
					orderSqlTransaction.UpdateMedicineAmount(medicine, (medicine.Amount - quantity));
					orderSqlTransaction.InsertOrderWithoutPrescription(new Order(null, null, medicine.ID, DateTime.Now, quantity));

					Console.WriteLine($"\nSuma zamówienia: {medicine.Price * quantity}zł. Czy klient zapłacił? (Y/N): ");
					if (Console.ReadLine() != "Y")
					{
						ConsoleEx.WriteLineInRed("Anulowano akcję!");
						orderSqlTransaction.TransactionRollback();
						return;
					}

					orderSqlTransaction.TransactionCommit();
					ConsoleEx.WriteLineInGreen("Sukces!");
				}
				catch (Exception e)
				{
					ConsoleEx.WriteLineInRed($"Wyjątek przy finalizacji: {e.GetType()}: {e.Message}");
					orderSqlTransaction.TransactionRollback();
				}
			}
		}
	}
}