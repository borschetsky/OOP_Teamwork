﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AutoService.Core.Factory;
using AutoService.Models.Assets;
using AutoService.Models.Assets.Contracts;
using AutoService.Models.BusinessProcess.Contracts;
using AutoService.Models.BusinessProcess.Enums;
using AutoService.Models.Common.Models;
using AutoService.Models.Contracts;
using AutoService.Models.Vehicles.Contracts;

namespace AutoService.Core
{
    public sealed class Engine : IEngine
    {
        private readonly IList<IEmployee> employees;
        private readonly ICollection<IAsset> bankAccounts;
        private readonly ICollection<ICounterparty> clients;
        private readonly ICollection<ICounterparty> vendors;
        private readonly IDictionary<IClient, List<ISell>> notInvoicedSells;

        private DateTime lastInvoiceDate = DateTime.ParseExact("2017-01-15", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        private int lastInvoiceNumber = 0;
        private IAutoServiceFactory factory;


        private static readonly IEngine SingleInstance = new Engine();

        private Engine()
        {
            this.factory = new AutoServiceFactory();
            this.employees = new List<IEmployee>();
            this.bankAccounts = new List<IAsset>();
            this.clients = new List<ICounterparty>();
            this.vendors = new List<ICounterparty>();
            this.notInvoicedSells = new Dictionary<IClient, List<ISell>>();
        }

        public static IEngine Instance
        {
            get
            {
                return SingleInstance;
            }
        }

      
        public void Run()
        {
            var command = ReadCommand();
            var commandParameters = new string[] { string.Empty };

            while (command != "exit")
            {
                commandParameters = ParseCommand(command);
                try
                {
                    ExecuteSingleCommand(commandParameters);
                }
                catch (NotSupportedException e)
                {
                    Console.Write(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
                command = ReadCommand();
            }
        }

        private string ReadCommand()
        {
            return Console.ReadLine();
        }

        private string[] ParseCommand(string command)
        {
            return command.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void ExecuteSingleCommand(string[] commandParameters)
        {
            var commandType = commandParameters[0];
            switch (commandType)
            {
                case "showEmployees":

                    if (this.employees.Count == 0)
                    {
                        throw new ArgumentException("There are no employees! Hire them!");
                    }
                    this.ShowEmployees();
                    break;
                case "hireEmployee":
                    if (commandParameters.Length != 7)
                    {
                        throw new NotSupportedException(
                            "Employee constructor with less or more than 6 parameters not supported.");
                    }
                    var firstName = commandParameters[1];
                    var lastName = commandParameters[2];
                    var position = commandParameters[3];
                    decimal salary = decimal.TryParse(commandParameters[4], out salary)
                        ? salary
                        : throw new ArgumentException("Please provide a valid decimal value for salary!");
                    decimal ratePerMinute = decimal.TryParse(commandParameters[5], out ratePerMinute)
                        ? ratePerMinute
                        : throw new ArgumentException("Please provide a valid decimal value for rate per minute!");
                    ;
                    DepartmentType department;
                    if (!Enum.TryParse(commandParameters[6], out department))
                    {
                        throw new ArgumentException("Department not valid!");
                    }

                    this.AddEmployee(firstName, lastName, position, salary, ratePerMinute, department);
                    break;
                case "fireEmployee":
                    if (commandParameters.Length != 2)
                    {
                        throw new NotSupportedException(
                            "Fire employee command must be with only 2 parameters!");
                    }

                    if (this.employees.Count == 0)
                    {
                        throw new InvalidOperationException("No employees currently in the service!");
                    }

                    int employeeId = int.TryParse(commandParameters[1], out employeeId)
                        ? employeeId
                        : throw new ArgumentException("Please provide a valid integer value!");

                    if (employeeId <= 0)
                    {
                        throw new ArgumentException($"Please provide a valid employee value, i.e. between 1 and {this.employees.Count}!");
                    }
                    
                    IEmployee employee = this.employees.Count >= employeeId
                        ? this.employees[employeeId - 1]
                        : throw new ArgumentException("This employee does not exist!");
                    this.FireEmployee(employee);
                    break;
                case "changeEmployeesRate":
                    firstName = commandParameters[1];
                    lastName = commandParameters[2];
                    var rate = decimal.Parse(commandParameters[3]);
                    if (employees.Any(x => x.FirstName != firstName && x.LastName != lastName))
                    {
                        throw new ArgumentException("The are no employee with this name!");
                    }
                    foreach (var employe in employees)
                    {
                        if (employee.FirstName == firstName && employee.LastName == lastName)
                        {
                            employee.ChangeRate(rate);
                            Console.WriteLine($"Employee {employee.FirstName.ToString()}{employee.LastName.ToString()} rate were changed to {rate}!");
                        }
                    }
                    break;
                case "issueInvoices":
                    this.IssueInvoices();
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void ShowEmployees()
        {
            int counter = 1;
            foreach (var currentEmployee in this.employees)
            {   
                Console.WriteLine(counter + ". " +  currentEmployee.ToString());
                counter++;
            }
        }

        private void IssueInvoices()
        {
            int invoiceCount = 0;
            foreach (var client in this.notInvoicedSells.OrderBy(o => o.Key.Name))
            {
                this.lastInvoiceNumber++;
                invoiceCount++;
                string invoiceNumber = this.lastInvoiceNumber.ToString().PadLeft(10, '0');
                this.lastInvoiceDate.AddDays(1);
                IInvoice invoice = new Invoice(invoiceNumber, this.lastInvoiceDate, client.Key);

                foreach (var sell in client.Value)
                {
                    invoice.InvoiceItems.Add(sell);
                }
                var clientToAddInvoice = this.clients.FirstOrDefault(f => f.UniqueNumber == client.Key.UniqueNumber);
                clientToAddInvoice.Invoices.Add(invoice);
            }

            this.notInvoicedSells.Clear();
            Console.WriteLine($"{invoiceCount} invoices were successfully issued!");
        }

        private void FireEmployee(IEmployee employee)
        {
            employee.FireEmployee();

            Console.WriteLine($"Employee {employee.FirstName} {employee.LastName} was fired!");
        }

        private void AddEmployee(string firstName, string lastName, string position, decimal salary,
            decimal ratePerMinute, DepartmentType department)
        {
            IEmployee employee =
                this.factory.CreateEmployee(firstName, lastName, position, salary, ratePerMinute, department);

            this.employees.Add(employee);
            Console.WriteLine(employee);
            Console.WriteLine($"Employee {firstName} {lastName} added successfully with Id {this.employees.Count}");
        }
    }
}