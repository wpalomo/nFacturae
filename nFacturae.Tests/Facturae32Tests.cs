﻿#region License

//===================================================================================
//Copyright 2011 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using Gallio.Common.Markup;
using Gallio.Framework;
using MbUnit.Framework;
using nFacturae.Facturae32;
using nFacturae.Tests.Properties;

namespace nFactura.Tests
{
    public class Facturae32Tests
    {
        [Test]
        [Ignore("We must create a self signed pfx dfor test purposes")]
        public void Factuare_Sign()
        {
            var signer = new nFacturae.Signer();
            signer.Sign(@"..\..\Samples\sample_32.xml", "signed32.xml", "certificate.pfx", "password");
            Assert.IsTrue(true);
        }

        [Test]
        public void Factuare_From_Xml()
        {
            var inv = Facturae.FromXml(Resource.sample_32);
            inv.Validate();
            Assert.IsTrue(true);
        }

        [Test]
        public Facturae Factuare_Create_Simple_Invoice_And_validate()
        {
            var fe32 = new Facturae()
                .AddLegalParty(true, ResidenceTypeCodeType.E, "79065414H",
                    "HexaSystems Corporation", "HexaSystems Address", "28019", "Madrid", "Madrid", CountryType.ESP)
                .AddIndividualParty(false, ResidenceTypeCodeType.E, "3422357Y", "Carlos",
                    "Mendible", "Carlos Address", "28019", "Madrid", "Madrid", CountryType.ESP)
                .AddInvoice((i) =>
                {
                    i.HeatherAndIssueData("231418", InvoiceDocumentTypeType.FC,
                        InvoiceClassType.OO, DateTime.Now, CurrencyCodeType.EUR, LanguageCodeType.es)
                    .AddLine(l => l.Item("Item Description", 1, 1000).AddTax(TaxTypeCodeType.Item01, 18).AddDiscount(100, "YES", 10))
                    .AddLine(l => l.Item("Item Description 2", 2, 100).AddTax(TaxTypeCodeType.Item01, 18));
                });

            Assert.IsNotNull(fe32);

            fe32.Validate();

            Assert.IsTrue(true);

            TestLog.AttachXml("Facturae_3.2", fe32.ToString());

            return fe32;
        }

        [Test]
        public void Factuare_To_Html()
        {
            var fe32 = Factuare_Create_Simple_Invoice_And_validate();

            var logoPath = "https://a248.e.akamai.net/assets.github.com/images/modules/header/logov6-hover.png";

            var html = fe32.ToHtml(logoPath);

            Assert.IsNotNull(html);
            Assert.IsNotEmpty(html);

            TestLog.AttachHtml("invoice_html", html);
        }

        [Test]
        public void Factuare_To_Pdf()
        {
            var fe32 = Factuare_Create_Simple_Invoice_And_validate();

            var logoPath = "https://a248.e.akamai.net/assets.github.com/images/modules/header/logov6-hover.png";

            var unedoc = fe32.ToPdf(logoPath);

            Assert.IsNotNull(unedoc);

            var attachmentData = new BinaryAttachment("invoice.pdf", "application/pdf",unedoc.ToArray());

            TestLog.Attach(attachmentData);
        }
    }
}
