﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Core.Model;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.IntegrationTests
{
    public class CompanyTest: IntegrationTestBase
    {
        private bool AreCompaniesEqual(Company firstCompany, Company secondCompany)
        {
            return firstCompany.ID == secondCompany.ID &&
                   firstCompany.BankName.Equals(secondCompany.BankName) &&
                   firstCompany.BankNumber.Equals(secondCompany.BankNumber) &&
                   firstCompany.Name.Equals(secondCompany.Name) &&
                   firstCompany.RegNumber.Equals(secondCompany.RegNumber) &&
                   firstCompany.ActualAddress_ID == secondCompany.ActualAddress_ID &&
                   firstCompany.LegalAddress_ID == secondCompany.LegalAddress_ID;
        }

        #region GetCompany
        [Test]
        public async Task GetCompany_CorrectID_IsSuccessStatusCodeAndSameObjectReturned()
        {
            //Setup
            Element element = InvoiceTest.GetElementSeed();
            Invoice invoice = element.Invoice;
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["id"] = invoice.Sender.Company.ID.ToString();

            //Act
            var response = await _internalClient.GetAsync("Company/GetCompany?" + parameters);
            Company company = JsonConvert.DeserializeObject<Company>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Server responded with Success code");
            Assert.IsTrue(AreCompaniesEqual(company, invoice.Sender.Company), "Companies are equal");//check if object received is the same
        }

        [Test]
        public async Task GetCompany_WrongId_IsSuccessStatusCodeAndNullObjectReturned()
        {
            //Setup
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["id"] = 0.ToString();

            //Act
            var response = await _internalClient.GetAsync("Company/GetCompany?" + parameters);
            Company company = JsonConvert.DeserializeObject<Company>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Server responded with Success code");
            Assert.IsNull(company, "No company received");
        }
        #endregion

        #region GetCompanies
        [Test]
        public async Task GetCompanies_MethodCalled_IsSuccessStatusCodeAndCompaniesReturned()
        {
            //Setup

            //Act
            var response = await _internalClient.GetAsync("Company/GetCompanies");
            List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Server responded with Success code");
            Assert.IsNotNull(companies, "There were companies in database");
        }
        #endregion
    }
}
