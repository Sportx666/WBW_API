
using wbm_common.NonDatabaseObjects;
using WBM_API.WBM_API_DB;
using wbm_common.DataObjects;
using wbm_common.NonDatabaseObjects;

namespace WBM_API.Helpers.Costing
{
    public class TransactionCosting
    {
        public WBMDatabase _WBMDB { get; set; }
        public Transaction_CostingHolder TransactionCostingHolder { get; set; }

        public List<RateCode.dbRow> ListRateCode = new List<RateCode.dbRow>();

        public List<TableRate.dbRow> ListTableRate = new List<TableRate.dbRow>();

        List<int> ListRateCodeIDFetched = new List<int>();
        List<int> ListRateCodeIDToFetch = new List<int>();

        public async Task<Result<Boolean>> GetData()
        {
            Boolean rtnVal = true;

            // Get a list of RateCodeIDS to read reecords for.
            // This routine builds cumulatively so we may have alreasdy fetched some RateCodes and TableRates in a previous call.

            foreach (Transaction_Costing tc in TransactionCostingHolder.ListTransactionCosting)
            {
                foreach (Transaction_Costing.TransactionDetail_Costing tdc in tc.ListTransactionDetail_Costing)
                {
                    if (!ListRateCodeIDFetched.Contains(tdc.tdRow.RateCodeID))
                    {
                        if (!ListRateCodeIDToFetch.Contains(tdc.tdRow.RateCodeID))
                            ListRateCodeIDToFetch.Add(tdc.tdRow.RateCodeID);
                    }
                }
            }

            if (ListRateCodeIDToFetch.Count > 0)
            {
                // Get List of RateCode
                Result<List<RateCode.dbRow>> r_lrc = await _WBMDB.RateCode_ListByListIDs(ListRateCodeIDToFetch);

                if (r_lrc.IsSuccess)
                {
                    ListRateCode.AddRange(r_lrc.Value);
                    foreach (RateCode.dbRow rc in r_lrc.Value)
                    {
                        ListRateCodeIDFetched.Add(rc.ID);
                    }
                }
                else
                {
                    rtnVal = false;
                }

                if (rtnVal)
                {
                    // Get List of TableRate
                    Result<List<TableRate.dbRow>> r_ltr = await _WBMDB.TableRate_ListByListRateCodeIDsNoLookup(ListRateCodeIDToFetch);

                    if (r_ltr.IsSuccess)
                    {
                        ListTableRate.AddRange(r_ltr.Value);
                    }
                    else
                    {
                        rtnVal = false;
                    }
                }
            }

            if (rtnVal)
                return Result.Success(true);
            else
                return Result.Failure<Boolean>("Failed to get costing data");
        }

        private Transaction_Costing _currentTransactionCosting;
        private TransactionHeader.dbRow _currentTransactionHeader;
        private List<Transaction_Costing.TransactionDetail_Costing> _currentListTransactionDetailCosting;
        private TransactionDetail.dbRow _currentTransactionDetail;
        private RateCode.dbRow _currentRateCode;
        private List<TableRate.dbRow> _currentListTableRate;
        private DateTime _currentCostDate;

        private Decimal _costQuantity;

        private Decimal _invoiceExFuel = 0;
        private Decimal _invoiceFuel = 0;
        private Decimal _invoiceGST = 0;


        public TransactionCosting(Transaction_CostingHolder tch, WBMDatabase _WBMDB)
        {
            this._WBMDB = _WBMDB;
            this.TransactionCostingHolder = tch;
        }

