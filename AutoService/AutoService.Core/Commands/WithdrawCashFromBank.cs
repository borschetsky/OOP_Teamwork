﻿using System;
using AutoService.Core.Contracts;
using AutoService.Core.Validator;
using AutoService.Models.Assets;
using AutoService.Models.Assets.Contracts;
using AutoService.Models.Common.Contracts;
using AutoService.Models.Common.Enums;

namespace AutoService.Core.Commands
{
    public class WithdrawCashFromBank : ICommand
    {
        private readonly IDatabase database;
        private readonly IValidateCore coreValidator;

        public WithdrawCashFromBank(IDatabase database, IValidateCore coreValidator)
        {
            this.database = database;
            this.coreValidator = coreValidator;
        }

        public void ExecuteThisCommand(string[] commandParameters)
        {
            this.coreValidator.ExactParameterLength(commandParameters, 4);

            this.coreValidator.BankAccountsCount(this.database.BankAccounts.Count);
            int employeeId = this.coreValidator.IntFromString(commandParameters[1], "employeeId");

            var employee = this.coreValidator.EmployeeById(this.database.Employees, employeeId);

            int bankAccountId = this.coreValidator.IntFromString(commandParameters[2], "bankAccountId");

            var bankAccount = this.coreValidator.BankAccountById(this.database.BankAccounts, bankAccountId);

            decimal withdrawAmount = this.coreValidator.DecimalFromString(commandParameters[3], "depositAmount");

            this.WithdrawCashFromBankMethod(bankAccount, withdrawAmount, employee);
        }

        private void WithdrawCashFromBankMethod(BankAccount bankAccount, decimal withdrawAmount, IEmployee employee)
        {
            if (employee.Responsibilities.Contains(ResponsibilityType.Account) || employee.Responsibilities.Contains(ResponsibilityType.Manage))
            {
                bankAccount.WithdrawFunds(withdrawAmount);
                Console.WriteLine($"{withdrawAmount} BGN were successfully withdrawn by {employee.FirstName} {employee.LastName}");
            }
            else
            {
                throw new ArgumentException($"Employee {employee.FirstName} {employee.LastName} is not allowed to withdraw!");
            }
        }
    }
}