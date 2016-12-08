#region Using directives

using System;

#endregion

namespace ngen.Data
{
    [Flags]
    public enum SystemPermission
    {
        NotSet = 1 << 0,
        Administrator = 1 << 1,
        ViewFinancialDetails = 1 << 2,
        ManageEmployeeAccounts = 1 << 3,
        ManageCustomerAccounts = 1 << 4,
        ManageSupplierAccounts = 1 << 5,
        ManageParts = 1 << 6,
        ManageDocuments = 1 << 7,
        CheckOutDocuments = 1 << 8
    }
}