        public async Task<Result<Boolean>> ProcessList()
        {
            Boolean Process_Status_Result = true;
            Boolean rtnVal = true;

            Result<Boolean> td_result = null;
            string ErrorMessage = null;

            Result<Boolean> get_data_result = await GetData();

            if (get_data_result.IsSuccess)
            {
                foreach (Transaction_Costing tc in TransactionCostingHolder.ListTransactionCosting)
                {
                    rtnVal = true;
                    _invoiceExFuel = 0;
                    _invoiceFuel = 0;

                    _currentTransactionCosting = tc;
                    _currentTransactionHeader = tc.thRow;
                    _currentListTransactionDetailCosting = tc.ListTransactionDetail_Costing;

                    _currentCostDate = _currentTransactionHeader.TransactionDateTime;

                    _currentTransactionCosting.CostedOK = true; // assume ok unless fault

                    Transaction_Costing.TransactionDetail_Costing minCharge = null;

                    foreach (Transaction_Costing.TransactionDetail_Costing tdc in _currentListTransactionDetailCosting)
                    {
                        // Exclude min charge line from costing
                        if (tdc.tdRow.RateCode == "MINCHG")
                        {
                            tdc.CostedOK = true;
                            minCharge = tdc;
                            continue;
                        };
                        _currentTransactionDetail = tdc.tdRow;
                        td_result = await ProcessTransactionDetail();

                        if (!td_result.IsSuccess)
                        {
                            rtnVal = false;
                            ErrorMessage = td_result.ErrorMessage;
                            tdc.ErrorMessage = ErrorMessage;
                            tdc.CostedOK = false;
                            _currentTransactionCosting.CostedOK = false;
                        }
                        else
                        {
                            tdc.CostedOK = true;
                        }
                    }

                    #region Check for Minimum Charge

                    if (rtnVal && minCharge != null)
                    {
                        Result<Decimal> rc = Calculate_MinimimCharge(minCharge.tdRow.RateCodeID);

                        if (rc.IsSuccess)
                        {
                            if (rc.Value > 0)
                            {
                                minCharge.tdRow.CalculatedExtended = rc.Value;
                                minCharge.tdRow.CalculatedFuel = 0;
                                minCharge.tdRow.Override = false;
                                minCharge.tdRow.ActualCustomerExtended = rc.Value;
                                minCharge.tdRow.ActualCustomerFuel = 0;

                                _invoiceExFuel += minCharge.tdRow.ActualCustomerExtended;
                                _invoiceFuel += minCharge.tdRow.ActualCustomerFuel;
                            }
                        }
                        else
                        {
                            rtnVal = false;
                            ErrorMessage = rc.ErrorMessage;
                            minCharge.ErrorMessage = ErrorMessage;
                            minCharge.CostedOK = false;
                            _currentTransactionCosting.CostedOK = false;
                        }
                    }

                    #endregion

                    if (rtnVal)
                    {
                        // Populate Header and calcuate GST
                        _currentTransactionHeader.InvoiceExFuel = _invoiceExFuel;
                        _currentTransactionHeader.InvoiceFuel = _invoiceFuel;

                        _currentTransactionHeader.InvoiceGST = CalculateGST(_invoiceExFuel + _invoiceFuel);
                    }
                    else
                        Process_Status_Result = false;

                }
            }
            else
            {
                Process_Status_Result = false;
                ErrorMessage = get_data_result.ErrorMessage;
            }

            if (Process_Status_Result)
            {
                TransactionCostingHolder.CostedOK = true;
                return Result.Success(true);
            }
            else
            {
                TransactionCostingHolder.CostedOK = true;
                TransactionCostingHolder.ErrorMessage = ErrorMessage;
                return Result.Failure<Boolean>(ErrorMessage);
            }
        }

        private int _previousRateCodeID = 0;

