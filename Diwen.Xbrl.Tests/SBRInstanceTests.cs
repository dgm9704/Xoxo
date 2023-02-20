//
//  SBRInstanceTests.cs
//
//  Author:
//       John Nordberg <john.nordberg@gmail.com>
//
//  Copyright (c) 2015-2020 John Nordberg
//
//  Free Public License 1.0.0
//  Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted.
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES 
//  OF MERCHANTABILITY AND FITNESS.IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES 
//  OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS 
//  ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

namespace Diwen.Xbrl.Tests
{
	using System.IO;
	using Xunit;
	using Xbrl;

	public static class SBRInstanceTests
	{
		static Instance CreatePaymentSummaryInstance()
		{
			// Sets default namespaces
			var instance = new Instance();

			// When an explicit member is added, check that the namespace for the domain has been set
			instance.CheckExplicitMemberDomainExists = true;

			// Set fact attribute nil=true when value is empty string
			//instance.FactsNillable = true;

			// Initialize to the correct framework, module, taxonomy
			// The content is NOT validated against taxonomy or module schema
			// set module
			instance.SchemaReference = new SchemaReference("simple", "http://sbr.gov.au/taxonomy/sbr_au_reports/ato/ps/ps_0003/ps.0003.lodge.request.02.00.report.xsd");

			// set taxonomy
			instance.TaxonomyVersion = "ps_0003";

			// "basic" namespaces
			// These are used for adding correct prefixes for different elements
			instance.SetDimensionNamespace("RprtPyType.02.05", "http://sbr.gov.au/dims/RprtPyType.02.05.dims");

			// Namespaces for actual reported values that belong to a domain (explicit members)
			instance.AddDomainNamespace("ps.0003.lodge.req.02.00", "http://sbr.gov.au/rprt/ato/ps/ps.0003.lodge.request.02.00.report");
			//instance.AddDomainNamespace("RprtPyType.02.05", "http://sbr.gov.au/dims/RprtPyType.02.05.dims");

			instance.AddDomainNamespace("dtyp.02.18", "http://sbr.gov.au/fdtn/sbr.02.18.dtyp");
			instance.AddDomainNamespace("dtyp.02.08", "http://sbr.gov.au/fdtn/sbr.02.08.dtyp");
			instance.AddDomainNamespace("tech.01.02", "http://sbr.gov.au/fdtn/sbr.01.02.tech");
			instance.AddDomainNamespace("pyde.02.01", "http://sbr.gov.au/icls/py/pyde/pyde.02.01.data");
			instance.AddDomainNamespace("pyde.02.05", "http://sbr.gov.au/icls/py/pyde/pyde.02.05.data");
			instance.AddDomainNamespace("pyin.02.00", "http://sbr.gov.au/icls/py/pyin/pyin.02.00.data");
			instance.AddDomainNamespace("lrla.02.01", "http://sbr.gov.au/icls/lr/lrla/lrla.02.01.data");
			instance.AddDomainNamespace("bafpr1.02.00", "http://sbr.gov.au/icls/baf/bafpr/bafpr1.02.00.data");
			instance.AddDomainNamespace("ps.0003.prv.02.00", "http://sbr.gov.au/rprt/ato/ps/ps.0003.private.02.00.module");
			instance.AddDomainNamespace("period1.02.01", "http://sbr.gov.au/comnmdle/comnmdle.perioddetails1.02.01.module");
			instance.AddDomainNamespace("rvctc2.02.11", "http://sbr.gov.au/icls/rvc/rvctc/rvctc2.02.11.data");
			instance.AddDomainNamespace("rvctc2.02.03", "http://sbr.gov.au/icls/rvc/rvctc/rvctc2.02.03.data");
			instance.AddDomainNamespace("crigi.02.00", "http://sbr.gov.au/icls/cri/crigi/crigi.02.00.data");
			instance.AddDomainNamespace("gfati.02.00", "http://sbr.gov.au/icls/gfa/gfati/gfati.02.00.data");
			instance.AddDomainNamespace("dtyp.02.13", "http://sbr.gov.au/fdtn/sbr.02.13.dtyp");
			instance.AddDomainNamespace("gfati.02.00", "http://sbr.gov.au/icls/gfa/gfati/gfati.02.00.data");
			instance.AddDomainNamespace("dtyp.02.03", "http://sbr.gov.au/fdtn/sbr.02.03.dtyp");
			instance.AddDomainNamespace("email1.02.00", "http://sbr.gov.au/comnmdle/comnmdle.electroniccontactelectronicmail1.02.00.module");
			instance.AddDomainNamespace("fax1.02.00", "http://sbr.gov.au/comnmdle/comnmdle.electroniccontactfacsimile1.02.00.module");
			instance.AddDomainNamespace("pyde.02.00", "http://sbr.gov.au/icls/py/pyde/pyde.02.00.data");
			instance.AddDomainNamespace("tech.01.03", "http://sbr.gov.au/fdtn/sbr.01.03.tech");
			instance.AddDomainNamespace("pyde.02.08", "http://sbr.gov.au/icls/py/pyde/pyde.02.08.data");
			instance.AddDomainNamespace("prsnunstrcnm1.02.01", "http://sbr.gov.au/comnmdle/comnmdle.personunstructuredname1.02.01.module");
			instance.AddDomainNamespace("lrla.02.00", "http://sbr.gov.au/icls/lr/lrla/lrla.02.00.data");
			instance.AddDomainNamespace("rvctc2.02.10", "http://sbr.gov.au/icls/rvc/rvctc/rvctc2.02.10.data");
			instance.AddDomainNamespace("rvctc2.02.00", "http://sbr.gov.au/icls/rvc/rvctc/rvctc2.02.00.data");
			instance.AddDomainNamespace("pyin.02.05", "http://sbr.gov.au/icls/py/pyin/pyin.02.05.data");
			instance.AddDomainNamespace("prsnstrcnm2.02.00", "http://sbr.gov.au/comnmdle/comnmdle.personstructuredname2.02.00.module");
			instance.AddDomainNamespace("rvctc2.02.02", "http://sbr.gov.au/icls/rvc/rvctc/rvctc2.02.02.data");
			instance.AddDomainNamespace("orgname1.02.00", "http://sbr.gov.au/comnmdle/comnmdle.organisationname1.02.00.module");
			instance.AddDomainNamespace("pyin.02.07", "http://sbr.gov.au/icls/py/pyin/pyin.02.07.data");
			instance.AddDomainNamespace("phone1.02.00", "http://sbr.gov.au/comnmdle/comnmdle.electroniccontacttelephone1.02.00.module");
			instance.AddDomainNamespace("address1.02.02", "http://sbr.gov.au/comnmdle/comnmdle.addressdetails1.02.02.module");
			instance.AddDomainNamespace("pyid.02.00", "http://sbr.gov.au/icls/py/pyid/pyid.02.00.data");
			instance.AddDomainNamespace("dtyp.02.04", "http://sbr.gov.au/fdtn/sbr.02.04.dtyp");
			instance.AddDomainNamespace("dtyp.02.16", "http://sbr.gov.au/fdtn/sbr.02.16.dtyp");
			instance.AddDomainNamespace("dtyp.02.06", "http://sbr.gov.au/fdtn/sbr.02.06.dtyp");
			instance.AddDomainNamespace("emsup.02.02", "http://sbr.gov.au/icls/em/emsup/emsup.02.02.data");

			// Add reporter and period
			// These will be reused across all contexts by default
			// Scheme or value are NOT validated
			instance.Entity = new Entity("http://www.ato.gov.au/abn", "14088411787");
			instance.Period = new Period(2012, 7, 1, 2013, 6, 30);

			// Any units that aren't used will be excluded during serialization
			// So it's safe to add extra units if needed
			instance.Units.Add("U1", "iso4217:AUD");

			// A scenario contains the dimensions and their values for a datapoint
			var rp_segment = new Segment();
			rp_segment.Instance = instance;

			// Dimensions and domains can be given with or without namespaces
			// The namespace prefixes are added internally if needed
			// Explicit member values DO NEED the prefix
			rp_segment.AddExplicitMember("ReportPartyTypeDimension", "RprtPyType.02.05:ReportingParty");

			// set context id
			var context = instance.GetContext(rp_segment);
			context.Id = "RP";

			var node = instance.AddFact(rp_segment, "orgname1.02.00:OrganisationNameDetails", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:OrganisationNameDetails.OrganisationalNameType.Code", "", "", "MN");
			node.AddFact(rp_segment, "pyde.02.00:OrganisationNameDetails.OrganisationalName.Text", "", "", "RP OrganisationalNameText");

			node = instance.AddFact(rp_segment, "orgname1.02.00:OrganisationNameDetails", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:OrganisationNameDetails.OrganisationalNameType.Code", "", "", "MTR");
			node.AddFact(rp_segment, "pyde.02.00:OrganisationNameDetails.OrganisationalName.Text", "", "", "RP OrganisationalTradeNameText");

			node = instance.AddFact(rp_segment, "prsnunstrcnm1.02.01:PersonUnstructuredName", "", "", "");
			node.AddFact(rp_segment, "pyde.02.05:PersonUnstructuredName.Usage.Code", "", "", "Contact");
			node.AddFact(rp_segment, "pyde.02.00:PersonUnstructuredName.FullName.Text", "", "", "RPPersonUnstructuredNameFullNameText");

			node = instance.AddFact(rp_segment, "phone1.02.00:ElectronicContactTelephone", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:ElectronicContact.Telephone.Usage.Code", "", "", "03");
			node.AddFact(rp_segment, "pyde.02.00:ElectronicContact.Telephone.ServiceLine.Code", "", "", "02");
			node.AddFact(rp_segment, "pyde.02.00:ElectronicContact.Telephone.Area.Code", "", "", "02");
			node.AddFact(rp_segment, "pyde.02.00:ElectronicContact.Telephone.Minimal.Number", "", "", "85263425");

			node = instance.AddFact(rp_segment, "address1.02.02:AddressDetails", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.OverseasAddress.Indicator", "", "", "true");
			node.AddFact(rp_segment, "pyde.02.01:AddressDetails.Usage.Code", "", "", "POS");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.AttentionTo.Text", "", "", "Mr Tugnait");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line1.Text", "", "", "RP_POS_Addr 1");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line2.Text", "", "", "RP_POS_Addr 2");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line3.Text", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line4.Text", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.LocalityName.Text", "", "", "Illinois");
			// Postcode and State are mandatory, but message accepted if missing or element has attribute xsi:nil="true"
			//node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Postcode.Text", "", "", "");
			//node.AddFact(rp_segment, "pyde.02.00:AddressDetails.StateOrTerritory.Code", "", "", "");
			node.AddFact(rp_segment, "pyde.02.08:AddressDetails.CountryName.Text", "", "", "UNITED STATES");
			node.AddFact(rp_segment, "pyde.02.08:AddressDetails.Country.Code", "", "", "us");

			node = instance.AddFact(rp_segment, "address1.02.02:AddressDetails", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.OverseasAddress.Indicator", "", "", "false");
			node.AddFact(rp_segment, "pyde.02.01:AddressDetails.Usage.Code", "", "", "BUS");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.AttentionTo.Text", "", "", "Mr Tugnait");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line1.Text", "", "", "RP_BUS_Addr 1");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line2.Text", "", "", "RP_BUS_Addr 2");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line3.Text", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line4.Text", "", "", "");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.LocalityName.Text", "", "", "Civic");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.Postcode.Text", "", "", "2600");
			node.AddFact(rp_segment, "pyde.02.00:AddressDetails.StateOrTerritory.Code", "", "", "ACT");
			node.AddFact(rp_segment, "pyde.02.08:AddressDetails.CountryName.Text", "", "", "AUSTRALIA");
			//node.AddFact(rp_segment, "pyde.02.08:AddressDetails.Country.Code", "", "", "");

			instance.AddFact(rp_segment, "pyde.02.00:OrganisationDetails.OrganisationBranch.Code", "", "", "101");

			node = instance.AddFact(rp_segment, "ps.0003.lodge.req.02.00:Payee", "", "", "");
			node.AddFact(rp_segment, "pyid.02.00:Identifiers.TaxFileNumber.Identifier", "", "", "32989432");
			//node.AddFact(rp_segment, "pyid.02.00:IdentificationExemptionDetails.TFNExemptionType.Code", "", "", "");
			node.AddFact(rp_segment, "pyid.02.00:Identifiers.AustralianBusinessNumber.Identifier", "", "", "76089884284");
			node.AddFact(rp_segment, "pyde.02.00:PersonDemographicDetails.Birth.Date", "", "", "1958-09-28");

			var subnode = node.AddFact(rp_segment, "prsnstrcnm2.02.00:PersonNameDetails", "", "", "");
			subnode.AddFact(rp_segment, "pyde.02.00:PersonNameDetails.PersonNameType.Code", "", "", "LGL");
			subnode.AddFact(rp_segment, "pyde.02.00:PersonNameDetails.Currency.Code", "", "", "C");
			subnode.AddFact(rp_segment, "pyde.02.00:PersonNameDetails.FamilyName.Text", "", "", "Payee1 FamilyName");
			subnode.AddFact(rp_segment, "pyde.02.00:PersonNameDetails.GivenName.Text", "", "", "Payee1 GivenName");
			subnode.AddFact(rp_segment, "pyde.02.00:PersonNameDetails.OtherGivenName.Text", "", "", "Payee1 OtherGivenName");

			subnode = node.AddFact(rp_segment, "address1.02.02:AddressDetails", "", "", "");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.OverseasAddress.Indicator", "", "", "false");
			subnode.AddFact(rp_segment, "pyde.02.01:AddressDetails.Usage.Code", "", "", "RES");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.AttentionTo.Text", "", "", "Mr Young");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line1.Text", "", "", "Payee1 Addr1");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line2.Text", "", "", "Payee1 Addr2");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line3.Text", "", "", "");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.Line4.Text", "", "", "");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.LocalityName.Text", "", "", "Payee1 Locality");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.Postcode.Text", "", "", "2601");
			subnode.AddFact(rp_segment, "pyde.02.00:AddressDetails.StateOrTerritory.Code", "", "", "ACT");
			subnode.AddFact(rp_segment, "pyde.02.08:AddressDetails.CountryName.Text", "", "", "AUSTRALIA");
			//subnode.AddFact(rp_segment, "pyde.02.08:AddressDetails.Country.Code", "", "", "");

