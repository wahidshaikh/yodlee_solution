using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yodleemodel
{
    public class RefreshedUserItemsModel
    {
        public string loginName { get; set; }
        public List<RefreshedUserItemsMemItemRefreshInfoModel> memItemRefreshInfo { get; set; }
    }

    public class RefreshedUserItemsMemItemRefreshInfoModel
    {
        public Int64 memItemId { get; set; }
        public Int64 errorCode { get; set; }
        public string refreshTime { get; set; }
    }

    public class UserTransactionCategoriesModel
    {
        public Int64 categoryId { get; set; }
        public string categoryName { get; set; }
        public Int64 transactionCategoryTypeId { get; set; }

        public Int64 isBudgetable { get; set; }
        public string localizedCategoryName { get; set; }
        public bool isHidden { get; set; }

        public bool revertCustomName { get; set; }
        public Int64 categoryLevelId { get; set; }
        public bool isBusiness { get; set; }
    }

    public class TransactionCategoryTypesModel
    {
        public Int64 typeId { get; set; }
        public string typeName { get; set; }
        public string localizedTypeName { get; set; }
    }

    public class SearchSiteModel
    {
        public Int64 popularity { get; set; }
        public Int64 siteId { get; set; }
        public Int64 orgId { get; set; }
        public string defaultDisplayName { get; set; }
        public string defaultOrgDisplayName { get; set; }
        public string baseUrl { get; set; }
        public List<SearchSiteContentServiceInfosModel> contentServiceInfos { get; set; }
    }

    public class SearchSiteContentServiceInfosModel
    {
        public Int64 contentServiceId { get; set; }
        public Int64 siteId { get; set; }
        public SearchSiteContentServiceInfosContainerInfoModel containerInfo { get; set; }
    }

    public class SearchSiteContentServiceInfosContainerInfoModel
    {
        public string containerName { get; set; }
        public Int64 assetType { get; set; }
    }

    public class ExecuteUserSearchRequestModel
    {
        public ExecuteUserSearchRequestSearchIdentifierModel searchIdentifier { get; set; }
        public Int64 numberOfHits { get; set; }
        public ExecuteUserSearchRequestSearchResultModel searchResult { get; set; }

        public Int64 countOfAllTransaction { get; set; }
        public Int64 countOfProjectedTxns { get; set; }

        public ExecuteUserSearchRequestTotalOfTxnsModel debitTotalOfTxns { get; set; }
        public ExecuteUserSearchRequestTotalOfTxnsModel creditTotalOfTxns { get; set; }
        public ExecuteUserSearchRequestTotalOfTxnsModel debitTotalOfProjectedTxns { get; set; }
        public ExecuteUserSearchRequestTotalOfTxnsModel creditTotalOfProjectedTxns { get; set; }
    }

    public class ExecuteUserSearchRequestSearchIdentifierModel
    {
        public string identifier { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultModel
    {
        public List<ExecuteUserSearchRequestSearchResultTransactionsModel> transactions { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsModel
    {
        public string identifier { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsViewKeyModel viewKey { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsDescriptionModel description { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsCheckNumberModel checkNumber { get; set; }
        //public ExecuteUserSearchRequestSearchResultTransactionsMemoModel memo { get; set; }
        public string postDate { get; set; }
        public Int64 transactionPostingOrder { get; set; }
        public string transactionDate { get; set; }

        public ExecuteUserSearchRequestSearchResultTransactionsCategoryModel category { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsAmountModel amount { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsStatusModel status { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsAccountModel account { get; set; }

        public bool isTaxable { get; set; }
        public bool isMedical { get; set; }
        public bool isBusiness { get; set; }
        public bool isReimbursable { get; set; }
        public bool isPersonal { get; set; }
        public string transactionType { get; set; }
        public string localizedTransactionType { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsPriceModel price { get; set; }
        public Int64 runningBalance { get; set; }
        public string transactionSearchResultType { get; set; }
        public string classUpdationSource { get; set; }

        public string transactionBaseType { get; set; }
        public Int64 transactionBaseTypeId { get; set; }
        public string localizedTransactionBaseType { get; set; }
        public Int64 accessLevelRequired { get; set; }
        public string link { get; set; }
        public Int64 categorisationSourceId { get; set; }
        public Int64 isClosingTxn { get; set; }
    }

    public class ExecuteUserSearchRequestTotalOfTxnsModel
    {
        public Int64 amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsViewKeyModel
    {
        public Int64 transactionId { get; set; }
        public string containerType { get; set; }
        public Int64 transactionCount { get; set; }
        public Int64 rowNumber { get; set; }
        public bool isParentMatch { get; set; }
        public bool isSystemGeneratedSplit { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsDescriptionModel
    {
        public string description { get; set; }
        public bool viewPref { get; set; }
        public bool isOlbUserDescription { get; set; }
        public string simpleDescription { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsCheckNumberModel
    {
        public string checkNumber { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsMemoModel
    {
        public string checkNumber { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsCategoryModel
    {
        public Int64 categoryId { get; set; }
        public string categoryName { get; set; }
        public Int64 categoryTypeId { get; set; }
        public string localizedCategoryName { get; set; }
        public bool isBusiness { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsAmountModel
    {
        public Int64 amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsStatusModel
    {
        public string description { get; set; }
        public Int64 statusId { get; set; }
        public string localizedDescription { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsAccountModel
    {
        public Int64 itemAccountId { get; set; }
        public string accountName { get; set; }
        public bool decryptionStatus { get; set; }
        public Int64 sumInfoId { get; set; }
        public Int64 isAccountName { get; set; }
        public string siteName { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsAccountAccountDisplayNameModel accountDisplayName { get; set; }
        public Int64 itemAccountStatusId { get; set; }
        public string accountNumber { get; set; }
        public ExecuteUserSearchRequestSearchResultTransactionsAccountAccountBalanceModel accountBalance { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsAccountAccountDisplayNameModel
    {
        public string defaultNormalAccountName { get; set; }

    }

    public class ExecuteUserSearchRequestSearchResultTransactionsAccountAccountBalanceModel
    {
        public double amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class ExecuteUserSearchRequestSearchResultTransactionsPriceModel
    {
        public Int64 amount { get; set; }
        public string currencyCode { get; set; }
    }
}