        private async Task<Result<Boolean>> ProcessTransactionDetail()
        {
            Boolean rtnVal = true;
            string ErrorMessage = null;

            // Get RateCode for this TransactionDetail

            // NOCHARGE has a ratecodeID of 0 - don't process
            if (_currentTransactionDetail.RateCodeID > 0)
            {
                #region Has RateCodeID changed from previous ?
                if (_currentTransactionDetail.RateCodeID != _previousRateCodeID)
                {
                    _currentRateCode = ListRateCode.FirstOrDefault(rc => rc.ID == _currentTransactionDetail.RateCodeID);
                    if (_currentRateCode == null)
                    {
                        rtnVal = false;
                        ErrorMessage = "RateCode not found for TransactionDetail ID " + _currentTransactionDetail.ID.ToString();
                    }
                    else
                    {
                        _previousRateCodeID = _currentTransactionDetail.RateCodeID;

                        // Get TableRate for this RateCode
                        _currentListTableRate = ListTableRate.Where(tr => tr.RateID == _currentRateCode.ID).OrderBy(x => x.TableBreak).ToList();
                    }
                }
                #endregion

                #region Main costing
                if (rtnVal)
                {
                    Decimal minCharge = 0;
                    int IncludedUnits = 0;
                    Decimal CalculatedCost = 0;

                    if (_currentRateCode.DateOfChange.HasValue && _currentCostDate >= _currentRateCode.DateOfChange.Value)
                    {
                        minCharge = _currentRateCode.NewMinimumCharge;
                        IncludedUnits = _currentRateCode.NewIncludedUnits;

                        // Basic
                        CalculatedCost = _currentRateCode.NewBasic;
                    }
                    else
                    {
                        minCharge = _currentRateCode.MinimumCharge;
                        IncludedUnits = _currentRateCode.IncludedUnits;

                        // Basic
                        CalculatedCost = _currentRateCode.Basic;
                    }

                    _costQuantity = _currentTransactionDetail.Quantity - IncludedUnits;

                    Result<Decimal> _table_result = null;

                    #region Cost Quantity
                    // Units to cost
                    if (_costQuantity > 0)
                    {
                        // It's possible for some rates not to have any table rate records - but if we've got a quantity then we should
                        // or, the ratecode needs to include more units

                        if (_currentListTableRate == null || _currentListTableRate.Count == 0)
                        {
                            rtnVal = false;
                            ErrorMessage = "No TableRate found for RateCodeID " + _currentRateCode.ID.ToString();
                        }
                        else
                        {
                            // Calculate cost based on RateType

                            switch (_currentRateCode.RateType)
                            {
                                case 1:
                                    // Flat Break point rate
                                    _table_result = await Calculate_FlatBreakPoint();
                                    break;
                                case 2:
                                    // Flat per unit rate
                                    _table_result = await Calculate_FlatPerUnit();
                                    break;
                                case 3:
                                    // Cumulative break point rate
                                    _table_result = await Calculate_CumulativeBreakPoint();
                                    break;
                                case 4:
                                    // Cumulative per unit rate
                                    _table_result = await Calculate_CumulativePerUnit();
                                    break;
                            }

                            if (_table_result.IsSuccess)
                            {
                                CalculatedCost += _table_result.Value;
                            }
                            else
                            {
                                rtnVal = false;
                                ErrorMessage = _table_result.ErrorMessage;
                            }
                        }
                    }
                    #endregion

                    #region min charge and update extended
                    if (rtnVal)
                    {
                        // Minimum charge
                        if (CalculatedCost < minCharge)
                        {
                            CalculatedCost = minCharge;
                        }

                        // Update TransactionDetail with CalculatedCost
                        _currentTransactionDetail.CalculatedExtended = CalculatedCost;
                        _currentTransactionDetail.CalculatedFuel = 0;
                        if (!_currentTransactionDetail.Override)
                        {
                            _currentTransactionDetail.ActualCustomerExtended = CalculatedCost;
                            _currentTransactionDetail.ActualCustomerFuel = 0;
                        }

                        _invoiceExFuel += _currentTransactionDetail.ActualCustomerExtended;
                        _invoiceFuel += _currentTransactionDetail.ActualCustomerFuel;
                    }
                    #endregion
                }
                #endregion
            }

            if (rtnVal)
                return Result.Success(true);
            else
                return Result.Failure<Boolean>(ErrorMessage);

        }

