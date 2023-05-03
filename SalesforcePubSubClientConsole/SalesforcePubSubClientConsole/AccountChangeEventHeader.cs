using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public class ChangeEventHeader
    {
        public string EntityName { get; set; }
        public List<string> RecordIds { get; set; }
        public string ChangeType { get; set; }
        public string ChangeOrigin { get; set; }
        public string TransactionKey { get; set; }
        public int SequenceNumber { get; set; }
        public long CommitTimestamp { get; set; }
        public long CommitNumber { get; set; }
        public string CommitUser { get; set; }
        public List<string> NulledFields { get; set; }
        public List<string> DiffFields { get; set; }
        public List<string> ChangedFields { get; set; }
    }

    public class Name
    {
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string InformalName { get; set; }
        public string Suffix { get; set; }
    }

    public class AccountChangeEventHeader
    {
        public ChangeEventHeader ChangeEventHeader { get; set; }
        public Name Name { get; set; }
        public string Type { get; set; }
        public string RecordTypeId { get; set; }
        public string ParentId { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string AccountNumber { get; set; }
        public string Website { get; set; }
        public string Sic { get; set; }
        public string Industry { get; set; }
        public string AnnualRevenue { get; set; }
        public string NumberOfEmployees { get; set; }
        public string Ownership { get; set; }
        public string TickerSymbol { get; set; }
        public string Description { get; set; }
        public string Rating { get; set; }
        public string Site { get; set; }
        public string CurrencyIsoCode { get; set; }
        public string OwnerId { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public long LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public string PersonContactId { get; set; }
        public string PersonMailingAddress { get; set; }
        public string PersonOtherAddress { get; set; }
        public string PersonMobilePhone { get; set; }
        public string PersonHomePhone { get; set; }
        public string PersonOtherPhone { get; set; }
        public string PersonAssistantPhone { get; set; }
        public string PersonEmail { get; set; }
        public string PersonTitle { get; set; }
        public string PersonDepartment { get; set; }
        public string PersonAssistantName { get; set; }
        public string PersonLeadSource { get; set; }
        public string PersonBirthdate { get; set; }
        public string PersonHasOptedOutOfEmail { get; set; }
        public string PersonHasOptedOutOfFax { get; set; }
        public string PersonDoNotCall { get; set; }
        public string PersonLastCURequestDate { get; set; }
        public string PersonLastCUUpdateDate { get; set; }
        public string PersonEmailBouncedReason { get; set; }
        public string PersonEmailBouncedDate { get; set; }
        public string PersonIndividualId { get; set; }
        public string Jigsaw { get; set; }
        public string JigsawCompanyId { get; set; }
        public string AccountSource { get; set; }
        public string SicDesc { get; set; }
        public string ShopifyCustomerIDC { get; set; }
        public string ShopifyRequestBodyC { get; set; }
        public string CompanyC { get; set; }
        public string AssignedLocationAccountPc { get; set; }
    }


}
