import csv
import os

base_dir = '/Users/kaichanghuang/Documents/Phoenix Code/DmsSystem/丟/新總代'
target_csv_path = os.path.join(base_dir, 'CG基金清單all_FAS191 1.csv')

# 手動建立對照表 (基於 Schema COMMENTS 與 CSV Headers 的分析)
mapping = {
    'No.': 'ORDINAL',
    '主基金': 'FUND_MASTER_NO',
    '基金代號': 'FUND_NO',
    '基金中文名稱': 'NAME',
    '基金英文名稱': 'ENAME',
    '幣別': 'CURRENCY_NO',
    '基金簡稱': 'SNAME',
    '基金簡簡稱': 'SSNAME',
    '配息說明(前台查詢用)': 'DIVIDEND_DESC',
    'Fund Code': 'ID',
    'ISIN Code': 'ISIN_CODE',
    '基金經理公司': 'AMC_NO',
    '基金專戶名稱': 'AC_NAME',
    '基金保管機構': 'CUSTODIAN',
    '基金註冊地': 'REGISTRATION',
    '避險級別': 'HEDGING_TYPE',
    '全球資產公告幣別': 'GLOBAL_CURRENCY',
    '銷售狀態': 'SALES_TYPE',
    '經理費率': 'MF_RATE',
    '變動經理費率': 'VARIABLE_MF',
    '保管費率': 'SF_RATE',
    '分銷費率': 'CF_RATE',
    '其他費用率': 'OF_RATE',
    '募集方式': 'OFFERING_TYPE',
    '投資區域': 'AREA_TYPE',
    '級別': 'SHARE_CLASS',
    '風險收益等級': 'RISK_CATEGORY',
    '基金種類2': 'FUND_TYPE',
    '投資類型': 'INVESTMENT_TYPE',
    '投信投顧公會基金類型': 'SITCA_FUND_TYPE',
    '衛星基金': 'CORE_SATELLITE',
    '收益分配設定': 'DIVIDEND_FREQ',
    '基金淨值日設定': 'CALENDAR_CODE',
    '基金交易日設定': 'TX_CALENDAR_CODE',
    '成立日': 'INCEPTION_DATE',
    '開賣日': 'SALES_DATE',
    '開始買回日': 'FIRST_RED_DATE',
    '開放集保NAV上傳': 'TSCD_NAV_UPLOAD_TYPE',
    '申購淨值日': 'PUR_NAV_DAY',
    '買回淨值日': 'RED_NAV_DAY',
    '買回付款日': 'PAY_DAY',
    '記帳日落差天數': 'BEHIND_DAYS',
    'EC交易日落差天數': 'EC_DIFF',
    '買回基金主帳戶銀行': 'RED_BANK_NO',
    '收益分配配息專戶銀行': 'DIVIDEND_BANK_NO',
    '開放集保': 'IS_TDCC',
    '手續費收取時點': 'SERVICE_CHARGE_TYPE',
    '首次申購原幣下限': 'MIN_INITIAL_PURCHASE',
    '再次申購原幣下限': 'MIN_AMOUNT',
    '收益分配下限': 'DIVIDEND_MIN',
    '結存原幣下限': 'MIN_BAL_AMOUNT',
    '淨值小數位數': 'NAV_DECIMAL',
    '價金小數位數': 'AMT_DECIMAL',
    '單位數小數位數': 'SHARE_DECIMAL',
    '單位數捨位方式': 'ROUNDING_SHARE',
    '買回價金計算方式': 'REDEMPTION_RULE',
    '客戶支付買回匯費': 'IS_WIRE_FEE',
    '買回分行': 'RED_BRANCH_NO',
    '受益分配分行': 'DIVIDEND_BRANCH_NO',
    '扣款時間': 'TDCC_DEBIT_CUT_OFF',
    '短線交易最小持有天數': 'EARLY_RED_MIN_DAYS',
    '是否收取買回費用': 'FORMULA_RED_FEE',
    '是否收取績效管理費': 'IS_PERFORMANCE_FEE',
    '是否年化報酬率': 'ANNUALIZED_ROI',
    '匯款/ATM銀行': 'BANK_NO',
    '匯款分行代碼': 'BRANCH_NO',
    '申購關帳時間': 'PUR_CUT_OFF',
    '買回關帳時間': 'RED_CUT_OFF',
    'EC及RSP申購關帳時間': 'EC_PUR_CUT_OFF',
    'EC買回關帳時間': 'EC_RED_CUT_OFF',
    '買回基金主帳戶': 'RED_AC_CODE',
    '收益分配配息專戶': 'DIVIDEND_AC_CODE',
    '匯款時間': 'TDCC_REMIT_CUT_OFF',
    '開放投資型連結保單': 'INVESTMENT_LINK',
    '開放申購': 'IS_PURCHASE',
    '開放定期定額': 'IS_PERIODIC',
    '開放語音交易': 'IS_VC',
    '開放網路交易': 'IS_EC',
    '開放網路申購': 'IS_EC_PURCHASE',
    'EC變更書面RSP': 'RSP_CHANGE',
    '開放網路轉申購': 'IS_EC_SWITCH_IN',
    '開放網路RSP': 'IS_EC_PERIODIC',
    '開放網路DRSP': 'IS_EC_DRSP',
    '開放ATM轉帳': 'EC_ATM',
    '開放網路買回': 'IS_EC_REDEMPTION',
    '開放銀行扣款': 'EC_DEDUCTION',
    '開放銀行匯款': 'EC_REMITTANCE',
    '開放ROBO': 'IS_ROBO',
    '匯費': 'BANK_WIRE_FEE',
    '登錄代理人代號': 'SUBTA_NO',
    '贖回時間': 'TDCC_RED_CUT_OFF'
}

# 讀取 CSV
try:
    with open(target_csv_path, 'r', encoding='utf-8') as f:
        reader = csv.reader(f)
        data = list(reader)
        
    if not data:
        print("CSV is empty")
        exit(1)
        
    headers = data[0]
    # Remove BOM
    if headers[0].startswith('\ufeff'):
        headers[0] = headers[0][1:]
        
    # Apply mapping
    new_headers = []
    for h in headers:
        new_headers.append(mapping.get(h, h))
        
    # Replace headers
    data[0] = new_headers
    
    # Write back
    with open(target_csv_path, 'w', encoding='utf-8', newline='') as f:
        writer = csv.writer(f)
        writer.writerows(data)
        
    print(f"Updated {len(headers)} headers. {sum(1 for h in headers if h in mapping)} mapped.")
    
except Exception as e:
    print(f"Error: {e}")
