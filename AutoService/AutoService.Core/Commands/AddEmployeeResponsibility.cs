﻿using AutoService.Core.Contracts;
using AutoService.Core.Validator;
using AutoService.Models.Common.Contracts;
using AutoService.Models.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoService.Core.Commands
{
    public class AddEmployeeResponsibility : ICommand
    {
        private readonly IDatabase database;
        private readonly IValidateCore coreValidator;
        private readonly IEmployeeManager employeeManager;

        public AddEmployeeResponsibility(IDatabase database, IValidateCore coreValidator, IEmployeeManager employeeManager)
        {
            this.database = database;
            this.coreValidator = coreValidator;
            this.employeeManager = employeeManager;
        }


        public void ExecuteThisCommand(string[] commandParameters)
        {
            this.coreValidator.MinimumParameterLength(commandParameters, 3);

            this.coreValidator.EmployeeCount(database.Employees.Count);

            var employeeId = this.coreValidator.IntFromString(commandParameters[1], "employeeId");

            var employee = this.coreValidator.EmployeeById(database.Employees, employeeId);

            var responsibilitiesToAdd = commandParameters.Skip(2).ToArray();
            this.AddResponsibilitiesToEmployee(employee, responsibilitiesToAdd, employeeManager);

        }

        private void AddResponsibilitiesToEmployee(IEmployee employee, string[] responsibilities, IEmployeeManager employeeManager)
        {
            var responsibilitesToAdd = new List<ResponsibilityType>();
            foreach (var responsibility in responsibilities)
            {
                bool isValid = this.coreValidator.IsValidResponsibilityTypeFromString(responsibility);

                if (isValid)
                {
                    ResponsibilityType currentResponsibility;
                    Enum.TryParse(responsibility, out currentResponsibility);
                    responsibilitesToAdd.Add(currentResponsibility);
                }
            }
            employeeManager.SetEmployee(employee);
            employeeManager.AddResponsibilities(responsibilitesToAdd);
        }
    }
}