			subnode = node.AddFact(rp_segment, "ps.0003.lodge.req.02.00:IndividualNonBusinessWithholdingPaymentDetails", "", "", "");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.WithholdingPaymentIncomeType.Code", "", "", "006");
			var subnode2 = subnode.AddFact(rp_segment, "period1.02.01:PeriodDetails", "", "", "");
			subnode2.AddFact(rp_segment, "pyin.02.05:Period.Type.Code", "", "", "Payment");
			subnode2.AddFact(rp_segment, "pyin.02.00:Period.Start.Date", "", "", "2012-09-01");
			subnode2.AddFact(rp_segment, "pyin.02.00:Period.End.Date", "", "", "2013-06-30");
			subnode.AddFact(rp_segment, "rvctc2.02.00:IncomeTax.PayAsYouGoWithholding.TaxWithheld.Amount", "U1", "0", "22975");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.IndividualNonBusinessGross.Amount", "U1", "0", "85064");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.IndividualNonBusinessEmploymentAllowances.Amount", "U1", "0", "12000");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.UnusedAnnualOrLongServiceLeavePaymentLumpSumA.Amount", "U1", "0", "5700");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.UnusedAnnualOrLongServiceLeavePaymentLumpSumA.Code", "", "", "R");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.UnusedAnnualOrLongServiceLeavePaymentLumpSumB.Amount", "U1", "0", "4700");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.UnusedAnnualOrLongServiceLeavePaymentLumpSumD.Amount", "U1", "0", "2750");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.UnusedAnnualOrLongServiceLeavePaymentLumpSumE.Amount", "U1", "0", "3500");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.IndividualNonBusinessCommunityDevelopmentEmploymentProject.Amount", "U1", "0", "800");
			subnode.AddFact(rp_segment, "lrla.02.00:Remuneration.FringeBenefits.Reportable.Amount", "U1", "0", "3500");
			subnode.AddFact(rp_segment, "emsup.02.02:SuperannuationContribution.EmployerContributions.Amount", "U1", "0", "7650");
			subnode.AddFact(rp_segment, "bafpr1.02.00:Expense.WorkplaceGiving.Amount", "U1", "0", "125");
			subnode.AddFact(rp_segment, "bafpr1.02.00:Expense.UnionOrProfessionalAssociationFee.Amount", "U1", "0", "150");
			subnode.AddFact(rp_segment, "lrla.02.01:Remuneration.IndividualNonBusinessExemptForeignEmploymentIncome.Amount", "U1", "0", "570");
			subnode.AddFact(rp_segment, "rvctc2.02.03:IncomeTax.Deduction.PensionOrAnnuityPurchasePriceUndeducted.Amount", "U1", "0", "450");
			subnode.AddFact(rp_segment, "pyin.02.07:Report.Amendment.Indicator", "", "", "false");

			return instance;
		}

		[Fact]
		public static void WritePaymentSummaryInstance()
		{
			var instance = CreatePaymentSummaryInstance();

			// Write the instace to a file
			var path = "output.sbr.xml";
			instance.ToFile(path);
		}

		[Fact]
		public static void ComparePaymentSummaryInstance()
		{
			var instance = CreatePaymentSummaryInstance();

			// unless done when loading, duplicate objects 
			// aren't automatically removed until serialization so do it before comparisons
			instance.RemoveUnusedObjects();

			var referencePath = Path.Combine("data", "sbr_reference.xbrl");
			var referenceInstance = Instance.FromFile(referencePath);

			// Instances are functionally equivalent:
			// They have the same number of contexts and scenarios of the contexts match member-by-member
			// Members are checked by dimension, domain and value, namespaces included
			// They have the same facts matched by metric, value, decimals and unit
			// Entity and Period are also matched
			// Some things are NOT checked, eg. taxonomy version, context names
			//Assert.Equal(instance, referenceInstance);

			string tempFile = "sbr_temp.xbrl";
			instance.ToFile(tempFile);

			var newInstance = Instance.FromFile(tempFile);

			Assert.True(newInstance.Equals(instance));

			Assert.True(newInstance.Equals(referenceInstance));

			newInstance.Contexts[0].Entity.AddExplicitMember("AM", "s2c_AM:x1");

			Assert.False(newInstance.Equals(referenceInstance));
		}

	}
}

