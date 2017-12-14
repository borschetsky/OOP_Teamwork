﻿using System;
using System.Linq;
using AutoService.Models.Contracts;
using AutoService.Models.Enums;

namespace AutoService.Models.Models
{
    public class Employee : IEmployee
    {
        private string firstName;
        private string lastName;
        private decimal salary;
        private Position position;
        private EmploymentType employmentType;
        private bool isStillHired;

        public Employee(string firstName, string lastName, decimal salary, Position position, EmploymentType employmentType)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Salary = salary;
            this.Position = position;
            this.EmploymentType = employmentType;
            this.IsStillHired = true;
        }

        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsDigit))
                {
                    throw new ArgumentException("Invalid first name!");
                }
                this.firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }
            private set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsDigit))
                {
                    throw new ArgumentException("Invalid last name!");
                }
                this.lastName = value;
            }
        }
        public decimal Salary
        {
            get
            {
                return this.salary;
            }
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Salary cannot be negative!");
                }
                this.salary = value;
            }
        }
        public Position Position
        {
            get
            {
                return this.position;
            }
            set { this.position = value; }
        }

        public EmploymentType EmploymentType
        {
            get { return this.employmentType; }
            set { this.employmentType = value; }
        }

        public bool IsStillHired
        {
            get { return this.isStillHired; }
            private set { this.isStillHired = value; }
        }

        public void ChangeSalary(decimal salary)
        {
            this.Salary = salary;
        }

        public void ChangePosition(Position position)
        {
            this.Position = position;
        }

        public void FireEmployee()
        {
            this.IsStillHired = false;
        }
    }
}