        private async Task<Result<Decimal>> Calculate_FlatBreakPoint()
        {
            TableRate.dbRow tr = _currentListTableRate.Where(x => x.TableBreak >= _costQuantity).FirstOrDefault();

            if (tr != null)
            {
                if (tr.DateOfChange.HasValue && _currentCostDate >= tr.DateOfChange.Value)
                    return Result.Success<Decimal>(tr.NewTableCharge);
                else
                    return Result.Success<Decimal>(tr.TableCharge);
            }
            else
                return Result.Failure<Decimal>("Quantity exceeds table break");
        }

        private async Task<Result<Decimal>> Calculate_FlatPerUnit()
        {
            TableRate.dbRow tr = _currentListTableRate.Where(x => x.TableBreak >= _costQuantity).FirstOrDefault();

            if (tr != null)
            {
                if (tr.DateOfChange.HasValue && _currentCostDate >= tr.DateOfChange.Value)
                    return Result.Success<Decimal>(tr.NewTableCharge * _costQuantity);
                else
                    return Result.Success<Decimal>(tr.TableCharge * _costQuantity);
            }
            else
                return Result.Failure<Decimal>("Quantity exceeds table break");
        }

        private async Task<Result<Decimal>> Calculate_CumulativeBreakPoint()
        {
            Decimal CumulativeCost = 0;

            foreach (TableRate.dbRow tr in _currentListTableRate)
            {
                CumulativeCost += tr.TableCharge;
                if (tr.TableBreak > _costQuantity)
                {
                    break;
                }
            }

            return Result.Success<Decimal>(CumulativeCost);
        }

        private async Task<Result<Decimal>> Calculate_CumulativePerUnit()
        {
            Boolean HasZeroUnits = false;

            Decimal CumulativeCost = 0;

            decimal tableCharge;
            decimal tableBreak;
            decimal tableUnits;
            decimal chargedUnits = 0;

            foreach (TableRate.dbRow tr in _currentListTableRate)
            {
                if (tr.TableUnits == 0)
                {
                    // Cannot divide by zero
                    HasZeroUnits = true;
                    break;
                }
                else
                {
                    tableBreak = tr.TableBreak;
                    tableUnits = tr.TableUnits;
                    tableCharge = tr.TableCharge;

                    if (tableBreak > _costQuantity)
                        tableBreak = _costQuantity;

                    decimal units = tableBreak - chargedUnits;

                    // This code from S2ME; do we need to have an option for doNotRoundUpRate ?
                    Boolean doNotRoundUpRate = false;

                    if (doNotRoundUpRate)
                        CumulativeCost += (units / tableUnits) * tableCharge;
                    else
                        CumulativeCost += Math.Ceiling(units / tableUnits) * tableCharge;
                }
            }

            if (HasZeroUnits)
                return Result.Failure<Decimal>("TableRate contains zero units, cannot calculate cumulative per unit rate");
            else
                return Result.Success<Decimal>(CumulativeCost);
        }

        private Result<Decimal> Calculate_MinimimCharge(int RateCodeID)
        {
            RateCode.dbRow rc = ListRateCode.FirstOrDefault(x => x.ID == RateCodeID);

            decimal minCharge = 0;

            if (rc != null)
            {
                if (rc.DateOfChange.HasValue && _currentCostDate >= rc.DateOfChange.Value)
                    minCharge = rc.NewMinimumCharge;
                else
                    minCharge = rc.MinimumCharge;

                if (_invoiceExFuel < minCharge)
                {
                    return Result.Success<Decimal>(minCharge - _invoiceExFuel);
                }
                else
                    return Result.Success<Decimal>(0m);

            }
            else
                return Result.Failure<Decimal>("RatecodeID of " + RateCodeID + " not found.");
        }

        private Decimal CalculateGST(Decimal amount)
        {
            return Math.Round(amount * 0.1M, 2);
        }
    }
}
