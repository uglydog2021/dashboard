using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_ReportForms.Core
{
    public class Constants
    {
        public const string GROUP_CUSTOMER = "Customer";
        public const string GROUP_SALES = "Sales";
        public const string GROUP_OPERATION = "Operation";

        /// <summary>
        /// 是否允许无库存下单 (默认true)
        /// </summary>
        public const bool Default_CanNonInventory = true;

        public const string DefaultCurrencySymbol = "HKD";

        public const string DefaultCreateUserSystem = "SYSTEM";
    }
}
