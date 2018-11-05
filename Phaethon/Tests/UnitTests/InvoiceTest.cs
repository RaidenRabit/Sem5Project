﻿using System;
using Core.Model;
using InternalApi.Controller;
using NUnit.Framework;

namespace Tests.UnitTests
{
    public class InvoiceTest
    {
        [Test]
        public void CreateInvoiceSuccess()
        {
            Company receiverCompany = new Company();
            receiverCompany.ID = 64;
            receiverCompany.Location = "Nibevej 2344 location";
            receiverCompany.Name = "shufle";
            receiverCompany.RegNumber = "54555556";
            receiverCompany.Address = "Nibevej 2344 Address";
            receiverCompany.BankNumber = "DK34745624547";

            Representative receiver = new Representative();
            receiver.Name = "Kabola";
            receiver.Company = receiverCompany;

            Company senderCompany = new Company();
            senderCompany.Location = "Wallstreet location";
            senderCompany.Name = "Pop-top-spain";
            senderCompany.RegNumber = "1233333";
            senderCompany.Address = "Wallstreet Address";
            senderCompany.BankNumber = "DK3477777777";

            Representative sender = new Representative();
            sender.Name = "Kasper";
            sender.Company = senderCompany;
            
            Invoice invoice = new Invoice();
            invoice.DocNumber = "136381022";
            invoice.PaymentDate = DateTime.Now;
            invoice.PrescriptionDate = DateTime.Now;
            invoice.ReceptionDate = DateTime.Now;
            invoice.Receiver = receiver;
            invoice.Sender = sender;
            invoice.Transport = 5;

            InvoiceController invoiceController = new InvoiceController();
            invoice = invoiceController.CreateInvoice(invoice);
            Assert.IsNotNull(invoice);
        }
    }
}
